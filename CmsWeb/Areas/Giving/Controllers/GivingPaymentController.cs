using CmsData;
using CmsData.Codes;
using CmsData.Finance;
using CmsWeb.Areas.Giving.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Giving.Controllers
{
    [RouteArea("GivingPayment", AreaPrefix = "GivingPayment"), Route("{action}/{id?}")]
    public class GivingPaymentController : CmsStaffController
    {
        public GivingPaymentController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult MethodsList()
        {
            var paymentMethodTypes = new PaymentMethodTypeCode();
            var list = paymentMethodTypes.GetType().GetFields().Select(p => new { id = p.GetValue(paymentMethodTypes), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CurrentUserPaymentMethodsList()
        {
            var currentUserPaymentMethods = (from p in CurrentDatabase.PaymentMethods where p.PeopleId == CurrentDatabase.CurrentPeopleId select new { p.PaymentMethodId, p.PeopleId, p.PaymentMethodTypeId, p.IsDefault, p.Name, p.NameOnAccount, p.MaskedDisplay, p.Last4, p.ExpiresMonth, p.ExpiresYear }).ToList();
            var returnList = new List<PaymentMethod>();
            foreach (var item in currentUserPaymentMethods)
            {
                var paymentMethod = new PaymentMethod();
                paymentMethod.PaymentMethodId = item.PaymentMethodId;
                paymentMethod.PeopleId = item.PeopleId;
                paymentMethod.PaymentMethodTypeId = item.PaymentMethodTypeId;
                paymentMethod.IsDefault = item.IsDefault;
                paymentMethod.Name = item.Name;
                paymentMethod.NameOnAccount = Util.Decrypt(item.NameOnAccount);
                paymentMethod.MaskedDisplay = Util.Decrypt(item.MaskedDisplay);
                paymentMethod.Last4 = Util.Decrypt(item.Last4);
                paymentMethod.ExpiresMonth = item.ExpiresMonth;
                paymentMethod.ExpiresYear = item.ExpiresYear;
                returnList.Add(paymentMethod);
            }
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MethodsCreate(GivingPaymentViewModel viewModel)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.CreateMethod(viewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MethodsDelete(Guid? paymentMethodId = null, int? peopleId = null)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.DeleteMethod(paymentMethodId, peopleId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SchedulesList()
        {
            var paymentSchedules = new ScheduledGiftTypeCode();
            var list = paymentSchedules.GetType().GetFields().Select(p => new { id = p.GetValue(paymentSchedules), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CurrentUserSchedulesList()
        {
            var givingFrequencyList = (from t in CurrentDatabase.ScheduledGiftTypes orderby t.Id select new { Id = t.Id, Name = t.Description }).ToList();

            if (!CurrentDatabase.Setting("UseQuarterlyRecurring"))
                givingFrequencyList = givingFrequencyList.Where(f => f.Name != "Quarterly").ToList();

            if (!CurrentDatabase.Setting("UseAnnualRecurring"))
                givingFrequencyList = givingFrequencyList.Where(f => f.Name != "Annually").ToList();

            if (CurrentDatabase.Setting("HideBiWeeklyRecurring"))
                givingFrequencyList = givingFrequencyList.Where(f => f.Name != "Biweekly").ToList();

            if (CurrentDatabase.Setting("HideSemiMonthlyRecurring"))
                givingFrequencyList = givingFrequencyList.Where(f => f.Name != "Semi-monthly").ToList();

            return Json(givingFrequencyList, JsonRequestBehavior.AllowGet);
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
        public ActionResult SchedulesDelete(Guid? scheduledGiftId, int? peopleId = null)
        {
            var model = new GivingPaymentModel(CurrentDatabase);
            var result = model.DeleteSchedule(scheduledGiftId, peopleId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
