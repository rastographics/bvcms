using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.Classes.Barcodes;
using CmsWeb.Areas.Public.Models.CheckInAPIv2;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Results;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Searches;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using ImageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UtilityExtensions;
using Country = CmsWeb.Areas.Public.Models.CheckInAPIv2.Country;
using Family = CmsWeb.Areas.Public.Models.CheckInAPIv2.Family;
using Gender = CmsWeb.Areas.Public.Models.CheckInAPIv2.Gender;
using MaritalStatus = CmsWeb.Areas.Public.Models.CheckInAPIv2.MaritalStatus;

namespace CmsWeb.Areas.Public.Controllers
{
	public class CheckInAPIv2Controller : CMSBaseController
	{
		public CheckInAPIv2Controller( IRequestManager requestManager ) : base( requestManager ) { }

		public ActionResult Exists()
		{
			return Content( "1" );
		}

		private static bool Auth()
		{
			CMSDataContext db = CMSDataContext.Create( HttpContextFactory.Current );
			CMSImageDataContext idb = CMSImageDataContext.Create( HttpContextFactory.Current );

			return AccountModel.AuthenticateMobile( db, idb, "Checkin" ).IsValid;
		}

		public ActionResult Authenticate( string data )
		{
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			List<SettingsEntry> settings = (from s in CurrentDatabase.CheckInSettings
														where s.Version == 2
														select new SettingsEntry {
															id = s.Id,
															name = s.Name,
															settings = s.Settings,
															version = s.Version
														}).ToList();

			List<State> states = (from s in CurrentDatabase.StateLookups
										orderby s.StateName
										select new State {
											code = s.StateCode,
											name = s.StateName
										}).ToList();

			List<Country> countries = (from c in CurrentDatabase.Countries
												orderby c.Id
												select new Country {
													id = c.Id,
													code = c.Code,
													name = c.Description
												}).ToList();

			List<Campus> campuses = (from c in CurrentDatabase.Campus
											where c.Organizations.Any( o => o.CanSelfCheckin.Value )
											orderby c.Id
											select new Campus {
												id = c.Id,
												name = c.Description
											}).ToList();

			campuses.Insert( 0, new Campus {
				id = 0,
				name = "All Campuses"
			} );

			List<Gender> genders = (from g in CurrentDatabase.Genders
											orderby g.Id
											select new Gender {
												id = g.Id,
												name = g.Description
											}).ToList();

			List<MaritalStatus> maritalStatuses = (from m in CurrentDatabase.MaritalStatuses
																orderby m.Id
																select new MaritalStatus {
																	id = m.Id,
																	code = m.Code,
																	name = m.Description
																}).ToList();

			Information information = new Information {
				userID = Util.UserId,
				userName = Util.UserFullName,
				settings = settings,
				states = states,
				countries = countries,
				campuses = campuses,
				genders = genders,
				maritals = maritalStatuses
			};

			Message response = new Message();
			response.setNoError();
			response.data = JsonConvert.SerializeObject( information );

			return response;
		}

		[HttpPost]
		public ActionResult Search( string data )
		{
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			NumberSearch cns = JsonConvert.DeserializeObject<NumberSearch>( message.data );

			DbUtil.LogActivity( "Check-In Number Search: " + cns.search );

            Message response = new Message();
            response.setNoError();

            if (cns.search.Contains("!"))
            {
                var list = cns.search.Split('!').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                string id = list.First();
                var pending = CurrentDatabase.CheckInPendings.Where(p => p.Id == id).SingleOrDefault();
                if (pending != null)
                {
                    return UpdateAttend(pending.Data);
                }
                else
                {
                    return Message.createErrorReturn("Invalid barcode.", Message.API_ERROR_PERSON_NOT_FOUND);
                }
            }

            bool returnPictureUrls = message.device == Message.API_DEVICE_WEB;
            List<Family> families = Family.forSearch(CurrentDatabase, CurrentImageDatabase, cns.search, cns.campus, cns.date, returnPictureUrls);
            response.data = SerializeJSON(families, message.version);
            
			return response;
		}
        
