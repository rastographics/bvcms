using System.Web.Mvc;
using CmsWeb.Models;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Admin")]
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
