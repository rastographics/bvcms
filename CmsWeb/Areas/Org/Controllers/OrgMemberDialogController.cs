using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;

namespace CmsWeb.Areas.Org.Controllers
{    
    [RouteArea("Org", AreaUrl = "OrgMemberDialog2")]
    public class OrgMemberDialogController : CmsStaffController
    {
        [POST("OrgMemberDialog2/Display/{oid}/{pid}")]
        public ActionResult Display(int oid, int pid)
        {
            var m = new OrgMemberModel {OrgId = oid, PeopleId = pid};
            return View(m);
        }
        [POST("OrgMemberDialog2/SmallGroupChecked/{oid:int}/{pid:int}/{sgtagid:int}")]
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

        [POST("OrgMemberDialog2/Edit")]
        public ActionResult Edit(OrgMemberModel m)
        {
            return View(m);
        }
        [POST("OrgMemberDialog2/Update")]
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
        [POST("OrgMemberDialog2/Drop/{oid:int}/{pid:int}")]
        public ActionResult Drop(int oid, int pid)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == oid);
            if (om != null)
            {
                om.Drop(DbUtil.Db, addToHistory:true);
                DbUtil.Db.SubmitChanges();
            }
            return Content("dropped");
        }
        [POST("OrgMemberDialog2/Move/{oid:int}/{pid:int}")]
        public ActionResult Move(int oid, int pid)
        {
            var mm = new OrgMemberMoveModel {OrgId = oid, PeopleId = pid};
            mm.Pager.SetWithPageOnly("/OrgMemberDialog2/MoveResults", 1);
            return View(mm);
        }
        [POST("OrgMemberDialog2/MoveResults/{page}")]
        public ActionResult MoveResults(int page, OrgMemberMoveModel m)
        {
            m.Pager.SetWithPageOnly("/OrgMemberDialog2/MoveResults", page);
            return View("Move", m);
        }
        [POST("OrgMemberDialog2/MoveSelect/{oid:int}/{pid:int}/{toid:int}")]
        public ActionResult MoveSelect(int oid, int pid, int toid)
        {
            var om1 = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == oid);
            var om2 = CmsData.OrganizationMember.InsertOrgMembers(DbUtil.Db,
                toid, om1.PeopleId, om1.MemberTypeId, DateTime.Now, om1.InactiveDate, om1.Pending ?? false);
			DbUtil.Db.UpdateMainFellowship(om2.OrganizationId);
			om2.EnrollmentDate = om1.EnrollmentDate;
			if (om2.EnrollmentDate.Value.Date == DateTime.Today)
				om2.EnrollmentDate = DateTime.Today; // force it to be midnight, so you can check them in.
            om2.Request = om1.Request;
            om2.Amount = om1.Amount;
            om2.UserData = om1.UserData;
            om1.Drop(DbUtil.Db, addToHistory:true);
            DbUtil.Db.SubmitChanges();
            return Content("moved");
        }
        public string HelpLink()
        {
            return "";
        }
        [POST("OrgMemberDialog2/MissionSupport/{oid}/{pid}")]
        public ActionResult MissionSupport(int oid, int pid)
        {            
            var m = new MissionSupportModel {OrgId = oid, PeopleId = pid};
            return View(m);
        }
        [POST("OrgMemberDialog2/AddMissionSupport/{oid}/{pid}")]
        public ActionResult AddMissionSupport(int oid, int pid, MissionSupportModel m)
        {
            m.PostContribution();
            return View("MissionSupportDone", m);
        }

    }
}
