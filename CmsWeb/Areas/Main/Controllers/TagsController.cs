using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "Tags"), Route("{action}/{id?}")]
    public class TagsController : CmsStaffController
    {
        public TagsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Tags")]
        public ActionResult Index(string tag)
        {
            var m = new TagsModel();
            if (tag.HasValue())
            {
                m.tag = tag;
            }

            m.SetCurrentTag();
            InitExportToolbar();
            return View(m);
        }

        [HttpPost]
        public ActionResult Results(TagsModel m)
        {
            m.SetCurrentTag();
            InitExportToolbar();
            return View(m);
        }

        [HttpPost]
        public ActionResult SetShared(TagsModel m)
        {
            m.SetShareIds();
            return View("Results", m);
        }

        [HttpPost]
        public ActionResult Delete()
        {
            var t = CurrentDatabase.TagCurrent();
            if (t.TagShares.Count() > 0 || t.PeopleId != Util.UserPeopleId)
            {
                return Content("error");
            }

            t.DeleteTag(CurrentDatabase);
            CurrentDatabase.SubmitChanges();
            Util2.CurrentTag = "UnNamed";
            var m = new TagsModel();
            return View("Tags", m);
        }

        [HttpPost]
        public ActionResult RenameTag(TagsModel m, string renamedTag = null)
        {
            if (renamedTag == null || !renamedTag.HasValue())
            {
                return View("Tags", m);
            }

            m.tagname = renamedTag.Replace("!", "_");
            var t = CurrentDatabase.TagCurrent();
            t.Name = m.tagname;
            CurrentDatabase.SubmitChanges();
            Util2.CurrentTag = m.tagname;
            return View("Tags", m);
        }

        [HttpPost]
        public ActionResult NewTag(TagsModel m)
        {
            Util2.CurrentTag = m.tagname.Replace("!", "_");
            CurrentDatabase.TagCurrent();
            return View("Tags", m);
        }

        private void InitExportToolbar()
        {
            var qid = CurrentDatabase.QueryHasCurrentTag().QueryId;
            ViewBag.queryid = qid;
            ViewBag.TagAction = $"/Tags/TagAll/{qid}";
            ViewBag.UnTagAction = $"/Tags/UnTagAll/{qid}";
            ViewBag.AddContact = "/Tags/AddContact/" + qid;
            ViewBag.AddTasks = "/Tags/AddTasks/" + qid;
        }

        [HttpPost]
        public ActionResult ToggleTag(int id)
        {
            var t = Person.ToggleTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            CurrentDatabase.SubmitChanges();
            return Content(t ? "Remove" : "Add");
        }

        [HttpPost]
        public ContentResult TagAll(Guid id, string tagname, bool? cleartagfirst)
        {
            if (!tagname.HasValue())
            {
                return Content("error: no tag name");
            }

            CurrentDatabase.SetNoLock();
            var q = CurrentDatabase.PeopleQuery(id);
            if (Util2.CurrentTagName == tagname && !(cleartagfirst ?? false))
            {
                CurrentDatabase.TagAll(q);
                return Content("Remove");
            }
            var tag = CurrentDatabase.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            if (cleartagfirst ?? false)
            {
                CurrentDatabase.ClearTag(tag);
            }

            CurrentDatabase.TagAll(q, tag);
            Util2.CurrentTag = tagname;
            CurrentDatabase.TagCurrent();
            return Content("Manage");
        }

        [HttpPost]
        public ContentResult UnTagAll(Guid id)
        {
            var q = CurrentDatabase.PeopleQuery(id);
            CurrentDatabase.UnTagAll(q);
            return Content("Add");
        }

        [HttpPost]
        public ContentResult ClearTag()
        {
            var tag = CurrentDatabase.TagCurrent();
            CurrentDatabase.ExecuteCommand("delete dbo.TagPerson where Id = {0}", tag.Id);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult AddContact(Guid id)
        {
            var cid = Contact.AddContact(id);
            return Content("/Contact2/" + cid);
        }

        [HttpPost]
        public ActionResult AddTasks(Guid id)
        {
            return Content(Task.AddTasks(CurrentDatabase, id).ToString());
        }

        public ActionResult SharedTags()
        {
            var t = CurrentDatabase.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            CurrentDatabase.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            CurrentDatabase.SubmitChanges();
            var tag = CurrentDatabase.TagCurrent();
            foreach (var ts in tag.TagShares)
            {
                t.PersonTags.Add(new TagPerson { PeopleId = ts.PeopleId });
            }

            CurrentDatabase.SubmitChanges();
            return Redirect("/SearchUsers");
        }

        [HttpPost]
        public ActionResult UpdateShared()
        {
            var t = CurrentDatabase.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var tag = CurrentDatabase.TagCurrent();
            var selected_pids = (from p in t.People(CurrentDatabase)
                                 where p.PeopleId != Util.UserPeopleId
                                 select p.PeopleId).ToArray();
            var userDeletes = tag.TagShares.Where(ts => !selected_pids.Contains(ts.PeopleId));
            CurrentDatabase.TagShares.DeleteAllOnSubmit(userDeletes);
            var tag_pids = tag.TagShares.Select(ts => ts.PeopleId).ToArray();
            var userAdds = from pid in selected_pids
                           join tpid in tag_pids on pid equals tpid into j
                           from p in j.DefaultIfEmpty(-1)
                           where p == -1
                           select pid;
            foreach (var pid in userAdds)
            {
                tag.TagShares.Add(new TagShare { PeopleId = pid });
            }

            CurrentDatabase.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            CurrentDatabase.Tags.DeleteOnSubmit(t);
            CurrentDatabase.SubmitChanges();
            return Content(CurrentDatabase.TagShares.Count(tt => tt.TagId == tag.Id).ToString());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ConvertTagToExtraValue(string tag, string field, string value)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                var success = (string)TempData["success"];
                if (success.HasValue())
                {
                    ViewData["success"] = success;
                }

                ViewData["tag"] = tag;
                ViewData["field"] = tag;
                ViewData["value"] = "true";
                return View();
            }
            var t = CurrentDatabase.Tags.FirstOrDefault(tt =>
                tt.Name == tag && tt.PeopleId == Util.UserPeopleId && tt.TypeId == DbUtil.TagTypeId_Personal);
            if (t == null)
            {
                TempData["message"] = "tag not found";
                return Redirect("/Tags/ConvertTagToExtraValue");
            }

            var q = t.People(CurrentDatabase);
            foreach (var p in q)
            {
                p.AddEditExtraCode(field, value);
                CurrentDatabase.SubmitChanges();
            }
            TempData["message"] = "success";
            return Redirect("/Tags/ConvertTagToExtraValue");
        }
    }
}
