using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "MoveOrgMembers"), Route("{action}/{id?}")]
    public class MoveOrgMembersController : CmsStaffController
    {
        public MoveOrgMembersController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/MoveOrgMembers")]
        public ActionResult Index(MoveOrgMembersModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(MoveOrgMembersModel model)
        {
            model.UpdateLongRunningOp(CurrentDatabase, MoveOrgMembersModel.Op);

            if (model.Started.HasValue)
            {
                return View(model);
            }

            if (model.TargetId == 0)
            {
                return Content("!Target required");
            }

            DbUtil.LogActivity("Move Org Members");
            model.ProcessMove(CurrentDatabase);
            return View(model);
        }
    }
}
