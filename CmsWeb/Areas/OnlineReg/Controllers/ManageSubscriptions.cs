using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        public ActionResult ManageSubscriptions(string id)
        {
            if (!id.HasValue())
                return Content("bad link");
            ManageSubsModel m;
            var td = TempData["PeopleId"];
            if (td != null)
                m = new ManageSubsModel(td.ToInt(), id.ToInt());
            else
            {
                var guid = id.ToGuid();
                if (guid == null)
                    return Content("invalid link");
                var ot = CurrentDatabase.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                if (ot == null)
                    return Content("invalid link");
                if (ot.Used)
                    return Content("link used");
                if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                    return Content("link expired");
                var a = ot.Querystring.Split(',');
                m = new ManageSubsModel(a[1].ToInt(), a[0].ToInt());
                id = a[0];
                ot.Used = true;
                CurrentDatabase.SubmitChanges();
            }
            m.Log("Start");
            SetHeaders(id.ToInt());
            return View("ManageSubscriptions/Choose", m);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfirmSubscriptions(ManageSubsModel m)
        {
            m.UpdateSubscriptions();

            var Staff = CurrentDatabase.StaffPeopleForOrg(m.masterorgid);

            var msg = CurrentDatabase.ContentHtml("ConfirmSubscriptions", Resource1.ConfirmSubscriptions);
            var orgname = m.Description();
            msg = msg.Replace("{org}", orgname).Replace("{details}", m.Summary);
            CurrentDatabase.Email(Staff.First().FromEmail, m.person, "Subscription Confirmation", msg);

            CurrentDatabase.Email(m.person.FromEmail, Staff, "Subscriptions managed",
                $@"{m.person.Name} managed subscriptions to {m.Description()}<br/>{m.Summary}");

            SetHeaders(m.masterorgid);
            m.Log("Confirm");
            return View("ManageSubscriptions/Confirm", m);
        }
    }
}
