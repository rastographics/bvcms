using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AuthorizeNet;
using AuthorizeNet.APICore;
using UtilityExtensions;
using UtilityExtensions.Extensions;

namespace CmsData.Finance
{
    public class AuthorizeNetGateway : IGateway
    {
        private readonly string _login;
        private readonly string _key;
        private readonly CMSDataContext db;

        private bool IsLive { get; set; }
        private ServiceMode ServiceMode => IsLive ? ServiceMode.Live : ServiceMode.Test;

        public string GatewayType => "AuthorizeNet";

        public AuthorizeNetGateway(CMSDataContext db, bool testing)
        {
            this.db = db;
            IsLive = !(testing || db.Setting("GatewayTesting"));
            if (!IsLive)
            {
                _login = "4q2w5BD5";
                _key = "9wE4j7M372ehz6Fy";
            }
            else
            {
                _login = db.GetSetting("x_login", "");
                _key = db.GetSetting("x_tran_key", "");

                if (string.IsNullOrWhiteSpace(_login))
                    throw new Exception("x_login setting not found, which is required for Authorize.net.");

                if (string.IsNullOrWhiteSpace(_key))
                    throw new Exception("x_tran_key setting not found, which is required for Authorize.net.");
            }
        }

        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode,
            string routing, string account, bool giving)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
            {
                paymentInfo = new PaymentInfo();
                person.PaymentInfos.Add(paymentInfo);
            }

            var billToAddress = new AuthorizeNet.Address
            {
                First = paymentInfo.FirstName ?? person.FirstName,
                Last = paymentInfo.LastName ?? person.LastName,
                Street = paymentInfo.Address ?? person.PrimaryAddress,
                City = paymentInfo.City ?? person.PrimaryCity,
                State = paymentInfo.State ?? person.PrimaryState,
                Country = paymentInfo.Country ?? person.PrimaryCountry,
                Zip = paymentInfo.Zip ?? person.PrimaryZip,
                Phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone
            };

            Customer customer;

            if (paymentInfo.AuNetCustId == null) // create a new profilein Authorize.NET CIM
            {
                // NOTE: this can throw an error if the email address already exists...
                // TODO: Authorize.net needs to release a new Nuget package, because they don't have a clean way to pass in customer ID (aka PeopleId) yet... the latest code has a parameter for this, though
                //       - we could call UpdateCustomer after the fact to do this if we wanted to
                customer = CustomerGateway.CreateCustomer(person.EmailAddress, person.Name);
                customer.ID = peopleId.ToString();

                paymentInfo.AuNetCustId = customer.ProfileID.ToInt();
            }
            else
            {
                customer = CustomerGateway.GetCustomer(paymentInfo.AuNetCustId.ToString());
            }

            customer.BillingAddress = billToAddress;
            var isSaved = CustomerGateway.UpdateCustomer(customer);
            if (!isSaved)
                throw new Exception($"UpdateCustomer failed to save for {peopleId}");

            if (type == PaymentType.Ach)
            {
                SaveECheckToProfile(routing, account, paymentInfo, customer);

                paymentInfo.MaskedAccount = Util.MaskAccount(account);
                paymentInfo.Routing = Util.Mask(new StringBuilder(routing), 2);
            }
            else if (type == PaymentType.CreditCard)
            {
                var normalizeExpires = DbUtil.NormalizeExpires(expires);
                if (normalizeExpires == null)
                    throw new ArgumentException($"Can't normalize date {expires}", nameof(expires));

                var expiredDate = normalizeExpires.Value;

                SaveCreditCardToProfile(cardNumber, cardCode, expiredDate, paymentInfo, customer);

                paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
                paymentInfo.Expires = expires;
            }
            else
                throw new ArgumentException($"Type {type} not supported", nameof(type));

            if (giving)
                paymentInfo.PreferredGivingType = type;
            else
                paymentInfo.PreferredPaymentType = type;

