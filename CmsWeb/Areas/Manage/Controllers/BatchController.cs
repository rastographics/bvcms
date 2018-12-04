using CmsData;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Areas.Manage.Models.BatchModel;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using LumenWorks.Framework.IO.Csv;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix = "Batch"), Route("{action}/{id?}")]
    public partial class BatchController : CmsStaffController
    {
        public BatchController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
                var m = CurrentDatabase.OrganizationMembers.Single(om => om.OrganizationId == i.oid && om.PeopleId == i.pid);
                m.Grade = i.grade;
            }
            CurrentDatabase.SubmitChanges();

            return Content("done");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateOrg()
        {
            ViewBag.Title = "Update Organizations";
            ViewBag.PageHeader = "Batch Update Organizations from spreadsheet";
            ViewBag.text = "";
            ViewBag.action = "/Batch/UpdateOrg";
            return View("BatchUpdate");
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
                return AjaxErrorMessage(ex);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateRegSettings()
        {
            ViewBag.Title = "Update Registration Settings";
            ViewBag.PageHeader = "Batch Update Registration Settings from xml";
            ViewBag.text = "";
            ViewBag.action = "/Batch/UpdateRegSettings";
            return View("BatchUpdate");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateRegSettings(string text)
        {
            try
            {
                BatchRegSettings.Update(text);
                return Content("<strong>Success!</strong> RegSettings were successfully updated.");
            }
            catch (Exception ex)
            {
                return AjaxErrorMessage(ex);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateRegOptions()
        {
            ViewBag.Title = "Update Registration Options";
            ViewBag.PageHeader = "Batch Update Registration Options from spreadsheet";
            ViewBag.text = "";
            ViewBag.action = "/Batch/UpdateRegOptions";
            return View("BatchUpdate");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateRegOptions(string text)
        {
            try
            {
                BatchRegOptions.Update(text);
                return Content("RegOptions were successfully updated.");
            }
            catch (Exception ex)
            {
                return AjaxErrorMessage(ex);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateRegMessages()
        {
            ViewBag.Title = "Update Registration Messages";
            ViewBag.PageHeader = "Batch Update Registration Messages";
            ViewBag.text = "";
            ViewBag.action = "/Batch/UpdateRegMessages";
            return View("BatchUpdate");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateRegMessages(string text)
        {
            try
            {
                BatchRegMessages.Update(text);
                return Content("RegMessages were successfully updated.");
            }
            catch (Exception ex)
            {
                return AjaxErrorMessage(ex);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateFields() // UpdateForATag
        {
            var m = new UpdateFieldsModel();
            var success = (string)TempData["success"];
            if (success.HasValue())
            {
                ViewData["success"] = success;
            }

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
            {
                return View("UpdateFields", m);
            }

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
            CurrentDatabase.UpdateStatusFlags();
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
            try
            {
                var list = BatchModel.FindTagPeople(text, tagname);
                return View(list);
            }
            catch (Exception ex)
            {
                return Message(ex);
            }
        }


        [Authorize(Roles = "Edit")]
        public ActionResult FindTagEmail(string emails, string tagname)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                return View();
            }

            if (!tagname.HasValue())
            {
                return Content("no tag");
            }

            var a = emails.SplitLines();
            var q = from p in CurrentDatabase.People
                    where a.Contains(p.EmailAddress) || a.Contains(p.EmailAddress2)
                    select p.PeopleId;
            foreach (var pid in q.Distinct())
            {
                Person.Tag(CurrentDatabase, pid, tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            }

            CurrentDatabase.SubmitChanges();
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
                    select line.GetCsvToken(1, sep: "\t").ToInt();
            if (newtag)
            {
                var tag = CurrentDatabase.FetchTag(name, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
                if (tag != null)
                {
                    CurrentDatabase.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
                }
            }
            foreach (var pid in q)
            {
                Person.Tag(CurrentDatabase, pid, name, CurrentDatabase.CurrentUser.PeopleId, DbUtil.TagTypeId_Personal);
                CurrentDatabase.SubmitChanges();
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
                var p = CurrentDatabase.LoadPersonById(a[0].ToInt());
                p.AddEditExtraCode(field, a[1]);
                CurrentDatabase.SubmitChanges();
            }
            return Redirect("/ExtraValue/Summary/People");
        }


        [HttpGet]
        [Authorize(Roles = "Finance")]
        public ActionResult DoGiving()
        {
            ManagedGiving.DoAllGiving(CurrentDatabase);
            return Content("done");
        }
        [HttpGet]
        [Authorize(Roles = "Developer")]
        public ActionResult DoGiving1(int id)
        {
            var rg = (from r in CurrentDatabase.ManagedGivings
                      where r.PeopleId == id
                      select r).Single();
            rg.DoGiving(CurrentDatabase);
            return Content($"done with {id}");
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
                var script = CurrentDatabase.Content(id);
                PythonModel.RunScript(Util.Host, script.Body);
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
