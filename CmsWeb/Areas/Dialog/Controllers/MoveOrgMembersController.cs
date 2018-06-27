using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix="MoveOrgMembers"), Route("{action}/{id?}")]
    public class MoveOrgMembersController : CmsStaffController
    {
        [HttpPost, Route("~/MoveOrgMembers")]
        public ActionResult Index(MoveOrgMembersModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(MoveOrgMembersModel model)
        {
            model.UpdateLongRunningOp(DbUtil.Db, MoveOrgMembersModel.Op);

            if (!model.Started.HasValue)
            { 
                if (model.TargetId == 0)
                    return Content("!Target required");
                DbUtil.LogActivity("Move Org Members");
                model.ProcessMove(DbUtil.Db);
            }
			return View(model);
		}
    }
}
