using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult ProspectGrid(MemberModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            m.GroupSelect = GroupSelectCode.Prospect;
            DbUtil.LogActivity("Viewing Prospects for {0}".Fmt(Session["ActiveOrganization"]));
            ViewBag.orgname = Session["ActiveOrganization"] + " - Prospects";
            return PartialView("Tabs/Prospects", m);
        }
        [HttpPost, Route("AddProspect/{oid:int}/{pid:int}")]
        public ActionResult AddProspect(int oid, int pid)
        {
            var org = DbUtil.Db.LoadOrganizationById(oid);
            OrganizationMember.InsertOrgMembers(DbUtil.Db,
                oid, pid, MemberTypeCode.Prospect,
                DateTime.Now, null, false);
            DbUtil.LogActivity("Adding Prospect {0}({1})".Fmt(org.OrganizationName, pid));
            return Content("ok");
        }
        [HttpPost, Route("ShowProspect/{oid:int}/{pid:int}/{show}")]
        public ActionResult ShowProspect(int oid, int pid, string show)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(aa => aa.OrganizationId == oid && aa.PeopleId == pid);
            if(om == null)
                return Content("member not found");
            om.Hidden = show.Equal("hide");
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("ShowProspect {0},{1},{2}".Fmt(oid, pid, show));
            return Content("ok");
        }
    }
}