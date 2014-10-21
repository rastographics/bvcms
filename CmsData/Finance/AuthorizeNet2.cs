using System;
using System.Linq;
using UtilityExtensions;
using AuthorizeNet;

namespace CmsData
{
    public class AuthorizeNet2 : IGateway
    {
        private readonly string _login;
        private readonly string _key;
        private readonly bool _testing;
        private readonly CMSDataContext db;

        private bool IsLive { get { return !_testing; } }
        private ServiceMode ServiceMode { get { return IsLive ? ServiceMode.Live : ServiceMode.Test; } }

        public string GatewayType { get { return "AuthorizeNet"; } }

        public AuthorizeNet2(CMSDataContext Db, bool testing)
        {
            _testing = testing || Db.Setting("GatewayTesting", "false").ToLower() == "true";

            db = Db;
            if (_testing)
            {
                _login = "9t8Pqzs4CW3S";
                _key = "9j33v58nuZB865WR";
            }
            else
            {
                _login = Db.Setting("x_login", "");
                _key = Db.Setting("x_tran_key", "");
            }
        }

        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode,
            string routing, string account, bool giving)
        {
            var normalizeExpires = DbUtil.NormalizeExpires(expires);
            if (normalizeExpires == null)
                throw new ArgumentException("Can't normalize date {0}".Fmt(expires), "expires");

            var expiredDate = normalizeExpires.Value;

            var person = db.LoadPersonById(peopleId);
            var billToAddress = new AuthorizeNet.Address
            {
                City = person.PrimaryCity,
                First = person.FirstName,
                Last = person.LastName,
                State = person.PrimaryState,
                Zip = person.PrimaryZip,
                Phone = person.HomePhone ?? person.CellPhone,
                Street = person.PrimaryAddress
            };

            Customer customer = null;

            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
            {
                paymentInfo = new PaymentInfo();
                person.PaymentInfos.Add(paymentInfo);
            }

            if (paymentInfo.AuNetCustId == null) // create a new profilein Authorize.NET CIM
            {
                // NOTE: this can throw an error if the email address already exists...
                // TODO: Authorize.net needs to release a new Nuget package, because they don't have a clean way to pass in customer ID (aka PeopleId) yet... the latest code has a parameter for this, though
                //       - we could call UpdateCustomer after the fact to do this if we wanted to
                customer = CustomerGateway.CreateCustomer(person.EmailAddress, person.Name);
                customer.ID = peopleId.ToString();

                // we only have to do this because we set the ID property and we want that saved...
                CustomerGateway.UpdateCustomer(customer);
            }

            if (type == "B")
                SaveECheckToProfile(routing, account, paymentInfo, customer, billToAddress);
            else if (type == "C")
                SaveCreditCardToProfile(cardNumber, cardCode, expiredDate, paymentInfo, customer, billToAddress);
            else
                throw new ArgumentException("Type {0} not supported".Fmt(type), "type");

            db.SubmitChanges();
        }

        private void SaveECheckToProfile(string routingNumber, string accountNumber, PaymentInfo paymentInfo, Customer customer, AuthorizeNet.Address billToAddress)
        {
            var foundPaymentProfile = customer.PaymentProfiles.SingleOrDefault(p => p.ProfileID == paymentInfo.AuNetCustPayBankId.ToString());

            var bankAccount = new BankAccount
            {
                accountType = BankAccountType.Checking,
                nameOnAccount = customer.Description
            };

            if (!accountNumber.StartsWith("X"))
                bankAccount.accountNumber = accountNumber;

            if (!routingNumber.StartsWith("X"))
                bankAccount.routingNumber = routingNumber;

            if (foundPaymentProfile == null)
            {
                var paymentProfileId = CustomerGateway.AddECheckBankAccount(customer.ProfileID, bankAccount, billToAddress);
                paymentInfo.AuNetCustPayBankId = paymentProfileId.ToInt();
            }
            else
            {
                foundPaymentProfile.eCheckBankAccount = bankAccount;
                foundPaymentProfile.BillingAddress = billToAddress;

                var isSaved = CustomerGateway.UpdatePaymentProfile(customer.ProfileID, foundPaymentProfile);
                if (!isSaved)
                    throw new Exception("UpdatePaymentProfile failed to save credit card for {0}".Fmt(paymentInfo.PeopleId));
            }
        }

