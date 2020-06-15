using CmsData;
using CmsData.Codes;
using CmsData.Finance;
using CmsWeb.Areas.Giving.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Areas.Giving.Controllers
{
    [Authorize(Roles = "Admin,Finance,FinanceViewOnly")]
    [RouteArea("GivingPayment", AreaPrefix = "GivingPayment"), Route("{action}/{id?}")]
    public class GivingPaymentController : CmsStaffController
    {
        public GivingPaymentController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult MethodsList()
        {
            var paymentMethods = new PaymentMethodTypeCode();
            var list = paymentMethods.GetType().GetFields().Select(p => new { id = p.GetValue(paymentMethods), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MethodsCreate(GivingPaymentViewModel viewModel)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.CreateMethod(viewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MethodsDelete(Guid? paymentMethodId = null)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.DeleteMethod(paymentMethodId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SchedulesList()
        {
            var paymentSchedules = new ScheduledGiftTypeCode();
            var list = paymentSchedules.GetType().GetFields().Select(p => new { id = p.GetValue(paymentSchedules), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SchedulesCreate(GivingPaymentViewModel viewModel)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.CreateSchedule(viewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SchedulesUpdate(GivingPaymentViewModel viewModel)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.UpdateSchedule(viewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SchedulesDelete(Guid? scheduledGiftId)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.DeleteSchedule(scheduledGiftId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
