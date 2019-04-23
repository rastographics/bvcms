using CmsData.Finance.Acceptiva;
using CmsData.Finance.Acceptiva.Charge;
using CmsData.Finance.Acceptiva.Core;
using CmsData.Finance.Acceptiva.Store;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityExtensions;

namespace CmsData.Finance
{
    public class AcceptivaGateway : IGateway
    {
        private readonly string _apiKey;
        private readonly string _merch_ach_id;
        private readonly string _merch_cc_id;
        private readonly CMSDataContext db;

        public string GatewayType => "Acceptiva";

        public AcceptivaGateway(CMSDataContext db, bool testing)
        {
            this.db = db;

            if (testing || db.Setting("GatewayTesting"))
            {
                _apiKey = "CZDWp7dXCo4W3xTA7LtWAijidvPdj2wa";
                _merch_ach_id = "dKdDFtqC";
                _merch_cc_id = "R6MLUevR";
            }
            else
            {
                _apiKey = db.GetSetting("AcceptivaApiKey", "");
                _merch_ach_id = db.GetSetting("AcceptivaAchId", "");
                _merch_cc_id = db.GetSetting("AcceptivaCCId", "");

                if (string.IsNullOrWhiteSpace(_apiKey))
                    throw new Exception("AcceptivaApiKey setting not found, which is required for TransNational.");
                if (string.IsNullOrWhiteSpace(_merch_ach_id))
                    throw new Exception("AcceptivaAcctId setting not found, which is required for TransNational.");
                if (string.IsNullOrWhiteSpace(_merch_cc_id))
                    throw new Exception("AcceptivaCCId setting not found, which is required for TransNational.");
            }
        }

        public bool CanVoidRefund => true;

        public bool CanGetSettlementDates => false;

        public bool UseIdsForSettlementDates => false;

        public bool CanGetBounces => false;

        public TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid)
        {
            throw new NotImplementedException();
        }

        public void CheckBatchSettlements(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public void CheckBatchSettlements(List<string> transactionids)
        {
            throw new NotImplementedException();
        }

        public BatchResponse GetBatchDetails(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
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

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
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
                0,
                tranid.ToString(CultureInfo.InvariantCulture),
                description,
                peopleId.ToString(CultureInfo.InvariantCulture));

            var response = cardCharge.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.TransStatus,
                Message = response.Response.Errors.FirstOrDefault()?.ErrorMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "")
        {
            throw new NotImplementedException();
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "")
        {
            throw new NotImplementedException();
        }

        public void RemoveFromVault(int peopleId)
        {
            throw new NotImplementedException();
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

            if (cardNumber.StartsWith("X"))
                cardNumber = string.Empty;

            if (account.StartsWith("X"))
                account = string.Empty;

            if (paymentInfo.AcceptivaPayerId == null)
            {
                paymentInfo.AcceptivaPayerId = StoreNewPayerData(person, paymentInfo, cardNumber, expires, routing, account);
            }
            else
            {
                StorePayerData(person, paymentInfo, cardNumber, expires, account, routing);
            }

            paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
            paymentInfo.Expires = expires;

            paymentInfo.MaskedAccount = Util.MaskAccount(account);
            paymentInfo.Routing = Util.Mask(new StringBuilder(routing), 2);

            if (giving)
                paymentInfo.PreferredGivingType = type;
            else
                paymentInfo.PreferredPaymentType = type;
            db.SubmitChanges();
        }

        private void StorePayerData(Person person, PaymentInfo paymentInfo, string cardNumber, string expires, string account, string routing)
        {
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
                    Country = ISO3166.Alpha3FromName(paymentInfo.Country??person.CountryName) ?? null,
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

        public string VaultId(int peopleId)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            throw new NotImplementedException();
        }

        private string AchType(int? pid)
        {
            var type = "checking";
            if (pid.HasValue)
            {
                var usesaving = db.Setting("UseSavingAccounts");
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
