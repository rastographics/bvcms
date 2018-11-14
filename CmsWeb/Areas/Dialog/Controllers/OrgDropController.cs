using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "OrgDrop"), Route("{action}/{id?}")]
    public class OrgDropController : CmsStaffController
    {
        public OrgDropController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/OrgDrop/{qid:guid}")]
        public ActionResult Index(Guid qid)
        {
            LongRunningOperation.RemoveExisting(CurrentDatabase, qid);
            var model = new OrgDrop(qid);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(OrgDrop model)
        {
            model.UpdateLongRunningOp(CurrentDatabase, OrgDrop.Op);
            if (!model.Started.HasValue)
            {
                model.Process(CurrentDatabase);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DropSingleMember(int orgId, int peopleId)
        {
            var model = new OrgDrop();
            model.DropSingleMember(orgId, peopleId);
            return Content("ok");
        }
    }
}
