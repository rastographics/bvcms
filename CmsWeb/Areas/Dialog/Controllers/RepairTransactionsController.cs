using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "RepairTransactions"), Route("{action}/{id?}")]
    public class RepairTransactionsController : CmsStaffController
    {
        public RepairTransactionsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/RepairTransactions/{id:int}")]
        public ActionResult Index(int id)
        {
            var model = new RepairTransactions(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(RepairTransactions model)
        {
            model.UpdateLongRunningOp(CurrentDatabase, RepairTransactions.Op);

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"Repair Transactions for {Session["ActiveOrganization"]}");
                model.Process(CurrentDatabase);
            }
            return View(model);
        }
    }
}
