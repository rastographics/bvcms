using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        public OrgController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost]
        public ActionResult Messages(int id)
        {
            var m = new SettingsMessagesModel(id);
            return PartialView("Registration/Messages", m);
        }

        [HttpPost]
        public ActionResult MessagesHelpToggle(int id)
        {
            CurrentDatabase.ToggleUserPreference("ShowMessagesHelp");
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
            var t = CurrentDatabase.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            CurrentDatabase.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            CurrentDatabase.SetCurrentOrgId(id);
            CurrentDatabase.SubmitChanges();
            var o = CurrentDatabase.LoadOrganizationById(id);
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
            var q = CurrentDatabase.PeopleFromPidString(notifyids).Select(p => p.PeopleId);
            foreach (var pid in q)
                t.PersonTags.Add(new TagPerson {PeopleId = pid});
            CurrentDatabase.SubmitChanges();
            return Redirect("/SearchUsers?ordered=true&topid=" + q.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult UpdateNotifyIds(int id, int topid, string field)
        {
            var t = CurrentDatabase.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var selected_pids = (from p in t.People(CurrentDatabase)
                                 orderby p.PeopleId == topid ? "0" : "1"
                                 select p.PeopleId).ToArray();
            var o = CurrentDatabase.LoadOrganizationById(id);
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
            CurrentDatabase.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            CurrentDatabase.Tags.DeleteOnSubmit(t);
            CurrentDatabase.SubmitChanges();
            ViewBag.OrgId = id;
            ViewBag.field = field;
            var view = ViewExtensions2.RenderPartialViewToString2(this, "Registration/NotifyList", notifyids);
            return Content(view);
            //return View("NotifyList2", notifyids);
        }

        [HttpPost]
        public ActionResult EventReminders(Guid id)
        {
            var m = new APIOrganization(CurrentDatabase);
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
            var org = CurrentDatabase.LoadOrganizationById(id);
            if(org == null)
                throw new Exception("Org not found");
            var m = new APIOrganization(CurrentDatabase);
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
