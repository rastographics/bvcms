using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix="OrgSearchDrop"), Route("{action}/{id?}")]
    public class OrgSearchDropController : CmsStaffController
    {
        [HttpPost, Route("~/OrgSearchDrop")]
        public ActionResult Index(OrgSearchModel m)
        {
            var model = new OrgSearchDrop(m);
            model.RemoveExistingLop(DbUtil.Db, Util.UserPeopleId ?? 0, OrgSearchDrop.Op);
            return View(model);
        }
        [HttpPost]
        public ActionResult Process(OrgSearchDrop model)
        {
            model.UpdateLongRunningOp(DbUtil.Db, OrgDrop.Op);
            if (!model.Started.HasValue)
            { 
                DbUtil.LogActivity("Drop Memers from Org {0}".Fmt(Session["ActiveOrganization"]));
                model.Process(DbUtil.Db);
            }
			return View(model);
		}
    }
}
