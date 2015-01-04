using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    // todo: use bootstrap
    [RouteArea("Dialog", AreaPrefix= "TransactionHistory"), Route("{action}/{id?}")]
    public class TransactionHistoryController : CmsStaffController
    {
        [Route("~/TransactionHistory/{id:int}/{oid:int}")]
        public ActionResult Index(int id, int oid)
        {
            var m = new TransactionHistoryModel(id, oid);
            ViewBag.orgid = oid;
            ViewBag.PeopleId = id;
            ViewBag.IsMember = DbUtil.Db.OrganizationMembers.Any(mm => mm.OrganizationId == oid && mm.PeopleId == id);
            return View(m);
        }
        public ActionResult Delete(int id)
        {
            var t = DbUtil.Db.EnrollmentTransactions.Single(tt => tt.TransactionId == id);
            var m = new TransactionHistoryModel(t.PeopleId, t.OrganizationId);
            DbUtil.Db.EnrollmentTransactions.DeleteOnSubmit(t);
            DbUtil.Db.SubmitChanges();
            return View("History", m.FetchHistory());
        }
        [Route("DeleteAll/{orgid:int}/{peopleid:int}")]
        public ActionResult DeleteAll(int orgid, int peopleid)
        {
            var q = DbUtil.Db.EnrollmentTransactions.Where(tt => tt.OrganizationId == orgid && tt.PeopleId == peopleid);
            DbUtil.Db.EnrollmentTransactions.DeleteAllOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
        [HttpPost]
        public ActionResult Edit(string id, DateTime value)
        {
            var iid = id.Substring(2).ToInt();
            var t = DbUtil.Db.EnrollmentTransactions.Single(tt => tt.TransactionId == iid);
            t.TransactionDate = value;
            DbUtil.Db.SubmitChanges();
            return Content(value.ToString("g"));
        }
    }
}