            db.SubmitChanges();
        }

        private void SaveECheckToProfile(string routingNumber, string accountNumber, PaymentInfo paymentInfo, Customer customer)
        {
            if (accountNumber.StartsWith("X"))
                return;

            var foundPaymentProfile = customer.PaymentProfiles.SingleOrDefault(p => p.ProfileID == paymentInfo.AuNetCustPayBankId.ToString());

            var bankAccount = new BankAccount
            {
                accountType = BankAccountType.Checking,
                nameOnAccount = customer.Description,
                accountNumber = accountNumber,
                routingNumber = routingNumber
            };

            if (foundPaymentProfile == null)
            {
                var paymentProfileId = CustomerGateway.AddECheckBankAccount(customer.ProfileID, BankAccountType.Checking, routingNumber, accountNumber, customer.Description);
                paymentInfo.AuNetCustPayBankId = paymentProfileId.ToInt();
            }
            else
            {
                foundPaymentProfile.eCheckBankAccount = bankAccount;

                var isSaved = CustomerGateway.UpdatePaymentProfile(customer.ProfileID, foundPaymentProfile);
                if (!isSaved)
                    throw new Exception($"UpdatePaymentProfile failed to save credit card for {paymentInfo.PeopleId}");
            }
        }

        private void SaveCreditCardToProfile(string cardNumber, string cardCode, DateTime expires, PaymentInfo paymentInfo, Customer customer)
        {
            var foundPaymentProfile = customer.PaymentProfiles.SingleOrDefault(p => p.ProfileID == paymentInfo.AuNetCustPayId.ToString());

            if (foundPaymentProfile == null)
            {
                var paymentProfileId = CustomerGateway.AddCreditCard(customer.ProfileID, cardNumber,
                    expires.Month, expires.Year, cardCode, customer.BillingAddress);

                paymentInfo.AuNetCustPayId = paymentProfileId.ToInt();
            }
            else
            {
                if (!cardNumber.StartsWith("X"))
                    foundPaymentProfile.CardNumber = cardNumber;

                if (cardCode != null && !cardCode.StartsWith("X"))
                    foundPaymentProfile.CardCode = cardCode;

                foundPaymentProfile.CardExpiration = expires.ToString("MMyy");

                var isSaved = CustomerGateway.UpdatePaymentProfile(customer.ProfileID, foundPaymentProfile);
                if (!isSaved)
                    throw new Exception($"UpdatePaymentProfile failed to save echeck for {paymentInfo.PeopleId}");
            }
        }

        public void RemoveFromVault(int peopleId)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return;

            if (CustomerGateway.DeleteCustomer(paymentInfo.AuNetCustId.ToString()))
            {
                paymentInfo.AuNetCustId = null;
                paymentInfo.AuNetCustPayId = null;
                paymentInfo.AuNetCustPayBankId = null;
                paymentInfo.MaskedCard = null;
                paymentInfo.MaskedAccount = null;
                paymentInfo.Expires = null;
                db.SubmitChanges();
            }
            else
            {
                throw new Exception($"Failed to delete customer {peopleId}");
            }
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            return VoidRequest(reference);
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            return VoidRequest(reference);
        }

        private TransactionResponse VoidRequest(string reference)
        {
            var req = new VoidRequest(reference);
            var response = Gateway.Send(req);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "")
        {
            if (string.IsNullOrWhiteSpace(lastDigits))
                throw new ArgumentException("Last four of credit card number are required for refunds against Authorize.net", "lastDigits");

            var req = new CreditRequest(reference, amt, lastDigits);
            var response = Gateway.Send(req);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "")
        {
            if (string.IsNullOrWhiteSpace(lastDigits))
                throw new ArgumentException("Last four of bank account number are required for refunds against Authorize.net", "lastDigits");

            var req = new EcheckCreditRequest(reference, amt, lastDigits);
            var response = Gateway.Send(req);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description,
            int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state,
            string country, string zip, string phone)
        {
            var request = new AuthorizationRequest(cardnumber, expires, amt, description, includeCapture: false);

            request.AddCustomer(peopleId.ToString(), email, first, last, addr, city, state, zip);
            request.Country = country;
            request.CardCode = cardcode;
            request.Phone = phone;
            request.InvoiceNum = tranid.ToString();

            var response = Gateway.Send(request);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var request = new AuthorizationRequest(cardnumber, expires, amt, description, includeCapture: true);

            request.AddCustomer(peopleId.ToString(),email, first, last, addr, city, state, zip);
            request.Country = country;
            request.CardCode = cardcode;
            request.Phone = phone;
            request.InvoiceNum = tranid.ToString();

            var response = Gateway.Send(request);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var request = new EcheckRequest(EcheckType.WEB, amt, routing, acct, BankAccountType.Checking, null, first + " " + last, null);

            request.AddCustomer(peopleId.ToString(), email, first, last, addr, city, state, zip);
            request.Country = country;
            request.Phone = phone;
            request.InvoiceNum = tranid.ToString();

            var response = Gateway.Send(request);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid)
        {
            var paymentInfo = db.PaymentInfos.Single(pp => pp.PeopleId == peopleId);
            if (paymentInfo?.AuNetCustPayId == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            var paymentProfileId = paymentInfo.AuNetCustPayId.ToString();

            var order = new Order(paymentInfo.AuNetCustId.ToString(), paymentProfileId, null)
            {
                Description = description,
                Amount = amt,
                InvoiceNumber = tranid.ToString()
            };
            var response = CustomerGateway.Authorize(order);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            var paymentInfo = db.PaymentInfos.Single(pp => pp.PeopleId == peopleId);
            if (paymentInfo == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            string paymentProfileId;
            if (type == PaymentType.Ach)
                paymentProfileId = paymentInfo.AuNetCustPayBankId.ToString();
            else if (type == PaymentType.CreditCard)
                paymentProfileId = paymentInfo.AuNetCustPayId.ToString();
            else
                throw new ArgumentException($"Type {type} not supported", nameof(type));

            var order = new Order(paymentInfo.AuNetCustId.ToString(), paymentProfileId, null)
            {
                Description = description,
                Amount = amt,
                InvoiceNumber = tranid.ToString()
            };
            var response = CustomerGateway.AuthorizeAndCapture(order);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public BatchResponse GetBatchDetails(DateTime start, DateTime end)
        {
            var batchTransactions = new List<BatchTransaction>();

            foreach (var batch in ReportingGateway.GetSettledBatchList(start, end, includeStats: true))
            {
                foreach (var transaction in ReportingGateway.GetTransactionList(batch.ID))
                {
                    var batchTransaction = new BatchTransaction
                    {
                        TransactionId = transaction.InvoiceNumber.ToInt(),
                        Reference = transaction.TransactionID,
                        BatchReference = batch.ID,
                        TransactionType = GetTransactionType(transaction.Status),
                        BatchType = GetBatchType(batch.PaymentMethod),
                        Name = $"{transaction.FirstName} {transaction.LastName}",
                        Amount = transaction.SettleAmount,
                        Approved = IsApproved(batch.State),
                        Message = batch.State.ToUpper(),
                        TransactionDate = transaction.DateSubmitted,
                        SettledDate = batch.SettledOn,
                        LastDigits = transaction.CardNumber.Last(4)
                    };

                    if (!string.IsNullOrWhiteSpace(transaction.eCheckBankAccount?.accountNumber))
                        batchTransaction.LastDigits = transaction.eCheckBankAccount.accountNumber.Last(4);

                    batchTransactions.Add(batchTransaction);
                }
            }

            return new BatchResponse(batchTransactions);
        }

        private static BatchType GetBatchType(string paymentMethod)
        {
            var pm = paymentMethod.ParseEnum<paymentMethodEnum>();
            switch (pm)
            {
                case paymentMethodEnum.creditCard:
                    return BatchType.CreditCard;
                case paymentMethodEnum.eCheck:
                    return BatchType.Ach;
                default:
                    return BatchType.Unknown;
            }
        }

        private static TransactionType GetTransactionType(string transactionStatus)
        {
            var transType = transactionStatus.ParseEnum<transactionStatusEnum>();
            switch (transType)
            {
                case transactionStatusEnum.chargeback:
                case transactionStatusEnum.refundPendingSettlement:
                case transactionStatusEnum.refundSettledSuccessfully:
                case transactionStatusEnum.returnedItem:
                     return TransactionType.Refund;
                default:
                    return TransactionType.Charge;

            }
        }

        public ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end)
        {
            var returnedChecks = new List<ReturnedCheck>();
            foreach (var transaction in ReportingGateway.GetTransactionList(start, end).Where(t => t.HasReturnedItems == NullableBooleanEnum.True))
            {
                // we only get the first one, because I can't think of a reason there would ever be more than one.
                var returnedItem = transaction.ReturnedItems.FirstOrDefault();
                if (returnedItem != null)
                {
                    returnedChecks.Add(new ReturnedCheck
                    {
                        TransactionId = transaction.InvoiceNumber.ToInt(),
                        Name = $"{transaction.FirstName} {transaction.LastName}",
                        RejectCode = returnedItem.code,
                        RejectAmount = transaction.RequestedAmount, // another guess here on amount, I'm really not sure about this field.
                        RejectDate = returnedItem.dateLocal
                    });
                }
            }

            return new ReturnedChecksResponse(returnedChecks);
        }

        private static bool IsApproved(string batchSettlementState)
        {
            return batchSettlementState.ParseEnum<settlementStateEnum>() == settlementStateEnum.settledSuccessfully;
        }

        public bool CanVoidRefund => true;

        public bool CanGetSettlementDates => true;

        public bool CanGetBounces => false;

        private AuthorizeNet.IGateway _gateway;
        private AuthorizeNet.IGateway Gateway
        {
            get
            {
                if (_gateway == null)
                    _gateway = new Gateway(_login, _key, !IsLive);
                return _gateway;
            }
        }

        private ICustomerGateway _customerGateway;
        /// <summary>
        /// For "vault"-like functionality
        /// </summary>
        private ICustomerGateway CustomerGateway
        {
            get
            {
                if (_customerGateway == null)
                    _customerGateway = new CustomerGateway(_login, _key, ServiceMode);
                return _customerGateway;
            }
        }

        private IReportingGateway _reportingGateway;
        /// <summary>
        /// For batches, settlement, etc.
        /// </summary>
        private IReportingGateway ReportingGateway
        {
            get
            {
                if (_reportingGateway == null)
                    _reportingGateway = new ReportingGateway(_login, _key, ServiceMode);
                return _reportingGateway;
            }
        }

        public bool UseIdsForSettlementDates => false;
        public void CheckBatchSettlements(DateTime start, DateTime end)
        {
            CheckBatchedTransactions.CheckBatchSettlements(db, this, start, end);
        }

        public void CheckBatchSettlements(List<string> transactionids)
        {
            throw new NotImplementedException();
        }

        public string VaultId(int peopleId)
        {
            var paymentInfo = db.PaymentInfos.Single(pp => pp.PeopleId == peopleId);
            return paymentInfo?.AuNetCustId.ToString();
        }
    }
}