        // NOTE: this can throw an error if the credit card number already exists...
        private void SaveCreditCardToProfile(string cardNumber, string cardCode, DateTime expires, PaymentInfo paymentInfo, Customer customer, AuthorizeNet.Address billToAddress)
        {
            var foundPaymentProfile = customer.PaymentProfiles.SingleOrDefault(p => p.ProfileID == paymentInfo.AuNetCustPayId.ToString());

            if (foundPaymentProfile == null)
            {
                var paymentProfileId = CustomerGateway.AddCreditCard(customer.ProfileID, cardNumber,
                    expires.Month, expires.Year, cardCode, billToAddress);

                paymentInfo.AuNetCustPayId = paymentProfileId.ToInt();
            }
            else
            {
                if (!cardNumber.StartsWith("X"))
                    foundPaymentProfile.CardNumber = cardNumber;

                if (!cardCode.StartsWith("X"))
                    foundPaymentProfile.CardCode = cardCode;

                foundPaymentProfile.CardExpiration = expires.ToString("MMyy");
                foundPaymentProfile.BillingAddress = billToAddress;

                var isSaved = CustomerGateway.UpdatePaymentProfile(customer.ProfileID, foundPaymentProfile);
                if (!isSaved)
                    throw new Exception("UpdatePaymentProfile failed to save echeck for {0}".Fmt(paymentInfo.PeopleId));
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
                paymentInfo.SageCardGuid = null;
                paymentInfo.SageBankGuid = null;
                paymentInfo.MaskedCard = null;
                paymentInfo.MaskedAccount = null;
                paymentInfo.Ccv = null;
                paymentInfo.AuNetCustId = null;
                paymentInfo.AuNetCustPayId = null;
                paymentInfo.AuNetCustPayBankId = null;
                db.SubmitChanges();
            }
            else
            {
                throw new Exception("Failed to delete customer {0}".Fmt(peopleId));
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

        public TransactionResponse RefundCreditCard(string reference, decimal amt)
        {
            return RefundRequest(reference, amt);
        }

        public TransactionResponse RefundCheck(string reference, decimal amt)
        {
            return RefundRequest(reference, amt);
        }

        private TransactionResponse RefundRequest(string reference, decimal amt)
        {
            // TODO: test passing in an empty string for card number... hopefully it will work as a refund (as opposed to a credit) when no card number is passed
            var req = new CreditRequest(reference, amt, string.Empty);
            var response = Gateway.Send(req);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            var request = new AuthorizationRequest(cardnumber, expires, amt, description, includeCapture: true);

            request.AddCustomer(peopleId.ToString(), first, last, addr, state, zip);
            request.City = city; // hopefully will be resolved with https://github.com/AuthorizeNet/sdk-dotnet/pull/41
            request.CardCode = cardcode;
            request.Phone = phone;
            request.Email = email;
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

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone)
        {
            var request = new EcheckRequest(EcheckType.WEB, amt, routing, acct, BankAccountType.Checking, null, first + " " + last, null);

            request.AddCustomer(peopleId.ToString(), first, last, addr, state, zip);
            request.City = city;  // hopefully will be resolved with https://github.com/AuthorizeNet/sdk-dotnet/pull/41
            request.Phone = phone;
            request.Email = email;

            var response = Gateway.Send(request);

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
            if (type == "B")
                paymentProfileId = paymentInfo.AuNetCustPayBankId.ToString();
            else if (type == "C")
                paymentProfileId = paymentInfo.AuNetCustPayId.ToString();
            else
                throw new ArgumentException("Type {0} not supported".Fmt(type), "type");

            var response = CustomerGateway.AuthorizeAndCapture(paymentInfo.AuNetCustId.ToString(), paymentProfileId, amt);

            return new TransactionResponse
            {
                Approved = response.Approved,
                AuthCode = response.AuthorizationCode,
                Message = response.Message,
                TransactionId = response.TransactionID
            };
        }

        public System.Data.DataSet SettledBatchSummary(DateTime start, DateTime end, bool includeCreditCard, bool includeVirtualCheck)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet SettledBatchListing(string batchref, string type)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet VirtualCheckRejects(DateTime startdt, DateTime enddt)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet VirtualCheckRejects(DateTime rejectdate)
        {
            throw new NotImplementedException();
        }

        public bool CanVoidRefund
        {
            get { return true; }
        }

        public bool CanGetSettlementDates
        {
            get { return false; }
        }

        public bool CanGetBounces
        {
            get { return false; }
        }
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
    }
}