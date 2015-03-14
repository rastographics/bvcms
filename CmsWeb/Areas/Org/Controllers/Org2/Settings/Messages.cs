using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org2.Controllers
{
    public partial class Org2Controller
    {
        [HttpPost]
        public ActionResult Messages(int id)
        {
            return PartialView("Registration/Messages", getRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult MessagesEdit(int id)
        {
            return PartialView("Registration/MessagesEdit", getRegSettings(id));
        }
        [HttpPost]
        public ActionResult MessagesUpdate(int id)
        {
            var m = getRegSettings(id);
            DbUtil.LogActivity("Update Messages {0}".Fmt(m.org.OrganizationName));
            //m.VoteTags.Clear();
            try
            {
                UpdateModel(m);
                var os = new Settings(m.ToString(), DbUtil.Db, id);
                m.org.RegSetting = os.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
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
            DbUtil.Db.CurrentOrg.Id = id;
            DbUtil.Db.SubmitChanges();
            var o = DbUtil.Db.LoadOrganizationById(id);
            string notifyids = null;
            switch (field)
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
                t.PersonTags.Add(new TagPerson { PeopleId = pid });
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
            switch (field)
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
            var view = ViewExtensions2.RenderPartialViewToString2(this, "Other/NotifyList2", notifyids);
            return Content(view);
            //return View("NotifyList2", notifyids);
        }

        [HttpPost]
        public ActionResult Reminders(int id, bool? emailall)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var m = new CmsData.API.APIOrganization(DbUtil.Db);
            try
            {
                if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                    m.SendVolunteerReminders(id, emailall ?? false);
                else
                    m.SendEventReminders(id);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }
    }
}