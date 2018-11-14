using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Admin,Finance")]
    [RouteArea("Finance", AreaPrefix = "Vault"), Route("{action}/{id?}")]
    public class VaultController : CmsController
    {
        public VaultController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost]
        public ActionResult DeleteVaultData(int id)
        {
            var manageGiving = new ManageGivingModel();
            manageGiving.CancelManagedGiving(id);

            return Content("ok");
        }
    }
}
