using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix="ValidateAddress"), Route("{action}/{id?}")]
    public class ValidateAddressController : CmsStaffController
    {
        [HttpPost, Route("~/ValidateAddress")]
        public ActionResult Index()
        {
            var model = new ValidateAddress();
            return View(model);
        }
        [HttpPost]
        public ActionResult Process(ValidateAddress model)
        {
            model.Validate(ModelState);

            if(!ModelState.IsValid) // show validation errors
                return View("Index", model);

            model.UpdateLongRunningOp(DbUtil.Db, ValidateAddress.Op);
            if(model.ShowCount(DbUtil.Db))
                return View("Index", model); // let them confirm by seeing the count and the tagname

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity("Validate Address");
                model.Process(DbUtil.Db);
            }

			return View(model);
		}
    }
}
