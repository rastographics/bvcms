using CmsData;
using CmsData.Finance;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using Elmah;
using Newtonsoft.Json;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPaymentModel : IDbBinder
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public GivingPaymentModel() { }
        public GivingPaymentModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        public CMSDataContext CurrentDatabase { get; set; }

        public Message CreateMethod(GivingPaymentViewModel viewModel)
        {
            if (viewModel.peopleId != CurrentDatabase.CurrentPeopleId)
                return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (viewModel.peopleId == null)
                return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (viewModel.paymentTypeId == null || viewModel.paymentTypeId == 0)
                return Message.createErrorReturn("No payment method type ID found.", Message.API_ERROR_PAYMENT_METHOD_TYPE_ID_NOT_FOUND);
            if (viewModel.firstName == "")
                return Message.createErrorReturn("First name required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            if (viewModel.lastName == "")
                return Message.createErrorReturn("First name required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            if (viewModel.paymentTypeId != 1)
            {
                if (viewModel.address == "")
                    return Message.createErrorReturn("Address required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                if (viewModel.city == "")
                    return Message.createErrorReturn("City required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                if (viewModel.state == "")
                    return Message.createErrorReturn("State required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                if (viewModel.country == "")
                    return Message.createErrorReturn("Country required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                if (viewModel.zip == "")
                    return Message.createErrorReturn("Zip required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.transactionTypeId == "")
                return Message.createErrorReturn("Transaction Type ID required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);

            var paymentMethod = new PaymentMethod();
            var cardValidation = new Message();
            var bankValidation = new Message();
            int currentPeopleId = 0;
            if (viewModel.incomingPeopleId == null)
            {
                currentPeopleId = (int)CurrentDatabase.UserPeopleId;
            }
            else
            {
                currentPeopleId = (int)viewModel.incomingPeopleId;
            }

            if (viewModel.testing == true)
            {
                switch (viewModel.paymentTypeId)
                {
                    case 1: // bank
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.bankAccount.Substring(viewModel.bankAccount.Length - 4, 4)
                        };
                        break;
                    case 2: // Visa
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        break;
                    case 3: // Mastercard
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        break;
                    case 4: // Amex
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        break;
                    case 5: // Discover
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        break;
                    case 99: // Other
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (viewModel.paymentTypeId)
                {
                    case 1: // bank
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.bankAccount.Substring(viewModel.bankAccount.Length - 4, 4)
                        };
                        bankValidation = PaymentValidator.ValidateBankAccountInfo(viewModel.bankAccount, viewModel.bankRouting);
                        if (bankValidation.error != 0)
                            return Message.createErrorReturn(bankValidation.data, bankValidation.error);
                        break;
                    case 2: // Visa
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        cardValidation = PaymentValidator.ValidateCreditCardInfo(viewModel.cardNumber, viewModel.cvv, viewModel.expiresMonth, viewModel.expiresYear);
                        if (cardValidation.error != 0)
                            return Message.createErrorReturn(cardValidation.data, cardValidation.error);
                        break;
                    case 3: // Mastercard
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        cardValidation = PaymentValidator.ValidateCreditCardInfo(viewModel.cardNumber, viewModel.cvv, viewModel.expiresMonth, viewModel.expiresYear);
                        if (cardValidation.error != 0)
                            return Message.createErrorReturn(cardValidation.data, cardValidation.error);
                        break;
                    case 4: // Amex
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        cardValidation = PaymentValidator.ValidateCreditCardInfo(viewModel.cardNumber, viewModel.cvv, viewModel.expiresMonth, viewModel.expiresYear);
                        if (cardValidation.error != 0)
                            return Message.createErrorReturn(cardValidation.data, cardValidation.error);
                        break;
                    case 5: // Discover
                        paymentMethod = new PaymentMethod
                        {
                            PeopleId = currentPeopleId,
                            PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                            IsDefault = viewModel.isDefault,
                            Name = viewModel.name,
                            NameOnAccount = viewModel.firstName + " " + viewModel.lastName,
                            MaskedDisplay = "•••• •••• •••• 1234",
                            Last4 = viewModel.cardNumber.Substring(viewModel.cardNumber.Length - 4, 4),
                            ExpiresMonth = Convert.ToInt32(viewModel.expiresMonth),
                            ExpiresYear = Convert.ToInt32(viewModel.expiresYear),
                        };
                        cardValidation = PaymentValidator.ValidateCreditCardInfo(viewModel.cardNumber, viewModel.cvv, viewModel.expiresMonth, viewModel.expiresYear);
                        if (cardValidation.error != 0)
                            return Message.createErrorReturn(cardValidation.data, cardValidation.error);
                        break;
                    case 99: // Other
                        break;
                    default:
                        break;
                }
            }

            var account = MultipleGatewayUtils.GetAccount(CurrentDatabase, PaymentProcessTypes.RecurringGiving);
            paymentMethod.GatewayAccountId = account.GatewayAccountId;
            var gateway = CurrentDatabase.Gateway(viewModel.testing, account, PaymentProcessTypes.RecurringGiving);

            if (viewModel.paymentTypeId == 1)
            {
                var type = PaymentType.Ach;
                gateway.StoreInVault(paymentMethod, type, null, null, viewModel.bankAccount, viewModel.bankRouting, null, null, viewModel.address, viewModel.address2, viewModel.city, viewModel.state, viewModel.country, viewModel.zip, viewModel.phone, viewModel.emailAddress);
            }
            else if (viewModel.paymentTypeId == 2 || viewModel.paymentTypeId == 3 || viewModel.paymentTypeId == 4 || viewModel.paymentTypeId == 5)
            {
                var expires = HelperMethods.FormatExpirationDate(Convert.ToInt32(viewModel.expiresMonth), Convert.ToInt32(viewModel.expiresYear));
                var dollarAmt = 1;
                var transactionResponse = gateway.AuthCreditCard(currentPeopleId, dollarAmt, viewModel.cardNumber, expires, "Recurring Giving Auth", 0, viewModel.cvv, string.Empty, viewModel.firstName, viewModel.lastName, viewModel.address, viewModel.address2, viewModel.city, viewModel.state, viewModel.country, viewModel.zip, viewModel.phone);
                if (transactionResponse.Approved == false)
                    return Message.createErrorReturn("Card authorization failed.", Message.API_ERROR_PAYMENT_METHOD_AUTHORIZATION_FAILED);
                else
                {
                    gateway.VoidCreditCardTransaction(transactionResponse.TransactionId);
                    var type = PaymentType.CreditCard;
                    gateway.StoreInVault(paymentMethod, type, viewModel.cardNumber, viewModel.cvv, null, null, Convert.ToInt32(viewModel.expiresMonth), Convert.ToInt32(viewModel.expiresYear), viewModel.address, viewModel.address2, viewModel.city, viewModel.state, viewModel.country, viewModel.zip, viewModel.phone, viewModel.emailAddress);
                }
            }
            else
                return Message.createErrorReturn("Payment method type not supported.", Message.API_ERROR_PAYMENT_METHOD_AUTHORIZATION_FAILED);

            paymentMethod.Encrypt();
            CurrentDatabase.PaymentMethods.InsertOnSubmit(paymentMethod);
            CurrentDatabase.SubmitChanges();
            return Message.successMessage("Payment method created.", Message.API_ERROR_NONE);
        }

        public Message DeleteMethod(Guid? paymentMethodId = null, int? peopleId = null)
        {
            if (peopleId != CurrentDatabase.CurrentPeopleId)
                return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (peopleId == null)
                return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (paymentMethodId == null)
                return Message.createErrorReturn("No payment method ID.", Message.API_ERROR_PAYMENT_METHOD_NOT_FOUND);

            var paymentMethod = CurrentDatabase.PaymentMethods.Where(p => p.PaymentMethodId == paymentMethodId).FirstOrDefault();
            if(paymentMethod == null)
                return Message.createErrorReturn("Payment method not found.", Message.API_ERROR_PAYMENT_METHOD_NOT_FOUND);

            var scheduledGiftList = CurrentDatabase.ScheduledGifts.Where(x => x.PaymentMethodId == paymentMethod.PaymentMethodId).ToList();
            if (scheduledGiftList.Count > 0)
                return Message.createErrorReturn("Please remove this payment method from all scheduled giving first.", Message.API_ERROR_PAYMENT_METHOD_IN_USE);
            else
            {
                CurrentDatabase.PaymentMethods.DeleteOnSubmit(paymentMethod);
                CurrentDatabase.SubmitChanges();
                return Message.successMessage("Payment method deleted.", Message.API_ERROR_NONE);
            }
        }

        public Message CreateSchedule(GivingPaymentViewModel viewModel)
        {
            if (viewModel.peopleId != CurrentDatabase.CurrentPeopleId)
                return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (viewModel.peopleId == null)
                return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (viewModel.scheduleTypeId == null || viewModel.scheduleTypeId == 0)
                return Message.createErrorReturn("No scheduled gift type ID found.", Message.API_ERROR_SCHEDULED_GIFT_TYPE_ID_NOT_FOUND);
            if (viewModel.start == null)
                return Message.createErrorReturn("No scheduled gift start date found.", Message.API_ERROR_SCHEDULED_GIFT_START_DATE_NOT_FOUND);
            if (viewModel.amount == null || viewModel.amount == 0 || viewModel.amount < 0)
                return Message.createErrorReturn("Contribution amount is null or a negative number.", Message.API_ERROR_SCHEDULED_GIFT_AMOUNT_NOT_FOUND);
            if (viewModel.paymentMethodId == null)
                return Message.createErrorReturn("No payment method ID.", Message.API_ERROR_PAYMENT_METHOD_NOT_FOUND);
            else
            {
                var paymentMethod = CurrentDatabase.PaymentMethods.Where(x => x.PaymentMethodId == viewModel.paymentMethodId).FirstOrDefault();
                if (paymentMethod == null)
                    return Message.createErrorReturn("Payment method not found.", Message.API_ERROR_PAYMENT_METHOD_NOT_FOUND);
            }
            if (viewModel.fundId == null || viewModel.fundId == 0)
                return Message.createErrorReturn("No fund ID found for scheduled gift.", Message.API_ERROR_SCHEDULED_GIFT_FUND_ID_NOT_FOUND);
            else
            {
                var contributionFund = CurrentDatabase.ContributionFunds.Where(x => x.FundId == viewModel.fundId).FirstOrDefault();
                if (contributionFund == null)
                    return Message.createErrorReturn("Contribution fund not found.", Message.API_ERROR_SCHEDULED_GIFT_FUND_ID_NOT_FOUND);
            }

            var scheduledGift = new ScheduledGift()
            {
                PeopleId = (int)CurrentDatabase.UserPeopleId,
                ScheduledGiftTypeId = (int)viewModel.scheduleTypeId,
                PaymentMethodId = (Guid)viewModel.paymentMethodId,
                IsEnabled = false,
                StartDate = (DateTime)viewModel.start.Value.Date,
                EndDate = viewModel.end?.Date
            };
            try
            {
                CurrentDatabase.ScheduledGifts.InsertOnSubmit(scheduledGift);
                CurrentDatabase.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Message.createErrorReturn("Could not create scheduled gift, database exception.", Message.API_ERROR_Database_Exception);
            }
            var scheduledGiftAmount = new ScheduledGiftAmount()
            {
                ScheduledGiftId = scheduledGift.ScheduledGiftId,
                FundId = (int)viewModel.fundId,
                Amount = (decimal)viewModel.amount
            };
            try
            {
                CurrentDatabase.ScheduledGiftAmounts.InsertOnSubmit(scheduledGiftAmount);
                CurrentDatabase.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Message.createErrorReturn("Could not create scheduled gift amount, database exception.", Message.API_ERROR_Database_Exception);
            }
            var givingPaymentScheduleItems = new GivingPaymentScheduleItems()
            {
                ScheduledGiftId = scheduledGift.ScheduledGiftId,
                PeopleId = scheduledGift.PeopleId,
                ScheduledGiftTypeId = scheduledGift.ScheduledGiftTypeId,
                PaymentMethodId = scheduledGift.PaymentMethodId,
                IsEnabled = scheduledGift.IsEnabled,
                StartDate = scheduledGift.StartDate,
                EndDate = scheduledGift.EndDate,
                ScheduledGiftAmountId = scheduledGiftAmount.ScheduledGiftAmountId,
                FundId = scheduledGiftAmount.FundId,
                Amount = scheduledGiftAmount.Amount
            };
            return Message.successMessage(JsonConvert.SerializeObject(givingPaymentScheduleItems));
        }

        public Message UpdateSchedule(GivingPaymentViewModel viewModel)
        {
            if (viewModel.peopleId != CurrentDatabase.CurrentPeopleId)
                return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (viewModel.peopleId == null)
                return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (viewModel.scheduledGiftId == null)
                return Message.createErrorReturn("No scheduled gift ID.", Message.API_ERROR_SCHEDULED_GIFT_NOT_FOUND);
            if (viewModel.scheduleTypeId == null || viewModel.scheduleTypeId == 0)
                return Message.createErrorReturn("No scheduled gift type ID found.", Message.API_ERROR_SCHEDULED_GIFT_TYPE_ID_NOT_FOUND);
            if (viewModel.start == null)
                return Message.createErrorReturn("No scheduled gift start date found.", Message.API_ERROR_SCHEDULED_GIFT_START_DATE_NOT_FOUND);
            if (viewModel.amount == null || viewModel.amount == 0 || viewModel.amount < 0)
                return Message.createErrorReturn("Contribution amount is null or a negative number.", Message.API_ERROR_SCHEDULED_GIFT_AMOUNT_NOT_FOUND);

            var scheduledGift = CurrentDatabase.ScheduledGifts.Where(s => s.ScheduledGiftId == viewModel.scheduledGiftId).FirstOrDefault();
            if (scheduledGift == null)
                return Message.createErrorReturn("Scheduled gift not found.", Message.API_ERROR_SCHEDULED_GIFT_NOT_FOUND);

            var scheduledGiftAmount = CurrentDatabase.ScheduledGiftAmounts.Where(sa => sa.ScheduledGiftId == scheduledGift.ScheduledGiftId).FirstOrDefault();
            if (scheduledGiftAmount == null)
                return Message.createErrorReturn("Scheduled gift amount not found.", Message.API_ERROR_SCHEDULED_GIFT_AMOUNT_NOT_FOUND);
            if (viewModel.paymentMethodId == null)
                return Message.createErrorReturn("No payment method ID.", Message.API_ERROR_PAYMENT_METHOD_NOT_FOUND);
            else
            {
                var paymentMethod = CurrentDatabase.PaymentMethods.Where(x => x.PaymentMethodId == viewModel.paymentMethodId).FirstOrDefault();
                if (paymentMethod == null)
                    return Message.createErrorReturn("Payment method not found.", Message.API_ERROR_PAYMENT_METHOD_NOT_FOUND);
            }

            if (viewModel.fundId == null || viewModel.fundId == 0)
                return Message.createErrorReturn("No fund ID found for scheduled gift.", Message.API_ERROR_SCHEDULED_GIFT_FUND_ID_NOT_FOUND);
            else
            {
                var contributionFund = CurrentDatabase.ContributionFunds.Where(x => x.FundId == viewModel.fundId).FirstOrDefault();
                if (contributionFund == null)
                    return Message.createErrorReturn("Contribution fund not found.", Message.API_ERROR_SCHEDULED_GIFT_FUND_ID_NOT_FOUND);
            }

            var newScheduledGiftAmount = new ScheduledGiftAmount();
            var updateScheduledGift = false;

            if (scheduledGift.ScheduledGiftTypeId != (int)viewModel.scheduleTypeId || scheduledGift.PaymentMethodId != (Guid)viewModel.paymentMethodId || scheduledGift.StartDate != (DateTime)viewModel.start.Value.Date || scheduledGift.EndDate != viewModel.end)
            {
                scheduledGift.ScheduledGiftTypeId = (int)viewModel.scheduleTypeId;
                scheduledGift.PaymentMethodId = (Guid)viewModel.paymentMethodId;
                scheduledGift.StartDate = (DateTime)viewModel.start.Value.Date;
                scheduledGift.EndDate = viewModel.end?.Date;
                updateScheduledGift = true;
            }
            if (scheduledGiftAmount.FundId != (int)viewModel.fundId || scheduledGiftAmount.Amount != (decimal)viewModel.amount)
            {
                newScheduledGiftAmount.ScheduledGiftId = scheduledGift.ScheduledGiftId;
                newScheduledGiftAmount.FundId = (int)viewModel.fundId;
                newScheduledGiftAmount.Amount = (decimal)viewModel.amount;
                CurrentDatabase.ScheduledGiftAmounts.DeleteOnSubmit(scheduledGiftAmount);
                CurrentDatabase.ScheduledGiftAmounts.InsertOnSubmit(newScheduledGiftAmount);
                updateScheduledGift = true;
            }
            else
            {
                newScheduledGiftAmount = null;
            }

            if (updateScheduledGift == true)
            {
                try
                {
                    CurrentDatabase.SubmitChanges();
                    var givingPaymentScheduleItems = new GivingPaymentScheduleItems()
                    {
                        ScheduledGiftId = scheduledGift.ScheduledGiftId,
                        PeopleId = scheduledGift.PeopleId,
                        ScheduledGiftTypeId = scheduledGift.ScheduledGiftTypeId,
                        PaymentMethodId = scheduledGift.PaymentMethodId,
                        IsEnabled = scheduledGift.IsEnabled,
                        StartDate = scheduledGift.StartDate,
                        EndDate = scheduledGift.EndDate,
                    };
                    switch (newScheduledGiftAmount)
                    {
                        case null:
                            if (scheduledGiftAmount != null)
                            {
                                givingPaymentScheduleItems.ScheduledGiftAmountId = scheduledGiftAmount.ScheduledGiftAmountId;
                                givingPaymentScheduleItems.FundId = scheduledGiftAmount.FundId;
                                givingPaymentScheduleItems.Amount = scheduledGiftAmount.Amount;
                            }
                            break;
                        default:
                            givingPaymentScheduleItems.ScheduledGiftAmountId = newScheduledGiftAmount.ScheduledGiftAmountId;
                            givingPaymentScheduleItems.FundId = newScheduledGiftAmount.FundId;
                            givingPaymentScheduleItems.Amount = newScheduledGiftAmount.Amount;
                            break;
                    }
                    return Message.successMessage(JsonConvert.SerializeObject(givingPaymentScheduleItems), Message.API_ERROR_NONE);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return Message.successMessage("Could not update scheduled gift, database exception.", Message.API_ERROR_Database_Exception);
                }
            }
            return Message.successMessage("No changes made.", Message.API_ERROR_NONE);
        }

        public Message DeleteSchedule(Guid? scheduledGiftId, int? peopleId = null)
        {
            if (peopleId != CurrentDatabase.CurrentPeopleId)
                return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (peopleId == null)
                return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            if (scheduledGiftId == null)
                return Message.createErrorReturn("No scheduled gift ID.", Message.API_ERROR_SCHEDULED_GIFT_NOT_FOUND);

            var scheduledGift = CurrentDatabase.ScheduledGifts.Where(s => s.ScheduledGiftId == scheduledGiftId).FirstOrDefault();
            if(scheduledGift == null)
                return Message.createErrorReturn("Scheduled gift not found.", Message.API_ERROR_SCHEDULED_GIFT_NOT_FOUND);

            var scheduledGiftAmount = CurrentDatabase.ScheduledGiftAmounts.Where(sa => sa.ScheduledGiftId == scheduledGift.ScheduledGiftId).FirstOrDefault();
            if(scheduledGiftAmount == null)
                return Message.createErrorReturn("Scheduled gift amount not found.", Message.API_ERROR_SCHEDULED_GIFT_AMOUNT_NOT_FOUND);

            try
            {
                CurrentDatabase.ScheduledGiftAmounts.DeleteOnSubmit(scheduledGiftAmount);
                CurrentDatabase.ScheduledGifts.DeleteOnSubmit(scheduledGift);
                CurrentDatabase.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Message.createErrorReturn("Could not delete scheduled gift, database exception.", Message.API_ERROR_Database_Exception);
            }

            var scheduledGiftList = (from sg in CurrentDatabase.ScheduledGifts
                                     join sga in CurrentDatabase.ScheduledGiftAmounts on sg.ScheduledGiftId equals sga.ScheduledGiftId
                                     where sg.PeopleId == CurrentDatabase.UserPeopleId
                                     select new GivingPaymentScheduleItems
                                     {
                                         ScheduledGiftId = sg.ScheduledGiftId,
                                         PeopleId = sg.PeopleId,
                                         ScheduledGiftTypeId = sg.ScheduledGiftTypeId,
                                         PaymentMethodId = sg.PaymentMethodId,
                                         IsEnabled = sg.IsEnabled,
                                         StartDate = sg.StartDate,
                                         EndDate = sg.EndDate,
                                         ScheduledGiftAmountId = sga.ScheduledGiftAmountId,
                                         FundId = sga.FundId,
                                         Amount = sga.Amount
                                     }).ToList();
            return Message.successMessage(JsonConvert.SerializeObject(scheduledGiftList));
        }
    }
    public class GivingPaymentScheduleItems
    {
        public Guid ScheduledGiftId { get; set; }
        public int PeopleId { get; set; }
        public int ScheduledGiftTypeId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int ScheduledGiftAmountId { get; set; }
        public int FundId { get; set; }
        public decimal Amount { get; set; }
    }
}