		[HttpGet]
		public ActionResult GetProfiles()
		{
			List<Profile> profiles = new List<Profile>();
			List<CheckinProfileSettings> profileSettings = CurrentDatabase.CheckinProfileSettings.ToList();

			foreach( CheckinProfileSettings settings in profileSettings ) {
				Profile profile = new Profile();
				profile.populate( settings );

				profiles.Add( profile );
			}

			return Json( profiles, JsonRequestBehavior.AllowGet );
		}

		[HttpPost]
		public ActionResult GetSubgroups( string data )
		{
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			SubgroupList sgl = JsonConvert.DeserializeObject<SubgroupList>( message.data );

			Message response = new Message();
			response.setNoError();

			using( SqlConnection db = new SqlConnection( Util.ConnectionString ) ) {
				List<Subgroup> subgroups = Subgroup.forGroupID( db, sgl.groupID, sgl.peopleID, sgl.scheduleID, sgl.meetingDate );

				response.data = SerializeJSON( subgroups, message.version );
			}

			return response;
		}

		[HttpPost]
		public ActionResult GetPerson( string data )
		{
			// Authenticate first
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );

			CmsData.Person p = CurrentDatabase.LoadPersonById( message.id );

			if( p == null ) {
				return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );
			}

			Models.CheckInAPIv2.Person person = new Models.CheckInAPIv2.Person();
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
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			Models.CheckInAPIv2.Person person = JsonConvert.DeserializeObject<Models.CheckInAPIv2.Person>( message.data );
			person.clean();

			// Create or Edit Family
			CmsData.Family f = person.familyID > 0 ? CurrentDatabase.Families.First( fam => fam.FamilyId == person.familyID ) : new CmsData.Family();
			person.fillFamily( f );

			if( person.familyID == 0 ) {
				CurrentDatabase.Families.InsertOnSubmit( f );
			}

			// Create Person
			CmsData.Person p = new CmsData.Person {
				CreatedDate = Util.Now,
				CreatedBy = Util.UserId,
				MemberStatusId = MemberStatusCode.JustAdded,
				AddressTypeId = 10,
				OriginId = OriginCode.Visit,
				EntryPoint = getCheckInEntryPointID(),
				CampusId = person.campus > 0 ? person.campus : (int?) null,
				Name = ""
			};

			person.fillPerson( p );

			// Calculate position before submitting changes so they aren't part of the calculation
			using( SqlConnection db = new SqlConnection( Util.ConnectionString ) ) {
				p.PositionInFamilyId = person.computePositionInFamily( db );
			}

			// p.PositionInFamilyId = CurrentDatabase.ComputePositionInFamily( person.getAge(),
			// 																						person.maritalStatusID == CmsData.Codes.MaritalStatusCode.Married, f.FamilyId ) ?? CmsData.Codes.PositionInFamily.PrimaryAdult;

			f.People.Add( p );

			CurrentDatabase.SubmitChanges();

			AddEditPersonResults results = new AddEditPersonResults {
				familyID = f.FamilyId,
				peopleID = p.PeopleId,
				positionID = p.PositionInFamilyId
			};

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
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			Models.CheckInAPIv2.Person person = JsonConvert.DeserializeObject<Models.CheckInAPIv2.Person>( message.data );
			person.clean();

			CmsData.Person p = CurrentDatabase.LoadPersonById( person.id );

