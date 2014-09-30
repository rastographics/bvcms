using System;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Controllers
{
	[Authorize(Roles = "Admin")]
    [RouteArea("Finance", AreaPrefix= "Vault"), Route("{action}/{id?}")]
    public class VaultController : CmsController
    {
		[HttpPost]
		public ActionResult DeleteVaultData(int id)
		{
		    var db = DbUtil.Db;
		    var gw = db.Gateway();

		    try
		    {
                gw.RemoveFromVault(id);
		    }
		    catch (Exception)
		    {
		    }

		    var p = db.LoadPersonById(id);
			db.RecurringAmounts.DeleteAllOnSubmit(p.RecurringAmounts);
			var mg = p.ManagedGiving();
			if (mg != null)
				db.ManagedGivings.DeleteOnSubmit(mg);
			var pi = p.PaymentInfo();
			if (pi != null)
				db.PaymentInfos.DeleteOnSubmit(pi);
			db.SubmitChanges();
			return Content("ok");
		}

    }
}
