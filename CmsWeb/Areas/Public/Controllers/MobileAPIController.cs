using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Models.iPhone;
using CmsWeb.MobileAPI;
using UtilityExtensions;
using Newtonsoft.Json;
using CmsWeb.Models;
using CmsData.Codes;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIController : Controller
	{
		private ActionResult Authenticate()
		{
			if (CmsWeb.Models.AccountModel.AuthenticateMobile()) return null;
			else
			{
				return BaseReturn.createErrorReturn("You are not authorized!");
			}
		}

		public ActionResult CheckLogin()
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			br.error = 0;
			br.id = Util.UserPeopleId ?? 0;

			List<MobilePerson> mp = new List<MobilePerson>();

			var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == Util.UserPeopleId);

			if (person != null)
			{
				mp.Add(new MobilePerson().populate(person));
				br.data = JsonConvert.SerializeObject(mp);
				br.count = 1;
			}

			return br;
		}

		public ActionResult SearchPeople(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_PEOPLE_SEARCH)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostSearch mps = JsonConvert.DeserializeObject<MobilePostSearch>(dataIn.data);

			BaseReturn br = new BaseReturn();
			List<MobilePerson> mp = new List<MobilePerson>();

			var m = new CmsWeb.Models.iPhone.SearchModel(mps.name, mps.comm, mps.addr);

			br.error = 0;
			br.type = BaseReturn.API_TYPE_PEOPLE_SEARCH;
			br.count = m.Count;

			foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(20))
			{
				mp.Add(new MobilePerson().populate(item));
			}

			br.data = JsonConvert.SerializeObject(mp);
			return br;
		}

		public ActionResult FetchPerson(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_PERSON_REFRESH)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostFetchPerson mpfs = JsonConvert.DeserializeObject<MobilePostFetchPerson>(dataIn.data);

			BaseReturn br = new BaseReturn();
			List<MobilePerson> mp = new List<MobilePerson>();

			var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == mpfs.id);

			if (person == null)
			{
				br.error = 1;
				br.data = "Person not found.";
				return br;
			}

			br.error = 0;
			br.type = BaseReturn.API_TYPE_PERSON_REFRESH;
			br.count = 1;

			mp.Add(new MobilePerson().populate(person));

			br.data = JsonConvert.SerializeObject(mp);

			return br;
		}

		public ActionResult FetchImage(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_PERSON_IMAGE_REFRESH)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostFetchImage mpfi = JsonConvert.DeserializeObject<MobilePostFetchImage>(dataIn.data);

			BaseReturn br = new BaseReturn();
			if (mpfi.id == 0) return br.setData("The ID for the person cannot be set to zero");

			br.data = "The picture was not found.";
			br.type = BaseReturn.API_TYPE_PERSON_IMAGE_REFRESH;

			var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == mpfi.id);

			if (person.PictureId != null)
			{
				ImageData.Image image = null;

				switch (mpfi.size)
				{
					case 0: // 50 x 50
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId);
						break;

					case 1: // 120 x 120
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);
						break;

					case 2: // 320 x 400
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId);
						break;

					case 3: // 570 x 800
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId);
						break;

				}

				if (image != null)
				{
					br.data = Convert.ToBase64String(image.Bits);
					br.count = 1;
					br.error = 0;
				}
			}

			return br;
		}

		public ActionResult SaveImage(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_PERSON_IMAGE_ADD)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostSaveImage mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>(dataIn.data);

			BaseReturn br = new BaseReturn();

			var imageBytes = Convert.FromBase64String(mpsi.image);

			var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == mpsi.id);

			if (person.Picture != null)
			{
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.ThumbId).SingleOrDefault());
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.SmallId).SingleOrDefault());
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.MediumId).SingleOrDefault());
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.LargeId).SingleOrDefault());

				person.Picture.ThumbId = ImageData.Image.NewImageFromBits(imageBytes, 50, 50).Id;
				person.Picture.SmallId = ImageData.Image.NewImageFromBits(imageBytes, 120, 120).Id;
				person.Picture.MediumId = ImageData.Image.NewImageFromBits(imageBytes, 320, 400).Id;
				person.Picture.LargeId = ImageData.Image.NewImageFromBits(imageBytes, 570, 800).Id;
			}
			else
			{
				var newPicture = new Picture();

				newPicture.ThumbId = ImageData.Image.NewImageFromBits(imageBytes, 50, 50).Id;
				newPicture.SmallId = ImageData.Image.NewImageFromBits(imageBytes, 120, 120).Id;
				newPicture.MediumId = ImageData.Image.NewImageFromBits(imageBytes, 320, 400).Id;
				newPicture.LargeId = ImageData.Image.NewImageFromBits(imageBytes, 570, 800).Id;

				person.Picture = newPicture;
			}

			DbUtil.Db.SubmitChanges();

			br.error = 0;
			br.data = "Image updated.";
			br.id = mpsi.id;
			br.count = 1;

			return br;
		}

		public ActionResult Organizations()
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
				return BaseReturn.createErrorReturn("Attendance roles is required to take attendance for organizations");

			var pid = Util.UserPeopleId;
			var oids = DbUtil.Db.GetLeaderOrgIds(pid);
			var dt = DateTime.Parse("8:00 AM");

			var roles = DbUtil.Db.CurrentRoles();

			IQueryable<Organization> q = null;

			if (Util2.OrgLeadersOnly)
			{
				q = from o in DbUtil.Db.Organizations
					 where o.LimitToRole == null || roles.Contains(o.LimitToRole)
					 where oids.Contains(o.OrganizationId)
					 where o.SecurityTypeId != 3
					 select o;
			}
			else
			{
				q = from o in DbUtil.Db.Organizations
					 where o.LimitToRole == null || roles.Contains(o.LimitToRole)
					 let sc = o.OrgSchedules.FirstOrDefault() // SCHED
					 where (o.OrganizationMembers.Any(om => om.PeopleId == pid // either a leader, who is not pending / inactive
								  && (om.Pending ?? false) == false
								  && (om.MemberTypeId != MemberTypeCode.InActive)
								  && (om.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
								  )
							 || oids.Contains(o.OrganizationId)) // or a leader of a parent org
					 where o.SecurityTypeId != 3
					 select o;
			}

			var orgs = from o in q
						  let sc = o.OrgSchedules.FirstOrDefault() // SCHED
						  select new OrganizationInfo
						  {
							  id = o.OrganizationId,
							  name = o.OrganizationName,
							  time = sc.SchedTime ?? dt,
							  day = sc.SchedDay ?? 0
						  };

			BaseReturn br = new BaseReturn();
			List<MobileOrganization> mo = new List<MobileOrganization>();

			br.error = 0;
			br.type = BaseReturn.API_TYPE_ORG_REFRESH;
			br.count = orgs.Count();

			foreach (var item in orgs)
			{
				mo.Add(new MobileOrganization().populate(item));
			}

			br.data = JsonConvert.SerializeObject(mo);
			return br;
		}

		public ActionResult RollList(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check Role
			if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
				return BaseReturn.createErrorReturn("Attendance roles is required to take attendance for organizations");

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_ORG_ROLL_REFRESH)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostRollList mprl = JsonConvert.DeserializeObject<MobilePostRollList>(dataIn.data);

			var meeting = Meeting.FetchOrCreateMeeting(DbUtil.Db, mprl.id, mprl.datetime);
			var people = RollsheetModel.RollList(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value);

			BaseReturn br = new BaseReturn();
			List<MobileAttendee> ma = new List<MobileAttendee>();

			br.error = 0;
			br.type = BaseReturn.API_TYPE_ORG_ROLL_REFRESH;
			br.count = people.Count();

			foreach (var person in people)
			{
				ma.Add(new MobileAttendee().populate(person));
			}

			br.data = JsonConvert.SerializeObject(ma);
			return br;
		}

		public ActionResult RecordAttend(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check Role
			if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
				return BaseReturn.createErrorReturn("Attendance roles is required to take attendance for organizations");

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_ORG_RECORD_ATTEND)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostAttend mpa = JsonConvert.DeserializeObject<MobilePostAttend>(dataIn.data);

			var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.OrganizationId == mpa.orgID && m.MeetingDate == mpa.datetime);
			if (meeting == null)
			{
				meeting = new CmsData.Meeting
				{
					OrganizationId = mpa.orgID,
					MeetingDate = mpa.datetime,
					CreatedDate = Util.Now,
					CreatedBy = Util.UserPeopleId ?? 0,
					GroupMeetingFlag = false,
				};
				DbUtil.Db.Meetings.InsertOnSubmit(meeting);
				DbUtil.Db.SubmitChanges();
				var acr = (from s in DbUtil.Db.OrgSchedules
							  where s.OrganizationId == mpa.orgID
							  where s.SchedTime.Value.TimeOfDay == mpa.datetime.TimeOfDay
							  where s.SchedDay == (int)mpa.datetime.DayOfWeek
							  select s.AttendCreditId).SingleOrDefault();
				meeting.AttendCreditId = acr;
			}

			Attend.RecordAttendance(mpa.peopleID, meeting.MeetingId, mpa.present);

			DbUtil.Db.UpdateMeetingCounters(mpa.orgID);
			DbUtil.LogActivity("Mobile RecAtt o:{0} p:{1} u:{2} a:{3}".Fmt(meeting.OrganizationId, mpa.peopleID, Util.UserPeopleId, mpa.present));

			BaseReturn br = new BaseReturn();

			br.error = 0;
			br.type = BaseReturn.API_TYPE_ORG_RECORD_ATTEND;

			return br;
		}

		/*
		public ActionResult AddPerson(string data)
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			// Check Role
			if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
				return BaseReturn.createErrorReturn("Attendance roles is required to take attendance for organizations");

			// Check to see if type matches
			BaseReturn dataIn = BaseReturn.createFromString(data);
			if (dataIn.type != BaseReturn.API_TYPE_ORG_ROLL_REFRESH)
				return BaseReturn.createTypeErrorReturn();

			// Everything is in order, start the return
			MobilePostAddPerson mpap = JsonConvert.DeserializeObject<MobilePostAddPerson>(dataIn.data);

			var p = new Person();

			p.CreatedDate = Util.Now;
			p.CreatedBy = Util.UserId;

			//Db.People.InsertOnSubmit(p);

			p.MemberStatusId = MemberStatusCode.JustAdded;
			//p.PositionInFamilyId = mpap.po;
			p.AddressTypeId = 10;

			p.FirstName = mpap.firstName;
			p.NickName = mpap.goesBy;
			p.LastName = mpap.lastName;

			p.BirthDay = mpap.birthday.Day;
			p.BirthMonth = mpap.birthday.Month;
			p.BirthYear = mpap.birthday.Year;

			p.GenderId = mpap.genderID;
			p.MaritalStatusId = mpap.maritalStatusID;

			CmsData.Family f;

			if (mpap.familyID > 0)
			{
				f = DbUtil.Db.Families.First(fam => fam.People.Any(pp => pp.PeopleId == mpap.familyID));
			}
			else
			{
				f = new Family();
				DbUtil.Db.Families.InsertOnSubmit(f);
			}

			f.People.Add(p);

			var position = PositionInFamily.Child;

			if (p.GetAge() >= 18)
			{
				if (f.People.Count(per => per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2)
					p.PositionInFamilyId = PositionInFamily.PrimaryAdult;
				else
					p.PositionInFamilyId = PositionInFamily.SecondaryAdult;
			}

			p.OriginId = OriginCode.Visit;
			p.FixTitle();
			DbUtil.Db.SubmitChanges();

			/*
			
			if (m.addtofamilyid == 0)
			{
				p.Family.HomePhone = m.home.GetDigits();
				p.Family.AddressLineOne = m.addr;
				p.Family.CityName = z != null ? z.City : null;
				p.Family.StateCode = z != null ? z.State : null;
				p.Family.ZipCode = m.zip;
			}
			p.EmailAddress = Trim(m.email);
			if (m.cell.HasValue())
				p.CellPhone = m.cell.GetDigits();
			p.MaritalStatusId = m.marital;
			p.GenderId = m.gender;
			DbUtil.Db.SubmitChanges();
			var meeting = DbUtil.Db.Meetings.Single(mm => mm.MeetingId == id);
			Attend.RecordAttendance(p.PeopleId, id, true);
			DbUtil.Db.UpdateMeetingCounters(id);
			return new RollListResult(meeting, p.PeopleId);
			
		}
		*/

		public ActionResult fetchMaritalStatuses()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var statuses = (from e in DbUtil.Db.MaritalStatuses
								 orderby e.Id
								 select e).ToList();

			BaseReturn br = new BaseReturn();
			List<MobileMaritalStatus> ma = new List<MobileMaritalStatus>();

			br.error = 0;
			br.type = BaseReturn.API_TYPE_SYSTEM_MARITAL_STATUSES;
			br.count = statuses.Count();

			foreach (var status in statuses)
			{
				ma.Add(new MobileMaritalStatus().populate(status));
			}

			br.data = JsonConvert.SerializeObject(ma);
			return br;
		}

		public ActionResult fetchStates()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var states = (from e in DbUtil.Db.StateLookups
							  orderby e.StateCode
							  select e).ToList();

			BaseReturn br = new BaseReturn();
			List<MobileState> ma = new List<MobileState>();

			br.error = 0;
			br.type = BaseReturn.API_TYPE_SYSTEM_STATES;
			br.count = states.Count();

			foreach (var state in states)
			{
				ma.Add(new MobileState().populate(state));
			}

			br.data = JsonConvert.SerializeObject(ma);
			return br;
		}

		public ActionResult fetchCountries()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var countries = (from e in DbUtil.Db.Countries
								  orderby e.Id
								  select e).ToList();

			BaseReturn br = new BaseReturn();
			List<MobileCountry> ma = new List<MobileCountry>();

			br.error = 0;
			br.type = BaseReturn.API_TYPE_SYSTEM_COUNTRIES;
			br.count = countries.Count();

			foreach (var country in countries)
			{
				ma.Add(new MobileCountry().populate(country));
			}

			br.data = JsonConvert.SerializeObject(ma);
			return br;
		}

		/*
		public ActionResult TaskList(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobileTask> mt = new List<MobileTask>();

			var list = from e in DbUtil.Db.Tasks
						  where e.OwnerId == ID
						  select e;

			br.type = 101;

			if (list != null)
			{
				br.count = list.Count();

				foreach (var item in list)
				{
					mt.Add(new MobileTask().populate(item));
				}

				br.data = JSONHelper.JsonSerializer<List<MobileTask>>(mt);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskItem(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			MobileTask mt = new MobileTask();

			var item = (from e in DbUtil.Db.Tasks
							where e.Id == ID
							select e).SingleOrDefault();

			br.type = 102;

			if (item != null)
			{
				br.count = 1;

				mt.populate(item);
				br.data = JSONHelper.JsonSerializer<MobileTask>(mt);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskBoxList(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobileTaskBox> mtb = new List<MobileTaskBox>();

			var list = from e in DbUtil.Db.TaskLists
						  where e.CreatedBy == ID
						  select e;

			br.type = 103;

			if (list != null)
			{
				br.count = list.Count();

				foreach (var item in list)
				{
					mtb.Add(new MobileTaskBox().populate(item));
				}

				br.data = JSONHelper.JsonSerializer<List<MobileTaskBox>>(mtb);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskBoxItem(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			MobileTaskBox mtb = new MobileTaskBox();

			var item = (from e in DbUtil.Db.TaskLists
							where e.CreatedBy == ID
							select e).SingleOrDefault();

			br.type = 104;

			if (item != null)
			{
				br.count = 1;

				mtb.populate(item);
				br.data = JSONHelper.JsonSerializer<MobileTaskBox>(mtb);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskStatusList()
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobileTaskStatus> ls = new List<MobileTaskStatus>();

			br.type = 105;

			var s = from e in DbUtil.Db.TaskStatuses
					  select e;

			foreach (var item in s)
			{
				ls.Add(new MobileTaskStatus().populate(item));
			}

			br.count = s.Count();
			br.data = JSONHelper.JsonSerializer<List<MobileTaskStatus>>(ls);

			return br;
		}

		[ValidateInput(false)]
		public ActionResult TaskCreate(string type, string data) // Type 1001
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			br.type = 1001;

			MobileTask mt = JSONHelper.JsonDeserialize<MobileTask>(data);
			if (mt != null) br.id = mt.addToDB();
			else
			{
				br.error = 1;
				br.data = "Task was not created.";
			}

			return br;
		}

		[ValidateInput(false)]
		public ActionResult TaskUpdate(string type, string data) // Type 1002
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			br.type = 1002;

			MobileTask mt = JSONHelper.JsonDeserialize<MobileTask>(data);

			var t = from e in DbUtil.Db.Tasks
					  where e.Id == mt.id
					  select e;

			if (t != null)
			{
				var task = t.Single();

				if (mt.updateDue > 0) task.Due = mt.due;
				if (mt.statusID > 0) task.StatusId = mt.statusID;
				if (mt.priority > 0) task.Priority = mt.priority;
				if (mt.notes.Length > 0) task.Notes = mt.notes;
				if (mt.description.Length > 0) task.Description = mt.description;
				if (mt.ownerID > 0) task.OwnerId = mt.ownerID;
				if (mt.boxID > 0) task.ListId = mt.boxID;
				if (mt.aboutID > 0) task.WhoId = mt.aboutID;
				if (mt.delegatedID > 0) task.CoOwnerId = mt.delegatedID;
				if (mt.notes.Length > 0) task.Notes = mt.notes;

				DbUtil.Db.SubmitChanges();

				br.data = "Task updated.";
			}
			else
			{
				br.error = 1;
				br.data = "Task not found.";
			}

			return br;
		}
		*/
	}
}