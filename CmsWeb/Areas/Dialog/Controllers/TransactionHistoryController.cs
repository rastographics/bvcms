using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    // todo: use bootstrap
    [RouteArea("Dialog", AreaPrefix = "TransactionHistory"), Route("{action}/{id?}")]
    public class TransactionHistoryController : CmsStaffController
    {
        public TransactionHistoryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/TransactionHistory/{id:int}/{oid:int}")]
        public ActionResult Index(int id, int oid)
        {
            var m = new TransactionHistoryModel(id, oid);
            ViewBag.orgid = oid;
            ViewBag.PeopleId = id;
            ViewBag.IsMember = CurrentDatabase.OrganizationMembers.Any(mm => mm.OrganizationId == oid && mm.PeopleId == id);
            return View(m);
        }
        [Route("~/TransactionHistory/Enrollment/{id:int}/{oid:int}")]
        public ActionResult TransHistory(int id, int oid)
        {
            var m = new TransactionHistoryModel(id, oid);
            ViewBag.orgid = oid;
            ViewBag.PeopleId = id;
            ViewBag.IsMember = CurrentDatabase.OrganizationMembers.Any(mm => mm.OrganizationId == oid && mm.PeopleId == id);
            return View(m);
        }
        [Route("Repair/{orgid:int}/{peopleid:int}")]
        public ActionResult Repair(int orgid, int peopleid)
        {
            CurrentDatabase.RepairEnrollmentTransaction(orgid, peopleid);
            var m = new TransactionHistoryModel(peopleid, orgid);
            return View("History", m.FetchHistory());
        }
        public ActionResult Delete(int id)
        {
            var t = CurrentDatabase.EnrollmentTransactions.Single(tt => tt.TransactionId == id);
            CurrentDatabase.DeleteEnrollmentTransaction(id);
            var m = new TransactionHistoryModel(t.PeopleId, t.OrganizationId);
            return View("History", m.FetchHistory());
        }
        [Route("DeleteAll/{orgid:int}/{peopleid:int}")]
        public ActionResult DeleteAll(int orgid, int peopleid)
        {
            var q = CurrentDatabase.EnrollmentTransactions.Where(tt => tt.OrganizationId == orgid && tt.PeopleId == peopleid);
            CurrentDatabase.EnrollmentTransactions.DeleteAllOnSubmit(q);
            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }
        [HttpPost]
        public ActionResult Edit(string id, DateTime value)
        {
            var iid = id.Substring(2).ToInt();
            var t = CurrentDatabase.EnrollmentTransactions.Single(tt => tt.TransactionId == iid);
            t.TransactionDate = value;
            CurrentDatabase.SubmitChanges();
            return Content(value.ToString("g"));
        }
    }
}
