using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Giving.Models;
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

        static Random randomizer = new Random();
        public static int RandomNumber(int min = 0, int max = 65535)
        {
            return randomizer.Next(min, max);
        }

        [HttpGet]
        public ActionResult MethodsList()
        {
            var paymentMethods = new PaymentMethodTypeCode();
            var list = paymentMethods.GetType().GetFields().Select(p => new { id = p.GetValue(paymentMethods), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MethodsCreate(int? paymentTypeId = null, bool isDefault = true, string name = "", string nameOnAccount = "", string bankAccount = "",
            string bankRouting = "", string cardNumber = "", string ccv = "", string expiresMonth = null, string expiresYear = null, int? processTypeId = null)
        {
            if (!paymentTypeId.HasValue)
            {
                throw new HttpException(404, "No payment method type ID found.");
            }

            //transactionResponse = gateway.AuthCreditCard(pid, dollarAmt, CreditCard, Expires,
            //               "Recurring Giving Auth", 0, CVV, string.Empty,
            //               FirstName, LastName, Address, Address2, City, State, Country, Zip, Phone);
            // if we got this far that means the auth worked so now let's do a void for that auth.
            //var voidResponse = gateway.VoidCreditCardTransaction(transactionResponse.TransactionId);

            var paymentMethod = new PaymentMethod();
            switch (paymentTypeId)
            {
                case 1: // bank
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        VaultId = $"A{RandomNumber(1000000, 99999999)}",
                        NameOnAccount = nameOnAccount,
                        //MaskedDisplay = "•••• •••• •••• 1234".PadLeft(1, '•'),
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = bankAccount.Substring(bankAccount.Length - 4, 4),
                        ExpiresMonth = Convert.ToInt32(expiresMonth),
                        ExpiresYear = Convert.ToInt32(expiresYear),
                        GatewayAccountId = 1, // find class multiple gateway utils, use to determine GatewayAccountId, may be different based on pledge type
                    };
                    //person.PaymentMethods.Add(paymentMethod);
                    paymentMethod.Encrypt();
                    break;
                case 2: // Visa
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        VaultId = $"A{RandomNumber(1000000, 99999999)}",
                        NameOnAccount = nameOnAccount,
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = cardNumber.Substring(cardNumber.Length - 4, 4),
                        ExpiresMonth = Convert.ToInt32(expiresMonth),
                        ExpiresYear = Convert.ToInt32(expiresYear),
                        GatewayAccountId = 1,
                    };
                    paymentMethod.Encrypt();
                    break;
                case 3: // Mastercard
                    break;
                case 4: // Amex
                    break;
                case 5: // Discover
                    break;
                case 99: // Other
                    break;
                default:
                    break;
            }

            CurrentDatabase.PaymentMethods.InsertOnSubmit(paymentMethod);
            CurrentDatabase.SubmitChanges();

            return View();
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
