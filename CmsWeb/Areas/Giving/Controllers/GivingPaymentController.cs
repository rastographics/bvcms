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

        public PaymentProcessTypes ProcessType { get; set; }
        [HttpPost]
        public ActionResult MethodsCreate(int? paymentTypeId = null, bool isDefault = true, string name = "", string firstName = "", string lastName = "", string bankAccount = "",
            string bankRouting = "", string cardNumber = "", string cvv = "", string expiresMonth = null, string expiresYear = null, string address ="", string address2 = "", string city = "",
            string state = "", string country = "", string zip = "", string phone = "", string transactionTypeId = "", string emailAddress = "")
        {
            if (paymentTypeId == null || paymentTypeId == 0)
            {
                return Models.Message.createErrorReturn("No payment method type ID found.", Models.Message.API_ERROR_PAYMENT_METHOD_TYPE_ID_NOT_FOUND);
            }
            if(firstName == "")
            {
                return Models.Message.createErrorReturn("First name required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (lastName == "")
            {
                return Models.Message.createErrorReturn("First name required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if(paymentTypeId != 1)
            {
                if (address == "")
                {
                    return Models.Message.createErrorReturn("Address required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (city == "")
                {
                    return Models.Message.createErrorReturn("City required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (state == "")
                {
                    return Models.Message.createErrorReturn("State required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (country == "")
                {
                    return Models.Message.createErrorReturn("Country required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (zip == "")
                {
                    return Models.Message.createErrorReturn("Zip required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
            }
            if (transactionTypeId == "")
            {
                return Models.Message.createErrorReturn("Transaction Type ID required.", Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }

            var paymentMethod = new PaymentMethod();
            var cardValidation = new Message();
            var bankValidation = new Message();
            switch (paymentTypeId)
            {
                case 1: // bank
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        NameOnAccount = firstName + " " + lastName,
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = bankAccount.Substring(bankAccount.Length - 4, 4)
                    };
                    bankValidation = PaymentValidator.ValidateBankAccountInfo(bankAccount, bankRouting);
                    if (bankValidation.error != 0)
                    {
                        return Models.Message.createErrorReturn(bankValidation.data, bankValidation.error);
                    }
                    break;
                case 2: // Visa
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        NameOnAccount = firstName + " " + lastName,
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = cardNumber.Substring(cardNumber.Length - 4, 4),
                        ExpiresMonth = Convert.ToInt32(expiresMonth),
                        ExpiresYear = Convert.ToInt32(expiresYear),
                    };
                    //cardValidation = PaymentValidator.ValidateCreditCardInfo(cardNumber, cvv, expiresMonth, expiresYear);
                    //if(cardValidation.error != 0)
                    //{
                    //    return Models.Message.createErrorReturn(cardValidation.data, cardValidation.error);
                    //}
                    break;
                case 3: // Mastercard
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        NameOnAccount = firstName + " " + lastName,
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = cardNumber.Substring(cardNumber.Length - 4, 4),
                        ExpiresMonth = Convert.ToInt32(expiresMonth),
                        ExpiresYear = Convert.ToInt32(expiresYear),
                    };
                    cardValidation = PaymentValidator.ValidateCreditCardInfo(cardNumber, cvv, expiresMonth, expiresYear);
                    if (cardValidation.error != 0)
                    {
                        return Models.Message.createErrorReturn(cardValidation.data, cardValidation.error);
                    }
                    break;
                case 4: // Amex
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        NameOnAccount = firstName + " " + lastName,
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = cardNumber.Substring(cardNumber.Length - 4, 4),
                        ExpiresMonth = Convert.ToInt32(expiresMonth),
                        ExpiresYear = Convert.ToInt32(expiresYear),
                    };
                    cardValidation = PaymentValidator.ValidateCreditCardInfo(cardNumber, cvv, expiresMonth, expiresYear);
                    if (cardValidation.error != 0)
                    {
                        return Models.Message.createErrorReturn(cardValidation.data, cardValidation.error);
                    }
                    break;
                case 5: // Discover
                    paymentMethod = new PaymentMethod
                    {
                        PeopleId = (int)CurrentDatabase.UserPeopleId,
                        PaymentMethodTypeId = (int)paymentTypeId,
                        IsDefault = isDefault,
                        Name = name,
                        NameOnAccount = firstName + " " + lastName,
                        MaskedDisplay = "•••• •••• •••• 1234",
                        Last4 = cardNumber.Substring(cardNumber.Length - 4, 4),
                        ExpiresMonth = Convert.ToInt32(expiresMonth),
                        ExpiresYear = Convert.ToInt32(expiresYear),
                    };
                    cardValidation = PaymentValidator.ValidateCreditCardInfo(cardNumber, cvv, expiresMonth, expiresYear);
                    if (cardValidation.error != 0)
                    {
                        return Models.Message.createErrorReturn(cardValidation.data, cardValidation.error);
                    }
                    break;
                case 99: // Other
                    break;
                default:
                    break;
            }

            var account = MultipleGatewayUtils.GetAccount(CurrentDatabase, PaymentProcessTypes.RecurringGiving);
            paymentMethod.GatewayAccountId = account.GatewayAccountId;

            var currentPeopleId = CurrentDatabase.UserPeopleId;
            var testing = true;
            var gateway = CurrentDatabase.Gateway(testing, account, PaymentProcessTypes.RecurringGiving);
            var dollarAmt = 1;

            if(paymentTypeId == 1)
            {
                var type = PaymentType.Ach;
                gateway.StoreInVault(paymentMethod, type, null, null, bankAccount, bankRouting, null, null, address, address2, city, state, country, zip, phone, emailAddress);
            }
            else if (paymentTypeId == 2 || paymentTypeId == 3 || paymentTypeId == 4 || paymentTypeId == 5)
            {
                var expires = HelperMethods.FormatExpirationDate(Convert.ToInt32(expiresMonth), Convert.ToInt32(expiresYear));
                var transactionResponse = gateway.AuthCreditCard((int)currentPeopleId, dollarAmt, cardNumber, expires, "Recurring Giving Auth", 0, cvv, string.Empty, firstName, lastName, address, address2, city, state, country, zip, phone);
                if(transactionResponse.Approved == false)
                {
                    return Models.Message.createErrorReturn("Card authorization failed.", Models.Message.API_ERROR_PAYMENT_METHOD_AUTHORIZATION_FAILED);
                }
                else
                {
                    var voidResponse = gateway.VoidCreditCardTransaction(transactionResponse.TransactionId);
                    var type = PaymentType.CreditCard;
                    gateway.StoreInVault(paymentMethod, type, cardNumber, cvv, null, null, Convert.ToInt32(expiresMonth), Convert.ToInt32(expiresYear), address, address2, city, state, country, zip, phone, emailAddress);
                }
            }
            else
            {
                return Models.Message.createErrorReturn("Payment method type not supported.", Models.Message.API_ERROR_PAYMENT_METHOD_AUTHORIZATION_FAILED);
            }

            paymentMethod.Encrypt();
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