			if( p == null ) {
				return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );
			}

			CmsData.Family f = CurrentDatabase.Families.First( fam => fam.FamilyId == p.FamilyId );

			person.fillPerson( p );
			person.fillFamily( f );

			CurrentDatabase.SubmitChanges();

			AddEditPersonResults results = new AddEditPersonResults {
				familyID = f.FamilyId,
				peopleID = p.PeopleId,
				positionID = p.PositionInFamilyId
			};

			Message response = new Message();
			response.setNoError();
			response.count = 1;
			response.data = SerializeJSON( results );

			return response;
		}

		private EntryPoint getCheckInEntryPointID()
		{
			EntryPoint checkInEntryPoint = (from e in CurrentDatabase.EntryPoints
														where e.Code == "CHECKIN"
														select e).FirstOrDefault();

			if( checkInEntryPoint != null ) {
				return checkInEntryPoint;
			} else {
				int maxEntryPointID = CurrentDatabase.EntryPoints.Max( e => e.Id );

				EntryPoint entry = new EntryPoint {
					Id = maxEntryPointID + 1,
					Code = "CHECKIN",
					Description = "Check-In",
					Hardwired = true
				};

				CurrentDatabase.EntryPoints.InsertOnSubmit( entry );
				CurrentDatabase.SubmitChanges();

				return entry;
			}
		}

        [HttpPost]
		public ActionResult PendingCheckIn( string data )
		{
			// Authenticate first
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}
            
            Message response = new Message();
			Message message = Message.createFromString( data );

            var nextId = CurrentDatabase.CheckInPendings.Max(c => c.Id) + 1;
            var pending = new CheckInPending
            {
                Id = nextId,
                Stamp = DateTime.Now,
                Data = message.data
            };

            CurrentDatabase.CheckInPendings.InsertOnSubmit(pending);
            CurrentDatabase.SubmitChanges();
            
            response.setNoError();
            response.count = 1;

            string qrCode = Convert.ToBase64String(BarcodeHelper.generateQRCode("!" + pending.Id, 300));

            response.data = qrCode;
            return response;
		}

		[HttpPost]
		public ActionResult UpdateAttend( string data )
		{
			// Authenticate first
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			Message response = new Message();

			AttendanceBundle bundle = JsonConvert.DeserializeObject<AttendanceBundle>( message.data );
			bundle.recordAttendance( CurrentDatabase );

			if( message.device == Message.API_DEVICE_WEB ) {
				string bundleData = SerializeJSON( bundle );

				CheckInModel checkInModel = new CheckInModel();
				checkInModel.SavePrintJob( message.kiosk, null, bundleData );

				response.setNoError();
				response.count = 1;
				response.data = bundleData;
			} else {
				response.setNoError();
				response.count = 1;
				response.data = SerializeJSON( bundle.createLabelData( CurrentDatabase ) );
			}

			return response;
		}

		public ActionResult PrintJobs( string data )
		{
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			Message response = new Message();

			string[] kiosks = message.getArgStringAsArray( "," );

			List<PrintJob> printJobs = (from label in CurrentDatabase.PrintJobs
												where kiosks.Contains( label.Id )
												select label).ToList();

			List<List<Label>> labels = new List<List<Label>>();

			foreach( PrintJob printJob in printJobs ) {
				AttendanceBundle attendanceBundle = JsonConvert.DeserializeObject<AttendanceBundle>( printJob.JsonData );
				attendanceBundle.labelSize = message.argInt;

				List<Label> labelGroup = attendanceBundle.createLabelData( CurrentDatabase );

				labels.Add( labelGroup );
			}

			response.setNoError();
			response.count = labels.Count;
			response.data = SerializeJSON( labels );

			return response;
		}

		[HttpPost]
		public ActionResult GroupSearch( string data )
		{
			if( !Auth() ) {
				return Message.createErrorReturn( "Authentication failed, please try again", Message.API_ERROR_INVALID_CREDENTIALS );
			}

			Message message = Message.createFromString( data );
			GroupSearch search = JsonConvert.DeserializeObject<GroupSearch>( message.data );

			CmsData.Person person = (from p in CurrentDatabase.People
											where p.PeopleId == search.peopleID
											select p).SingleOrDefault();

			if( person == null ) {
				return Message.createErrorReturn( "Person not found", Message.API_ERROR_PERSON_NOT_FOUND );
			}

			DbUtil.LogActivity( $"Check-In Group Search: {person.PeopleId}: {person.Name}" );

			List<Group> groups;

			using( SqlConnection db = new SqlConnection( Util.ConnectionString ) ) {
				groups = Group.forGroupFinder( db, person.BirthDate, search.campusID, search.dayID, search.showAll ? 1 : 0 );
			}

			Message response = new Message();
			response.setNoError();
			response.data = SerializeJSON( groups );

			return response;
		}

		// Version for future API changes
		[SuppressMessage( "ReSharper", "UnusedParameter.Local" )]
		private static string SerializeJSON( object item, int version = 0 )
		{
			return JsonConvert.SerializeObject( item, new IsoDateTimeConverter {
				DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss"
			} );
		}
	}
}
