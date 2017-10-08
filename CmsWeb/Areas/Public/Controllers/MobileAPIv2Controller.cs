using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Areas.People.Models.Task;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.MobileAPI;
using CmsWeb.Models.iPhone;
using Dapper;
using ImageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIv2Controller : Controller
	{
		public ActionResult Exists()
		{
			return Content( "1" );
		}

		[HttpPost]
		public ActionResult CreateUser( string data )
		{
			MobileMessage dataIn = MobileMessage.createFromString( data );
			MobilePostCreate mpc = JsonConvert.DeserializeObject<MobilePostCreate>( dataIn.data );

			MobileAccount account = MobileAccount.Create( mpc.first, mpc.last, mpc.email, mpc.phone, mpc.dob );

			MobileMessage br = new MobileMessage();

			if( account.Result == MobileAccount.ResultCode.BadEmailAddress || account.Result == MobileAccount.ResultCode.FoundMultipleMatches ) {
				br.setError( (int) MobileMessage.Error.CREATE_FAILED );
			} else {
				br.setNoError();
				br.data = account.User.Username;
			}

			return br;
		}

		[HttpPost]
		public ActionResult Authenticate( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			IQueryable<string> roles = from r in DbUtil.Db.UserRoles
												where r.UserId == user.UserId
												orderby r.Role.RoleName
												select r.Role.RoleName;

			MobileSettings ms = new MobileSettings
			{
				peopleID = user.PeopleId ?? 0,
				userID = user.UserId,
				userName = user.Person.Name,
				campusID = user.Person.CampusId ?? 0,
				campusName = user.Person.Campu?.Description ?? "",
				roles = roles.ToList()
			};

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = SerializeJSON( ms, message.version );

			return br;
		}

		[HttpPost]
		public ActionResult SetDevicePIN( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			if( message.instance.Length == 0 ) {
				return MobileMessage.createErrorReturn( "Invalid instance ID", (int) MobileMessage.Error.INVALID_INSTANCE_ID );
			}

			if( message.argString.Length == 0 ) {
				return MobileMessage.createErrorReturn( "Invalid PIN", (int) MobileMessage.Error.INVALID_PIN );
			}

			authentication.setPIN( message.device, message.instance, message.key, message.argString );

			MobileMessage br = new MobileMessage();
			br.setNoError();

			return br;
		}

		[HttpPost]
		public ActionResult AuthenticatedLink( string data )
		{
			// Link in data string is path only include leading slash
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			OneTimeLink ot = new OneTimeLink
			{
				Id = Guid.NewGuid(),
				Querystring = user.Username,
				Expires = DateTime.Now.AddMinutes( 15 )
			};

			DbUtil.Db.OneTimeLinks.InsertOnSubmit( ot );
			DbUtil.Db.SubmitChanges();

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = $"{DbUtil.Db.ServerLink( $"Logon?ReturnUrl={HttpUtility.UrlEncode( message.argString )}&otltoken={ot.Id.ToCode()}" )}";

			return br;
		}

		[HttpPost]
		public ActionResult GivingLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			const string sql = @"SELECT OrganizationId FROM dbo.Organizations WHERE RegistrationTypeId = 8 AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";

			int? givingOrgId = DbUtil.Db.Connection.ExecuteScalar( sql ) as int?;

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = DbUtil.Db.ServerLink( $"OnlineReg/{givingOrgId}?{message.getSourceQueryString()}" );

			return br;
		}

		[HttpPost]
		public ActionResult OneTimeGivingLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			int? orgID;

			if( message.argBool ) {
				// Managed Giving
				orgID = DbUtil.Db.Organizations
									.Where( o => o.RegistrationTypeId == RegistrationTypeCode.ManageGiving )
									.Select( x => x.OrganizationId ).FirstOrDefault();
			} else {
				// Normal Giving
				const string sql = @"SELECT OrganizationId FROM dbo.Organizations WHERE RegistrationTypeId = 8 AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";

				orgID = DbUtil.Db.Connection.ExecuteScalar( sql ) as int?;
			}

			OneTimeLink ot = GetOneTimeLink( orgID ?? 0, user.PeopleId ?? 0 );

			DbUtil.Db.OneTimeLinks.InsertOnSubmit( ot );
			DbUtil.Db.SubmitChanges();

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = DbUtil.Db.ServerLink( $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{message.getSourceQueryString()}" );

			return br;
		}

		[HttpPost]
		public ActionResult OneTimeRegisterLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();
			int orgId = message.argInt;

			OneTimeLink ot = GetOneTimeLink( orgId, user.PeopleId ?? 0 );

			DbUtil.Db.OneTimeLinks.InsertOnSubmit( ot );
			DbUtil.Db.SubmitChanges();

			MobileMessage br = new MobileMessage();
			br.setNoError();

			if( message.argBool ) {
				br.data = DbUtil.Db.ServerLink( $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?showfamily=true&{message.getSourceQueryString()}" );
			} else {
				br.data = DbUtil.Db.ServerLink( $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{message.getSourceQueryString()}" );
			}

			return br;
		}

		[HttpPost]
		public ActionResult FetchPeople( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobilePostSearch mps = JsonConvert.DeserializeObject<MobilePostSearch>( message.data );

			SearchModel m = new SearchModel( mps.name, mps.comm, mps.addr );

			MobileMessage br = new MobileMessage();

			switch( (MobileMessage.Device) message.device ) {
				case MobileMessage.Device.ANDROID: {
					Dictionary<int, MobilePerson> mpl = new Dictionary<int, MobilePerson>();

					foreach( Person item in m.ApplySearch( mps.guest ).OrderBy( p => p.Name2 ).Take( 100 ) ) {
						MobilePerson mp = new MobilePerson().populate( item );

						mpl.Add( mp.id, mp );
					}

					br.data = SerializeJSON( mpl, message.version );
					break;
				}

				case MobileMessage.Device.IOS: {
					List<MobilePerson> mp = new List<MobilePerson>();

					foreach( Person item in m.ApplySearch( mps.guest ).OrderBy( p => p.Name2 ).Take( 100 ) ) {
						mp.Add( new MobilePerson().populate( item ) );
					}

					br.data = SerializeJSON( mp, message.version );
					break;
				}
			}

			br.setNoError();
			br.count = m.Count( mps.guest );

			return br;
		}

		[AcceptVerbs( HttpVerbs.Post )]
		public JsonResult RegCategories( string id )
		{
			string[] a = id.Split( '-' );
			string val = null;
			if( a.Length > 0 ) {
				Organization org = DbUtil.Db.LoadOrganizationById( a[1].ToInt() );
				if( org != null )
					val = org.AppCategory ?? "Other";
			}
			Dictionary<string, string> categories = new Dictionary<string, string>();
			string lines = DbUtil.Db.Content( "AppRegistrations", "Other\tRegistrations" ).TrimEnd();
			Regex re = new Regex( @"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase );
			Match line = re.Match( lines );
			while( line.Success ) {
				string code = line.Groups[1].Value;
				string text = line.Groups[2].Value.TrimEnd();
				categories.Add( code, text );
				line = line.NextMatch();
			}
			if( !categories.ContainsKey( "Other" ) )
				categories.Add( "Other", "Registrations" );
			if( val.HasValue() )
				categories.Add( "selected", val );
			return Json( categories );
		}

		private List<MobileRegistrationCategory> GetRegistrations()
		{
			Dictionary<string, string> categories = new Dictionary<string, string>();
			List<MobileRegistrationCategory> list = new List<MobileRegistrationCategory>();

			DateTime dt = DateTime.Now;
			Regex re = new Regex( @"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase );

			string lines = DbUtil.Db.Content( "AppRegistrations", "Other\tRegistrations" ).TrimEnd();
			Match line = re.Match( lines );

			while( line.Success ) {
				categories.Add( line.Groups[1].Value, line.Groups[2].Value.TrimEnd() );
				line = line.NextMatch();
			}

			if( !categories.ContainsKey( "Other" ) ) {
				categories.Add( "Other", "Registrations" );
			}

			List<MobileRegistration> registrations = (from o in DbUtil.Db.ViewAppRegistrations
																	let sort = o.PublicSortOrder == null || o.PublicSortOrder.Length == 0 ? "10" : o.PublicSortOrder
																	select new MobileRegistration
																	{
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
				List<MobileRegistration> current = (from mm in registrations
																where mm.Category == cat.Key
																where mm.RegStart <= dt
																orderby mm.PublicSortOrder, mm.Description
																select mm).ToList();
				if( current.Count > 0 ) {
					list.Add( new MobileRegistrationCategory()
					{
						Current = true,
						Title = cat.Value,
						Registrations = current
					} );
				}

				List<MobileRegistration> future = (from mm in registrations
																where mm.Category == cat.Key
																where mm.RegStart > dt
																orderby mm.PublicSortOrder, mm.Description
																select mm).ToList();
				if( future.Count > 0 ) {
					list.Add( new MobileRegistrationCategory()
					{
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

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = SerializeJSON( GetRegistrations(), message.version );
			return br;
		}

		[HttpPost]
		public ActionResult FetchPerson( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobileMessage br = new MobileMessage();

			Person person = DbUtil.Db.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				br.setError( (int) MobileMessage.Error.PERSON_NOT_FOUND );
				br.data = "Person not found.";
				return br;
			}

			br.setNoError();
			br.count = 1;

			if( message.device == (int) MobileMessage.Device.ANDROID ) {
				br.data = SerializeJSON( new MobilePerson().populate( person ), message.version );
			} else {
				List<MobilePerson> mp = new List<MobilePerson> {new MobilePerson().populate( person )};
				br.data = SerializeJSON( mp, message.version );
			}

			return br;
		}

		[HttpPost]
		public ActionResult FetchPersonExtended( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobileMessage br = new MobileMessage();

			Person person = DbUtil.Db.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				br.setError( (int) MobileMessage.Error.PERSON_NOT_FOUND );
				br.data = "Person not found.";
				return br;
			}

			br.setNoError();
			br.count = 1;

			if( message.device == (int) MobileMessage.Device.ANDROID ) {
				br.data = SerializeJSON( new MobilePersonExtended().populate( person, message.argBool ), message.version );
			} else {
				List<MobilePersonExtended> mp = new List<MobilePersonExtended> {new MobilePersonExtended().populate( person, message.argBool )};
				br.data = SerializeJSON( mp, message.version );
			}

			return br;
		}

		[HttpPost]
		public ActionResult UpdatePerson( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			Person userPerson = DbUtil.Db.People.FirstOrDefault( p => p.PeopleId == user.PeopleId.Value );

			if( userPerson == null ) {
				return MobileMessage.createErrorReturn( "User not found!" );
			}

			if( userPerson.PositionInFamilyId == PositionInFamily.Child ) {
				return MobileMessage.createErrorReturn( "Childern cannot edit records" );
			}

			if( userPerson.PositionInFamilyId == PositionInFamily.SecondaryAdult && user.PeopleId != message.argInt ) {
				return MobileMessage.createErrorReturn( "Secondary adults can only modify themselves" );
			}

			if( userPerson.PositionInFamilyId == PositionInFamily.PrimaryAdult && userPerson.Family.People.SingleOrDefault( fm => fm.PeopleId == message.argInt ) == null ) {
				return MobileMessage.createErrorReturn( "Person must be in the same family" );
			}

			MobileMessage br = new MobileMessage();

			Person person = DbUtil.Db.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				br.setError( (int) MobileMessage.Error.PERSON_NOT_FOUND );
				br.data = "Person not found.";
				return br;
			}

			List<MobilePostEditField> fields = JsonConvert.DeserializeObject<List<MobilePostEditField>>( message.data );

			List<ChangeDetail> personChangeList = new List<ChangeDetail>();
			List<ChangeDetail> familyChangeList = new List<ChangeDetail>();

			foreach( MobilePostEditField field in fields ) {
				field.updatePerson( person, personChangeList, familyChangeList );
			}

			if( personChangeList.Count > 0 ) {
				person.LogChanges( DbUtil.Db, personChangeList );
			}

			if( familyChangeList.Count > 0 ) {
				person.Family.LogChanges( DbUtil.Db, familyChangeList, person.PeopleId );
			}

			DbUtil.Db.SubmitChanges();

			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult FetchGivingHistory( string data )
		{
			// Authenticate first
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( user.PeopleId != message.argInt ) {
				return BaseMessage.createErrorReturn( "Giving history is not available for other people" );
			}

			BaseMessage br = new BaseMessage();

			Person person = DbUtil.Db.People.SingleOrDefault( p => p.PeopleId == message.argInt );

			if( person == null ) {
				br.setError( BaseMessage.API_ERROR_PERSON_NOT_FOUND );
				br.data = "Person not found.";
				return br;
			}

			int thisYear = DateTime.Now.Year;
			int lastYear = DateTime.Now.Year - 1;

			decimal lastYearTotal = (from c in DbUtil.Db.Contributions
											where c.PeopleId == person.PeopleId
													|| (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
											where !ContributionTypeCode.ReturnedReversedTypes.Contains( c.ContributionTypeId )
											where c.ContributionStatusId == ContributionStatusCode.Recorded
											where c.ContributionDate.Value.Year == lastYear
											orderby c.ContributionDate descending
											select c).AsEnumerable().Sum( c => c.ContributionAmount ?? 0 );

			List<MobileGivingEntry> entries = (from c in DbUtil.Db.Contributions
															let online = c.BundleDetails.Single().BundleHeader.BundleHeaderType.Description.Contains( "Online" )
															where c.PeopleId == person.PeopleId
																	|| (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
															where !ContributionTypeCode.ReturnedReversedTypes.Contains( c.ContributionTypeId )
															where c.ContributionTypeId != ContributionTypeCode.Pledge
															where c.ContributionStatusId == ContributionStatusCode.Recorded
															where c.ContributionDate.Value.Year == thisYear
															orderby c.ContributionDate descending
															select new MobileGivingEntry()
															{
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

			br.data = SerializeJSON( history, message.version );
			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult FetchInvolvement( string data )
		{
			// Authenticate first
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( user.PeopleId != message.argInt ) {
				return BaseMessage.createErrorReturn( "Involvement is not available for other people" );
			}

			bool limitvisibility = user.InRole( "OrgLeadersOnly" ) || !user.InRole( "Access" );
			int[] oids = new int[0];

			if( user.InRole( "OrgLeadersOnly" ) ) {
				oids = DbUtil.Db.GetLeaderOrgIds( user.PeopleId );
			}

			List<MobileInvolvement> orgList = (from om in DbUtil.Db.OrganizationMembers
															let org = om.Organization
															where om.PeopleId == user.PeopleId
															where (om.Pending ?? false) == false
															where oids.Contains( om.OrganizationId ) || !(limitvisibility && om.Organization.SecurityTypeId == 3)
															where org.LimitToRole == null || user.Roles.Contains( org.LimitToRole )
															orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
															select new MobileInvolvement
															{
																name = om.Organization.OrganizationName,
																leader = om.Organization.LeaderName ?? "",
																type = om.MemberType.Description,
																division = om.Organization.Division.Name,
																program = om.Organization.Division.Program.Name,
																@group = om.Organization.OrganizationType.Description ?? "Other",
																enrolledDate = om.EnrollmentDate,
																attendancePercent = (int) (om.AttendPct == null ? 0 : om.AttendPct * 100)
															}).ToList();

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = SerializeJSON( orgList, message.version );
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult FetchImage( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobilePostFetchImage mpfi = JsonConvert.DeserializeObject<MobilePostFetchImage>( message.data );

			MobileMessage br = new MobileMessage();
			if( mpfi.id == 0 ) return br.setData( "The ID for the person cannot be set to zero" );

			br.data = "The picture was not found.";

			Person person = DbUtil.Db.People.SingleOrDefault( pp => pp.PeopleId == mpfi.id );

			if( person == null || person.PictureId == null ) return br;

			Image image = null;

			switch( mpfi.size ) {
				case 0: // 50 x 50
					image = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.ThumbId );
					break;

				case 1: // 120 x 120
					image = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.SmallId );
					break;

				case 2: // 320 x 400
					image = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.MediumId );
					break;

				case 3: // 570 x 800
					image = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.LargeId );
					break;
			}

			if( image == null ) return br;

			br.data = Convert.ToBase64String( image.Bits );
			br.count = 1;
			br.setNoError();

			return br;
		}

		[HttpPost]
		public ActionResult SaveImage( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			MobileMessage br = new MobileMessage();

			byte[] imageBytes = Convert.FromBase64String( message.argString );

			Person person = DbUtil.Db.People.SingleOrDefault( pp => pp.PeopleId == message.argInt );

			if( person != null && person.Picture != null ) {
				// Thumb image
				Image imageDataThumb = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.ThumbId );

				if( imageDataThumb != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataThumb );

				// Small image
				Image imageDataSmall = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.SmallId );

				if( imageDataSmall != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataSmall );

				// Medium image
				Image imageDataMedium = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.MediumId );

				if( imageDataMedium != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataMedium );

				// Large image
				Image imageDataLarge = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.LargeId );

				if( imageDataLarge != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataLarge );

				person.Picture.ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id;
				person.Picture.SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id;
				person.Picture.MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id;
				person.Picture.LargeId = Image.NewImageFromBits( imageBytes ).Id;
			} else {
				Picture newPicture = new Picture
				{
					ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id,
					SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id,
					MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id,
					LargeId = Image.NewImageFromBits( imageBytes ).Id
				};

				if( person != null ) person.Picture = newPicture;
			}

			DbUtil.Db.SubmitChanges();

			person?.LogPictureUpload( DbUtil.Db, user.PeopleId ?? 1 );

			br.setNoError();
			br.data = "Image updated.";
			br.id = message.argInt;
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult SaveFamilyImage( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			MobilePostSaveImage mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>( message.data );

			MobileMessage br = new MobileMessage();

			byte[] imageBytes = Convert.FromBase64String( mpsi.image );

			Family family = DbUtil.Db.Families.SingleOrDefault( pp => pp.FamilyId == mpsi.id );

			if( family != null && family.Picture != null ) {
				// Thumb image
				Image imageDataThumb = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == family.Picture.ThumbId );

				if( imageDataThumb != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataThumb );

				// Small image
				Image imageDataSmall = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == family.Picture.SmallId );

				if( imageDataSmall != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataSmall );

				// Medium image
				Image imageDataMedium = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == family.Picture.MediumId );

				if( imageDataMedium != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataMedium );

				// Large image
				Image imageDataLarge = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == family.Picture.LargeId );

				if( imageDataLarge != null )
					ImageData.DbUtil.Db.Images.DeleteOnSubmit( imageDataLarge );

				family.Picture.ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id;
				family.Picture.SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id;
				family.Picture.MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id;
				family.Picture.LargeId = Image.NewImageFromBits( imageBytes ).Id;
			} else {
				Picture newPicture = new Picture
				{
					ThumbId = Image.NewImageFromBits( imageBytes, 50, 50 ).Id,
					SmallId = Image.NewImageFromBits( imageBytes, 120, 120 ).Id,
					MediumId = Image.NewImageFromBits( imageBytes, 320, 400 ).Id,
					LargeId = Image.NewImageFromBits( imageBytes ).Id
				};

				if( family != null ) family.Picture = newPicture;
			}

			DbUtil.Db.SubmitChanges();

			br.setNoError();
			br.data = "Image updated.";
			br.id = mpsi.id;
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult FetchTasks( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			IQueryable<IncompleteTask> tasks = from t in DbUtil.Db.ViewIncompleteTasks
															orderby t.CreatedOn, t.StatusId, t.OwnerId, t.CoOwnerId
															where t.OwnerId == user.PeopleId || t.CoOwnerId == user.PeopleId
															select t;

			IQueryable<Task> complete = (from c in DbUtil.Db.Tasks
													where c.StatusId == TaskStatusCode.Complete
													where c.OwnerId == user.PeopleId || c.CoOwnerId == user.PeopleId
													orderby c.CreatedOn descending
													select c).Take( 20 );

			MobileMessage br = new MobileMessage();

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

					br.data = SerializeJSON( taskList, message.version );
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

					br.data = SerializeJSON( taskList, message.version );
					break;
				}
			}

			br.count = tasks.Count();
			br.setNoError();
			return br;
		}

		[HttpPost]
		public ActionResult AcceptTask( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			TaskModel.AcceptTask( message.argInt );

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult DeclineTask( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			TaskModel.DeclineTask( message.argInt, message.argString );

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult CompleteTask( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			TaskModel.CompleteTask( message.argInt );

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult FetchCompleteWithContactLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			int contactid = TaskModel.AddCompletedContact( message.argInt, user );

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.data = GetOneTimeLoginLink( $"/Contact2/{contactid}?edit=true&{message.getSourceQueryString()}", user.Username );
			br.count = 1;
			return br;
		}

		[HttpPost]
		public ActionResult FetchCompletedContactLink( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			Task task = (from t in DbUtil.Db.Tasks
							where t.Id == message.argInt
							select t).SingleOrDefault();

			MobileMessage br = new MobileMessage();

			if( task == null || task.CompletedContactId == null ) return br;

			br.setNoError();
			br.data = GetOneTimeLoginLink( $"/Contact2/{task.CompletedContactId}?{message.getSourceQueryString()}", user.Username );
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult FetchOrgs( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) )
				return MobileMessage.createErrorReturn( "Attendance roles is required to take attendance for organizations" );

			int? pid = user.PeopleId;
			int[] oids = DbUtil.Db.GetLeaderOrgIds( pid );

			IQueryable<Organization> q;

			if( user.InRole( "OrgLeadersOnly" ) ) {
				q = from o in DbUtil.Db.Organizations
					where o.LimitToRole == null || user.Roles.Contains( o.LimitToRole )
					where oids.Contains( o.OrganizationId )
					where o.OrganizationStatusId == OrgStatusCode.Active
					select o;
			} else {
				q = from o in DbUtil.Db.Organizations
					where o.LimitToRole == null || user.Roles.Contains( o.LimitToRole )
					where (o.OrganizationMembers.Any( om => om.PeopleId == pid // either a leader, who is not pending / inactive
																		&& (om.Pending ?? false) == false
																		&& (om.MemberTypeId != MemberTypeCode.InActive)
																		&& (om.MemberType.AttendanceTypeId == AttendTypeCode.Leader) )
							|| oids.Contains( o.OrganizationId ))
					// or a leader of a parent org
					where o.OrganizationStatusId == OrgStatusCode.Active
					select o;
			}

			var orgs = from o in q
							//let sc = o.OrgSchedules.FirstOrDefault() // SCHED
							//join sch in DbUtil.Db.OrgSchedules on o.OrganizationId equals sch.OrganizationId
							from sch in DbUtil.Db.ViewOrgSchedules2s.Where( s => o.OrganizationId == s.OrganizationId ).DefaultIfEmpty()
							from mtg in DbUtil.Db.Meetings.Where( m => o.OrganizationId == m.OrganizationId && m.MeetingDate < DateTime.Today.AddDays( 1 ) ).OrderByDescending( m => m.MeetingDate ).Take( 1 ).DefaultIfEmpty()
							orderby sch.SchedDay, sch.SchedTime
							select new OrganizationInfo
							{
								id = o.OrganizationId,
								name = o.OrganizationName,
								time = sch.SchedTime,
								day = sch.SchedDay,
								lastMeetting = mtg.MeetingDate
							};

			MobileMessage br = new MobileMessage();
			List<MobileOrganization> mo = new List<MobileOrganization>();

			br.setNoError();
			br.count = orgs.Count();

			foreach( OrganizationInfo item in orgs ) {
				MobileOrganization org = new MobileOrganization().populate( item );

				mo.Add( org );
			}

			br.data = SerializeJSON( mo, message.version );

			return br;
		}

		[HttpPost]
		public ActionResult FetchOrgRollList( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			// Check Role
			if( !user.InRole( "Attendance" ) )
				return MobileMessage.createErrorReturn( "Attendance roles is required to take attendance for organizations" );

			MobilePostRollList mprl = JsonConvert.DeserializeObject<MobilePostRollList>( message.data );

			int meetingId = DbUtil.Db.CreateMeeting( mprl.id, mprl.datetime );

			Meeting meeting = DbUtil.Db.Meetings.SingleOrDefault( m => m.MeetingId == meetingId );

			bool attendanceBySubGroup = meeting != null && (meeting.Organization.AttendanceBySubGroups ?? false);

			List<RollsheetModel.AttendInfo> people = attendanceBySubGroup
				? RollsheetModel.RollListFilteredBySubgroup( meetingId, mprl.id, mprl.datetime, fromMobile: true )
				: RollsheetModel.RollList( meetingId, mprl.id, mprl.datetime, fromMobile: true );

			MobileRollList mrl = new MobileRollList
			{
				attendees = new List<MobileAttendee>(),
				meetingID = meetingId,
				headcountEnabled = DbUtil.Db.Setting( "RegularMeetingHeadCount", "true" )
			};

			if( meeting != null ) mrl.headcount = meeting.HeadCount ?? 0;

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.id = meetingId;
			br.count = people.Count;

			foreach( RollsheetModel.AttendInfo person in people )
				mrl.attendees.Add( new MobileAttendee().populate( person ) );

			br.data = SerializeJSON( mrl, message.version );
			return br;
		}

		[HttpPost]
		public ActionResult RecordAttend( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			// Check Role
			if( !user.InRole( "Attendance" ) )
				return MobileMessage.createErrorReturn( "Attendance roles is required to take attendance for organizations" );

			MobileMessage dataIn = MobileMessage.createFromString( data );

			MobilePostAttend mpa = JsonConvert.DeserializeObject<MobilePostAttend>( dataIn.data );

			Meeting meeting = DbUtil.Db.Meetings.SingleOrDefault( m => m.OrganizationId == mpa.orgID && m.MeetingDate == mpa.datetime );

			if( meeting == null ) {
				DbUtil.Db.CreateMeeting( mpa.orgID, mpa.datetime );
			}

			Attend.RecordAttend( DbUtil.Db, mpa.peopleID, mpa.orgID, mpa.present, mpa.datetime );

			DbUtil.Db.UpdateMeetingCounters( mpa.orgID );
			DbUtil.LogActivity( $"Mobile RecAtt o:{mpa.orgID} p:{mpa.peopleID} u:{user.PeopleId} a:{mpa.present}" );

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult RecordHeadcount( string data )
		{
			if( DbUtil.Db.Setting( "RegularMeetingHeadCount", "true" ) == "disabled" ) {
				return MobileMessage.createErrorReturn( "Headcounts for meetings are disabled" );
			}

			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			// Check Role
			if( !user.InRole( "Attendance" ) )
				return MobileMessage.createErrorReturn( "Attendance roles is required to take attendance for organizations" );

			MobilePostHeadcount mph = JsonConvert.DeserializeObject<MobilePostHeadcount>( message.data );

			MobileMessage br = new MobileMessage();

			Meeting meeting = DbUtil.Db.Meetings.SingleOrDefault( m => m.OrganizationId == mph.orgID && m.MeetingDate == mph.datetime );

			if( meeting == null ) return br;

			meeting.HeadCount = mph.headcount;

			DbUtil.Db.SubmitChanges();
			DbUtil.LogActivity( $"Mobile Headcount o:{meeting.OrganizationId} m:{meeting.MeetingId} h:{mph.headcount}" );

			br.setNoError();
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult AddPerson( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			if( !user.InRole( "Attendance" ) )
				return MobileMessage.createErrorReturn( "Attendance role is required to take attendance for organizations." );

			MobileMessage dataIn = MobileMessage.createFromString( data );
			MobilePostAddPerson mpap = JsonConvert.DeserializeObject<MobilePostAddPerson>( dataIn.data );
			mpap.clean();

			Person p = new Person
			{
				CreatedDate = DateTime.Now,
				CreatedBy = user.UserId,
				MemberStatusId = MemberStatusCode.JustAdded,
				AddressTypeId = 10,
				FirstName = mpap.firstName,
				LastName = mpap.lastName,
				AltName = mpap.altName,
				Name = ""
			};

			if( mpap.goesBy.Length > 0 )
				p.NickName = mpap.goesBy;

			if( mpap.birthday != null ) {
				p.BirthDay = mpap.birthday.Value.Day;
				p.BirthMonth = mpap.birthday.Value.Month;
				p.BirthYear = mpap.birthday.Value.Year;
			}

			p.GenderId = mpap.genderID;
			p.MaritalStatusId = mpap.maritalStatusID;

			Family f;

			if( mpap.familyID > 0 ) {
				f = DbUtil.Db.Families.First( fam => fam.FamilyId == mpap.familyID );
			} else {
				f = new Family();

				if( mpap.homePhone.Length > 0 )
					f.HomePhone = mpap.homePhone;

				if( mpap.address.Length > 0 )
					f.AddressLineOne = mpap.address;

				if( mpap.address2.Length > 0 )
					f.AddressLineTwo = mpap.address2;

				if( mpap.city.Length > 0 )
					f.CityName = mpap.city;

				if( mpap.state.Length > 0 )
					f.StateCode = mpap.state;

				if( mpap.zipcode.Length > 0 )
					f.ZipCode = mpap.zipcode;

				if( mpap.country.Length > 0 )
					f.CountryName = mpap.country;

				DbUtil.Db.Families.InsertOnSubmit( f );
			}

			f.People.Add( p );

			p.PositionInFamilyId = DbUtil.Db.ComputePositionInFamily( mpap.getAge(), mpap.maritalStatusID == MaritalStatusCode.Married, f.FamilyId ) ?? PositionInFamily.PrimaryAdult;

			p.OriginId = OriginCode.Visit;
			p.FixTitle();

			if( mpap.eMail.Length > 0 && !mpap.eMail.Equal( "na" ) )
				p.EmailAddress = mpap.eMail;

			if( mpap.cellPhone.Length > 0 )
				p.CellPhone = mpap.cellPhone;

			if( mpap.homePhone.Length > 0 )
				p.HomePhone = mpap.homePhone;

			p.MaritalStatusId = mpap.maritalStatusID;
			p.GenderId = mpap.genderID;

			DbUtil.Db.SubmitChanges();

			if( mpap.visitMeeting > 0 ) {
				Meeting meeting = DbUtil.Db.Meetings.Single( mm => mm.MeetingId == mpap.visitMeeting );

				Attend.RecordAttendance( p.PeopleId, mpap.visitMeeting, true );

				DbUtil.Db.UpdateMeetingCounters( mpap.visitMeeting );

				p.CampusId = meeting.Organization.CampusId;
				p.EntryPoint = meeting.Organization.EntryPoint;

				DbUtil.Db.SubmitChanges();
			}

			if( !user.InRole( "OrgLeadersOnly" ) ) {
				Task.AddNewPerson( DbUtil.Db, p.PeopleId );
			} else {
				IEnumerable<Person> np = DbUtil.Db.GetNewPeopleManagers();

				if( np != null ) {
					DbUtil.Db.Email( DbUtil.Db.Setting( "AdminMail", "" ), np, $"Just Added Person on {DbUtil.Db.Host}", $"<a href='{DbUtil.Db.ServerLink( "/Person2/" + p.PeopleId )}'>{p.Name}</a>" );
				}
			}

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.id = p.PeopleId;
			br.count = 1;

			return br;
		}

		[HttpPost]
		public ActionResult JoinOrg( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobileAuthentication authentication = new MobileAuthentication();
			authentication.authenticate( message.instance );

			if( authentication.hasError() ) {
				return MobileMessage.createLoginErrorReturn( authentication );
			}

			User user = authentication.getUser();

			// Check Role
			if( !user.InRole( "Attendance" ) )
				return MobileMessage.createErrorReturn( "Attendance or Checkin role is required to take attendance for organizations." );

			MobileMessage dataIn = MobileMessage.createFromString( data );
			MobilePostJoinOrg mpjo = JsonConvert.DeserializeObject<MobilePostJoinOrg>( dataIn.data );

			OrganizationMember om = DbUtil.Db.OrganizationMembers.SingleOrDefault( m => m.PeopleId == mpjo.peopleID && m.OrganizationId == mpjo.orgID );

			if( om == null && mpjo.join )
				om = OrganizationMember.InsertOrgMembers( DbUtil.Db, mpjo.orgID, mpjo.peopleID, MemberTypeCode.Member, DateTime.Today );

			if( om != null && !mpjo.join ) {
				om.Drop( DbUtil.Db, DateTime.Now );

				DbUtil.LogActivity( $"Dropped {om.PeopleId} for {om.Organization.OrganizationId} via {dataIn.getSourceOS()} app", peopleid: om.PeopleId, orgid: om.OrganizationId );
			}

			DbUtil.Db.SubmitChanges();

			MobileMessage br = new MobileMessage();
			br.setNoError();
			br.count = 1;

			return br;
		}

		public ActionResult SystemLists( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			MobilePersonCreateLists allLists = new MobilePersonCreateLists();

			allLists.campuses = (from e in DbUtil.Db.Campus
										orderby e.Description
										select new MobileCampus
										{
											id = e.Id,
											name = e.Description
										}).ToList();

			allLists.countries = (from e in DbUtil.Db.Countries
										orderby e.Id
										select new MobileCountry
										{
											id = e.Id,
											code = e.Code,
											description = e.Description
										}).ToList();

			allLists.states = (from e in DbUtil.Db.StateLookups
									orderby e.StateCode
									select new MobileState
									{
										code = e.StateCode,
										name = e.StateName
									}).ToList();

			allLists.maritalStatuses = (from e in DbUtil.Db.MaritalStatuses
												orderby e.Id
												select new MobileMaritalStatus
												{
													id = e.Id,
													code = e.Code,
													description = e.Description
												}).ToList();

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.count = 3;
			br.data = JsonConvert.SerializeObject( allLists );

			return br;
		}

		public ActionResult MapInfo( string data )
		{
			MobileMessage message = MobileMessage.createFromString( data );

			IQueryable<MobileCampus> campuses = from p in DbUtil.Db.MobileAppBuildings
															where p.Enabled
															orderby p.Order
															select new MobileCampus
															{
																id = p.Id,
																name = p.Name
															};

			List<MobileCampus> campusList = campuses.ToList();

			foreach( MobileCampus campus in campusList ) {
				IQueryable<MobileFloor> floors = from p in DbUtil.Db.MobileAppFloors
															where p.Enabled
															where p.Campus == campus.id
															orderby p.Order
															select new MobileFloor
															{
																id = p.Id,
																name = p.Name,
																image = p.Image,
															};

				List<MobileFloor> floorList = floors.ToList();

				foreach( MobileFloor floor in floorList ) {
					IQueryable<MobileRoom> rooms = from p in DbUtil.Db.MobileAppRooms
															where p.Enabled
															where p.Floor == floor.id
															orderby p.Room
															select new MobileRoom
															{
																name = p.Name,
																room = p.Room,
																x = p.X,
																y = p.Y
															};

					floor.rooms = rooms.ToList();
				}

				campus.floors = floorList;
			}

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.count = campusList.Count();
			br.data = JsonConvert.SerializeObject( campusList );

			return br;
		}

		private static OneTimeLink GetOneTimeLink( int orgId, int peopleId )
		{
			return new OneTimeLink
			{
				Id = Guid.NewGuid(),
				Querystring = $"{orgId},{peopleId},0",
				Expires = DateTime.Now.AddMinutes( 10 ),
			};
		}

		private static string GetOneTimeLoginLink( string url, string user )
		{
			OneTimeLink oneTimeLink = new OneTimeLink
			{
				Id = Guid.NewGuid(),
				Querystring = user,
				Expires = DateTime.Now.AddMinutes( 15 )
			};

			DbUtil.Db.OneTimeLinks.InsertOnSubmit( oneTimeLink );
			DbUtil.Db.SubmitChanges();

			return $"{DbUtil.Db.ServerLink( $"Logon?ReturnUrl={HttpUtility.UrlEncode( url )}&otltoken={oneTimeLink.Id.ToCode()}" )}";
		}

		// ReSharper disable once UnusedParameter.Local
		private static string SerializeJSON( object item, int version )
		{
			return JsonConvert.SerializeObject( item, new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss"} );
		}
	}
}
