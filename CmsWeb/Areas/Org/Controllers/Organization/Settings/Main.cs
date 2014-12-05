using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult Main(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/Main", m);
        }
        [HttpPost]
        public ActionResult MainEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/MainEdit", m);
        }
        [HttpPost]
        public ActionResult MainUpdate(OrganizationModel m)
        {
            if (m.Org.CampusId == 0)
                m.Org.CampusId = null;
            if (m.Org.OrganizationTypeId == 0)
                m.Org.OrganizationTypeId = null;
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Update OrgMain {0}".Fmt(m.Org.OrganizationName));
            return PartialView("Settings/Main", m);
        }
    }
}