using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Areas.People.Models.Task;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Lifecycle;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using CmsWeb.Models.iPhone;
using Dapper;
using ImageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;
using MobileAccount = CmsWeb.Areas.Public.Models.MobileAPIv2.MobileAccount;
using MobileAccountV1 = CmsWeb.MobileAPI.MobileAccount;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIv2Controller : CMSBaseController
	{
		public MobileAPIv2Controller( IRequestManager requestManager ) : base( requestManager ) { }

		public ActionResult Exists()
		{
			return Content( "1" );
		}

		[HttpPost]
		public ActionResult CreateUser( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobilePostCreate mpc = JsonConvert.DeserializeObject<MobilePostCreate>( message.data );
			mpc.lowerEmail();

			if( message.version < (int) MobileMessage.Version.EIGHT ) {
				MobileAccountV1 account = MobileAccountV1.Create( mpc.first, mpc.last, mpc.email, mpc.phone, mpc.dob );

				MobileMessage response = new MobileMessage();

				if( account.Result == MobileAccountV1.ResultCode.BadEmailAddress || account.Result == MobileAccountV1.ResultCode.FoundMultipleMatches ) {
					response.setError( (int) MobileMessage.Error.CREATE_FAILED );
				} else {
					response.setNoError();
					response.data = account.User.Username;
				}

				return response;
			} else {
				bool useMobileMessages = CurrentDatabase.Setting( "UseMobileMessages", "false" ) == "true";

				MobileAccount account = new MobileAccount( CurrentDatabase );
				account.setCreateFields( mpc.first, mpc.last, mpc.email, mpc.phone, mpc.dob, message.device, message.instance, message.key );
				account.create();

				return account.getMobileResponse( useMobileMessages );
			}
		}

		[HttpPost]
		public ActionResult UserPrivileges( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAppDevice device = CurrentDatabase.MobileAppDevices.FirstOrDefault( d => d.InstanceID == message.instance && d.UserID == message.argInt );

			MobileUserPrivileges privileges = new MobileUserPrivileges();

			if( device != null ) {
				StatusFlagColumn statusFlagColumn = CurrentDatabase.ViewStatusFlagColumns.FirstOrDefault( f => f.PeopleId == device.PeopleID );

				if( statusFlagColumn != null ) {
					PropertyInfo[] properties = statusFlagColumn.GetType().GetProperties();

					foreach( PropertyInfo property in properties ) {
						object value = property.GetValue( statusFlagColumn );

						if( value != null && value.GetType().Name == "String" && (string) value == "X" ) {
							privileges.flags.Add( property.Name );
						}
					}
				}

				IQueryable<string> roles = from r in CurrentDatabase.UserRoles where r.UserId == device.UserID orderby r.Role.RoleName select r.Role.RoleName;

				privileges.roles.AddRange( roles );
			}

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = SerializeJSON( privileges, message.version );

			return response;
		}

		[HttpPost]
		public ActionResult Authenticate( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance, message.data );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			IQueryable<string> roles = from r in CurrentDatabase.UserRoles
												where r.UserId == user.UserId
												orderby r.Role.RoleName
												select r.Role.RoleName;

			MobileSettings ms = new MobileSettings {
				peopleID = user.PeopleId ?? 0,
				userID = user.UserId,
				userName = user.Person.Name,
				campusID = user.Person.CampusId ?? 0,
				campusName = user.Person.Campu?.Description ?? "",
				roles = roles.ToList()
			};

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.instance = authentication.getInstanceID();
			response.data = SerializeJSON( ms, message.version );

			return response;
		}

		[HttpPost]
		public ActionResult QuickSignIn( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );
			message.lowerArgString();

			if( message.instance.Length == 0 ) {
				return MobileMessage.createErrorReturn( "Invalid instance ID", (int) MobileMessage.Error.INVALID_INSTANCE_ID );
			}

			if( message.argString.Length == 0 ) {
				return MobileMessage.createErrorReturn( "Invalid email address", (int) MobileMessage.Error.INVALID_EMAIL );
			}

			bool useMobileMessages = CurrentDatabase.Setting( "UseMobileMessages", "false" ) == "true";

			MobileAccount account = new MobileAccount( CurrentDatabase );
			account.setDeepLinkFields( message.device, message.instance, message.key, message.argString );
			account.sendDeepLink();

			return account.getMobileResponse( useMobileMessages );
		}

		[HttpPost]
		public ActionResult QuickSignInUsers( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance, allowQuick: true );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobileAppDevice device = authentication.getDevice();
			MobileMessage response = new MobileMessage();

			if( device == null ) {
				return response;
			}

			List<Person> people = (from p in CurrentDatabase.People
											where p.EmailAddress == device.CodeEmail || p.EmailAddress2 == device.CodeEmail
											orderby p.FirstName, p.LastName
											select p).ToList();

			if( people.Count <= 0 ) {
				return response;
			}

			List<MobileQuickSignInUser> users = new List<MobileQuickSignInUser>();

			foreach( Person person in people ) {
				if( person.Users.Count == 0 ) {
					users.Add( new MobileQuickSignInUser( person, null ) );
				} else {
					foreach( User user in person.Users ) {
						users.Add( new MobileQuickSignInUser( person, user ) );
					}
				}
			}

			response.setNoError();
			response.count = users.Count;
			response.setData( SerializeJSON( users, message.version ) );

			return response;
		}

		[HttpPost]
		public ActionResult QuickSignInCreateUser( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance, allowQuick: true );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobileMessage response = new MobileMessage();
			MobileAppDevice device = authentication.getDevice();

			if( !CurrentDatabase.People.Any( p => (p.EmailAddress == device.CodeEmail || p.EmailAddress2 == device.CodeEmail) && p.PeopleId == message.argInt ) ) {
				return response;
			}

			User user = AccountModel.AddUser( message.argInt );

			response.id = user.UserId;
			response.data = user.Username;
			response.setNoError();

			return response;
		}

		[HttpPost]
		public ActionResult SetDevicePIN( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance, allowQuick: true, userID: message.argInt );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			if( message.instance.Length == 0 ) {
				return MobileMessage.createErrorReturn( "Invalid instance ID", (int) MobileMessage.Error.INVALID_INSTANCE_ID );
			}

			if( message.argString.Length == 0 ) {
				return MobileMessage.createErrorReturn( "Invalid PIN", (int) MobileMessage.Error.INVALID_PIN );
			}

			if( !authentication.setPIN( message.device, message.instance, message.key, message.argString ) ) {
				return MobileMessage.createErrorReturn( "PIN was not set", (int) MobileMessage.Error.PIN_NOT_SET );
			}

			if( message.argInt > 0 && authentication.getType() == MobileAuthentication.Type.QUICK ) {
				authentication.setDeviceUser();
			}

			User user = authentication.getUser();

			IQueryable<string> roles = from r in CurrentDatabase.UserRoles
												where r.UserId == user.UserId
												orderby r.Role.RoleName
												select r.Role.RoleName;

			MobileSettings ms = new MobileSettings {
				peopleID = user.PeopleId ?? 0,
				userID = user.UserId,
				userName = user.Person.Name,
				campusID = user.Person.CampusId ?? 0,
				campusName = user.Person.Campu?.Description ?? "",
				roles = roles.ToList()
			};

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.instance = authentication.getInstanceID();
			response.data = SerializeJSON( ms, message.version );

			return response;
		}

		[HttpPost]
		public ActionResult AuthenticatedLink( string data )
		{
			// Link in data string is path only include leading slash
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			OneTimeLink ot = new OneTimeLink {
				Id = Guid.NewGuid(),
				Querystring = user.Username,
				Expires = DateTime.Now.AddMinutes( 15 )
			};

			CurrentDatabase.OneTimeLinks.InsertOnSubmit( ot );
			CurrentDatabase.SubmitChanges();

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = $"{CurrentDatabase.ServerLink( $"Logon?ReturnUrl={HttpUtility.UrlEncode( $"{message.argString}?{message.getSourceQueryString()}" )}&otltoken={ot.Id.ToCode()}" )}";

			return response;
		}

		[HttpPost]
		public ActionResult GivingLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			const string sql = @"SELECT OrganizationId FROM dbo.Organizations WHERE RegistrationTypeId = 8 AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";

			int? givingOrgId = CurrentDatabase.Connection.ExecuteScalar( sql ) as int?;

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = CurrentDatabase.ServerLink( $"OnlineReg/{givingOrgId}?{message.getSourceQueryString()}" );

			return response;
		}

		[HttpPost]
		public ActionResult OneTimeGivingLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			int? orgID;

			if( message.argBool ) {
				// Managed Giving
				orgID = CurrentDatabase.Organizations.Where( o => o.RegistrationTypeId == RegistrationTypeCode.ManageGiving ).Select( x => x.OrganizationId ).FirstOrDefault();
			} else {
				// Normal Giving
				const string sql = @"SELECT OrganizationId FROM dbo.Organizations WHERE RegistrationTypeId = 8 AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";

				orgID = CurrentDatabase.Connection.ExecuteScalar( sql ) as int?;
			}

			OneTimeLink ot = GetOneTimeLink( orgID ?? 0, user.PeopleId ?? 0 );

			CurrentDatabase.OneTimeLinks.InsertOnSubmit( ot );
			CurrentDatabase.SubmitChanges();

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = CurrentDatabase.ServerLink( $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{message.getSourceQueryString()}" );

			return response;
		}

		[HttpPost]
		public ActionResult OneTimeRegisterLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();
			int orgId = message.argInt;

			OneTimeLink ot = GetOneTimeLink( orgId, user.PeopleId ?? 0 );

			CurrentDatabase.OneTimeLinks.InsertOnSubmit( ot );
			CurrentDatabase.SubmitChanges();

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = CurrentDatabase.ServerLink( message.argBool ? $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?showfamily=true&{message.getSourceQueryString()}" : $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{message.getSourceQueryString()}" );

			return response;
		}

		[HttpPost]
		public ActionResult FetchPeople( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobilePostSearch mps = JsonConvert.DeserializeObject<MobilePostSearch>( message.data );

			SearchModel m = new SearchModel( mps.name, mps.comm, mps.addr );

			MobileMessage response = new MobileMessage();

			switch( (MobileMessage.Device) message.device ) {
				case MobileMessage.Device.ANDROID: {
					Dictionary<int, MobilePerson> mpl = new Dictionary<int, MobilePerson>();

					foreach( Person item in m.ApplySearch( mps.guest ).OrderBy( p => p.Name2 ).Take( 100 ) ) {
						MobilePerson mp = new MobilePerson().populate( item );

						mpl.Add( mp.id, mp );
					}

					response.data = SerializeJSON( mpl, message.version );

					break;
				}

				case MobileMessage.Device.IOS: {
					List<MobilePerson> mp = new List<MobilePerson>();

					foreach( Person item in m.ApplySearch( mps.guest ).OrderBy( p => p.Name2 ).Take( 100 ) ) {
						mp.Add( new MobilePerson().populate( item ) );
					}

					response.data = SerializeJSON( mp, message.version );

					break;
				}
			}

			response.setNoError();
			response.count = m.Count( mps.guest );

			return response;
		}

		[AcceptVerbs( HttpVerbs.Post )]
		public JsonResult RegCategories( string id )
		{
			string val = null;
			string[] a = id.Split( '-' );

			if( a.Length > 0 ) {
				Organization org = CurrentDatabase.LoadOrganizationById( a[1].ToInt() );

				if( org != null ) {
					val = org.AppCategory ?? "Other";
				}
			}

			Dictionary<string, string> categories = new Dictionary<string, string>();

			string lines = CurrentDatabase.Content( "AppRegistrations", "Other\tRegistrations" ).TrimEnd();

			Regex re = new Regex( @"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase );
			Match line = re.Match( lines );

			while( line.Success ) {
				string code = line.Groups[1].Value;
				string text = line.Groups[2].Value.TrimEnd();
				categories.Add( code, text );
				line = line.NextMatch();
			}

			if( !categories.ContainsKey( "Other" ) ) {
				categories.Add( "Other", "Registrations" );
			}

			if( val.HasValue() ) {
				categories.Add( "selected", val );
			}

			return Json( categories );
		}

		private List<MobileRegistrationCategory> GetRegistrations()
		{
			Dictionary<string, string> categories = new Dictionary<string, string>();
			List<MobileRegistrationCategory> list = new List<MobileRegistrationCategory>();

			DateTime dt = DateTime.Now;
			Regex re = new Regex( @"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase );

			string lines = CurrentDatabase.Content( "AppRegistrations", "Other\tRegistrations" ).TrimEnd();
			Match line = re.Match( lines );

			while( line.Success ) {
				categories.Add( line.Groups[1].Value, line.Groups[2].Value.TrimEnd() );
				line = line.NextMatch();
			}

			if( !categories.ContainsKey( "Other" ) ) {
				categories.Add( "Other", "Registrations" );
			}

			List<MobileRegistration> registrations = (from o in CurrentDatabase.ViewAppRegistrations
																	let sort = o.PublicSortOrder == null || o.PublicSortOrder.Length == 0 ? "10" : o.PublicSortOrder
																	select new MobileRegistration {
																		OrgId = o.OrganizationId,
																		Name = o.Title ?? o.OrganizationName,
																		UseRegisterLink2 = o.UseRegisterLink2 ?? false,
																		Description = o.Description,
																		PublicSortOrder = sort,
																		Category = o.AppCategory,
																		RegStart = o.RegStart,
																		RegEnd = o.RegEnd
																	}).ToList();

			foreach( KeyValuePair<string, string> cat in categories ) {
				List<MobileRegistration> current = (from mm in registrations where mm.Category == cat.Key where mm.RegStart <= dt orderby mm.PublicSortOrder, mm.Description select mm).ToList();

				if( current.Count > 0 ) {
					list.Add( new MobileRegistrationCategory {
						Current = true,
						Title = cat.Value,
						Registrations = current
					} );
				}

				List<MobileRegistration> future = (from mm in registrations where mm.Category == cat.Key where mm.RegStart > dt orderby mm.PublicSortOrder, mm.Description select mm).ToList();

				if( future.Count > 0 ) {
					list.Add( new MobileRegistrationCategory {
						Current = false,
						Title = cat.Value,
						Registrations = future.ToList()
					} );
				}
			}

			return list;
		}

		public ActionResult FetchRegistrations( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = SerializeJSON( GetRegistrations(), message.version );
			return response;
		}

		[HttpPost]
		public ActionResult FetchPerson( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobileMessage response = new MobileMessage();

			Person person = CurrentDatabase.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				response.setError( (int) MobileMessage.Error.PERSON_NOT_FOUND );
				response.data = "Person not found.";
				return response;
			}

			response.setNoError();
			response.count = 1;

			if( message.device == (int) MobileMessage.Device.ANDROID ) {
				response.data = SerializeJSON( new MobilePerson().populate( person ), message.version );
			} else {
				response.data = SerializeJSON( new List<MobilePerson> {
					new MobilePerson().populate( person )
				}, message.version );
			}

			return response;
		}

		[HttpPost]
		public ActionResult FetchPersonExtended( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobileMessage response = new MobileMessage();

			Person person = CurrentDatabase.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				response.setError( (int) MobileMessage.Error.PERSON_NOT_FOUND );
				response.data = "Person not found.";
				return response;
			}

			response.setNoError();
			response.count = 1;

			if( message.device == (int) MobileMessage.Device.ANDROID ) {
				response.data = SerializeJSON( new MobilePersonExtended().populate( person, message.argBool ), message.version );
			} else {
				List<MobilePersonExtended> mp = new List<MobilePersonExtended> {
					new MobilePersonExtended().populate( person, message.argBool )
				};
				response.data = SerializeJSON( mp, message.version );
			}

			return response;
		}

		[HttpPost]
		public ActionResult UpdatePerson( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			Person userPerson = CurrentDatabase.People.FirstOrDefault( p => p.PeopleId == user.PeopleId.Value );

			if( userPerson == null ) {
				return MobileMessage.createErrorReturn( "User not found!" );
			}

			switch( userPerson.PositionInFamilyId ) {
				case PositionInFamily.Child: {
					return MobileMessage.createErrorReturn( "Children cannot edit records" );
				}

				case PositionInFamily.SecondaryAdult when user.PeopleId != message.argInt: {
					return MobileMessage.createErrorReturn( "Secondary adults can only modify themselves" );
				}

				case PositionInFamily.PrimaryAdult when userPerson.Family.People.SingleOrDefault( fm => fm.PeopleId == message.argInt ) == null: {
					return MobileMessage.createErrorReturn( "Person must be in the same family" );
				}
			}

			MobileMessage response = new MobileMessage();

			Person person = CurrentDatabase.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				response.setError( (int) MobileMessage.Error.PERSON_NOT_FOUND );
				response.data = "Person not found.";
				return response;
			}

			List<MobilePostEditField> fields = JsonConvert.DeserializeObject<List<MobilePostEditField>>( message.data );

			List<ChangeDetail> personChangeList = new List<ChangeDetail>();
			List<ChangeDetail> familyChangeList = new List<ChangeDetail>();

			foreach( MobilePostEditField field in fields ) {
				field.updatePerson( person, personChangeList, familyChangeList );
			}

			if( personChangeList.Count > 0 ) {
				person.LogChanges( CurrentDatabase, personChangeList );
			}

			if( familyChangeList.Count > 0 ) {
				person.Family.LogChanges( CurrentDatabase, familyChangeList, person.PeopleId );
			}

			CurrentDatabase.SubmitChanges();

			response.setNoError();
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchGivingHistory( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( user.PeopleId != message.argInt ) {
				return BaseMessage.createErrorReturn( "Giving history is not available for other people" );
			}

			BaseMessage response = new BaseMessage();

			Person person = CurrentDatabase.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				response.setError( BaseMessage.API_ERROR_PERSON_NOT_FOUND );
				response.data = "Person not found.";
				return response;
			}

			int thisYear = DateTime.Now.Year;
			int lastYear = DateTime.Now.Year - 1;

			decimal lastYearTotal = (from c in CurrentDatabase.Contributions
											where c.PeopleId == person.PeopleId
													|| c.PeopleId == person.SpouseId
													&& (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint
											where !ContributionTypeCode.ReturnedReversedTypes.Contains( c.ContributionTypeId )
											where c.ContributionStatusId == ContributionStatusCode.Recorded
											where c.ContributionDate.Value.Year == lastYear
											orderby c.ContributionDate descending
											select c).AsEnumerable().Sum( c => c.ContributionAmount ?? 0 );

			List<MobileGivingEntry> entries = (from c in CurrentDatabase.Contributions
															let online = c.BundleDetails.Single().BundleHeader.BundleHeaderType.Description.Contains( "Online" )
															where c.PeopleId == person.PeopleId
																	|| c.PeopleId == person.SpouseId
																	&& (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint
															where !ContributionTypeCode.ReturnedReversedTypes.Contains( c.ContributionTypeId )
															where c.ContributionTypeId != ContributionTypeCode.Pledge
															where c.ContributionStatusId == ContributionStatusCode.Recorded
															where c.ContributionDate.Value.Year == thisYear
															orderby c.ContributionDate descending
															select new MobileGivingEntry {
																id = c.ContributionId,
																date = c.ContributionDate ?? DateTime.Now,
																fund = c.ContributionFund.FundName,
																giver = c.Person.Name,
																check = c.CheckNo,
																amount = (int) (c.ContributionAmount == null ? 0 : c.ContributionAmount * 100),
																type = ContributionTypeCode.SpecialTypes.Contains( c.ContributionTypeId )
																	? c.ContributionType.Description
																	: !online
																		? c.ContributionType.Description
																		: c.ContributionDesc == "Recurring Giving"
																			? c.ContributionDesc
																			: "Online"
															}).ToList();

			MobileGivingHistory history = new MobileGivingHistory();
			history.updateEntries( thisYear, entries );
			history.setLastYearTotal( lastYear, (int) (lastYearTotal * 100) );

			response.data = SerializeJSON( history, message.version );
			response.setNoError();
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchInvolvement( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( user.PeopleId != message.argInt ) {
				return BaseMessage.createErrorReturn( "Involvement is not available for other people" );
			}

			bool limitVisibility = user.InRole( "OrgLeadersOnly" ) || !user.InRole( "Access" );

			int[] orgIDs = new int[0];

			if( user.InRole( "OrgLeadersOnly" ) ) {
				orgIDs = CurrentDatabase.GetLeaderOrgIds( user.PeopleId );
			}

			List<MobileInvolvement> orgList = (from om in CurrentDatabase.OrganizationMembers
															let org = om.Organization
															where om.PeopleId == user.PeopleId
															where (om.Pending ?? false) == false
															where orgIDs.Contains( om.OrganizationId ) || !(limitVisibility && om.Organization.SecurityTypeId == 3)
															where org.LimitToRole == null || user.Roles.Contains( org.LimitToRole )
															orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
															select new MobileInvolvement {
																name = om.Organization.OrganizationName,
																leader = om.Organization.LeaderName ?? "",
																type = om.MemberType.Description,
																division = om.Organization.Division.Name,
																program = om.Organization.Division.Program.Name,
																@group = om.Organization.OrganizationType.Description ?? "Other",
																enrolledDate = om.EnrollmentDate,
																attendancePercent = (int) (om.AttendPct == null ? 0 : om.AttendPct * 100)
															}).ToList();

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = SerializeJSON( orgList, message.version );
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchImage( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobilePostFetchImage mpfi = JsonConvert.DeserializeObject<MobilePostFetchImage>( message.data );

			MobileMessage response = new MobileMessage();

			if( mpfi.id == 0 ) {
				return response.setData( "The ID for the person cannot be set to zero" );
			}

			response.data = "The picture was not found.";

			Person person = CurrentDatabase.People.SingleOrDefault( pp => pp.PeopleId == mpfi.id );

			if( person?.PictureId == null ) {
				return response;
			}

			Image image;

			switch( mpfi.size ) {
				// 50 x 50
				case 0: {
					image = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.ThumbId );
					break;
				}

				// 120 x 120
				case 1: {
					image = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.SmallId );
					break;
				}

				// 320 x 400
				case 2: {
					image = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.MediumId );
					break;
				}

				// 570 x 800
				case 3: {
					image = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.LargeId );
					break;
				}

				default: {
					return response;
				}
			}

			if( image == null ) {
				return response;
			}

			response.data = Convert.ToBase64String( image.Bits );
			response.count = 1;
			response.setNoError();

			return response;
		}

		[HttpPost]
		public ActionResult SaveImage( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			MobileMessage response = new MobileMessage();

			byte[] imageBytes = Convert.FromBase64String( message.argString );

			Person person = CurrentDatabase.People.SingleOrDefault( pp => pp.PeopleId == message.argInt );

			if( person?.Picture != null ) {
				Image imageDataThumb = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.ThumbId );

				if( imageDataThumb != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataThumb );
				}

				Image imageDataSmall = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.SmallId );

				if( imageDataSmall != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataSmall );
				}

				Image imageDataMedium = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.MediumId );

				if( imageDataMedium != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataMedium );
				}

				Image imageDataLarge = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == person.Picture.LargeId );

				if( imageDataLarge != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataLarge );
				}

				person.Picture.ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id;
				person.Picture.SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id;
				person.Picture.MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id;
				person.Picture.LargeId = Image.NewImageFromBits( imageBytes ).Id;
			} else {
				Picture newPicture = new Picture {
					ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id,
					SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id,
					MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id,
					LargeId = Image.NewImageFromBits( imageBytes ).Id
				};

				if( person != null ) {
					person.Picture = newPicture;
				}
			}

			CurrentDatabase.SubmitChanges();

			person?.LogPictureUpload( CurrentDatabase, user.PeopleId ?? 1 );

			response.setNoError();
			response.data = "Image updated.";
			response.id = message.argInt;
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult SaveFamilyImage( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobilePostSaveImage mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>( message.data );

			MobileMessage response = new MobileMessage();

			byte[] imageBytes = Convert.FromBase64String( mpsi.image );

			Family family = CurrentDatabase.Families.SingleOrDefault( pp => pp.FamilyId == mpsi.id );

			if( family?.Picture != null ) {
				Image imageDataThumb = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == family.Picture.ThumbId );

				if( imageDataThumb != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataThumb );
				}

				Image imageDataSmall = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == family.Picture.SmallId );

				if( imageDataSmall != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataSmall );
				}

				Image imageDataMedium = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == family.Picture.MediumId );

				if( imageDataMedium != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataMedium );
				}

				Image imageDataLarge = CurrentImageDatabase.Images.SingleOrDefault( i => i.Id == family.Picture.LargeId );

				if( imageDataLarge != null ) {
					CurrentImageDatabase.Images.DeleteOnSubmit( imageDataLarge );
				}

				family.Picture.ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id;
				family.Picture.SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id;
				family.Picture.MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id;
				family.Picture.LargeId = Image.NewImageFromBits( imageBytes ).Id;
			} else {
				Picture newPicture = new Picture {
					ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id,
					SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id,
					MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id,
					LargeId = Image.NewImageFromBits( imageBytes ).Id
				};

				if( family != null ) {
					family.Picture = newPicture;
				}
			}

			CurrentDatabase.SubmitChanges();

			response.setNoError();
			response.data = "Image updated.";
			response.id = mpsi.id;
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchTasks( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			IQueryable<IncompleteTask> tasks = from t in CurrentDatabase.ViewIncompleteTasks
															where t.OwnerId == user.PeopleId || t.CoOwnerId == user.PeopleId
															orderby t.CreatedOn, t.StatusId, t.OwnerId, t.CoOwnerId
															select t;

			IQueryable<Task> complete = (from c in CurrentDatabase.Tasks
													where c.StatusId == TaskStatusCode.Complete
													where c.OwnerId == user.PeopleId || c.CoOwnerId == user.PeopleId
													orderby c.CreatedOn descending
													select c).Take( 20 );

			MobileMessage response = new MobileMessage();

			switch( (MobileMessage.Device) message.device ) {
				case MobileMessage.Device.ANDROID: {
					Dictionary<int, MobileTask> taskList = new Dictionary<int, MobileTask>();

					foreach( IncompleteTask item in tasks ) {
						MobileTask task = new MobileTask().populate( item, user.PeopleId ?? 0 );
						taskList.Add( task.id, task );
					}

					foreach( Task item in complete ) {
						MobileTask task = new MobileTask().populate( item, user.PeopleId ?? 0 );
						taskList.Add( task.id, task );
					}

					response.data = SerializeJSON( taskList, message.version );
					break;
				}

				case MobileMessage.Device.IOS: {
					List<MobileTask> taskList = new List<MobileTask>();

					foreach( IncompleteTask item in tasks ) {
						MobileTask task = new MobileTask().populate( item, user.PeopleId ?? 0 );
						taskList.Add( task );
					}

					foreach( Task item in complete ) {
						MobileTask task = new MobileTask().populate( item, user.PeopleId ?? 0 );
						taskList.Add( task );
					}

					response.data = SerializeJSON( taskList, message.version );
					break;
				}
			}

			response.count = tasks.Count();
			response.setNoError();
			return response;
		}

		[HttpPost]
		public ActionResult AcceptTask( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			MobileMessage response = new MobileMessage();

			if( TaskModel.AcceptTask( user, message.argInt, CurrentDatabase.Host, CurrentDatabase ) ) {
				response.setNoError();
			} else {
				response.setError( (int) MobileMessage.Error.TASK_UPDATE_FAILED );
			}

			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult DeclineTask( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			MobileMessage response = new MobileMessage();

			if( TaskModel.DeclineTask( user, message.argInt, message.argString, CurrentDatabase.Host, CurrentDatabase ) ) {
				response.setNoError();
			} else {
				response.setError( (int) MobileMessage.Error.TASK_UPDATE_FAILED );
			}

			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult CompleteTask( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			MobileMessage response = new MobileMessage();

			if( TaskModel.CompleteTask( user, message.argInt, CurrentDatabase.Host, CurrentDatabase ) ) {
				response.setNoError();
			} else {
				response.setError( (int) MobileMessage.Error.TASK_UPDATE_FAILED );
			}

			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchCompleteWithContactLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			int contactID = TaskModel.AddCompletedContact( message.argInt, user, CurrentDatabase.Host, CurrentDatabase );

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.data = GetOneTimeLoginLink( $"/Contact2/{contactID}?edit=true&{message.getSourceQueryString()}", user.Username );
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchCompletedContactLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			Task task = (from t in CurrentDatabase.Tasks
							where t.Id == message.argInt
							select t).SingleOrDefault();

			MobileMessage response = new MobileMessage();

			if( task?.CompletedContactId == null ) {
				return response;
			}

			response.setNoError();
			response.data = GetOneTimeLoginLink( $"/Contact2/{task.CompletedContactId}?{message.getSourceQueryString()}", user.Username );
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult FetchOrgs( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance role is required to take attendance for organizations" );
			}

			int? pid = user.PeopleId;
			int[] orgIDs = CurrentDatabase.GetLeaderOrgIds( pid );

			IQueryable<Organization> q;

			if( user.InRole( "OrgLeadersOnly" ) ) {
				q = from o in CurrentDatabase.Organizations where o.LimitToRole == null || user.Roles.Contains( o.LimitToRole ) where orgIDs.Contains( o.OrganizationId ) where o.OrganizationStatusId == OrgStatusCode.Active select o;
			} else {
				// either a leader, who is not pending / inactive
				// or a leader of a parent org
				q = from o in CurrentDatabase.Organizations
					where o.LimitToRole == null || user.Roles.Contains( o.LimitToRole )
					where o.OrganizationMembers.Any( om => om.PeopleId == pid
																		&& (om.Pending ?? false) == false
																		&& om.MemberTypeId != MemberTypeCode.InActive
																		&& om.MemberType.AttendanceTypeId == AttendTypeCode.Leader ) || orgIDs.Contains( o.OrganizationId )
					where o.OrganizationStatusId == OrgStatusCode.Active
					select o;
			}

			IQueryable<OrganizationInfo> orgs = from o in q
															from sch in CurrentDatabase.ViewOrgSchedules2s.Where( s => o.OrganizationId == s.OrganizationId ).DefaultIfEmpty()
															from mtg in CurrentDatabase.Meetings.Where( m => o.OrganizationId == m.OrganizationId && m.MeetingDate < DateTime.Today.AddDays( 1 ) ).OrderByDescending( m => m.MeetingDate ).Take( 1 ).DefaultIfEmpty()
															orderby sch.SchedDay, sch.SchedTime
															select new OrganizationInfo {
																id = o.OrganizationId,
																name = o.OrganizationName,
																time = sch.SchedTime,
																day = sch.SchedDay,
																lastMeetting = mtg.MeetingDate
															};

			MobileMessage response = new MobileMessage();
			List<MobileOrganization> mo = new List<MobileOrganization>();

			response.setNoError();
			response.count = orgs.Count();

			foreach( OrganizationInfo item in orgs ) {
				MobileOrganization org = new MobileOrganization().populate( item );

				mo.Add( org );
			}

			response.data = SerializeJSON( mo, message.version );

			return response;
		}

		[HttpPost]
		public ActionResult FetchOrgRollList( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance role is required to take attendance for organizations" );
			}

			MobilePostRollList mprl = JsonConvert.DeserializeObject<MobilePostRollList>( message.data );

			int meetingId = CurrentDatabase.CreateMeeting( mprl.id, mprl.datetime );

			Meeting meeting = CurrentDatabase.Meetings.SingleOrDefault( m => m.MeetingId == meetingId );

			bool attendanceBySubGroup = meeting != null && (meeting.Organization.AttendanceBySubGroups ?? false);

			List<RollsheetModel.AttendInfo> people = attendanceBySubGroup
				? RollsheetModel.RollListFilteredBySubgroup( meetingId, mprl.id, mprl.datetime, fromMobile: true )
				: RollsheetModel.RollList( meetingId, mprl.id, mprl.datetime, fromMobile: true );

			List<MobileMeetingCategory> categories = (from c in CurrentDatabase.MeetingCategories
																	where c.NotBeforeDate == null || c.NotBeforeDate.Value <= DateTime.UtcNow
																	where c.NotAfterDate == null || c.NotAfterDate.Value > DateTime.UtcNow
																	select new MobileMeetingCategory {
																		id = c.Id,
																		category = c.Description
																	}).ToList();

			MobileRollList mrl = new MobileRollList {
				meetingID = meetingId,
				headcountEnabled = CurrentDatabase.Setting( "RegularMeetingHeadCount", "true" ),
				attendees = new List<MobileAttendee>(),
				categoriesEnabled = CurrentDatabase.Setting( "CheckinUseMeetingCategory", "false" ),
				category = meeting?.Description ?? "",
				categories = categories
			};

			if( meeting != null ) {
				mrl.headcount = meeting.HeadCount ?? 0;
			}

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.id = meetingId;
			response.count = people.Count;

			foreach( RollsheetModel.AttendInfo person in people ) {
				mrl.attendees.Add( new MobileAttendee().populate( person ) );
			}

			response.data = SerializeJSON( mrl, message.version );
			return response;
		}

		[HttpPost]
		public ActionResult RecordAttend( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance role is required to take attendance for organizations" );
			}

			MobileMessage dataIn = MobileMessage.createFromString( data );

			MobilePostAttend mpa = JsonConvert.DeserializeObject<MobilePostAttend>( dataIn.data );

			Meeting meeting = CurrentDatabase.Meetings.SingleOrDefault( m => m.OrganizationId == mpa.orgID && m.MeetingDate == mpa.datetime );

			if( meeting == null ) {
				CurrentDatabase.CreateMeeting( mpa.orgID, mpa.datetime );
			}

			Attend.RecordAttend( CurrentDatabase, mpa.peopleID, mpa.orgID, mpa.present, mpa.datetime );

			CurrentDatabase.UpdateMeetingCounters( mpa.orgID );
			DbUtil.LogActivity( $"Mobile RecAtt o:{mpa.orgID} p:{mpa.peopleID} u:{user.PeopleId} a:{mpa.present}" );

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult RecordHeadcount( string data )
		{
			if( CurrentDatabase.Setting( "RegularMeetingHeadCount", "true" ) == "disabled" ) {
				return MobileMessage.createErrorReturn( "Headcounts for meetings are disabled" );
			}

			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance role is required to take attendance for organizations" );
			}

			MobilePostHeadcount mph = JsonConvert.DeserializeObject<MobilePostHeadcount>( message.data );

			MobileMessage response = new MobileMessage();

			Meeting meeting = CurrentDatabase.Meetings.SingleOrDefault( m => m.OrganizationId == mph.orgID && m.MeetingDate == mph.datetime );

			if( meeting == null ) {
				return response;
			}

			meeting.HeadCount = mph.headcount;

			CurrentDatabase.SubmitChanges();
			DbUtil.LogActivity( $"Mobile Headcount o:{meeting.OrganizationId} m:{meeting.MeetingId} h:{mph.headcount}" );

			response.setNoError();
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult RecordMeetingCategory( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance role is required to set the meeting category", MobileMessage.Error.USER_MISSING_ROLE.ToInt() );
			}

			Meeting meeting = CurrentDatabase.Meetings.SingleOrDefault( m => m.MeetingId == message.id );
			CmsData.MeetingCategory category = CurrentDatabase.MeetingCategories.SingleOrDefault( c => c.Id == message.argInt );

			if( meeting == null || category == null ) {
				return meeting == null ? MobileMessage.createErrorReturn( "Meeting not found!", MobileMessage.Error.MEETING_NOT_FOUND.ToInt() ) : MobileMessage.createErrorReturn( "Meeting category not found!", MobileMessage.Error.MEETING_CATEGORY_NOT_FOUND.ToInt() );
			}

			meeting.Description = category.Description;

			CurrentDatabase.SubmitChanges();

			MobileMessage response = new MobileMessage();
			response.setNoError();

			return response;
		}

		[HttpPost]
		public ActionResult AddPerson( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance role is required to take attendance for organizations." );
			}

			MobileMessage dataIn = MobileMessage.createFromString( data );

			MobilePostAddPerson mpap = JsonConvert.DeserializeObject<MobilePostAddPerson>( dataIn.data );
			mpap.clean();

			Person p = new Person {
				CreatedDate = DateTime.Now,
				CreatedBy = user.UserId,
				MemberStatusId = MemberStatusCode.JustAdded,
				AddressTypeId = 10,
				FirstName = mpap.firstName,
				LastName = mpap.lastName,
				AltName = mpap.altName,
				Name = ""
			};

			if( mpap.goesBy.Length > 0 ) {
				p.NickName = mpap.goesBy;
			}

			if( mpap.birthday != null ) {
				p.BirthDay = mpap.birthday.Value.Day;
				p.BirthMonth = mpap.birthday.Value.Month;
				p.BirthYear = mpap.birthday.Value.Year;
			}

			p.GenderId = mpap.genderID;
			p.MaritalStatusId = mpap.maritalStatusID;

			Family f;

			if( mpap.familyID > 0 ) {
				f = CurrentDatabase.Families.First( fam => fam.FamilyId == mpap.familyID );
			} else {
				f = new Family();

				if( mpap.homePhone.Length > 0 ) {
					f.HomePhone = mpap.homePhone;
				}

				if( mpap.address.Length > 0 ) {
					f.AddressLineOne = mpap.address;
				}

				if( mpap.address2.Length > 0 ) {
					f.AddressLineTwo = mpap.address2;
				}

				if( mpap.city.Length > 0 ) {
					f.CityName = mpap.city;
				}

				if( mpap.state.Length > 0 ) {
					f.StateCode = mpap.state;
				}

				if( mpap.zipcode.Length > 0 ) {
					f.ZipCode = mpap.zipcode;
				}

				if( mpap.country.Length > 0 ) {
					f.CountryName = mpap.country;
				}

				CurrentDatabase.Families.InsertOnSubmit( f );
			}

			f.People.Add( p );

			p.PositionInFamilyId = CurrentDatabase.ComputePositionInFamily( mpap.getAge(), mpap.maritalStatusID == MaritalStatusCode.Married, f.FamilyId ) ?? PositionInFamily.PrimaryAdult;

			p.OriginId = OriginCode.Visit;
			p.FixTitle();

			if( mpap.eMail.Length > 0 && !mpap.eMail.Equal( "na" ) ) {
				p.EmailAddress = mpap.eMail;
			}

			if( mpap.cellPhone.Length > 0 ) {
				p.CellPhone = mpap.cellPhone;
			}

			if( mpap.homePhone.Length > 0 ) {
				p.HomePhone = mpap.homePhone;
			}

			p.MaritalStatusId = mpap.maritalStatusID;
			p.GenderId = mpap.genderID;

			CurrentDatabase.SubmitChanges();

			if( mpap.visitMeeting > 0 ) {
				Meeting meeting = CurrentDatabase.Meetings.Single( mm => mm.MeetingId == mpap.visitMeeting );

				Attend.RecordAttendance( p.PeopleId, mpap.visitMeeting, true );

				CurrentDatabase.UpdateMeetingCounters( mpap.visitMeeting );

				p.CampusId = meeting.Organization.CampusId;
				p.EntryPoint = meeting.Organization.EntryPoint;

				CurrentDatabase.SubmitChanges();
			}

			if( !user.InRole( "OrgLeadersOnly" ) ) {
				Task.AddNewPerson( CurrentDatabase, p.PeopleId );
			} else {
				IEnumerable<Person> np = CurrentDatabase.GetNewPeopleManagers();

				if( np != null ) {
					CurrentDatabase.Email( CurrentDatabase.Setting( "AdminMail", "" ), np, $"Just Added Person on {CurrentDatabase.Host}", $"<a href='{CurrentDatabase.ServerLink( "/Person2/" + p.PeopleId )}'>{p.Name}</a>" );
				}
			}

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.id = p.PeopleId;
			response.count = 1;

			return response;
		}

		[HttpPost]
		public ActionResult JoinOrg( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) ) {
				return MobileMessage.createErrorReturn( "Attendance or Checkin role is required to take attendance for organizations." );
			}

			MobileMessage dataIn = MobileMessage.createFromString( data );

			MobilePostJoinOrg mpjo = JsonConvert.DeserializeObject<MobilePostJoinOrg>( dataIn.data );

			OrganizationMember om = CurrentDatabase.OrganizationMembers.SingleOrDefault( m => m.PeopleId == mpjo.peopleID && m.OrganizationId == mpjo.orgID );

			if( om == null && mpjo.join ) {
				om = OrganizationMember.InsertOrgMembers( CurrentDatabase, mpjo.orgID, mpjo.peopleID, MemberTypeCode.Member, DateTime.Today );
			}

			if( om != null && !mpjo.join ) {
				om.Drop( CurrentDatabase, DateTime.Now );

				DbUtil.LogActivity( $"Dropped {om.PeopleId} for {om.Organization.OrganizationId} via {dataIn.getSourceOS()} app", peopleid: om.PeopleId, orgid: om.OrganizationId );
			}

			CurrentDatabase.SubmitChanges();

			MobileMessage response = new MobileMessage();
			response.setNoError();
			response.count = 1;

			return response;
		}

		public ActionResult SystemLists( string data )
		{
			MobileMessage unused = MobileMessage.createFromString( data );

			MobilePersonCreateLists allLists = new MobilePersonCreateLists {
				campuses = (from e in CurrentDatabase.Campus
								orderby e.Description
								select new MobileCampus {
									id = e.Id,
									name = e.Description
								}).ToList(),
				countries = (from e in CurrentDatabase.Countries
								orderby e.Id
								select new MobileCountry {
									id = e.Id,
									code = e.Code,
									description = e.Description
								}).ToList(),
				states = (from e in CurrentDatabase.StateLookups
							orderby e.StateCode
							select new MobileState {
								code = e.StateCode,
								name = e.StateName
							}).ToList(),
				maritalStatuses = (from e in CurrentDatabase.MaritalStatuses
										orderby e.Id
										select new MobileMaritalStatus {
											id = e.Id,
											code = e.Code,
											description = e.Description
										}).ToList()
			};

			BaseMessage response = new BaseMessage {
				error = 0,
				count = 3,
				data = JsonConvert.SerializeObject( allLists )
			};

			return response;
		}

		public ActionResult MapInfo( string data )
		{
			MobileMessage unused = MobileMessage.createFromString( data );

			IQueryable<MobileCampus> campuses = from p in CurrentDatabase.MobileAppBuildings
															where p.Enabled
															orderby p.Order
															select new MobileCampus {
																id = p.Id,
																name = p.Name
															};

			List<MobileCampus> campusList = campuses.ToList();

			foreach( MobileCampus campus in campusList ) {
				IQueryable<MobileFloor> floors = from p in CurrentDatabase.MobileAppFloors
															where p.Enabled
															where p.Campus == campus.id
															orderby p.Order
															select new MobileFloor {
																id = p.Id,
																name = p.Name,
																image = p.Image
															};

				List<MobileFloor> floorList = floors.ToList();

				foreach( MobileFloor floor in floorList ) {
					IQueryable<MobileRoom> rooms = from p in CurrentDatabase.MobileAppRooms
															where p.Enabled
															where p.Floor == floor.id
															orderby p.Room
															select new MobileRoom {
																name = p.Name,
																room = p.Room,
																x = p.X,
																y = p.Y
															};

					floor.rooms = rooms.ToList();
				}

				campus.floors = floorList;
			}

			BaseMessage response = new BaseMessage();
			response.setNoError();
			response.count = campusList.Count;
			response.data = JsonConvert.SerializeObject( campusList );

			return response;
		}

		public ActionResult DetailedGivingStatement( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication( CurrentDatabase );
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Admin" ) ) {
				return MobileMessage.createErrorReturn( "Admin roles is require while in testing" );
			}

			int year = 2016;

			DateTime firstOfYear = new DateTime( year, 1, 1, 0, 0, 0 );
			DateTime lastOfYear = new DateTime( year, 12, 31, 23, 59, 59 );

			MobileGivingStatement statement = new MobileGivingStatement( CurrentDatabase, 835077, firstOfYear, lastOfYear );
			statement.loadContributions( CurrentDatabase );

			BaseMessage response = new BaseMessage();
			response.setNoError();

			return response;
		}

		private static OneTimeLink GetOneTimeLink( int orgId, int peopleId )
		{
			return new OneTimeLink {
				Id = Guid.NewGuid(),
				Querystring = $"{orgId},{peopleId},0",
				Expires = DateTime.Now.AddMinutes( 10 )
			};
		}

		private string GetOneTimeLoginLink( string url, string user )
		{
			OneTimeLink oneTimeLink = new OneTimeLink {
				Id = Guid.NewGuid(),
				Querystring = user,
				Expires = DateTime.Now.AddMinutes( 15 )
			};

			CurrentDatabase.OneTimeLinks.InsertOnSubmit( oneTimeLink );
			CurrentDatabase.SubmitChanges();

			return $"{CurrentDatabase.ServerLink( $"Logon?ReturnUrl={HttpUtility.UrlEncode( url )}&otltoken={oneTimeLink.Id.ToCode()}" )}";
		}

		// ReSharper disable once UnusedParameter.Local
		// version argument for future use. Was used in the past, may be needed again
		private static string SerializeJSON( object item, int version )
		{
			return JsonConvert.SerializeObject( item, new IsoDateTimeConverter {
				DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss"
			} );
		}
	}
}