using System;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using Drawing = System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Areas.People.Models.Person;
using UtilityExtensions;
using System.Text;
using System.Diagnostics;
using System.Web.Routing;
using System.Threading;
using System.Web.Security;
using CmsData.Codes;
using System.Globalization;

namespace CmsWeb.Areas.People.Controllers
{
    [ValidateInput(false)]
    [SessionExpire]
    [RouteArea("People", AreaUrl = "Person2")]
    public class PersonController : CmsStaffController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            NoCheckRole = true;
            base.Initialize(requestContext);
        }

        [GET("Person2/Current")]
        public ActionResult Current()
        {
            return Redirect("/Person2/" + Util2.CurrentPeopleId);
        }

        [GET("Person2/{id:int}")]
        [GET("{id:int}")]
        public ActionResult Person(int? id)
        {
            if (!id.HasValue)
                return Content("no id");
            //            if (!DbUtil.Db.UserPreference("newlook3", "false").ToBool()
            //                || !DbUtil.Db.UserPreference("newpeoplepage", "false").ToBool())
            //            {
            //                var url = Regex.Replace(Request.RawUrl, @"(.*)/(Person2(/Index)*)/(\d*)", "$1/Person/Index/$4", RegexOptions.IgnoreCase);
            //                return Redirect(url);
            //            }

            var m = new PersonModel(id.Value);
            var noview = m.CheckView();
            if (noview.HasValue())
                return Content(noview);

            ViewBag.Comments = Util.SafeFormat(m.Person.Comments);
            ViewBag.PeopleId = id.Value;
            Util2.CurrentPeopleId = id.Value;
            Session["ActivePerson"] = m.Person.Name;
            DbUtil.LogActivity("Viewing Person: {0}".Fmt(m.Person.Name));
            InitExportToolbar(id);
            return View(m);
        }

        [POST("Person2/ProfileHeader/{id}")]
        public ActionResult ProfileHeader(int id)
        {
            var m = new PersonModel(id);
            return View(m);
        }

        [POST("Person2/PersonalDisplay/{id}")]
        public ActionResult PersonalDisplay(int id)
        {
            InitExportToolbar(id);
            var m = new BasicPersonInfo(id);
            return View("MainTabs/PersonalDisplay", m);
        }
        [POST("Person2/PersonalEdit/{id}")]
        public ActionResult PersonalEdit(int id)
        {
            var m = new BasicPersonInfo(id);
            return View("MainTabs/PersonalEdit", m);
        }
        [POST("Person2/PersonalUpdate/{id}")]
        public ActionResult Personalpdate(int id, BasicPersonInfo m)
        {
            m.UpdatePerson();
            //m = new BasicPersonInfo(id);
            DbUtil.LogActivity("Update Basic Info for: {0}".Fmt(m.person.Name));
            InitExportToolbar(id);
            return View("MainTabs/PersonalDisplay", m);
        }

        [POST("Person2/EnrollGrid/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult EnrollGrid(int id, int? page, int? size, string sort, string dir)
        {
            var m = new CurrentEnrollments(id);
            m.Pager.Set("/Person2/EnrollGrid/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Current", m);
        }
        [POST("Person2/PrevEnrollGrid/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult PrevEnrollGrid(int id, int? page, int? size, string sort, string dir)
        {
            var m = new PreviousEnrollments(id);
            m.Pager.Set("/Person2/PrevEnrollGrid/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Prev Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Previous", m);
        }
        [POST("Person2/PendingEnrollGrid/{id}")]
        public ActionResult PendingEnrollGrid(int id)
        {
            var m = new PendingEnrollments(id);
            DbUtil.LogActivity("Viewing Pending Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Pending", m);
        }
        [POST("Person2/Attendance/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Attendance(int id, int? page, int? size, string sort, string dir)
        {
            var m = new PersonAttendHistoryModel(id, future: false);
            m.Pager.Set("/Person2/Attendance/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            UpdateModel(m.Pager);
            return View("Enrollment/Attendance", m);
        }
        [POST("Person2/AttendanceFuture/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult AttendanceFuture(int id, int? page, int? size, string sort, string dir)
        {
            var m = new PersonAttendHistoryModel(id, future: true);
            m.Pager.Set("/Person2/AttendanceFuture/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            UpdateModel(m.Pager);
            return View("Enrollment/Attendance", m);
        }

        [POST("Person2/MembershipDisplay/{id}")]
        public ActionResult MembershipDisplay(int id)
        {
            var m = new MemberInfo(id);
            return View("Membership/Display", m);
        }
        [POST("Person2/MembershipEdit/{id}")]
        public ActionResult MembershipEdit(int id)
        {
            var m = new MemberInfo(id);
            return View("Membership/Edit", m);
        }
        [POST("Person2/MembershipUpdate/")]
        public ActionResult MembershipUpdate(MemberInfo m)
        {
            m.UpdateMember();
            DbUtil.LogActivity("Update Membership Info for: {0}".Fmt(m.person.Name));
            return View("Membership/Display", m);
        }
        [POST("Person2/MembershipNotes/{id}")]
        public ActionResult MembershipNotes(int id)
        {
            var m = new PersonModel(id);
            return View("Membership/Notes", m);
        }
        [POST("Person2/ExtraValues/{id}")]
        public ActionResult ExtraValues1(int id)
        {
            var m = new PersonModel(id);
            return View("Membership/ExtraValues", m);
        }

        [POST("Person2/Giving/{id}")]
        public ActionResult Giving(int id)
        {
            var m = new PersonModel(id);
            return View("Main/Giving", m);
        }

        [POST("Person2/Comments/{id}")]
        public ActionResult Comments(int id)
        {
            var m = new PersonModel(id);
            return View("Main/Comments", m);
        }

        [POST("Person2/Account/{id}")]
        public ActionResult Account(int id)
        {
            var m = new PersonModel(id);
            return View("Main/Account", m);
        }

        [POST("Person2/FamilyMembers/{id}")]
        public ActionResult FamilyMembers(int id)
        {
            var m = new PersonModel(id);
            //UpdateModel(m.Pager);
            return View("FamilySidebar/Members", m);
        }
        [POST("Person2/RelatedFamilies/{id}")]
        public ActionResult RelatedFamilies(int id)
        {
            var m = new PersonModel(id);
            return View("FamilySidebar/Related", m);
        }
        [POST("Person2/UpdateRelation/{id}/{id1}/{id2}")]
        public ActionResult UpdateRelation(int id, int id1, int id2, string value)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rr => rr.FamilyId == id1 && rr.RelatedFamilyId == id2);
            r.FamilyRelationshipDesc = value;
            DbUtil.Db.SubmitChanges();
            var m = new PersonModel(id);
            return View("FamilySidebar/Related", m);
        }
        [POST("Person2/DeleteRelation/{id}/{id1}/{id2}")]
        public ActionResult DeleteRelation(int id, int id1, int id2)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            DbUtil.Db.RelatedFamilies.DeleteOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            var m = new PersonModel(id);
            return View("FamilySidebar/Related", m);
        }
        [POST("Person2/RelatedFamilyEdit/{id}/{id1}/{id2}")]
        public ActionResult RelatedFamilyEdit(int id, int id1, int id2)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            ViewBag.Id = id;
            return View("FamilySidebar/RelatedEdit", r);
        }

        [POST("Person2/UploadPicture/{id:int}/{picture}")]
        public ActionResult UploadPicture(int id, HttpPostedFileBase picture)
        {
            var person = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            DbUtil.LogActivity("Uploading Picture for {0}".Fmt(person.Name));
            person.UploadPicture(DbUtil.Db, picture.InputStream);
            return Redirect("/Person2/" + id);
        }
        [GET("Person2/Image/{s}/{id}/{v}")]
        public ActionResult Image(int id, int s, string v)
        {
            return new PictureResult(id, s);
        }
        [POST("Person2/PostData")]
        public ActionResult PostData(int pk, string name, string value)
        {
            var p = DbUtil.Db.LoadPersonById(pk);
            switch (name)
            {
                case "position":
                    p.UpdatePosition(DbUtil.Db, value.ToInt());
                    break;
                case "campus":
                    p.UpdateCampus(DbUtil.Db, value.ToInt());
                    break;
            }
            return new EmptyResult();
        }

        [POST("Person2/UserEdit/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UserEdit(int? id)
        {
            User u = null;
            if (id.HasValue)
                u = DbUtil.Db.Users.Single(us => us.UserId == id);
            else
            {
                u = CmsWeb.Models.AccountModel.AddUser(Util2.CurrentPeopleId);
                DbUtil.LogActivity("New User for: {0}".Fmt(Session["ActivePerson"]));
            }
            ViewBag.sendwelcome = false;
            return View(u);
        }
        [POST("Person2/UserUpdate/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UserUpdate(int id, string username, string password, bool islockedout, bool sendwelcome, string[] role)
        {
            var u = DbUtil.Db.Users.Single(us => us.UserId == id);
            if (u.Username != username)
            {
                var uu = DbUtil.Db.Users.SingleOrDefault(us => us.Username == username);
                if (uu != null)
                    return Content("error: username already exists");
            }
            u.Username = username;
            u.IsLockedOut = islockedout;
            u.SetRoles(DbUtil.Db, role, User.IsInRole("Finance"));
            if (password.HasValue())
                u.ChangePassword(password);
            DbUtil.Db.SubmitChanges();
            if (sendwelcome)
                CmsWeb.Models.AccountModel.SendNewUserEmail(username);
            DbUtil.LogActivity("Update User for: {0}".Fmt(Session["ActivePerson"]));
            var m = new PersonModel(u.PeopleId.Value);
            return View("Toolbar/Toolbar", m);
        }
        [POST("Person2/UserDelete/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UserDelete(int id)
        {
            var u = DbUtil.Db.Users.Single(us => us.UserId == id);
            DbUtil.Db.PurgeUser(id);
            var m = new PersonModel(u.PeopleId.Value);
            return View("Toolbar/Toolbar", m);
        }
        [GET("Person2/Impersonate/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Impersonate(int id)
        {
            var user = DbUtil.Db.Users.SingleOrDefault(uu => uu.UserId == id);
            if (user == null)
                return Content("no user");
            if (user.Roles.Contains("Finance") && !User.IsInRole("Finance"))
                return Content("cannot impersonate finance");
            Session.Remove("CurrentTag");
            FormsAuthentication.SetAuthCookie(user.Username, false);
            CmsWeb.Models.AccountModel.SetUserInfo(user.Username, Session);
            Util.FormsBasedAuthentication = true;
            Util.UserPeopleId = user.PeopleId;
            Util.UserPreferredName = user.Username;
            return Redirect("/");
        }

        //        [HttpPost]
        //        public ActionResult UserInfoGrid(int id)
        //        {
        //            var p = DbUtil.Db.LoadPersonById(id);
        //            return View(p);
        //        }

        [POST("Person2/Split/{id}")]
        [Authorize(Roles = "Edit")]
        public ActionResult Split(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogActivity("Splitting Family for {0}".Fmt(p.Name));
            p.SplitFamily(DbUtil.Db);
            return Content("/Person2/" + id);
        }
        [Authorize(Roles = "Admin")]
        [POST("Person2/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            Util.Auditing = false;
            var person = DbUtil.Db.LoadPersonById(id);
            if (person == null)
                return Content("error, bad peopleid");

            var p = person.Family.People.FirstOrDefault(m => m.PeopleId != id);
            if (p != null)
            {
                Util2.CurrentPeopleId = p.PeopleId;
                Session["ActivePerson"] = p.Name;
            }
            else
            {
                Util2.CurrentPeopleId = 0;
                Session.Remove("ActivePerson");
            }
            DbUtil.Db.PurgePerson(id);
            DbUtil.LogActivity("Deleted Record {0}".Fmt(person.PeopleId));
            return Content("/Person2/DeletedPerson");
        }
        [GET("Person2/DeletedPerson")]
        public ActionResult DeletedPerson()
        {
            return View();
        }

        [GET("Person2/Campuses")]
        public JsonResult Campuses()
        {
            var q = from c in DbUtil.Db.Campus
                    select new { value = c.Id, text = c.Description };
            return Json(q.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [POST("Person2/Schools")]
        public JsonResult Schools(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.SchoolOther.Contains(query)
                     group p by p.SchoolOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [POST("Person2/Employers")]
        public JsonResult Employers(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.EmployerOther.Contains(query)
                     group p by p.EmployerOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [POST("Person2/Occupations")]
        public JsonResult Occupations(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.OccupationOther.Contains(query)
                     group p by p.OccupationOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [POST("Person2/Churches")]
        public JsonResult Churches(string query)
        {
            var qu = from r in DbUtil.Db.ViewChurches
                     where r.C.Contains(query)
                     select r.C;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [POST("Person2/Changes/{id:int}")]
        public ActionResult Changes(int id)
        {
            var m = new PersonModel(id);
            return View("System/Changes", m);
        }
        [POST("Person2/Duplicates/{id:int}")]
        public ActionResult Duplicates(int id)
        {
            var m = new DuplicatesModel(id);
            return View(m);
        }

        #region ToDo

        [Authorize(Roles = "Admin")]
        public ActionResult Move(int id, int to)
        {
            var p = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            try
            {
                p.MovePersonStuff(DbUtil.Db, to);
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }

        [HttpPost]
        public ActionResult Tag(int id)
        {
            CmsData.Person.Tag(DbUtil.Db, id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UnTag(int id)
        {
            CmsData.Person.UnTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }

        //[HttpPost]
        //public ActionResult Reverse(int id, string field, string value, string pf)
        //{
        //    var m = new PersonModel(id);
        //    m.Reverse(field, value, pf);
        //    return View("ChangesGrid", m);
        //}

        //		[HttpPost]
        //		public ActionResult ContactsMadeGrid(int id)
        //		{
        //			var m = new PersonContactsMadeModel(id);
        //			DbUtil.LogActivity("Viewing Contacts Tab for: {0}".Fmt(Session["ActivePerson"]));
        //			UpdateModel(m.Pager);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult ContactsReceivedGrid(int id)
        //		{
        //			var m = new PersonContactsReceivedModel(id);
        //			UpdateModel(m.Pager);
        //			return View(m);
        //		}

        //[HttpPost]
        //public ActionResult IncompleteTasksGrid(int id)
        //{
        //    var m = new CmsWeb.Models.TaskModel();
        //    return View(m.IncompleteTasksList(id));
        //}

        //[HttpPost]
        //public ActionResult PendingTasksGrid(int id)
        //{
        //    var m = new CmsWeb.Models.TaskModel();
        //    return View(m.TasksAboutList(id));
        //}

        [HttpPost]
        public ActionResult AddContactMade(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogActivity("Adding contact from: {0}".Fmt(p.Name));
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date,
            };

            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();

            var cp = new Contactor
            {
                PeopleId = p.PeopleId,
                ContactId = c.ContactId
            };

            DbUtil.Db.Contactors.InsertOnSubmit(cp);
            DbUtil.Db.SubmitChanges();

            return Content("/Contact/" + c.ContactId);
        }

        [POST("Person2/AddContactReceived/{id:int}")]
        public ActionResult AddContactReceived(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogActivity("Adding contact to: {0}".Fmt(p.Name));
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date,
            };

            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();

            c.contactees.Add(new Contactee { PeopleId = p.PeopleId });
            c.contactsMakers.Add(new Contactor { PeopleId = Util.UserPeopleId.Value });
            DbUtil.Db.SubmitChanges();

            TempData["ContactEdit"] = true;
            return Content("/Contact/{0}".Fmt(c.ContactId));
        }

        [HttpPost]
        public ActionResult AddAboutTask(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            if (p == null || !Util.UserPeopleId.HasValue)
                return Content("no id");
            var t = p.AddTaskAbout(DbUtil.Db, Util.UserPeopleId.Value, "Please Contact");
            DbUtil.Db.SubmitChanges();
            return Content("/Task/List/{0}".Fmt(t.Id));
        }

        //		[HttpPost]
        //		public ActionResult MemberDisplay(int id)
        //		{
        //			var m = MemberInfo.GetMemberInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult MemberEdit(int id)
        //		{
        //			var m = MemberInfo.GetMemberInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult MemberUpdate(int id)
        //		{
        //			var m = MemberInfo.GetMemberInfo(id);
        //			UpdateModel(m);
        //			var ret = m.UpdateMember();
        //			if (ret != "ok")
        //			{
        //				ModelState.AddModelError("MemberTab", ret);
        //				return View("MemberEdit", m);
        //			}
        //			m = MemberInfo.GetMemberInfo(id);
        //			DbUtil.LogActivity("Update Member Info for: {0}".Fmt(Session["ActivePerson"]));
        //			return View("MemberDisplay", m);
        //		}
        //		[HttpPost]
        //		public ActionResult GrowthDisplay(int id)
        //		{
        //			var m = GrowthInfo.GetGrowthInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult GrowthEdit(int id)
        //		{
        //			var m = GrowthInfo.GetGrowthInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult GrowthUpdate(int id)
        //		{
        //			var m = GrowthInfo.GetGrowthInfo(id);
        //			UpdateModel(m);
        //			m.UpdateGrowth();
        //			DbUtil.LogActivity("Update Growth Info for: {0}".Fmt(Session["ActivePerson"]));
        //			return View("GrowthDisplay", m);
        //		}
        //		[HttpPost]
        //		public ActionResult CommentsDisplay(int id)
        //		{
        //			ViewData["Comments"] = Util.SafeFormat(DbUtil.Db.People.Single(p => p.PeopleId == id).Comments);
        //			ViewData["PeopleId"] = id;
        //			return View();
        //		}
        //		[HttpPost]
        //		public ActionResult CommentsEdit(int id)
        //		{
        //			ViewData["Comments"] = DbUtil.Db.People.Single(p => p.PeopleId == id).Comments;
        //			ViewData["PeopleId"] = id;
        //			return View();
        //		}
        //		[HttpPost]
        //		public ActionResult CommentsUpdate(int id, string Comments)
        //		{
        //			var p = DbUtil.Db.LoadPersonById(id);
        //			p.Comments = Comments;
        //			DbUtil.Db.SubmitChanges();
        //			ViewData["Comments"] = Util.SafeFormat(Comments);
        //			ViewData["PeopleId"] = id;
        //			DbUtil.LogActivity("Update Comments for: {0}".Fmt(Session["ActivePerson"]));
        //			return View("CommentsDisplay");
        //		}
        //		[HttpPost]
        //		public ActionResult MemberNotesDisplay(int id)
        //		{
        //			var m = MemberNotesInfo.GetMemberNotesInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult MemberNotesEdit(int id)
        //		{
        //			var m = MemberNotesInfo.GetMemberNotesInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult MemberNotesUpdate(int id)
        //		{
        //			var m = MemberNotesInfo.GetMemberNotesInfo(id);
        //			UpdateModel(m);
        //			m.UpdateMemberNotes();
        //			DbUtil.LogActivity("Update Member Notes for: {0}".Fmt(Session["ActivePerson"]));
        //			return View("MemberNotesDisplay", m);
        //		}
        //		[HttpPost]
        //		public ActionResult RecRegDisplay(int id)
        //		{
        //			var m = RecRegInfo.GetRecRegInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult RecRegEdit(int id)
        //		{
        //			var m = RecRegInfo.GetRecRegInfo(id);
        //			return View(m);
        //		}
        //		[HttpPost]
        //		public ActionResult RecRegUpdate(int id)
        //		{
        //			var m = RecRegInfo.GetRecRegInfo(id);
        //			UpdateModel(m);
        //			m.UpdateRecReg();
        //			DbUtil.LogActivity("Update Registration Tab for: {0}".Fmt(Session["ActivePerson"]));
        //			return View("RecRegDisplay", m);
        //		}

        [HttpPost]
        public ActionResult AddContact(int id)
        {
            return Content(Contact.AddContact(id).ToString());
        }

        [HttpPost]
        public ActionResult AddTasks(int id)
        {
            var c = new ContentResult();
            c.Content = Task.AddTasks(id).ToString();
            return c;
        }

        //[HttpPost]
        //public ActionResult OptoutsGrid(int id)
        //{
        //    var p = DbUtil.Db.LoadPersonById(id);
        //    return View(p);
        //}

        [HttpPost]
        public ActionResult DeleteOptout(int id, string email)
        {
            var oo = DbUtil.Db.EmailOptOuts.SingleOrDefault(o => o.FromEmail == email && o.ToPeopleId == id);
            if (oo == null)
                return Content("not found");
            DbUtil.Db.EmailOptOuts.DeleteOnSubmit(oo);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }

        //        [HttpPost]
        //        public ActionResult VolunteerDisplay(int id)
        //        {
        //            var m = new Main.Models.Other.VolunteerModel(id);
        //            return View(m);
        //        }

        [HttpPost]
        public ContentResult DeleteExtra(int id, string field)
        {
            var e = DbUtil.Db.PeopleExtras.First(ee => ee.PeopleId == id && ee.Field == field);
            DbUtil.Db.PeopleExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            return Content("done");
        }

        [HttpPost]
        public ContentResult EditExtra(string id, string value)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var p = DbUtil.Db.LoadPersonById(b[1].ToInt());
            switch (a[0])
            {
                case "s":
                    p.AddEditExtraValue(b[0], value);
                    break;
                case "t":
                    p.AddEditExtraData(b[0], value);
                    break;
                case "d":
                    {
                        DateTime dt;
                        if (DateTime.TryParse(value, out dt))
                        {
                            p.AddEditExtraDate(b[0], dt);
                            value = dt.ToShortDateString();
                        }
                        else
                        {
                            p.RemoveExtraValue(DbUtil.Db, b[0]);
                            value = "";
                        }
                    }
                    break;
                case "i":
                    p.AddEditExtraInt(b[0], value.ToInt());
                    break;
                case "b":
                    if (value == "True")
                        p.AddEditExtraBool(b[0], true);
                    else
                        p.RemoveExtraValue(DbUtil.Db, b[0]);
                    break;
                case "m":
                    {
                        if (value == null)
                            value = Request.Form["value[]"];
                        var cc = Code.StandardExtraValues.ExtraValueBits(b[0], b[1].ToInt());
                        var aa = value.Split(',');
                        foreach (var c in cc)
                        {
                            if (aa.Contains(c.Key)) // checked now
                                if (!c.Value) // was not checked before
                                    p.AddEditExtraBool(c.Key, true);
                            if (!aa.Contains(c.Key)) // not checked now
                                if (c.Value) // was checked before
                                    p.RemoveExtraValue(DbUtil.Db, c.Key);
                        }
                        DbUtil.Db.SubmitChanges();
                        break;
                    }
            }
            DbUtil.Db.SubmitChanges();
            if (value == "null")
                return Content(null);
            return Content(value);
        }

        //		[HttpPost]
        //		public ContentResult EditExtra2()
        //		{
        //			var a = Request.Form["id"].SplitStr("-", 2);
        //			var b = a[1].SplitStr(".", 2);
        //			var values = Request.Form["value[]"];
        //			if (a[0] == "m")
        //			{
        //				var p = DbUtil.Db.LoadPersonById(b[1].ToInt());
        //				DbUtil.Db.SubmitChanges();
        //			}
        //			return Content(values);
        //		}

        [HttpPost]
        public JsonResult ExtraValues(string id)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var c = Code.StandardExtraValues.Codes(b[0]);
            var j = Json(c);
            return j;
        }
        [HttpPost]
        public JsonResult ExtraValues2(string id)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var c = Code.StandardExtraValues.ExtraValueBits(b[0], b[1].ToInt());
            var j = Json(c);
            return j;
        }

        [HttpPost]
        public ActionResult NewExtraValue(int id, string field, string type, string value)
        {
            field = field.Replace('/', '-');
            var v = new PeopleExtra { PeopleId = id, Field = field };
            DbUtil.Db.PeopleExtras.InsertOnSubmit(v);
            switch (type)
            {
                case "string":
                    v.StrValue = value;
                    break;
                case "text":
                    v.Data = value;
                    break;
                case "date":
                    var dt = DateTime.MinValue;
                    DateTime.TryParse(value, out dt);
                    v.DateValue = dt;
                    break;
                case "int":
                    v.IntValue = value.ToInt();
                    break;
            }
            try
            {
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.Message);
            }
            return Content("ok");
        }

        //        [HttpPost]
        //        public ActionResult ExtrasGrid(int id)
        //        {
        //            var p = DbUtil.Db.LoadPersonById(id);
        //            return View(p);
        //        }


        public ActionResult ShowMeetings(int id, bool all)
        {
            if (all == true)
                Session["showallmeetings"] = true;
            else
                Session.Remove("showallmeetings");
            return Redirect("/Person/Index/" + id);
        }

        private void InitExportToolbar(int? id)
        {
            var qb = DbUtil.Db.QueryBuilderIsCurrentPerson();
            ViewBag.queryid = qb.QueryId;
            ViewBag.TagAction = "/Person/Tag/" + id;
            ViewBag.UnTagAction = "/Person2/UnTag/" + id;
            ViewBag.AddContact = "/Person2/AddContactReceived/" + id;
            ViewBag.AddTasks = "/Person2/AddAboutTask/" + id;
        }
        public class CurrentRegistration
        {
            public int OrgId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        //        public ActionResult CurrentRegistrations(bool? html)
        //        {
        //            var types = new[] 
        //			{
        //				CmsData.Codes.RegistrationTypeCode.JoinOrganization,
        //				CmsData.Codes.RegistrationTypeCode.ComputeOrganizationByAge2,
        //				CmsData.Codes.RegistrationTypeCode.UserSelectsOrganization2,
        //				CmsData.Codes.RegistrationTypeCode.ChooseVolunteerTimes,
        //			};
        //            var picklistorgs = DbUtil.Db.ViewPickListOrgs.Select(pp => pp.OrgId).ToArray();
        //            var dt = DateTime.Today;
        //            var q = from o in DbUtil.Db.Organizations
        //                    where !picklistorgs.Contains(o.OrganizationId)
        //                    where types.Contains(o.RegistrationTypeId ?? 0)
        //                    where (o.RegistrationClosed ?? false) == false
        //                    where (o.ClassFilled ?? false) == false
        //                    where o.RegEnd > dt || o.RegEnd == null
        //                    where o.RegStart <= dt || o.RegStart == null
        //                    where o.OrganizationStatusId == OrgStatusCode.Active
        //                    orderby o.OrganizationName
        //                    select new CurrentRegistration()
        //                    {
        //                        OrgId = o.OrganizationId,
        //                        Name = o.OrganizationName,
        //                        Description = o.Description
        //                    };
        //            if ((html ?? false) == true)
        //                return View("CurrentRegistrationsHtml", q);
        //            return View(q);
        //        }
        //
        public ActionResult ContributionStatement(int id, string fr, string to)
        {
            if (Util.UserPeopleId != id && !User.IsInRole("Finance"))
                return Content("No permission to view statement");
            var p = DbUtil.Db.LoadPersonById(id);
            if (p == null)
                return Content("Invalid Id");

            var frdt = Util.ParseMMddyy(fr);
            var todt = Util.ParseMMddyy(to);
            if (!(frdt.HasValue && todt.HasValue))
                return Content("date formats invalid");

            DbUtil.LogActivity("Contribution Statement for ({0})".Fmt(id));

            return new CmsWeb.Areas.Finance.Models.Report.ContributionStatementResult
            {
                PeopleId = id,
                FromDate = frdt.Value,
                ToDate = todt.Value,
                typ = p.PositionInFamilyId == PositionInFamily.PrimaryAdult ? 2 : 1,
                noaddressok = true,
                useMinAmt = false,
            };
        }
        #endregion

    }
}
