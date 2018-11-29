using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Areas.Public.Models.CheckInAPIv2;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Results;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Searches;
using CmsWeb.Models;
using Microsoft.Scripting.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
	public class CheckInAPIv2Controller : Controller
	{
		public ActionResult Exists()
		{
			// CheckInMessage br = new CheckInMessage();
			// br.setNoError();
			//
			// using( var db = new SqlConnection( Util.ConnectionString ) ) {
			// 	List<CheckInFamily> families = CheckInFamily.forSearch( db, "2407179", 0, new DateTime( 2018, 6, 10 ) );
			//
			// 	br.data = SerializeJSON( families, 0 );
			// }
			//
			// return br;

			return Content( "1" );
		}

		private static bool Auth()
		{
			return AccountModel.AuthenticateMobile( "Checkin" ).IsValid;
		}

		public ActionResult Authenticate( string data )
		{
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			List<SettingsEntry> settings = (from s in CmsData.DbUtil.Db.CheckInSettings
														where s.Version == 2
														select new SettingsEntry
														{
															id = s.Id,
															name = s.Name,
															settings = s.Settings,
															version = s.Version
														}).ToList();

			List<State> states = (from s in CmsData.DbUtil.Db.StateLookups
										orderby s.StateName
										select new State
										{
											code = s.StateCode,
											name = s.StateName
										}).ToList();

			List<Country> countries = (from c in CmsData.DbUtil.Db.Countries
												orderby c.Id
												select new Country
												{
													id = c.Id,
													code = c.Code,
													name = c.Description
												}).ToList();

			List<Campus> campuses = (from c in CmsData.DbUtil.Db.Campus
											where c.Organizations.Any( o => o.CanSelfCheckin.Value )
											orderby c.Id
											select new Campus
											{
												id = c.Id,
												name = c.Description
											}).ToList();

			campuses.Insert( 0, new Campus {id = 0, name = "All Campuses"} );

			List<Gender> genders = (from g in CmsData.DbUtil.Db.Genders
											orderby g.Id
											select new Gender
											{
												id = g.Id,
												name = g.Description
											}).ToList();

			List<MaritalStatus> maritals = (from m in CmsData.DbUtil.Db.MaritalStatuses
														orderby m.Id
														select new MaritalStatus
														{
															id = m.Id,
															code = m.Code,
															name = m.Description
														}).ToList();

			Information information = new Information
			{
				userID = Util.UserId,
				userName = Util.UserFullName,
				settings = settings,
				states = states,
				countries = countries,
				campuses = campuses,
				genders = genders,
				maritals = maritals
			};

			Message response = new Message();
			response.setNoError();
			response.data = JsonConvert.SerializeObject( information );

			return response;
		}

		[HttpPost]
		public ActionResult Search( string data )
		{
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );
			NumberSearch cns = JsonConvert.DeserializeObject<NumberSearch>( message.data );

			CmsData.DbUtil.LogActivity( "Check-In Number Search: " + cns.search );

			Message response = new Message();
			response.setNoError();

			using( var db = new SqlConnection( Util.ConnectionString ) ) {
				List<Family> families = Family.forSearch( db, cns.search, cns.campus, cns.date );

				response.data = SerializeJSON( families, message.version );
			}

			return response;
		}

		[HttpPost]
		public ActionResult GetSubgroups( string data )
		{
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );
			SubgroupList sgl = JsonConvert.DeserializeObject<SubgroupList>( message.data );

			Message response = new Message();
			response.setNoError();

			using( var db = new SqlConnection( Util.ConnectionString ) ) {
				List<Subgroup> subgroups = Subgroup.forGroupID( db, sgl.groupID, sgl.peopleID, sgl.scheduleID, sgl.meetingDate );

				response.data = SerializeJSON( subgroups, message.version );
			}

			return response;
		}

		[HttpPost]
		public ActionResult GetPerson( string data )
		{
			// Authenticate first
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );

			CmsData.Person p = CmsData.DbUtil.Db.LoadPersonById( message.id );

			if( p == null ) return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );

			Person person = new Person();
			person.populate( p );

			Message response = new Message();
			response.setNoError();
			response.count = 1;
			response.data = SerializeJSON( person );

			return response;
		}

		[HttpPost]
		public ActionResult AddPerson( string data )
		{
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );
			Person person = JsonConvert.DeserializeObject<Person>( message.data );
			person.clean();

			// Create or Edit Family
			CmsData.Family f = person.familyID > 0 ? CmsData.DbUtil.Db.Families.First( fam => fam.FamilyId == person.familyID ) : new CmsData.Family();
			person.fillFamily( f );

			if( person.familyID == 0 ) {
				CmsData.DbUtil.Db.Families.InsertOnSubmit( f );
			}

			// Create Person
			CmsData.Person p = new CmsData.Person();
			p.CreatedDate = Util.Now;
			p.CreatedBy = Util.UserId;
			p.MemberStatusId = CmsData.Codes.MemberStatusCode.JustAdded;
			p.AddressTypeId = 10;
			p.OriginId = CmsData.Codes.OriginCode.Visit;
			p.EntryPoint = getCheckInEntryPointID();
			p.CampusId = person.campus > 0 ? person.campus : (int?) null;
			p.Name = "";

			person.fillPerson( p );

			// Calculate position before submitting changes so they aren't part of the calculation
			using( var db = new SqlConnection( Util.ConnectionString ) ) {
				p.PositionInFamilyId = person.computePositionInFamily( db );
			}

			// p.PositionInFamilyId = CmsData.DbUtil.Db.ComputePositionInFamily( person.getAge(),
			// 																						person.maritalStatusID == CmsData.Codes.MaritalStatusCode.Married, f.FamilyId ) ?? CmsData.Codes.PositionInFamily.PrimaryAdult;

			f.People.Add( p );

			CmsData.DbUtil.Db.SubmitChanges();

			AddEditPersonResults results = new AddEditPersonResults();
			results.familyID = f.FamilyId;
			results.peopleID = p.PeopleId;
			results.positionID = p.PositionInFamilyId;

			Message response = new Message();
			response.setNoError();
			response.count = 1;
			response.data = SerializeJSON( results );

			return response;
		}

		[HttpPost]
		public ActionResult EditPerson( string data )
		{
			// Authenticate first
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );
			Person person = JsonConvert.DeserializeObject<Person>( message.data );
			person.clean();

			CmsData.Person p = CmsData.DbUtil.Db.LoadPersonById( person.id );

			if( p == null ) return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );

			CmsData.Family f = CmsData.DbUtil.Db.Families.First( fam => fam.FamilyId == p.FamilyId );

			person.fillPerson( p );
			person.fillFamily( f );

			CmsData.DbUtil.Db.SubmitChanges();

			AddEditPersonResults results = new AddEditPersonResults();
			results.familyID = f.FamilyId;
			results.peopleID = p.PeopleId;
			results.positionID = p.PositionInFamilyId;

			Message response = new Message();
			response.setNoError();
			response.count = 1;
			response.data = SerializeJSON( results );

			return response;
		}

		private static CmsData.EntryPoint getCheckInEntryPointID()
		{
			CmsData.EntryPoint checkInEntryPoint = (from e in CmsData.DbUtil.Db.EntryPoints
																where e.Code == "CHECKIN"
																select e).FirstOrDefault();

			if( checkInEntryPoint != null ) {
				return checkInEntryPoint;
			} else {
				int maxEntryPointID = CmsData.DbUtil.Db.EntryPoints.Max( e => e.Id );

				CmsData.EntryPoint entry = new CmsData.EntryPoint
				{
					Id = maxEntryPointID + 1,
					Code = "CHECKIN",
					Description = "Check-In",
					Hardwired = true
				};

				CmsData.DbUtil.Db.EntryPoints.InsertOnSubmit( entry );
				CmsData.DbUtil.Db.SubmitChanges();

				return entry;
			}
		}

		[HttpPost]
		public ActionResult Barcode( string data )
		{
			// Authenticate first
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );

			CmsData.Person p = CmsData.DbUtil.Db.LoadPersonById( message.id );

			if( p == null ) return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );

			Message response = new Message();

			// argBool: True = set, False = get
			if( message.argBool ) {
				CmsData.PeopleExtra extra = p.GetExtraValue( "PIN" );
				extra.Data = message.data;

				CmsData.DbUtil.Db.SubmitChanges();

				response.setNoError();
				response.count = 1;
			} else {
				response.setNoError();
				response.count = 1;
				response.data = p.GetExtra( "PIN" );
			}

			return response;
		}

		[HttpPost]
		public ActionResult UpdateAttend( string data )
		{
			// Authenticate first
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );
            AttendanceBundle bundle = JsonConvert.DeserializeObject<AttendanceBundle>( message.data );

			foreach( Attendance attendance in bundle.attendances ) {
				foreach( AttendanceGroup group in attendance.groups ) {
					CmsData.Attend.RecordAttend( CmsData.DbUtil.Db, attendance.peopleID, group.groupID, group.present, group.datetime );

					CmsData.Attend attend = CmsData.DbUtil.Db.Attends.FirstOrDefault( a => a.PeopleId == attendance.peopleID && a.OrganizationId == group.groupID && a.MeetingDate == group.datetime );

					if( attend == null ) continue;

					if( group.present ) {
						attend.SubGroupID = group.subgroupID;
						attend.SubGroupName = group.subgroupName;
					} else {
						attend.SubGroupID = 0;
						attend.SubGroupName = "";
					}

					if( group.join ) {
						JoinToOrg( attendance.peopleID, group.groupID );
					}
				}

				CmsData.DbUtil.Db.SubmitChanges();
			}

			List<Label> labels = new List<Label>();
			string securityCode = CmsData.DbUtil.Db.NextSecurityCode().Select( c => c.Code ).Single().Trim();

			using( var db = new SqlConnection( Util.ConnectionString ) ) {
				Dictionary<int, LabelFormat> formats = LabelFormat.forSize( db, bundle.labelSize );

				foreach( Attendance attendance in bundle.attendances ) {
					attendance.load();
					attendance.labelSecurityCode = securityCode;

					labels.AddRange( attendance.getLabels( formats, bundle.securityLabels, bundle.guestLabels, bundle.locationLabels, bundle.nameTagAge ) );
				}

				if( labels.Count > 0 && bundle.attendances.Count > 0 && bundle.securityLabels == Attendance.SECURITY_LABELS_PER_FAMILY ) {
					labels.AddRange( bundle.attendances[0].getSecurityLabel( formats ) );
				}
			}

			Message response = new Message();
			response.setNoError();
			response.count = 1;
			response.data = SerializeJSON( labels );

			return response;
		}

		private void JoinToOrg( int peopleID, int orgID )
		{
			CmsData.OrganizationMember om = CmsData.DbUtil.Db.OrganizationMembers.SingleOrDefault( m => m.PeopleId == peopleID && m.OrganizationId == orgID );

			if( om == null ) {
				om = CmsData.OrganizationMember.InsertOrgMembers( CmsData.DbUtil.Db, orgID, peopleID, CmsData.Codes.MemberTypeCode.Member, DateTime.Today );

				CmsData.DbUtil.LogActivity( $"Joined {om.PeopleId} to {om.Organization.OrganizationId} via Check-In desktop client", peopleid: om.PeopleId, orgid: om.OrganizationId );

				CmsData.DbUtil.Db.SubmitChanges();

				// Check Entry Point and replace if Check-In
				CmsData.Person person = CmsData.DbUtil.Db.People.FirstOrDefault( p => p.PeopleId == peopleID );

				if( person?.EntryPoint != null && person.EntryPoint.Code == "CHECKIN" ) {
					person.EntryPoint = om.Organization.EntryPoint;

					CmsData.DbUtil.Db.SubmitChanges();
				}
			}
		}

		[HttpPost]
		public ActionResult GroupSearch( string data )
		{
			if( !Auth() )
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );

			Message message = Message.createFromString( data );
			GroupSearch search = JsonConvert.DeserializeObject<GroupSearch>( message.data );

			CmsData.Person person = (from p in CmsData.DbUtil.Db.People
											where p.PeopleId == search.peopleID
											select p).SingleOrDefault();

			if( person == null )
				return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );

			CmsData.DbUtil.LogActivity( $"Check-In Group Search: {person.PeopleId}: {person.Name}" );

			List<Group> groups;

			using( var db = new SqlConnection( Util.ConnectionString ) ) {
				groups = Group.forGroupFinder( db, person.BirthDate, search.campusID, search.dayID, search.showAll ? 1 : 0 );
			}

			// List<Group> groups = (from org in CmsData.DbUtil.Db.Organizations
			// 							from schedule in CmsData.DbUtil.Db.OrgSchedules.Where( sc => sc.OrganizationId == org.OrganizationId ).DefaultIfEmpty()
			// 							let birthdayStart = org.BirthDayStart ?? DateTime.MaxValue
			// 							where (org.SuspendCheckin ?? false) == false || search.showAll
			// 							where person.BirthDate == null || person.BirthDate <= org.BirthDayEnd || org.BirthDayEnd == null || search.showAll
			// 							where person.BirthDate == null || person.BirthDate >= org.BirthDayStart || org.BirthDayStart == null || search.showAll
			// 							where org.CanSelfCheckin != null && org.CanSelfCheckin == true
			// 							where (org.ClassFilled ?? false) == false
			// 							where (org.CampusId == null && org.AllowNonCampusCheckIn == true) || org.CampusId == search.campusID || search.campusID == 0
			// 							where org.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
			// 							orderby schedule.SchedTime.Value.TimeOfDay, birthdayStart, org.OrganizationName
			// 							select new Group
			// 							{
			// 								id = org.OrganizationId,
			// 								leaderName = org.LeaderName ?? "",
			// 								name = org.OrganizationName ?? "",
			// 								date = schedule.SchedTime,
			// 								scheduleID = schedule.Id,
			// 								birthdayStart = org.BirthDayStart,
			// 								birthdayEnd = org.BirthDayEnd,
			// 								location = org.Location ?? "",
			// 								allowOverlap = org.AllowAttendOverlap
			// 							}).ToList();

			Message response = new Message();
			response.setNoError();
			response.data = SerializeJSON( groups );

			return response;
		}

		// Version for future API changes
		[SuppressMessage( "ReSharper", "UnusedParameter.Local" )]
		private static string SerializeJSON( object item, int version = 0 )
		{
			return JsonConvert.SerializeObject( item, new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss"} );
		}
	}
}
