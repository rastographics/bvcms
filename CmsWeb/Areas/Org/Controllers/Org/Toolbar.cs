using System;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [Authorize(Roles = "Edit")]
        public ActionResult CopySettings(int id)
        {
            if (Util.SessionTimedOut())
                return Redirect("/");
            Session["OrgCopySettings"] = id;
            return Redirect("/OrgSearch/");
        }

        [HttpPost]
        public ActionResult ToggleTag(int id)
        {
            var t = Person.ToggleTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return Content(t ? "Remove" : "Add");
        }
        [HttpPost]
        public ContentResult TagAll(Guid id, string tagname, bool? cleartagfirst)
        {
            if (!tagname.HasValue())
                return Content("no tag name");
            DbUtil.Db.SetNoLock();
            var q = DbUtil.Db.PeopleQuery(id);
            if (Util2.CurrentTagName == tagname && !(cleartagfirst ?? false))
            {
                DbUtil.Db.TagAll(q);
                return Content("Remove");
            }
            var tag = DbUtil.Db.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            if (cleartagfirst ?? false)
                DbUtil.Db.ClearTag(tag);
            DbUtil.Db.TagAll(q, tag);
            Util2.CurrentTag = tagname;
            DbUtil.Db.TagCurrent();
            return Content("Manage");
        }
        [HttpPost]
        public ContentResult UnTagAll(Guid id)
        {
            DbUtil.Db.SetNoLock();
            var q = DbUtil.Db.PeopleQuery(id);
            DbUtil.Db.UnTagAll(q);
            return Content("Add");
        }
        [HttpPost]
        public ActionResult AddContact(Guid id)
        {
            var cid = CmsData.Contact.AddContact(id);
            return Content("/Contact2/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks(Guid id)
        {
            var c = new ContentResult();
            c.Content = Task.AddTasks(DbUtil.Db, id).ToString();
            return c;
        }
    }
}