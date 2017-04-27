using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult Messages(int id)
        {
            var m = new SettingsMessagesModel(id);
            return PartialView("Registration/Messages", m);
        }

        [HttpPost]
        public ActionResult MessagesHelpToggle(int id)
        {
            DbUtil.Db.ToggleUserPreference("ShowMessagesHelp");
            var m = new SettingsMessagesModel(id);
            return PartialView("Registration/Messages", m);
        }

        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult MessagesEdit(int id)
        {
            var m = new SettingsMessagesModel(id);
            return PartialView("Registration/MessagesEdit", m);
        }

        [HttpPost]
        public ActionResult MessagesUpdate(SettingsMessagesModel m)
        {
            if (!ModelState.IsValid)
                return PartialView("Registration/MessagesEdit", m);
            DbUtil.LogActivity($"Update Fees {m.Org.OrganizationName}");
            try
            {
                m.Update();
                if (!m.Org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("Registration/Messages", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("Registration/MessagesEdit", m);
            }
        }

        public ActionResult NotifyIds(int id, string field)
        {
            if (Util.SessionTimedOut())
                return Content("<script type='text/javascript'>window.onload = function() { parent.location = '/'; }</script>");
            Response.NoCache();
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            DbUtil.Db.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            DbUtil.Db.SetCurrentOrgId(id);
            DbUtil.Db.SubmitChanges();
            var o = DbUtil.Db.LoadOrganizationById(id);
            string notifyids = null;
            switch (field.ToLower())
            {
                case "notifyids":
                    notifyids = o.NotifyIds;
                    break;
                case "giftnotifyids":
                    notifyids = o.GiftNotifyIds;
                    break;
            }
            var q = DbUtil.Db.PeopleFromPidString(notifyids).Select(p => p.PeopleId);
            foreach (var pid in q)
                t.PersonTags.Add(new TagPerson {PeopleId = pid});
            DbUtil.Db.SubmitChanges();
            return Redirect("/SearchUsers?ordered=true&topid=" + q.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult UpdateNotifyIds(int id, int topid, string field)
        {
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var selected_pids = (from p in t.People(DbUtil.Db)
                                 orderby p.PeopleId == topid ? "0" : "1"
                                 select p.PeopleId).ToArray();
            var o = DbUtil.Db.LoadOrganizationById(id);
            var notifyids = string.Join(",", selected_pids);
            switch (field.ToLower())
            {
                case "notifyids":
                    o.NotifyIds = notifyids;
                    break;
                case "giftnotifyids":
                    o.GiftNotifyIds = notifyids;
                    break;
            }
            DbUtil.Db.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            DbUtil.Db.Tags.DeleteOnSubmit(t);
            DbUtil.Db.SubmitChanges();
            ViewBag.OrgId = id;
            ViewBag.field = field;
            var view = ViewExtensions2.RenderPartialViewToString2(this, "Registration/NotifyList", notifyids);
            return Content(view);
            //return View("NotifyList2", notifyids);
        }

        [HttpPost]
        public ActionResult EventReminders(Guid id)
        {
            var m = new APIOrganization(DbUtil.Db);
            try
            {
                m.SendEventReminders(id);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }
        [HttpPost]
        public ActionResult VolunteerReminders(int id, bool? emailall)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            if(org == null)
                throw new Exception("Org not found");
            var m = new APIOrganization(DbUtil.Db);
            try
            {
                m.SendVolunteerReminders(id, emailall ?? false);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }
    }
}
