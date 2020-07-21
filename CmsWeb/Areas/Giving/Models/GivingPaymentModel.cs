using CmsData;
using CmsData.Finance;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using Elmah;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mail;
using UtilityExtensions;

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
            if(viewModel.testing == false)
            {
                if (viewModel.peopleId != CurrentDatabase.CurrentPeopleId)
                {
                    return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
                }
                if (viewModel.peopleId == null)
                {
                    return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
                }
            }
            if (viewModel.billingInfo == null)
            {
                return Message.createErrorReturn("Billing info object is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.firstName == "")
            {
                return Message.createErrorReturn("First name required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.lastName == "")
            {
                return Message.createErrorReturn("Last name required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.address == "")
            {
                return Message.createErrorReturn("Address required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.city == "")
            {
                return Message.createErrorReturn("City required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.state == "")
            {
                return Message.createErrorReturn("State required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.zip == "")
            {
                return Message.createErrorReturn("Zip required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.country == "")
            {
                return Message.createErrorReturn("Country required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.paymentTypeId == null || viewModel.paymentTypeId == 0)
            {
                return Message.createErrorReturn("No payment method type ID found.", Message.API_ERROR_PAYMENT_METHOD_TYPE_ID_NOT_FOUND);
            }
            if (viewModel.paymentTypeId == 1)
            {
                if (viewModel.bankInfo.accountName == null)
                {
                    return Message.createErrorReturn("Bank account name is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.bankInfo.accountNumber == null)
                {
                    return Message.createErrorReturn("Bank account number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.bankInfo.routingNumber == null)
                {
                    return Message.createErrorReturn("Bank account routing number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
            }
            else
            {
                if (viewModel.cardInfo.cardNumber == "")
                {
                    return Message.createErrorReturn("Card number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.cardInfo.expDateMonth == "")
                {
                    return Message.createErrorReturn("Card expiration month is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.cardInfo.expDateYear == "")
                {
                    return Message.createErrorReturn("Card expiration year is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.cardInfo.cardCode == "")
                {
                    return Message.createErrorReturn("Card CVV number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
            }

            var paymentMethod = new PaymentMethod();
            var cardValidation = new Message();
            var bankValidation = new Message();
            int currentPeopleId = 0;

            // this is for testing purposes
            if (viewModel.incomingPeopleId == null)
            {
                currentPeopleId = (int)CurrentDatabase.UserPeopleId;
            }
            else
            {
                currentPeopleId = (int)viewModel.incomingPeopleId;
            }

            if (viewModel.paymentTypeId == 1)
            {
                paymentMethod = new PaymentMethod
                {
                    PeopleId = currentPeopleId,
                    PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                    IsDefault = viewModel.isDefault,
                    Name = viewModel.bankInfo.accountName,
                    NameOnAccount = viewModel.billingInfo.firstName + " " + viewModel.billingInfo.lastName,
                    MaskedDisplay = "•••• •••• •••• 1234",
                    Last4 = viewModel.bankInfo.accountNumber.Substring(viewModel.bankInfo.accountNumber.Length - 4, 4)
                };
                if (viewModel.testing == false)
                {
                    bankValidation = PaymentValidator.ValidateBankAccountInfo(viewModel.bankInfo.accountNumber, viewModel.bankInfo.routingNumber);
                    if (bankValidation.error != 0)
                        return Message.createErrorReturn(bankValidation.data, bankValidation.error);
                }
            }
            else
            {
                paymentMethod = new PaymentMethod
                {
                    PeopleId = currentPeopleId,
                    PaymentMethodTypeId = (int)viewModel.paymentTypeId,
                    IsDefault = viewModel.isDefault,
                    Name = viewModel.cardInfo.accountName,
                    NameOnAccount = viewModel.billingInfo.firstName + " " + viewModel.billingInfo.lastName,
                    MaskedDisplay = "•••• •••• •••• 1234",
                    Last4 = viewModel.cardInfo.cardNumber.Substring(viewModel.cardInfo.cardNumber.Length - 4, 4),
                    ExpiresMonth = Convert.ToInt32(viewModel.cardInfo.expDateMonth),
                    ExpiresYear = Convert.ToInt32(viewModel.cardInfo.expDateYear),
                };
                if (viewModel.testing == false)
                {
                    cardValidation = PaymentValidator.ValidateCreditCardInfo(viewModel.cardInfo.cardNumber, viewModel.cardInfo.cardCode, viewModel.cardInfo.expDateMonth, viewModel.cardInfo.expDateYear);
                    if (cardValidation.error != 0)
                        return Message.createErrorReturn(cardValidation.data, cardValidation.error);
                }
            }

            var account = MultipleGatewayUtils.GetAccount(CurrentDatabase, PaymentProcessTypes.RecurringGiving);
            paymentMethod.GatewayAccountId = account.GatewayAccountId;
            var gateway = CurrentDatabase.Gateway(viewModel.testing, account, PaymentProcessTypes.RecurringGiving);

            if (viewModel.paymentTypeId == 1)
            {
                var type = PaymentType.Ach;
                gateway.StoreInVault(paymentMethod, type, null, null, viewModel.bankInfo.accountNumber, viewModel.bankInfo.routingNumber, null, null, viewModel.billingInfo.address, viewModel.billingInfo.address2, viewModel.billingInfo.city, viewModel.billingInfo.state, viewModel.billingInfo.country, viewModel.billingInfo.zip, viewModel.billingInfo.phone, viewModel.billingInfo.email, viewModel.testing);
            }
            else
            {
                var expires = HelperMethods.FormatExpirationDate(Convert.ToInt32(viewModel.cardInfo.expDateMonth), Convert.ToInt32(viewModel.cardInfo.expDateYear));
                var dollarAmt = 1;
                //var description = "Recurring Giving Auth";
                var description = "test transaction";
                var transactionResponse = gateway.AuthCreditCard(currentPeopleId, dollarAmt, viewModel.cardInfo.cardNumber, expires, description, 0, viewModel.cardInfo.cardCode, string.Empty, viewModel.billingInfo.firstName, viewModel.billingInfo.lastName, viewModel.billingInfo.address, viewModel.billingInfo.address2, viewModel.billingInfo.city, viewModel.billingInfo.state, viewModel.billingInfo.country, viewModel.billingInfo.zip, viewModel.billingInfo.phone, viewModel.testing);
                if (transactionResponse.Approved == true)
                {
                    gateway.VoidCreditCardTransaction(transactionResponse.TransactionId);
                    var type = PaymentType.CreditCard;
                    gateway.StoreInVault(paymentMethod, type, viewModel.cardInfo.cardNumber, viewModel.cardInfo.cardCode, null, null, Convert.ToInt32(viewModel.cardInfo.expDateMonth), Convert.ToInt32(viewModel.cardInfo.expDateYear), viewModel.billingInfo.address, viewModel.billingInfo.address2, viewModel.billingInfo.city, viewModel.billingInfo.state, viewModel.billingInfo.country, viewModel.billingInfo.zip, viewModel.billingInfo.phone, viewModel.billingInfo.email);
                }
                else
                {
                    return Message.createErrorReturn("Card authorization failed. Message: " + transactionResponse.Message, Message.API_ERROR_PAYMENT_METHOD_AUTHORIZATION_FAILED);
                }
            }

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
            if (viewModel.testing == false)
            {
                if (viewModel.peopleId != CurrentDatabase.CurrentPeopleId)
                    return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
                if (viewModel.peopleId == null)
                    return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
            }
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

            // this is for testing purposes
            int currentPeopleId = 0;
            if (viewModel.incomingPeopleId == null)
                currentPeopleId = (int)CurrentDatabase.UserPeopleId;
            else
                currentPeopleId = (int)viewModel.incomingPeopleId;

            var scheduledGift = new ScheduledGift()
            {
                PeopleId = currentPeopleId,
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
            return Message.successMessage(JsonConvert.SerializeObject(givingPaymentScheduleItems), Message.API_ERROR_NONE);
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
            return Message.successMessage(JsonConvert.SerializeObject(scheduledGiftList), Message.API_ERROR_NONE);
        }

        public Message ProcessOneTimePayment(GivingPaymentViewModel viewModel)
        {
            // checking for all required inputs
            #region
            if (viewModel.testing == false)
            {
                if (viewModel.peopleId != CurrentDatabase.CurrentPeopleId)
                {
                    return Message.createErrorReturn("People Id does not match the current people id.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
                }
                if (viewModel.peopleId == null)
                {
                    return Message.createErrorReturn("People Id is a required field, but is missing.", Message.API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND);
                }
            }
            if (viewModel.billingInfo == null)
            {
                return Message.createErrorReturn("Billing info object is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.firstName == "")
            {
                return Message.createErrorReturn("First name required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.lastName == "")
            {
                return Message.createErrorReturn("Last name required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.email == "")
            {
                return Message.createErrorReturn("Email required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.phone == "")
            {
                return Message.createErrorReturn("Phone number required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.address == "")
            {
                return Message.createErrorReturn("Address required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.city == "")
            {
                return Message.createErrorReturn("City required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.state == "")
            {
                return Message.createErrorReturn("State required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.zip == "")
            {
                return Message.createErrorReturn("Zip required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.billingInfo.country == "")
            {
                return Message.createErrorReturn("Country required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.gifts.Count == 0)
            {
                return Message.createErrorReturn("There must be a minimum of at least one gift.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (viewModel.givingPageId == null)
            {
                return Message.createErrorReturn("Giving page ID is required.", Message.API_ERROR_GIVING_PAGE_ID_NOT_FOUND);
            }
            if (viewModel.paymentTypeId == null || viewModel.paymentTypeId == 0)
            {
                return Message.createErrorReturn("No payment method type ID found.", Message.API_ERROR_PAYMENT_METHOD_TYPE_ID_NOT_FOUND);
            }
            if (viewModel.paymentTypeId == 1)
            {
                if (viewModel.bankInfo.accountName == null)
                {
                    return Message.createErrorReturn("Bank account name is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.bankInfo.accountNumber == null)
                {
                    return Message.createErrorReturn("Bank account number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.bankInfo.routingNumber == null)
                {
                    return Message.createErrorReturn("Bank account routing number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
            }
            else
            {
                if (viewModel.cardInfo.cardNumber == "")
                {
                    return Message.createErrorReturn("Card number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.cardInfo.expDateMonth == "")
                {
                    return Message.createErrorReturn("Card expiration month is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.cardInfo.expDateYear == "")
                {
                    return Message.createErrorReturn("Card expiration year is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
                if (viewModel.cardInfo.cardCode == "")
                {
                    return Message.createErrorReturn("Card CVV number is required.", Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
                }
            }
            #endregion

            // this is for testing purposes, currentPeopleId
            #region
            var currentPeopleId = 0;
            if (viewModel.testing == true)
            {
                currentPeopleId = (int)viewModel.incomingPeopleId;
            }
            #endregion

            // setting up gateway
            #region
            var account = MultipleGatewayUtils.GetAccount(CurrentDatabase, PaymentProcessTypes.OneTimeGiving);
            var gateway = CurrentDatabase.Gateway(viewModel.testing, account, PaymentProcessTypes.OneTimeGiving);
            #endregion

            // get totalAmount
            #region
            decimal totalAmount = 0;
            if (viewModel.gifts.Count == 1)
            {
                totalAmount = viewModel.gifts[0].amount;
            }
            else
            {
                foreach (var item in viewModel.gifts)
                {
                    totalAmount += item.amount;
                }
            }
            #endregion

            // execute payment and return results from gateway
            #region
            if (viewModel.paymentTypeId == 1)
            {
                var transactionResponseForBankPayment = gateway.ChargeBankAccountOneTime(totalAmount, viewModel.bankInfo.accountNumber, viewModel.bankInfo.routingNumber, viewModel.bankInfo.accountName, viewModel.bankInfo.nameOnAccount, viewModel.billingInfo.firstName, viewModel.billingInfo.lastName, viewModel.billingInfo.address, viewModel.billingInfo.address2, viewModel.billingInfo.city, viewModel.billingInfo.state, viewModel.billingInfo.country, viewModel.billingInfo.zip, viewModel.billingInfo.phone, viewModel.billingInfo.email, viewModel.testing);

                if (transactionResponseForBankPayment.Approved == true)
                {
                    foreach (var item in viewModel.gifts)
                    {
                        var contribution = new Contribution
                        {
                            CreatedBy = 0,
                            CreatedDate = DateTime.Now,
                            ContributionTypeId = 5,
                            ContributionDate = DateTime.Now,
                            ContributionAmount = item.amount,
                            ImageID = 0,
                            Origin = 0
                        };
                        var contributionFund = (from c in CurrentDatabase.ContributionFunds where c.FundId == item.fund.fundId select c).FirstOrDefault();
                        if (contributionFund != null)
                        {
                            contribution.FundId = item.fund.fundId;
                            if (contributionFund.Notes == true)
                            {
                                contribution.Notes = item.note;
                            }
                        }
                        else
                        {
                            contribution.FundId = CurrentDatabase.Setting("DefaultFundId", "1").ToInt();
                        }
                        if(viewModel.testing == true)
                        {
                            contribution.PeopleId = currentPeopleId;
                        }
                        CurrentDatabase.Contributions.InsertOnSubmit(contribution);
                        CurrentDatabase.SubmitChanges();
                    }
                    return Message.successMessage("Bank payment processed successfully.", Message.API_ERROR_NONE, totalAmount);
                }
                else
                {
                    return Message.createErrorReturn("Bank payment failed. Message: " + transactionResponseForBankPayment.Message, Message.API_ERROR_BANK_PAYMENT_FAILED);
                }
            }
            else
            {
                var expires = HelperMethods.FormatExpirationDate(Convert.ToInt32(viewModel.cardInfo.expDateMonth), Convert.ToInt32(viewModel.cardInfo.expDateYear));

                //var description = "Recurring Giving Auth";
                var description = "test transaction";
                var transactionResponse = gateway.AuthCreditCard(currentPeopleId, totalAmount, viewModel.cardInfo.cardNumber, expires, description, 0, viewModel.cardInfo.cardCode, string.Empty, viewModel.billingInfo.firstName, viewModel.billingInfo.lastName, viewModel.billingInfo.address, viewModel.billingInfo.address2, viewModel.billingInfo.city, viewModel.billingInfo.state, viewModel.billingInfo.country, viewModel.billingInfo.zip, viewModel.billingInfo.phone, viewModel.testing);

                if (transactionResponse.Approved == true)
                {
                    var transactionResponseForCreditCardPayment = gateway.ChargeCreditCardOneTime(totalAmount, viewModel.cardInfo.cardNumber, expires, viewModel.cardInfo.cardCode, viewModel.billingInfo.firstName, viewModel.billingInfo.lastName, viewModel.billingInfo.address, viewModel.billingInfo.address2, viewModel.billingInfo.city, viewModel.billingInfo.state, viewModel.billingInfo.country, viewModel.billingInfo.zip, viewModel.billingInfo.phone, viewModel.billingInfo.email, viewModel.testing);

                    if (transactionResponseForCreditCardPayment.Approved == true)
                    {
                        foreach (var item in viewModel.gifts)
                        {
                            var contribution = new Contribution
                            {
                                CreatedBy = 0,
                                CreatedDate = DateTime.Now,
                                ContributionTypeId = 5,
                                ContributionDate = DateTime.Now,
                                ContributionAmount = item.amount,
                                ImageID = 0,
                                Origin = 0
                            };
                            var contributionFund = (from c in CurrentDatabase.ContributionFunds where c.FundId == item.fund.fundId select c).FirstOrDefault();
                            if(contributionFund != null)
                            {
                                contribution.FundId = item.fund.fundId;
                                if (contributionFund.Notes == true)
                                {
                                    contribution.Notes = item.note;
                                }
                            }
                            else
                            {
                                contribution.FundId = CurrentDatabase.Setting("DefaultFundId", "1").ToInt();
                            }
                            if (viewModel.testing == true)
                            {
                                contribution.PeopleId = currentPeopleId;
                            }
                            CurrentDatabase.Contributions.InsertOnSubmit(contribution);
                            CurrentDatabase.SubmitChanges();
                        }

                        var givingPage = (from g in CurrentDatabase.GivingPages where g.GivingPageId == viewModel.givingPageId select g).FirstOrDefault();
                        //CurrentDatabase.SendEmail(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), CurrentDatabase.Setting("MobileQuickSignInCodeSubject", "Mobile Sign In Code"), body, mailAddresses);

                        return Message.successMessage("Credit card payment processed successfully.", Message.API_ERROR_NONE, totalAmount);
                    }
                    else
                    {
                        return Message.createErrorReturn("Card processing failed. Message: " + transactionResponse.Message, Message.API_ERROR_CREDIT_CARD_PAYMENT_FAILED);
                    }
                }
                else
                {
                    return Message.createErrorReturn("Card authorization failed. Message: " + transactionResponse.Message, Message.API_ERROR_CREDIT_CARD_AUTHORIZATION_FAILED);
                }
            }
            #endregion
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
