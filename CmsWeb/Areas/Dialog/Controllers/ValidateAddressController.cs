using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "ValidateAddress"), Route("{action}/{id?}")]
    public class ValidateAddressController : CmsStaffController
    {
        public ValidateAddressController(IRequestManager requestManager) : base(requestManager)
        {
        }

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

            if (!ModelState.IsValid) // show validation errors
            {
                return View("Index", model);
            }

            model.UpdateLongRunningOp(CurrentDatabase, ValidateAddress.Op);
            if (model.ShowCount(CurrentDatabase))
            {
                return View("Index", model); // let them confirm by seeing the count and the tagname
            }

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity("Validate Address");
                model.Process(CurrentDatabase);
            }

            return View(model);
        }
    }
}
