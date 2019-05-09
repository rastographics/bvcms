using CmsData.Finance.Acceptiva.Core;
using CmsData.Finance.Acceptiva.Core.Helpers;
using CmsData.Finance.Acceptiva.Get;
using CmsData.Finance.Acceptiva.Store;
using CmsData.Finance.Acceptiva.Transaction.Charge;
using CmsData.Finance.Acceptiva.Transaction.Get;
using CmsData.Finance.Acceptiva.Transaction.Refund;
using CmsData.Finance.Acceptiva.Transaction.Void;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Finance
{
    public class AcceptivaGateway : IGateway
    {
        private readonly string _apiKey;
        private readonly string _merch_ach_id;
        private readonly string _merch_cc_id;
        private readonly CMSDataContext db;
        private readonly PaymentProcessTypes? ProcessType;

        public string GatewayType => "Acceptiva";

        public AcceptivaGateway(CMSDataContext db, bool testing, PaymentProcessTypes? ProcessType)
        {
            this.db = db;
            this.ProcessType = ProcessType;

            if (testing || new MultipleGatewayUtils(db).GatewayTesting(ProcessType))
            {
                _apiKey = "CZDWp7dXCo4W3xTA7LtWAijidvPdj2wa";
                _merch_ach_id = "dKdDFtqC";
                _merch_cc_id = "R6MLUevR";
            }
            else
            {
                _apiKey = new MultipleGatewayUtils(db).Setting("AcceptivaApiKey", "", (int)ProcessType);
                _merch_ach_id = new MultipleGatewayUtils(db).Setting("AcceptivaAchId", "", (int)ProcessType);
                _merch_cc_id = new MultipleGatewayUtils(db).Setting("AcceptivaCCId", "", (int)ProcessType);

                if (string.IsNullOrWhiteSpace(_apiKey))
                    throw new Exception("AcceptivaApiKey setting not found, which is required for Acceptiva.");
                if (string.IsNullOrWhiteSpace(_merch_ach_id))
                    throw new Exception("AcceptivaAcctId setting not found, which is required for Acceptiva.");
                if (string.IsNullOrWhiteSpace(_merch_cc_id))
                    throw new Exception("AcceptivaCCId setting not found, which is required for Acceptiva.");
            }
        }

        public bool CanVoidRefund => true;
        public bool CanGetSettlementDates => true;
        public bool UseIdsForSettlementDates => false;
        public bool CanGetBounces => true;

        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
            {
                paymentInfo = new PaymentInfo();
                person.PaymentInfos.Add(paymentInfo);
            }
            //Set values to be ignored in API
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.StartsWith("X"))
            {
                cardNumber = string.Empty;
                expires = string.Empty;
            }
            else
            {
                paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
                paymentInfo.Expires = expires;
            }
            if (string.IsNullOrEmpty(account) || account.StartsWith("X"))
            {
                account = string.Empty;
                routing = string.Empty;
            }
            else
            {
                paymentInfo.MaskedAccount = Util.MaskAccount(account);
                paymentInfo.Routing = Util.Mask(new StringBuilder(routing), 2);
            }
            //Check if Accpetiva has this payer data
            string acceptivaPayerid = GetAcceptivaPayerId(peopleId);
            if (string.IsNullOrEmpty(acceptivaPayerid))
            {
                paymentInfo.AcceptivaPayerId = StoreNewPayerData(person, paymentInfo, cardNumber, expires, routing, account);
            }
            else
            {
                paymentInfo.AcceptivaPayerId = acceptivaPayerid;
                StorePayerData(person, paymentInfo, cardNumber, expires, account, routing);
            }
            if (giving)
                paymentInfo.PreferredGivingType = type;
            else
                paymentInfo.PreferredPaymentType = type;
            db.SubmitChanges();
        }

        public void RemoveFromVault(int peopleId)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return;

            // clear out local record and save changes.
            paymentInfo.AcceptivaPayerId = null;
            paymentInfo.MaskedCard = null;
            paymentInfo.MaskedAccount = null;
            paymentInfo.Expires = null;
            db.SubmitChanges();
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            return VoidTransaction(reference);
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            return VoidTransaction(reference);
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "")
        {
            return RefundTransaction(reference, amt);
        }

        public TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "")
        {
            return RefundTransaction(reference, amt);
        }

        public TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            return CreditCardCharge(peopleId, amt, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, addr2, city, state, country, zip, phone);
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            return CreditCardCharge(peopleId, amt, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, addr2, city, state, country, zip, phone);
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var achCharge = new AchCharge(
                _apiKey,
                _merch_ach_id,
                new Ach
                {
                    AchAccNum = acct,
                    AchRoutingNum = routing
                },
                new Payer
                {
                    LastName = last,
                    FirstName = first,
                    Address = addr,
                    Address2 = addr2,
                    City = city,
                    State = state,
                    Country = country,
                    Zip = zip,
                    Email = email,
                    Phone = phone
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                description,
                peopleId.ToString(CultureInfo.InvariantCulture));

            var response = achCharge.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.TransStatus,
                Message = response.Response.TransStatusMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        public TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo?.AcceptivaPayerId == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            return StoredPayerCharge(_merch_cc_id, paymentInfo.AcceptivaPayerId, amt, tranid.ToString(), description, 1, person.LastName, person.FirstName);
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
                return StoredPayerCharge(_merch_cc_id, paymentInfo.AcceptivaPayerId, amt, tranid.ToString(), description, 1, person.LastName, person.FirstName);
            else // bank account
                return StoredPayerCharge(_merch_ach_id, paymentInfo.AcceptivaPayerId, amt, tranid.ToString(), description, 2, person.LastName, person.FirstName);
        }

        public BatchResponse GetBatchDetails(DateTime start, DateTime end)
        {
            var GetTransDetails = new GetSettledTransDetails(_apiKey, start, end);
            var transactionsList = GetTransDetails.Execute();

            var batchTransactions = new List<BatchTransaction>();

            foreach (var item in transactionsList)
            {
                batchTransactions.Add(new BatchTransaction
                {
                    TransactionId = int.Parse(item.Response.Items[0].Id),
                    Reference = item.Response.TransIdStr,
                    BatchReference = GetBatchReference(start, end),
                    TransactionType = GetTransactionType(item.Response.AmtProcessed),
                    BatchType = GetBatchType(item.Response.PaymentType),
                    Name = $"{item.Response.PayerFname} {item.Response.PayerLname}",
                    Amount = item.Response.AmtProcessed,
                    Approved = true,
                    Message = item.Response.TransStatusMsg,
                    TransactionDate = item.Response.TransDatetime,
                    SettledDate = item.Response.TransDatetime.AddDays(1),
                    LastDigits = item.Response.AcctLastFour
                });
            }

            return new BatchResponse(batchTransactions);
        }        

        public ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end)
        {
            var returnedChecks = new List<ReturnedCheck>();
            var getECheckReturned = new GetReturnedEChecks(_apiKey, start, end);
            var response = getECheckReturned.Execute();

            foreach (var returnedCheck in response)
            {
                returnedChecks.Add(new ReturnedCheck
                {
                    TransactionId = returnedCheck.Response.AcctName.ToInt(),
                    Name = returnedCheck.Response.AcctName,
                    RejectCode = returnedCheck.Response.TransStatus.ToString(),
                    RejectAmount = returnedCheck.Response.AmtProcessed,
                    RejectDate = returnedCheck.Response.TransDatetime.AddDays(1)
                });
            }

            return new ReturnedChecksResponse(returnedChecks);
        }

        public void CheckBatchSettlements(List<string> transactionids)
        {
            throw new NotImplementedException();
        }

        public void CheckBatchSettlements(DateTime start, DateTime end)
        {
            CheckBatchedTransactions.CheckBatchSettlements(db, this, start, end);
        }

        public string VaultId(int peopleId)
        {
            return db.PaymentInfos.Single(pp => pp.PeopleId == peopleId).AcceptivaPayerId;
        }

        //private methods
        private TransactionResponse CreditCardCharge(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var cardCharge = new CreditCardCharge(
                _apiKey,
                _merch_cc_id,
                new CreditCard
                {
                    CardNum = cardnumber,
                    CardExpiration = expires,
                    CardCvv = cardcode
                },
                new Payer
                {
                    LastName = last,
                    FirstName = first,
                    Address = addr,
                    Address2 = addr2,
                    City = city,
                    State = UPSStateCodes.FromStateCountry(state, country, db) ?? state,
                    Country = ISO3166.Alpha3FromName(country) ?? country,
                    Zip = zip,
                    Email = email,
                    Phone = phone
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                description);

            var response = cardCharge.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.ProcessorResponseCode,
                Message = response.Response.Errors.FirstOrDefault()?.ErrorMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        private TransactionResponse StoredPayerCharge(string merchId, string acceptivaPayerId, decimal amt, string tranId, string description, int paymentType, string lname, string fname)
        {
            var storedPayerCharge = new StoredPayerCharge(_apiKey, merchId, acceptivaPayerId, amt, tranId, description, paymentType, lname, fname);
            var response = storedPayerCharge.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.TransStatus,
                Message = response.Response.TransStatusMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        private TransactionResponse VoidTransaction(string reference)
        {
            var voidTrans = new VoidTrans(_apiKey, reference);
            var response = voidTrans.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.ProcessorResponseCode,
                Message = response.Response.Errors.FirstOrDefault()?.ErrorMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        private TransactionResponse RefundTransaction(string reference, decimal amt)
        {
            int tranId = db.Transactions.SingleOrDefault(p => p.TransactionId == reference).Id;
            var voidTrans = new RefundTransPartial(_apiKey, reference, tranId, amt);
            var response = voidTrans.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.ProcessorResponseCode,
                Message = response.Response.Errors.FirstOrDefault()?.ErrorMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        private BatchType GetBatchType(int paymentType)
        {
            switch (paymentType)
            {
                case 1:
                    return BatchType.CreditCard;
                case 2:
                    return BatchType.Ach;
                default:
                    return BatchType.Unknown;
            }
        }

        private TransactionType GetTransactionType(decimal amtProcessed)
        {
            if (amtProcessed > 0)
            {
                return TransactionType.Charge;
            }
            else
            {
                return TransactionType.Refund;
            }
        }

        private string GetBatchReference(DateTime start, DateTime end)
        {
            return $"{start.ToString("yyMMdd")}{end.ToString("yyMMdd")}{DateTime.Now.ToString("MMddhhmm")}";
        }

        private string GetAcceptivaPayerId(int peopleId)
        {
            var getPayerData = new GetPayerData(_apiKey, peopleId);
            var response = getPayerData.Execute();
            if (response.Response.Status != "success")
            {
                return null;
            }
            return response.Response.PayerIdStr;
        }

        private void StorePayerData(Person person, PaymentInfo paymentInfo, string cardNumber, string expires, string account, string routing)
        {
            //var creditCard = new CreditCard();
            //creditCard.CardNum = cardNumber;
            //creditCard.CardExpiration = expires;
            //var storePayer = new StorePayer();

            var storePayer = new StorePayer(
                _apiKey,
                new Payer
                {
                    LastName = paymentInfo.LastName ?? person.LastName,
                    FirstName = paymentInfo.FirstName ?? person.LastName,
                    Address = paymentInfo.Address ?? person.AddressLineOne,
                    Address2 = paymentInfo.Address2 ?? person.AddressLineTwo,
                    City = paymentInfo.City ?? person.CityName,
                    State = UPSStateCodes.FromStateCountry(paymentInfo.State ?? null, paymentInfo.Country, db) ?? paymentInfo.State,
                    Country = ISO3166.Alpha3FromName(paymentInfo.Country ?? person.CountryName) ?? null,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone
                },
                paymentInfo.AcceptivaPayerId,
                new CreditCard
                {
                    CardNum = cardNumber,
                    CardExpiration = expires
                },
                new Ach
                {
                    AchAccNum = account,
                    AchRoutingNum = routing
                });

            var response = storePayer.Execute();
            if (response.Response.Status != "success")
                throw new Exception(
                    $"Acceptiva failed to update the credit card for people id: {person.PeopleId}, responseCode: {response.Response.Errors.FirstOrDefault()?.ErrorNo}, responseText: {response.Response.Errors.FirstOrDefault()?.ErrorMsg}");
        }

        private string StoreNewPayerData(Person person, PaymentInfo paymentInfo, string cardNumber, string expires, string account, string routing)
        {
            var storePayer = new StoreNewPayer(
                _apiKey,
                new Payer
                {
                    LastName = paymentInfo.LastName ?? person.LastName,
                    FirstName = paymentInfo.FirstName ?? person.LastName,
                    Address = paymentInfo.Address ?? person.AddressLineOne,
                    Address2 = paymentInfo.Address2 ?? person.AddressLineTwo,
                    City = paymentInfo.City ?? person.CityName,
                    State = UPSStateCodes.FromStateCountry(paymentInfo.State ?? null, paymentInfo.Country, db) ?? paymentInfo.State,
                    Country = ISO3166.Alpha3FromName(paymentInfo.Country ?? person.CountryName) ?? null,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone
                },
                person.PeopleId.ToString(),
                new CreditCard
                {
                    CardNum = cardNumber,
                    CardExpiration = expires
                },
                new Ach
                {
                    AchAccNum = account,
                    AchRoutingNum = routing
                });

            var response = storePayer.Execute();
            if (response.Response.Status != "success")
                throw new Exception(
                    $"Acceptiva failed to update the credit card for people id: {person.PeopleId}, responseCode: {response.Response.Errors.FirstOrDefault()?.ErrorNo}, responseText: {response.Response.Errors.FirstOrDefault()?.ErrorMsg}");

            return response.Response.PayerIdStr;
        }

        private string AchType(int? pid)
        {
            var type = "checking";
            if (pid.HasValue)
            {
                var usesaving = new MultipleGatewayUtils(db).Setting("UseSavingAccounts", (int)ProcessType);
                if (usesaving)
                {
                    if (Person.GetExtraValue(db, pid.Value, "AchSaving")?.BitValue == true)
                        type = "savings";
                }
            }
            return type;
        }
    }
}
