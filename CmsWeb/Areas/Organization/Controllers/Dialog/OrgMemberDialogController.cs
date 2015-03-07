using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using CmsWeb.Areas.Org2.Dialog.Models;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Controllers
{
    [RouteArea("Organization", AreaPrefix = "OrgMemberDialog")]
    public class OrgMemberDialogController : CmsStaffController
    {
        [HttpPost, Route("Display/{oid}/{pid}")]
        public ActionResult Display(int oid, int pid)
        {
            var m = new OrgMemberModel(oid, pid);
            return View("Display", m);
        }
        [HttpPost, Route("SmallGroupChecked/{oid:int}/{pid:int}/{sgtagid:int}")]
        public ActionResult SmallGroupChecked(int oid, int pid, int sgtagid, bool ck)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == oid);
            if (om == null)
                return Content("error");
            if (ck)
                om.OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = sgtagid });
            else
            {
                var mt = om.OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == sgtagid);
                if (mt == null)
                    return Content("not found");
                DbUtil.Db.OrgMemMemTags.DeleteOnSubmit(mt);
            }
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }

        [HttpPost, Route("Edit/{oid:int}/{pid:int}")]
        public ActionResult Edit(int oid, int pid)
        {
            var m = new OrgMemberModel(oid, pid);
            return View(m);
        }
        [HttpPost, Route("Update")]
        public ActionResult Update(OrgMemberModel m)
        {
            try
            {
                m.UpdateModel();
            }
            catch (Exception)
            {
                ViewData["MemberTypes"] = CodeValueModel.ConvertToSelect(CodeValueModel.MemberTypeCodes(), "Id");
                return View("Edit", m);
            }
            return View("Display", m);
        }
        [HttpPost, Route("Drop")]
        public ActionResult Drop(OrgMemberModel m)
        {
            m.Drop();
            return View("Dropped", m);
        }
        [HttpPost, Route("Move/{oid:int}/{pid:int}")]
        public ActionResult Move(int oid, int pid)
        {
            var mm = new OrgMemberMoveModel { OrgId = oid, PeopleId = pid };
            return View(mm);
        }
        [HttpPost, Route("MoveResults")]
        public ActionResult MoveResults(OrgMemberMoveModel m)
        {
            return View("Move", m);
        }
        [HttpPost, Route("MoveSelect/{oid:int}/{pid:int}/{toid:int}")]
        public ActionResult MoveSelect(int oid, int pid, int toid)
        {
            var om1 = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == oid);
            var om2 = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                toid, om1.PeopleId, om1.MemberTypeId, DateTime.Now, om1.InactiveDate, om1.Pending ?? false);
            DbUtil.Db.UpdateMainFellowship(om2.OrganizationId);
            om2.EnrollmentDate = om1.EnrollmentDate;
            if (om2.EnrollmentDate.Value.Date == DateTime.Today)
                om2.EnrollmentDate = DateTime.Today; // force it to be midnight, so you can check them in.
            om2.TranId = om1.TranId;
            om2.ShirtSize = om1.ShirtSize;
            om2.Request = om1.Request;
            om2.Amount = om1.Amount;
            om2.UserData = om1.UserData;
            om1.Drop(DbUtil.Db);
            DbUtil.Db.SubmitChanges();
            return Content("moved");
        }
        public string HelpLink()
        {
            return "";
        }
        [HttpPost, Route("MissionSupport/{oid}/{pid}")]
        public ActionResult MissionSupport(int oid, int pid)
        {
            var m = new MissionSupportModel { OrgId = oid, PeopleId = pid };
            return View(m);
        }
        [HttpPost, Route("AddMissionSupport/{oid}/{pid}")]
        public ActionResult AddMissionSupport(int oid, int pid, MissionSupportModel m)
        {
            m.PostContribution();
            return View("MissionSupportDone", m);
        }
        [HttpPost, Route("AddTransaction/{oid}/{pid}")]
        public ActionResult AddTransaction(int oid, int pid)
        {
            var m = new OrgMemberTransactionModel { OrgId = oid, PeopleId = pid };
            return View(m);
        }
        [HttpPost, Route("AddFeeAdjustment/{oid}/{pid}")]
        public ActionResult AddFeeAdjustment(int oid, int pid)
        {
            var m = new OrgMemberTransactionModel { OrgId = oid, PeopleId = pid, AdjustFee = true};
            return View(m);
        }
        [HttpPost, Route("PostTransaction/{oid}/{pid}")]
        public ActionResult PostTransaction(int oid, int pid, OrgMemberTransactionModel m)
        {
            if (m.TransactionSummary != null && (m.Payment ?? 0) == 0)
                ModelState.AddModelError("Payment", "must have non zero value");
            if (m.TransactionSummary == null && (m.Amount ?? 0) == 0) 
                ModelState.AddModelError("Amount", "Initial Fee Must be > 0");
            if (!ModelState.IsValid)
                return View("AddTransaction", m);
            m.PostTransaction();
            return View("AddTransactionDone", m);
        }
        [HttpPost, Route("ShowDrop/{oid:int}/{pid:int}")]
        public ActionResult ShowDrop(int oid, int pid)
        {
            var m = new OrgMemberModel(oid, pid);
            return View(m);
        }
    }
}
