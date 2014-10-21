using System;
using System.Data;
using System.Globalization;
using CmsData.Finance.TransNational.Core;
using CmsData.Finance.TransNational.Transaction.Refund;
using CmsData.Finance.TransNational.Transaction.Sale;
using CmsData.Finance.TransNational.Transaction.Void;
using CmsData.Finance.TransNational.Vault;
using UtilityExtensions;

namespace CmsData.Finance
{
    internal class TransNationalGateway : IGateway
    {
        readonly string userName;
        readonly string password;
        CMSDataContext db;
        readonly bool testing;
        public string GatewayType { get { return "Transnational"; } }

        public TransNationalGateway(CMSDataContext db, bool testing)
        {
            this.testing = testing || db.Setting("GatewayTesting", "false").ToLower() == "true";
            this.db = db;
           
            if (this.testing)
            {
                userName = "faithbased";
                password = "bprogram2";
            }
            else
            {
                userName = db.Setting("x_login", "");
                password = db.Setting("x_tran_key", "");
            }
        }

        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
            {
                paymentInfo = new PaymentInfo();
                person.PaymentInfos.Add(paymentInfo);
            }

            if (type == "C") // credit card
            {
                if (paymentInfo.TbnCardVaultId == null) // create new vault.
                    paymentInfo.TbnCardVaultId = CreateCreditCardVault(person, cardNumber, expires);
                else 
                {
                    // update existing vault.
                    // check for updating the entire card or only expiration.
                    if (!cardNumber.StartsWith("X"))
                        UpdateCreditCardVault(paymentInfo.TbnCardVaultId.GetValueOrDefault(), person, cardNumber, expires);
                    else
                        UpdateCreditCardVault(paymentInfo.TbnCardVaultId.GetValueOrDefault(), person, expires);
                    
                }
            }
            else // bank account
            {
                if (paymentInfo.TbnBankVaultId == null) // create new vault
                    paymentInfo.TbnBankVaultId = CreateAchVault(person, account, routing);
                else
                {
                    // we can only update the ach account if there is a full account number.
                    if (!account.StartsWith("X"))
                        UpdateAchVault(paymentInfo.TbnBankVaultId.GetValueOrDefault(), person, account, routing);
                }
            }
        }

        private int CreateCreditCardVault(Person person, string cardNumber, string expiration)
        {
            var createCreditCardVaultRequest = new CreateCreditCardVaultRequest(userName,
                                                                                password,
                                                                                new CreditCard
                                                                                {
                                                                                    FirstName = person.FirstName,
                                                                                    LastName = person.LastName,
                                                                                    CardNumber = cardNumber,
                                                                                    Expiration = expiration,
                                                                                    BillingAddress = new BillingAddress
                                                                                    {
                                                                                        Address1 = person.PrimaryAddress,
                                                                                        City = person.PrimaryCity,
                                                                                        State = person.PrimaryState,
                                                                                        Zip = person.PrimaryZip,
                                                                                        Country = person.PrimaryCountry ?? "US",
                                                                                        Email = person.EmailAddress,
                                                                                        Phone = person.HomePhone ?? person.CellPhone
                                                                                    }
                                                                                });

            var response = createCreditCardVaultRequest.Execute();
            if (response.ResponseStatus != ResponseStatus.Approved)
                throw new Exception("TransNational failed to create the credit card for people id: {0}".Fmt(person.PeopleId));

            return response.VaultId.ToInt();
        }

        private void UpdateCreditCardVault(int vaultId, Person person, string cardNumber, string expiration)
        {
            var updateCreditCardVaultRequest = new UpdateCreditCardVaultRequest(userName,
                                                                                password,
                                                                                vaultId.ToString(CultureInfo.InvariantCulture),
                                                                                new CreditCard
                                                                                {
                                                                                    FirstName = person.FirstName,
                                                                                    LastName = person.LastName,
                                                                                    CardNumber = cardNumber,
                                                                                    Expiration = expiration,
                                                                                    BillingAddress = new BillingAddress
                                                                                    {
                                                                                        Address1 = person.PrimaryAddress,
                                                                                        City = person.PrimaryCity,
                                                                                        State = person.PrimaryState,
                                                                                        Zip = person.PrimaryZip,
                                                                                        Country = person.PrimaryCountry ?? "US",
                                                                                        Email = person.EmailAddress,
                                                                                        Phone = person.HomePhone ?? person.CellPhone
                                                                                    }
                                                                                });

            var response = updateCreditCardVaultRequest.Execute();
            if (response.ResponseStatus != ResponseStatus.Approved)
                throw new Exception("TransNational failed to update the credit card for people id: {0}".Fmt(person.PeopleId));
            
        }

