using System.Web.Mvc;
using CmsWeb.Areas.Dialog.Models;
using CmsData;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix="RepairTransactions"), Route("{action}/{id?}")]
    public class RepairTransactionsController : CmsStaffController
    {
        [HttpPost, Route("~/RepairTransactions/{id:int}")]
        public ActionResult Index(int id)
        {
            var model = new RepairTransactions(id);
            model.RemoveExistingLop(DbUtil.Db, RepairTransactions.Op, model.QueryId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(RepairTransactions model)
        {
            model.UpdateLongRunningOp(DbUtil.Db, RepairTransactions.Op);

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"Add to org from tag for {Session["ActiveOrganization"]}");
                model.Process(DbUtil.Db);
            }
            return View(model);
        }
    }
}
