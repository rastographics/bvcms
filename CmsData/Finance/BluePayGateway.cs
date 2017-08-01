using System;
using System.Collections.Generic;
using System.Linq;
using BPCSharp;
using UtilityExtensions;

namespace CmsData.Finance
{
    public class BluePayGateway : IGateway
    {
        private readonly string _login;
        private readonly string _key;
        private readonly CMSDataContext db;

        private bool IsLive { get; set; }
        private string ServiceMode => IsLive ? "LIVE" : "TEST";

        public string GatewayType => "BluePay";

        public BluePayGateway(CMSDataContext db, bool testing)
        {
            this.db = db;
            IsLive = !(testing || db.Setting("GatewayTesting"));

            _login = db.Setting("bluepay_accountId", "");
            _key = db.Setting("bluepay_secretKey", "");

            if (string.IsNullOrWhiteSpace(_login))
                throw new Exception("bluepay_accountId setting not found, which is required for BluePay.");

            if (string.IsNullOrWhiteSpace(_key))
                throw new Exception("bluepay_secretKey setting not found, which is required for BluePay.");

        }
        //BluePay stores payment methods by simply using a past transaction's ID as a token.
        //We will do an Auth for $0, and save the resulting TransactionID as a Token in the payment profile BluePayCardVaultId.
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

            var first = paymentInfo.FirstName ?? person.FirstName;
            var last = paymentInfo.LastName ?? person.LastName;
            var addr = paymentInfo.Address ?? person.PrimaryAddress;
            var city = paymentInfo.City ?? person.PrimaryCity;
            var state = paymentInfo.State ?? person.PrimaryState;
            var zip = paymentInfo.Zip ?? person.PrimaryZip;
            var phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone;
            var email = person.EmailAddress;
            const string description = "Storing payment info in BluePay vault.";

            var gateway = createGateway();

            if (type == PaymentType.CreditCard)
            {
                gateway.setupCCTransaction(peopleId, cardNumber, expires, description, null, cardCode, email, first, last, addr, city, state, zip, phone);

                if (string.IsNullOrEmpty(paymentInfo.BluePayCardVaultId))
                    gateway.auth("0.00");
                else //send MasterId if available (this will allow for updating payment profile without re-entering card number)
                    gateway.auth("0.00", paymentInfo.BluePayCardVaultId);

                gateway.Process();
                var response = getResponse(gateway);

                if (!response.Approved)
                    throw new Exception(
                        $"BluePay failed to save the credit card info for people id: {person.PeopleId}, message: {response.Message}; transactionID: {response.TransactionId}");

                //save for future
                paymentInfo.BluePayCardVaultId = response.TransactionId;

                paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
                paymentInfo.Expires = expires;
            }
            //TODO: Handle bank accounts too (not just credit cards)
            else
                throw new ArgumentException($"Type {type} not supported", nameof(type));


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

            if (!string.IsNullOrEmpty(paymentInfo?.BluePayCardVaultId))
            {
                // clear out local record and save changes.
                //there is nothing to do on BluePay server since vault is really only a previous transaction ID.
                paymentInfo.BluePayCardVaultId = null;
                paymentInfo.MaskedCard = null;
                paymentInfo.MaskedAccount = null;
                paymentInfo.Expires = null;
                db.SubmitChanges();
            }
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            var gateway = createGateway();
            gateway.voidTransaction(reference);
            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            var gateway = createGateway();
            gateway.voidTransaction(reference);
            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "")
        {
            var gateway = createGateway();
            gateway.refund(reference, amt.ToString("F2"));
            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "")
        {
            var gateway = createGateway();
            gateway.refund(reference, amt.ToString("F2"));
            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description,
            int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state,
            string country, string zip, string phone)
        {
            var gateway = createGateway();
            gateway.setupCCTransaction(peopleId, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, city, state, zip, phone);

            gateway.auth(amt.ToString("F2"));

            gateway.Process();
            return getResponse(gateway);
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var gateway = createGateway();
            gateway.setupCCTransaction(peopleId, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, city, state, zip, phone);

            gateway.sale(amt.ToString("F2"));

            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var gateway = createGateway();
            gateway.setupACHTransaction(peopleId, routing, acct, description, tranid, email, first, last, addr, city, state, zip, phone);

            gateway.sale(amt.ToString("F2"));

            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid)
        {
            var paymentInfo = db.PaymentInfos.Single(pp => pp.PeopleId == peopleId);
            if (paymentInfo == null || !string.IsNullOrEmpty(paymentInfo.BluePayCardVaultId))
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            var masterId = paymentInfo.BluePayCardVaultId;

            var gateway = createGateway();
            gateway.setupVaultTransaction(description, tranid);

            gateway.auth(amt.ToString("F2"), masterId);

            gateway.Process();
            return getResponse(gateway);
        }



