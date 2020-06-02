using CmsData;
using CmsData.Codes;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
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
        public ActionResult PaymentMethodsList()
        {
            var paymentMethods = new PaymentMethodTypeCode();
            var list = paymentMethods.GetType().GetFields().Select(p => new { id = p.GetValue(paymentMethods), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        [Route("~/Create")]
        public ActionResult PaymentMethodsCreate(string bankName = "", int? paymentTypeId = null, bool isDefault = true, string name = "",
            string nameOnAccount = "", string last4 = "", int? expiresMonth = null, int? expiresYear = null, int? processTypeId = null)
        {
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
                        BankName = "Test Bank",
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        VaultId = $"A{RandomNumber(1000000, 99999999)}",
                        NameOnAccount = nameOnAccount,
                        MaskedDisplay = "•••• •••• •••• 1234".PadLeft(1, '•'),
                        Last4 = last4,

                        ExpiresMonth = expiresMonth,
                        ExpiresYear = expiresYear,
                        GatewayAccountId = 1, // find class multiple gateway utils, use to determine GatewayAccountId, may be different based on pledge type
                    };
                    break;
                case 2: // Visa
                    paymentMethod = new PaymentMethod
                    {
                        BankName = "Test Bank",
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        ExpiresMonth = 10,
                        ExpiresYear = DateTime.Now.Year + 1,
                        GatewayAccountId = 1,
                        IsDefault = true,
                        Last4 = "1234",
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Name = "Visa Card",
                        //NameOnAccount = person.Name,
                        PaymentMethodTypeId = PaymentMethodTypeCode.Visa,
                        VaultId = $"A{RandomNumber(1000000, 99999999)}"
                    };
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
            
            return View();
        }

        [HttpPost]
        [Route("~/Delete/{id?}")]
        public ActionResult PaymentMethodsDelete()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PaymentSchedulesList()
        {
            var paymentSchedules = new ScheduledGiftTypeCode();
            var list = paymentSchedules.GetType().GetFields().Select(p => new { id = p.GetValue(paymentSchedules), p.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Create")]
        public ActionResult PaymentSchedulesCreate()
        {
            return View();
        }

        [HttpPost]
        [Route("~/Update")]
        public ActionResult PaymentSchedulesUpdate()
        {
            return View();
        }

        [HttpPost]
        [Route("~/Delete/{id?}")]
        public ActionResult PaymentSchedulesDelete()
        {
            return View();
        }
    }
}
