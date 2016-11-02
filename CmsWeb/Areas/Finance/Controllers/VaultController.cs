using System.Web.Mvc;
using CmsWeb.Areas.OnlineReg.Models;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Admin,Finance")]
    [RouteArea("Finance", AreaPrefix= "Vault"), Route("{action}/{id?}")]
    public class VaultController : CmsController
    {
        [HttpPost]
        public ActionResult DeleteVaultData(int id)
        {
            var manageGiving = new ManageGivingModel();
            manageGiving.CancelManagedGiving(id);

            return Content("ok");
        }
    }
}