        //As long as the transaction was within the last 3 years, we can send the transaction ID as a parameter to use all the same info that was used in that transaction.
        //Any info that needs to be overrided (invoiceID, amount, description, etc.) can be done by simply including it with the TransactionID
        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            var paymentInfo = db.PaymentInfos.Single(pp => pp.PeopleId == peopleId);
            if (paymentInfo == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            string masterId;

            //TODO: Handle checking accounts too
            if (type == PaymentType.CreditCard)
                masterId = paymentInfo.BluePayCardVaultId;
            else
                throw new ArgumentException($"Type {type} not supported", nameof(type));

            var gateway = createGateway();
            gateway.setupVaultTransaction(description, tranid);

            gateway.sale(amt.ToString("F2"), masterId);

            gateway.Process();
            return getResponse(gateway);
        }

        public BatchResponse GetBatchDetails(DateTime start, DateTime end)
        {
            var batchTransactions = new List<BatchTransaction>();

            var gateway = createGateway();
            gateway.getTransactionSettledReport(start, end, true, false, false);
            gateway.Process();

            var report = new BluePayReport(gateway.getBatchListResponse());

            foreach (var transaction in report.GetTransactionList())
            {
                var batchTransaction = new BatchTransaction
                {
                    TransactionId = transaction.invoice_id.ToInt(),
                    Reference = transaction.id,
                    BatchReference = transaction.settlement_id,
                    TransactionType = GetTransactionType(transaction.trans_type),
                    BatchType = GetBatchType(transaction.payment_type),
                    Name = $"{transaction.name1} {transaction.name2}",
                    Amount = transaction.amount,
                    Approved = IsApproved(transaction.status),
                    Message = transaction.message,
                    TransactionDate = transaction.issue_date,
                    SettledDate = transaction.settle_date,
                    LastDigits = transaction.payment_account.Last(4)
                };
                batchTransactions.Add(batchTransaction);
            }


            return new BatchResponse(batchTransactions);
        }

        private static BatchType GetBatchType(string paymentMethod)
        {
            switch (paymentMethod)
            {
                case "CREDIT":
                    return BatchType.CreditCard;
                case "ACH":
                    return BatchType.Ach;
                default:
                    return BatchType.Unknown;
            }
        }

        private static TransactionType GetTransactionType(string transactionStatus)
        {

            switch (transactionStatus)
            {
                case "REFUND":
                    return TransactionType.Refund;
                case "SALE":
                case "CAPTURE":
                    return TransactionType.Charge;
                case "CREDIT":
                    return TransactionType.Credit;
                default:
                    return TransactionType.Unknown;

            }
        }

        public ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end)
        {
            return null;
        }

        private static bool IsApproved(string transactionStatus)
        {
            return transactionStatus == "1";
        }

        public bool CanVoidRefund => true;

        public bool CanGetSettlementDates => true;

        public bool CanGetBounces => false;

        public bool CanTakeBankAccounts => false;


        private BluePayPayment createGateway()
        {
            return new BluePayPayment(_login, _key, ServiceMode);
        }

        private TransactionResponse getResponse(BluePayPayment gateway)
        {
            return new TransactionResponse
            {
                Approved = gateway.getIsApproved(),
                AuthCode = gateway.getAuthCode(),
                Message = gateway.getMessage(),
                TransactionId = gateway.getTransID()
            };

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
            return paymentInfo?.BluePayCardVaultId;
        }
    }
}
