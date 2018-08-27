using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Twilio"), Route("{action}/{id?}")]
    public class TwilioController : CmsStaffController
    {
        [Route("~/Twilio")]
        public ActionResult Index( int activeTab = 0 )
        {
            ViewBag.Tab = activeTab;
            return View();
        }

        public ActionResult GroupCreate( string name, string description, bool systemFlag )
        {
            var n = new SMSGroup();

            n.Name = name;
            n.Description = description;
            n.SystemFlag = systemFlag;

            DbUtil.Db.SMSGroups.InsertOnSubmit(n);
            DbUtil.Db.SubmitChanges();

            return RedirectToAction( "Index" );
        }

        public ActionResult GroupUpdate( int id, string name, string description, bool systemFlag)
        {
            var g = (from e in DbUtil.Db.SMSGroups
                     where e.Id == id
                     select e).Single();

            g.Name = name;
            g.Description = description;
            g.SystemFlag = systemFlag;

            DbUtil.Db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult GroupRemove(int id)
        {
            var g = (from e in DbUtil.Db.SMSGroups
                     where e.Id == id
                     select e).Single();

            var u = from e in DbUtil.Db.SMSGroupMembers
                    where e.GroupID == id
                    select e;

            var n = from e in DbUtil.Db.SMSNumbers
                    where e.GroupID == id
                    select e;

            DbUtil.Db.SMSNumbers.DeleteAllOnSubmit(n);
            DbUtil.Db.SMSGroupMembers.DeleteAllOnSubmit(u);
            DbUtil.Db.SMSGroups.DeleteOnSubmit(g);
            DbUtil.Db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult NumberAdd(int groupID, string newNumber)
        {
            var n = new SMSNumber();

            n.LastUpdated = DateTime.Now;
            n.GroupID = groupID;
            n.Number = newNumber;

            DbUtil.Db.SMSNumbers.InsertOnSubmit(n);
            DbUtil.Db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult NumberRemove(int id)
        {
            var n = (from e in DbUtil.Db.SMSNumbers
                     where e.Id == id
                     select e).First();

            DbUtil.Db.SMSNumbers.DeleteOnSubmit(n);
            DbUtil.Db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult UserAdd(int groupID, int userID)
        {
            var n = new SMSGroupMember();

            n.GroupID = groupID;
            n.UserID = userID;

            DbUtil.Db.SMSGroupMembers.InsertOnSubmit(n);
            DbUtil.Db.SubmitChanges();

            return Redirect("/Twilio");
        }

        public ActionResult UserRemove(int id)
        {
            var p = (from e in DbUtil.Db.SMSGroupMembers
                     where e.Id == id
                     select e).First();

            DbUtil.Db.SMSGroupMembers.DeleteOnSubmit(p);
            DbUtil.Db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Dialog(int id = 0, string viewName = "")
        {
            ViewBag.ID = id;
            return View(viewName);
        }
    }
}