        private void UpdateCreditCardVault(int vaultId, Person person, string expiration)
        {
            var updateCreditCardVaultRequest = new UpdateCreditCardVaultRequest(userName, password, vaultId.ToString(CultureInfo.InvariantCulture), expiration);

            var response = updateCreditCardVaultRequest.Execute();
            if (response.ResponseStatus != ResponseStatus.Approved)
                throw new Exception("TransNational failed to update the credit card expiration date for people id: {0}".Fmt(person.PeopleId));

        }

        private int CreateAchVault(Person person, string accountNumber, string routingNumber)
        {
            var createAchVaultRequest = new CreateAchVaultRequest(userName,
                                                                  password,
                                                                  new Ach
                                                                  {
                                                                      NameOnAccount = person.Name,
                                                                      AccountNumber = accountNumber,
                                                                      RoutingNumber = routingNumber
                                                                  });

            var response = createAchVaultRequest.Execute();
            if (response.ResponseStatus != ResponseStatus.Approved)
                throw new Exception("TransNational failed to create the ach account for people id: {0}".Fmt(person.PeopleId));

            return response.VaultId.ToInt();
        }

        private void UpdateAchVault(int vaultId, Person person, string accountNumber, string routingNumber)
        {
            var updateAchVaultRequest = new UpdateAchVaultRequest(userName,
                                                                  password,
                                                                  vaultId.ToString(CultureInfo.InvariantCulture),
                                                                  new Ach
                                                                  {
                                                                      NameOnAccount = person.Name,
                                                                      AccountNumber = accountNumber,
                                                                      RoutingNumber = routingNumber
                                                                  });

            var response = updateAchVaultRequest.Execute();
            if (response.ResponseStatus != ResponseStatus.Approved)
                throw new Exception("TransNational failed to update the ach account for people id: {0}".Fmt(person.PeopleId));
        }

        public void RemoveFromVault(int peopleId)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return;

            if (paymentInfo.TbnCardVaultId.HasValue)
                DeleteVault(paymentInfo.TbnCardVaultId.GetValueOrDefault(), person);

            if (paymentInfo.TbnBankVaultId.HasValue)
                DeleteVault(paymentInfo.TbnBankVaultId.GetValueOrDefault(), person);

