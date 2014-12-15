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
        public ActionResult CurrMemberGrid(OrgPeopleModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            m.GroupSelect = GroupSelectCode.Member;
            ViewBag.OrgMemberContext = true;
            ViewBag.orgname = Session["ActiveOrganization"] + " - Members";
            return PartialView("Tabs/Current",m);
        }
        [HttpPost]
        public ActionResult PendingMemberGrid(OrgPeopleModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            m.GroupSelect = GroupSelectCode.Pending;
            ViewBag.orgname = Session["ActiveOrganization"] + " - Pending Members";
            return PartialView("Tabs/Pending", m);
        }
        [HttpPost]
        public ActionResult InactiveMemberGrid(OrgPeopleModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            m.GroupSelect = GroupSelectCode.Inactive;
            DbUtil.LogActivity("Viewing Inactive for {0}".Fmt(Session["ActiveOrganization"]));
            ViewBag.orgname = Session["ActiveOrganization"] + " - Inactive Members";
            return PartialView("Tabs/Inactive", m);
        }
        [HttpPost]
        public ActionResult PrevMemberGrid(OrgPeopleModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            m.GroupSelect = GroupSelectCode.Previous;
            ViewBag.orgname = Session["ActiveOrganization"] + " - Previous Members";
            DbUtil.LogActivity("Viewing Prev Members for {0}".Fmt(Session["ActiveOrganization"]));
            return PartialView("Tabs/Previous", m);
        }
        public ActionResult DialogAdd(int id, string type)
        {
            ViewBag.OrgID = id;
            return View("DialogAdd" + type);
        }
        public ActionResult ReGenPaylinks(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var q = from om in org.OrganizationMembers
                    select om;

            foreach (var om in q)
            {
                if (!om.TranId.HasValue) continue;
                var estr = HttpUtility.UrlEncode(Util.Encrypt(om.TranId.ToString()));
                var link = Util.ResolveServerUrl("/OnlineReg/PayAmtDue?q=" + estr);
                om.PayLink = link;
            }
            DbUtil.Db.SubmitChanges();
            return View("Other/ReGenPaylinks", org);
        }
        
    }
}