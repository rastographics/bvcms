using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "OrgSearchDrop"), Route("{action}/{id?}")]
    public class OrgSearchDropController : CmsStaffController
    {
        public OrgSearchDropController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/OrgSearchDrop")]
        public ActionResult Index(OrgSearchModel m)
        {
            var model = new OrgSearchDrop(m);
            return View(model);
        }
        [HttpPost]
        public ActionResult Process(OrgSearchDrop model)
        {
            model.UpdateLongRunningOp(CurrentDatabase, OrgSearchDrop.Op);
            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"OrgSearchDrop {model.Count} Members from {model.OrgCount} Orgs");
                model.Process(CurrentDatabase);
            }
            return View(model);
        }
    }
}
