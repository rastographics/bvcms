using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System;
using System.Web.Mvc;
using UtilityExtensions;

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
            var model = new OrgDrop(CurrentDatabase, qid);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(OrgDrop model)
        {
            model.Host = CurrentDatabase.Host;
            model.UserId = Util.UserId;
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
            var model = new OrgDrop(CurrentDatabase);
            model.DropSingleMember(orgId, peopleId);
            return Content("ok");
        }
    }
}
