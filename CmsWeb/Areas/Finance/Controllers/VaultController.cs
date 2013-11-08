using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;

namespace CmsWeb.Areas.Finance.Controllers
{
	[Authorize(Roles = "Admin")]
    public class VaultController : CmsController
    {
		[HttpPost]
		public ActionResult DeleteVaultData(int id)
		{
			var sage = new SagePayments(DbUtil.Db, testing: true);
			sage.deleteVaultData(id);
			var p = DbUtil.Db.LoadPersonById(id);
			DbUtil.Db.RecurringAmounts.DeleteAllOnSubmit(p.RecurringAmounts);
			var mg = p.ManagedGiving();
			if (mg != null)
				DbUtil.Db.ManagedGivings.DeleteOnSubmit(mg);
			var pi = p.PaymentInfo();
			if (pi != null)
				DbUtil.Db.PaymentInfos.DeleteOnSubmit(pi);
			DbUtil.Db.SubmitChanges();
			return Content("ok");
		}

    }
}