            // clear out local record and save changes.
            paymentInfo.TbnCardVaultId = null;
            paymentInfo.TbnBankVaultId = null;
            paymentInfo.MaskedCard = null;
            paymentInfo.MaskedAccount = null;
            paymentInfo.Ccv = null;
            db.SubmitChanges();
        }

        private void DeleteVault(int vaultId, Person person)
        {
            var deleteVaultRequest = new DeleteVaultRequest(userName, password, vaultId.ToString(CultureInfo.InvariantCulture));

            var response = deleteVaultRequest.Execute();
            if (response.ResponseStatus != ResponseStatus.Approved)
                throw new Exception("TransNational failed to delete the vault for people id: {0}".Fmt(person.PeopleId));
        }
        
        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            return Void(reference);
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            return Void(reference);
        }

        private TransactionResponse Void(string reference)
        {
            var voidRequest = new VoidRequest(userName, password, reference);
            var response = voidRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ResponseStatus == ResponseStatus.Approved,
                AuthCode = response.AuthCode,
                Message = response.ResponseText,
                TransactionId = response.TransactionId
            };
        }

        public TransactionResponse RefundCreditCard(string reference, Decimal amt)
        {
            return Refund(reference, amt);
        }

        public TransactionResponse RefundCheck(string reference, Decimal amt)
        {
            return Refund(reference, amt);
        }

        private TransactionResponse Refund(string reference, Decimal amount)
        {
            var refundRequest = new RefundRequest(userName, password, reference, amount);
            var response = refundRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ResponseStatus == ResponseStatus.Approved,
                AuthCode = response.AuthCode,
                Message = response.ResponseText,
                TransactionId = response.TransactionId
            };
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            var creditCardSaleRequest = new CreditCardSaleRequest(userName,
                                                                  password,
                                                                  new CreditCard
                                                                  {
                                                                      FirstName = first,
                                                                      LastName = last,
                                                                      CardNumber = cardnumber,
                                                                      Expiration = expires,
                                                                      CardCode = cardcode,
                                                                      BillingAddress = new BillingAddress
                                                                      {
                                                                          Address1 = addr,
                                                                          City = city,
                                                                          State = state,
                                                                          Zip = zip,
                                                                          Country = "US",
                                                                          Email = email,
                                                                          Phone = phone
                                                                      }
                                                                  },
                                                                  amt,
                                                                  tranid.ToString(CultureInfo.InvariantCulture),
                                                                  description,
                                                                  peopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ResponseStatus == ResponseStatus.Approved,
                AuthCode = response.AuthCode,
                Message = response.ResponseText,
                TransactionId = response.TransactionId
            };
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone)
        {
            var achSaleRequest = new AchSaleRequest(userName,
                                                    password,
                                                    new Ach
                                                    {
                                                        NameOnAccount = string.Format("{0} {1}", first, last),
                                                        AccountNumber = acct,
                                                        RoutingNumber = routing,
                                                        BillingAddress = new BillingAddress
                                                        {
                                                            Address1 = addr,
                                                            City = city,
                                                            State = state,
                                                            Zip = zip,
                                                            Country = "US",
                                                            Email = email,
                                                            Phone = phone
                                                        }
                                                    },
                                                    amt,
                                                    tranid.ToString(CultureInfo.InvariantCulture),
                                                    description,
                                                    peopleId.ToString(CultureInfo.InvariantCulture));

            var response = achSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ResponseStatus == ResponseStatus.Approved,
                AuthCode = response.AuthCode,
                Message = response.ResponseText,
                TransactionId = response.TransactionId
            };
        }

        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            if (type == "C") // credit card
                return ChargeCreditCardVault(paymentInfo.TbnCardVaultId.GetValueOrDefault(), peopleId, amt, tranid, description);
            else // bank account
                return ChargeAchVault(paymentInfo.TbnBankVaultId.GetValueOrDefault(), peopleId, amt, tranid, description);

        }

        private TransactionResponse ChargeCreditCardVault(int vaultId, int peopleId, decimal amount, int tranid, string description)
        {
            var creditCardVaultSaleRequest = new CreditCardVaultSaleRequest(userName,
                                                                            password,
                                                                            vaultId.ToString(CultureInfo.InvariantCulture),
                                                                            amount,
                                                                            tranid.ToString(CultureInfo.InvariantCulture),
                                                                            description,
                                                                            peopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardVaultSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ResponseStatus == ResponseStatus.Approved,
                AuthCode = response.AuthCode,
                Message = response.ResponseText,
                TransactionId = response.TransactionId
            };
        }

        private TransactionResponse ChargeAchVault(int vaultId, int peopleId, decimal amount, int tranid, string description)
        {
            var achVaultSaleRequest = new AchVaultSaleRequest(userName,
                                                              password,
                                                              vaultId.ToString(CultureInfo.InvariantCulture),
                                                              amount,
                                                              tranid.ToString(CultureInfo.InvariantCulture),
                                                              description,
                                                              peopleId.ToString(CultureInfo.InvariantCulture));

            var response = achVaultSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ResponseStatus == ResponseStatus.Approved,
                AuthCode = response.AuthCode,
                Message = response.ResponseText,
                TransactionId = response.TransactionId
            };
        }

        public DataSet SettledBatchSummary(DateTime start, DateTime end, bool includeCreditCard, bool includeVirtualCheck)
        {
            //var queryRequest = new QueryRequest(userName,
            //                                    password,
            //                                    DateTime.Now.AddDays(-14),
            //                                    DateTime.Now,
            //                                    new List<Internal.Query.Condition> { Internal.Query.Condition.Complete },
            //                                    new List<ActionType> { ActionType.Settle });

            //var response = queryRequest.Execute();


            return null;
        }

        public DataSet SettledBatchListing(string batchref, string type)
        {
            return null;
        }

        public DataSet VirtualCheckRejects(DateTime startdt, DateTime enddt)
        {
            //var queryRequest = new QueryRequest(userName,
            //                                    password,
            //                                    startdt,
            //                                    enddt,
            //                                    new List<Internal.Query.Condition> { Internal.Query.Condition.Failed },
            //                                    new List<TransactionType> { TransactionType.Ach },
            //                                    new List<ActionType> { ActionType.CheckReturn, ActionType.CheckLateReturn });

            //var response = queryRequest.Execute();

            return null;
        }

        public DataSet VirtualCheckRejects(DateTime rejectdate)
        {
            return null;
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
    }
}