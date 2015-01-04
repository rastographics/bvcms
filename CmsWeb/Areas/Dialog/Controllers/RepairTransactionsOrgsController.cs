using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    [RouteArea("Dialog", AreaPrefix="RepairTransactionsOrgs"), Route("{action}/{id?}")]
    public class RepairTransactionsOrgsController : CmsStaffController
    {
        [HttpPost, Route("~/RepairTransactionsOrgs/{id:int}")]
        public ActionResult Index(int id, OrgSearchModel osm)
        {
            var model = new RepairTransactionsOrgs(id);
            var orgIds = osm.FetchOrgs().Select(oo => oo.OrganizationId).ToArray();
            model.Orgs = string.Join(",", orgIds);
            model.Count = orgIds.Length;
            model.RemoveExistingLop(DbUtil.Db, id, RepairTransactionsOrgs.Op);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(RepairTransactionsOrgs model)
        {
            model.UpdateLongRunningOp(DbUtil.Db, RepairTransactionsOrgs.Op);

            if (!model.Started.HasValue)
            { 
                DbUtil.LogActivity("Repair Transactions for Orgs");
                model.Process(DbUtil.Db);
            }
			return View(model);
		}
    }
}
