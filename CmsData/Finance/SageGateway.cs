using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CmsData.Finance.Sage.Core;
using CmsData.Finance.Sage.Report;
using CmsData.Finance.Sage.Transaction.Auth;
using CmsData.Finance.Sage.Transaction.Refund;
using CmsData.Finance.Sage.Transaction.Sale;
using CmsData.Finance.Sage.Transaction.Void;
using CmsData.Finance.Sage.Vault;
using UtilityExtensions;

namespace CmsData.Finance
{
    internal class SageGateway : IGateway
    {
        private readonly string _id;
        private readonly string _key;
        private readonly string _originatorId;
        private readonly CMSDataContext db;

        public string GatewayType => "Sage";

        public SageGateway(CMSDataContext db, bool testing)
        {
            this.db = db;
            var gatewayTesting = db.Setting("GatewayTesting");
            if (testing || gatewayTesting)
            {
                _id = "856423594649";
                _key = "M5Q4C9P2T4N5";
                _originatorId = "1111111111";
            }
            else
            {
                _id = db.GetSetting("M_ID", "");
                _key = db.GetSetting("M_KEY", "");

                if (string.IsNullOrWhiteSpace(_id))
                    throw new Exception("M_ID setting not found, which is required for Sage.");
                if (string.IsNullOrWhiteSpace(_key))
                    throw new Exception("M_KEY setting not found, which is required for Sage.");

                _originatorId = db.Setting("SageOriginatorId", "");
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

            if (type == PaymentType.CreditCard)
            {
                if (paymentInfo.SageCardGuid == null) // create new vault.
                    paymentInfo.SageCardGuid = CreateCreditCardVault(person, cardNumber, expires);
                else
                {
                    // update existing vault.
                    // check for updating the entire card or only expiration.
                    if (!cardNumber.StartsWith("X"))
                        UpdateCreditCardVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person, cardNumber, expires);
                    else
                        UpdateCreditCardVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person, expires);
                }

                paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
                paymentInfo.Expires = expires;
            }
            else if (type == PaymentType.Ach)
            {
                if (paymentInfo.SageBankGuid == null) // create new vault
                    paymentInfo.SageBankGuid = CreateAchVault(person, account, routing);
                else
                {
                    // we can only update the ach account if there is a full account number.
                    if (!account.StartsWith("X"))
                        UpdateAchVault(paymentInfo.SageBankGuid.GetValueOrDefault(), person, account, routing);
                }

                paymentInfo.MaskedAccount = Util.MaskAccount(account);
                paymentInfo.Routing = Util.Mask(new StringBuilder(routing), 2);
            }
            else
                throw new ArgumentException($"Type {type} not supported", nameof(type));

            if (giving)
                paymentInfo.PreferredGivingType = type;
            else
                paymentInfo.PreferredPaymentType = type;
            db.SubmitChanges();
        }

        private Guid CreateCreditCardVault(Person person, string cardNumber, string expiration)
        {
            var createCreditCardVaultRequest = new CreateCreditCardVaultRequest(_id, _key, expiration, cardNumber);

            var response = createCreditCardVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    $"Sage failed to create the credit card for people id: {person.PeopleId}, message: {response.Message}");

