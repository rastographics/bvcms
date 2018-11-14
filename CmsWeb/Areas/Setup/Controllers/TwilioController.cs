using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Twilio"), Route("{action}/{id?}")]
    public class TwilioController : CmsStaffController
    {
        public TwilioController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Twilio")]
        public ActionResult Index(int activeTab = 0)
        {
            ViewBag.Tab = activeTab;
            return View();
        }

        public ActionResult GroupCreate(string name, string description, bool systemFlag)
        {
            var group = new SMSGroup
            {
                Name = name,
                Description = description,
                SystemFlag = systemFlag,
                IsDeleted = false
            };

            CurrentDatabase.SMSGroups.InsertOnSubmit(group);
            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult GroupUpdate(int id, string name, string description, bool systemFlag)
        {
            var g = (from e in CurrentDatabase.SMSGroups
                     where e.Id == id
                     select e).Single();

            g.Name = name;
            g.Description = description;
            g.SystemFlag = systemFlag;

            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult GroupHide(int groupId)
        {
            var group = (from e in CurrentDatabase.SMSGroups
                         where e.Id == groupId
                         select e).Single();
            group.IsDeleted = true;
            group.SystemFlag = false;
            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult GroupRemove(int id)
        {
            var g = (from e in CurrentDatabase.SMSGroups
                     where e.Id == id
                     select e).Single();

            var u = from e in CurrentDatabase.SMSGroupMembers
                    where e.GroupID == id
                    select e;

            var n = from e in CurrentDatabase.SMSNumbers
                    where e.GroupID == id
                    select e;

            CurrentDatabase.SMSNumbers.DeleteAllOnSubmit(n);
            CurrentDatabase.SMSGroupMembers.DeleteAllOnSubmit(u);
            CurrentDatabase.SMSGroups.DeleteOnSubmit(g);
            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult NumberAdd(int groupID, string newNumber)
        {
            var n = new SMSNumber();

            n.LastUpdated = DateTime.Now;
            n.GroupID = groupID;
            n.Number = newNumber;

            CurrentDatabase.SMSNumbers.InsertOnSubmit(n);
            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult NumberRemove(int id)
        {
            var n = (from e in CurrentDatabase.SMSNumbers
                     where e.Id == id
                     select e).First();

            CurrentDatabase.SMSNumbers.DeleteOnSubmit(n);
            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult UserAdd(int groupID, int userID)
        {
            var n = new SMSGroupMember();

            n.GroupID = groupID;
            n.UserID = userID;

            CurrentDatabase.SMSGroupMembers.InsertOnSubmit(n);
            CurrentDatabase.SubmitChanges();

            return Redirect("/Twilio");
        }

        public ActionResult UserRemove(int id)
        {
            var p = (from e in CurrentDatabase.SMSGroupMembers
                     where e.Id == id
                     select e).First();

            CurrentDatabase.SMSGroupMembers.DeleteOnSubmit(p);
            CurrentDatabase.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Dialog(int id = 0, string viewName = "")
        {
            ViewBag.ID = id;
            return View(viewName);
        }
    }
}
