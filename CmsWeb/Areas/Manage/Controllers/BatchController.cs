using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Models;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix = "Batch"), Route("{action}/{id?}")]
    public class BatchController : CmsStaffController
    {
        public ActionResult Grade(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var batch = from s in text.Split('\n')
                        where s.HasValue()
                        let a = s.SplitStr("\t", 3)
                        select new { pid = a[0].ToInt(), oid = a[1].ToInt(), grade = a[2].ToInt() };
            foreach (var i in batch)
            {
                var m = DbUtil.Db.OrganizationMembers.Single(om => om.OrganizationId == i.oid && om.PeopleId == i.pid);
                m.Grade = i.grade;
            }
            DbUtil.Db.SubmitChanges();

            return Content("done");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RegistrationMail(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var batch = from s in text.Split('\n')
                        where s.HasValue()
                        let a = s.SplitStr("\t", 2)
                        select new { pid = a[0].ToInt(), em = a[1] };
            foreach (var i in batch)
            {
                var m = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == 88485 && om.PeopleId == i.pid);
                if (m == null)
                    continue;
                m.RegisterEmail = i.em;
            }
            DbUtil.Db.SubmitChanges();

            return Content("done");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateOrg()
        {
            ViewData["text"] = "";
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateOrg(string text)
        {
            try
            {
                BatchModel.UpdateOrgs(text);
                return Content("Organizations were successfully updated.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Message(ex.Message);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateFields() // UpdateForATag
        {
            var m = new UpdateFieldsModel();
            var success = (string)TempData["success"];
            if (success.HasValue())
                ViewData["success"] = success;
            ViewData["text"] = "";
            return View(m);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateLookupValueSelection(string field)
        {
            bool useCode = false;
            var m = UpdateFieldsLookups.Fetch(field, ref useCode);

            ViewBag.FieldName = field;
            ViewBag.UseCode = useCode;
            return View(m);
        }

        [HttpPost]
        public ActionResult UpdateFieldsRun(UpdateFieldsModel m)
        {
            m.Run(ModelState);
            if (!ModelState.IsValid)
                return View("UpdateFields", m);

            TempData["success"] = $"{m.Field} updated with the value '{m.NewValue}' for {m.Count} records ";
            return RedirectToAction("UpdateFields");
        }

        [HttpPost]
        public ActionResult UpdateFieldsCount(UpdateFieldsModel m)
        {
            var q = m.People();
            return Content(q.Count().ToString());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatusFlags()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateStatusFlags(FormCollection formCollection)
        {
            DbUtil.Db.DeleteQueryBitTags();
            var qbits = DbUtil.Db.StatusFlags().ToList();
            foreach (var a in qbits)
            {
                var t = DbUtil.Db.FetchOrCreateTag(a[0], null, DbUtil.TagTypeId_StatusFlags);
                var qq = DbUtil.Db.PeopleQuery2(a[0] + ":" + a[1]);
                if (qq == null)
                    continue;
                DbUtil.Db.TagAll2(qq, t);
            }
            return Content("Status flags were successfully updated.");
        }

        [HttpGet]
        [Authorize(Roles = "Edit")]
        public ActionResult FindTagPeople()
        {
            return View("FindTagPeople0");
        }


        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult FindTagPeople(string text, string tagname)
        {
            if (!tagname.HasValue())
                return Content("no tag");

            var error = "";
            var list = BatchModel.FindTagPeople(text, tagname, ref error);
            if(error.HasValue())
                return Message(error);

            return View(list);
        }


        [Authorize(Roles = "Edit")]
        public ActionResult FindTagEmail(string emails, string tagname)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
                return View();
            if (!tagname.HasValue())
                return Content("no tag");

            var a = emails.SplitLines();
            var q = from p in DbUtil.Db.People
                    where a.Contains(p.EmailAddress) || a.Contains(p.EmailAddress2)
                    select p.PeopleId;
            foreach (var pid in q.Distinct())
                Person.Tag(DbUtil.Db, pid, tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return Redirect("/Tags?tag=" + tagname);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "Edit")]
        public ActionResult TagPeopleIds()
        {
            return View();
        }

        public ActionResult TagUploadPeopleIds(string name, string text, bool newtag)
        {
            var q = from line in text.Split('\n')
                    select line.ToInt();
            if (newtag)
            {
                var tag = DbUtil.Db.FetchTag(name, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
                if (tag != null)
                    DbUtil.Db.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            }
            foreach (var pid in q)
            {
                Person.Tag(DbUtil.Db, pid, name, DbUtil.Db.CurrentUser.PeopleId, DbUtil.TagTypeId_Personal);
                DbUtil.Db.SubmitChanges();
            }
            return Redirect("/Tags?tag=" + name);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ExtraValuesFromPeopleIds()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExtraValuesFromPeopleIds(string text, string field)
        {
            var csv = new CsvReader(new StringReader(text), false, '\t').ToList();
            foreach (var a in csv)
            {
                var p = DbUtil.Db.LoadPersonById(a[0].ToInt());
                p.AddEditExtraValue(field, a[1]);
                DbUtil.Db.SubmitChanges();
            }
            return Redirect("/ExtraValue/Summary/People");
        }


        [HttpGet]
        [Authorize(Roles = "Finance")]
        public ActionResult DoGiving()
        {
            ManagedGiving.DoAllGiving(DbUtil.Db);
            return Content("done");
        }

//        [HttpGet]
//        [Authorize(Roles = "Admin")]
//        public ActionResult SQLView(string id)
//        {
//            try
//            {
//                var cmd = new SqlCommand("select * from guest." + id.Replace(" ", ""));
//                cmd.Connection = new SqlConnection(Util.ConnectionString);
//                cmd.Connection.Open();
//                var rdr = cmd.ExecuteReader();
//                return View(rdr);
//            }
//            catch (Exception)
//            {
//                return Content("cannot find view guest." + id);
//            }
//        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult RunScript(string id)
        {
            return Redirect("/PyScript/" + id);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Script(string id)
        {
            try
            {
                var script = DbUtil.Db.Content(id);
                PythonEvents.RunScript(Util.Host, script.Body);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            return Content("done");
        }

        [HttpGet]
        [Authorize(Roles = "Developer")]
        public ActionResult OtherDeveloperActions()
        {
            return View();
        }

    }
}