            return response.VaultGuid;
        }

        private void UpdateCreditCardVault(Guid vaultGuid, Person person, string cardNumber, string expiration)
        {
            var updateCreditCardVaultRequest = new UpdateCreditCardVaultRequest(_id,
                                                                                _key,
                                                                                vaultGuid,
                                                                                expiration,
                                                                                cardNumber);

            var response = updateCreditCardVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    $"Sage failed to update the credit card for people id: {person.PeopleId}, message: {response.Message}");
        }

        private void UpdateCreditCardVault(Guid vaultGuid, Person person, string expiration)
        {
            var updateCreditCardVaultRequest = new UpdateCreditCardVaultRequest(_id, _key, vaultGuid, expiration);

            var response = updateCreditCardVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    $"Sage failed to update the credit card expiration date for people id: {person.PeopleId}, message: {response.Message}");
        }

        private Guid CreateAchVault(Person person, string accountNumber, string routingNumber)
        {
            var createAchVaultRequest = new CreateAchVaultRequest(_id, _key, accountNumber, routingNumber);

            var response = createAchVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    $"Sage failed to create the ach account for people id: {person.PeopleId}, message: {response.Message}");

            return response.VaultGuid;
        }

        private void UpdateAchVault(Guid vaultGuid, Person person, string accountNumber, string routingNumber)
        {
            var updateAchVaultRequest = new UpdateAchVaultRequest(_id, _key, vaultGuid, accountNumber, routingNumber);

            var response = updateAchVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    $"Sage failed to update the ach account for people id: {person.PeopleId}, message: {response.Message}");
        }

        public void RemoveFromVault(int peopleId)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return;

            if (paymentInfo.SageCardGuid.HasValue)
                DeleteVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person);

            if (paymentInfo.SageBankGuid.HasValue)
                DeleteVault(paymentInfo.SageBankGuid.GetValueOrDefault(), person);

            // clear out local record and save changes.
            paymentInfo.SageCardGuid = null;
            paymentInfo.SageBankGuid = null;
            paymentInfo.MaskedCard = null;
            paymentInfo.MaskedAccount = null;
            paymentInfo.Expires = null;
            db.SubmitChanges();
        }

        private void DeleteVault(Guid vaultGuid, Person person)
        {
            var deleteVaultRequest = new DeleteVaultRequest(_id, _key, vaultGuid);

            var success = deleteVaultRequest.Execute();
            if (!success)
                throw new Exception($"Sage failed to delete the vault for people id: {person.PeopleId}");
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            var voidRequest = new CreditCardVoidRequest(_id, _key, reference);
            var response = voidRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            var voidRequest = new AchVoidRequest(_id, _key, reference);
            var response = voidRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "")
        {
            var refundRequest = new CreditCardRefundRequest(_id, _key, reference, amt);
            var response = refundRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "")
        {
            var refundRequest = new AchRefundRequest(_id, _key, reference, amt);
            var response = refundRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description,
            int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state,
            string country, string zip, string phone)
        {
            var creditCardAuthRequest = new CreditCardAuthRequest(
                _id,
                _key,
                new CreditCard
                {
                    NameOnCard = $"{first} {last}",
                    CardNumber = cardnumber,
                    Expiration = expires,
                    CardCode = cardcode,
                    BillingAddress = new BillingAddress
                    {
                        Address1 = addr,
                        City = city,
                        State = state,
                        Country = country,
                        Zip = zip,
                        Email = email,
                        Phone = phone
                    }
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                peopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardAuthRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var creditCardSaleRequest = new CreditCardSaleRequest(
                _id,
                _key,
                new CreditCard
                {
                    NameOnCard = $"{first} {last}",
                    CardNumber = cardnumber,
                    Expiration = expires,
                    CardCode = cardcode,
                    BillingAddress = new BillingAddress
                    {
                        Address1 = addr,
                        City = city,
                        State = state,
                        Country = country,
                        Zip = zip,
                        Email = email,
                        Phone = phone
                    }
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                peopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var achSaleRequest = new AchSaleRequest(_id,
                _key,
                _originatorId,
                new Ach
                {
                    FirstName = first,
                    MiddleInitial = middle.Truncate(1) ?? "",
                    LastName = last,
                    Suffix = suffix,
                    AccountNumber = acct,
                    RoutingNumber = routing,
                    BillingAddress = new BillingAddress
                    {
                        Address1 = addr,
                        City = city,
                        State = state,
                        Country = country,
                        Zip = zip,
                        Email = email,
                        Phone = phone
                    }

                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture));

            var response = achSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null || !paymentInfo.SageCardGuid.HasValue)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            var creditCardVaultAuthRequest = new CreditCardVaultAuthRequest(_id,
                _key,
                paymentInfo.SageCardGuid.GetValueOrDefault(),
                $"{paymentInfo.FirstName ?? person.FirstName} {paymentInfo.LastName ?? person.LastName}",
                new BillingAddress
                {
                    Address1 = paymentInfo.Address ?? person.PrimaryAddress,
                    City = paymentInfo.City ?? person.PrimaryCity,
                    State = paymentInfo.State ?? person.PrimaryState,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                person.PeopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardVaultAuthRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
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

            if (type == PaymentType.CreditCard) // credit card
                return ChargeCreditCardVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person, paymentInfo, amt, tranid);
            else // bank account
                return ChargeAchVault(paymentInfo.SageBankGuid.GetValueOrDefault(), person, paymentInfo, amt, tranid);

        }

        private TransactionResponse ChargeCreditCardVault(Guid vaultGuid, Person person, PaymentInfo paymentInfo, decimal amount, int tranid)
        {
            var creditCardVaultSaleRequest = new CreditCardVaultSaleRequest(_id,
                _key,
                vaultGuid,
                $"{paymentInfo.FirstName ?? person.FirstName} {paymentInfo.LastName ?? person.LastName}",
                new BillingAddress
                {
                    Address1 = paymentInfo.Address ?? person.PrimaryAddress,
                    City = paymentInfo.City ?? person.PrimaryCity,
                    State = paymentInfo.State ?? person.PrimaryState,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone
                },
                amount, tranid.ToString(CultureInfo.InvariantCulture),
                person.PeopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardVaultSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        private TransactionResponse ChargeAchVault(Guid vaultGuid, Person person, PaymentInfo paymentInfo, decimal amount, int tranid)
        {
            var achVaultSaleRequest = new AchVaultSaleRequest(_id,
                _key,
                _originatorId,
                vaultGuid,
                paymentInfo.FirstName ?? person.FirstName,
                (paymentInfo.MiddleInitial ?? person.MiddleName).Truncate(1) ?? "",
                paymentInfo.LastName ?? person.LastName,
                paymentInfo.Suffix ?? person.SuffixCode,
                new BillingAddress
                {
                    Address1 = paymentInfo.Address ?? person.PrimaryAddress,
                    City = paymentInfo.City ?? person.PrimaryCity,
                    State = paymentInfo.State ?? person.PrimaryState,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone
                },
                amount, tranid.ToString(CultureInfo.InvariantCulture));

            var response = achVaultSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        public BatchResponse GetBatchDetails(DateTime start, DateTime end)
        {
            var batchTransactions = new List<BatchTransaction>();
            var settledBatchSummaryRequest = new SettledBatchSummaryRequest(_id, _key, start, end, true, true);
            var settledBatchResponse = settledBatchSummaryRequest.Execute();

            foreach (var batch in settledBatchResponse.Batches)
            {
                var transactions = new List<Sage.Report.Transaction>();
                if (batch.Type == Sage.Report.BatchType.CreditCard)
                {
                    var creditCardSettledBatchListingRequest = new CreditCardSettledBatchListingRequest(_id, _key, batch.Reference);
                    var creditCardSettledBatchListingResponse = creditCardSettledBatchListingRequest.Execute();
                    transactions = creditCardSettledBatchListingResponse.Transactions.ToList();
                }
                else if (batch.Type == Sage.Report.BatchType.Ach)
                {
                    var achSettledBatchListingRequest = new AchSettledBatchListingRequest(_id, _key, batch.Reference);
                    var achSettledBatchListingResponse = achSettledBatchListingRequest.Execute();
                    transactions = achSettledBatchListingResponse.Transactions.ToList();
                }

                foreach (var transaction in transactions)
                {
                    batchTransactions.Add(new BatchTransaction
                    {
                        TransactionId = transaction.OrderNumber.ToInt(),
                        Reference = transaction.Reference,
                        BatchReference = batch.Reference,
                        TransactionType = GetTransactionType(transaction.TransactionType),
                        BatchType = GetBatchType(batch.Type),
                        Name = transaction.Name,
                        Amount = transaction.TotalAmount,
                        Approved = transaction.Approved,
                        Message = transaction.Message,
                        TransactionDate = transaction.Date,
                        SettledDate = transaction.SettleDate,
                        LastDigits = transaction.LastDigits
                    });
                }
            }

            return new BatchResponse(batchTransactions);
        }

        private static TransactionType GetTransactionType(Sage.Report.TransactionType transactionType)
        {
            switch (transactionType)
            {
                case Sage.Report.TransactionType.Sale:
                case Sage.Report.TransactionType.ForceAuthSale:
                    return TransactionType.Charge;
                case Sage.Report.TransactionType.Credit:
                    return TransactionType.Refund;
                default:
                    return TransactionType.Unknown;
            }
        }

        private static BatchType GetBatchType(Sage.Report.BatchType batchType)
        {
            switch (batchType)
            {
                case Sage.Report.BatchType.CreditCard:
                    return BatchType.CreditCard;
                case Sage.Report.BatchType.Ach:
                    return BatchType.Ach;
                default:
                    return BatchType.Unknown;
            }
        }

        public ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end)
        {
            var returnedChecks = new List<ReturnedCheck>();
            var virtualCheckRejectsRequest = new VirtualCheckRejectsRequest(_id, _key, start, end);
            var response = virtualCheckRejectsRequest.Execute();

            foreach (var returnedCheck in response.ReturnedChecks)
            {
                returnedChecks.Add(new ReturnedCheck
                {
                    TransactionId = returnedCheck.CustomerNumber.ToInt(),
                    Name = returnedCheck.CustomerName,
                    RejectCode = returnedCheck.RejectCode,
                    RejectAmount = returnedCheck.RejectAmount,
                    RejectDate = returnedCheck.RejectDate
                });
            }

            return new ReturnedChecksResponse(returnedChecks);
        }

        public bool CanVoidRefund => true;

        public bool CanGetSettlementDates => true;

        public bool CanGetBounces => true;
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
            switch (Util.PickFirst(paymentInfo.PreferredGivingType, "").ToLower())
            {
                case "c":
                    return paymentInfo.SageCardGuid.ToString();
                case "b":
                    return paymentInfo.SageBankGuid.ToString();
                default:
                    return (paymentInfo.SageCardGuid ?? paymentInfo.SageBankGuid).ToString();
            }
                
        }
    }
}
