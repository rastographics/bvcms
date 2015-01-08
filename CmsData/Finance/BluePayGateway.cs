using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UtilityExtensions;
using UtilityExtensions.Extensions;
using BPCSharp;

namespace CmsData.Finance
{
    public class BluePayGateway : IGateway
    {
        private readonly string _login;
        private readonly string _key;
        private readonly CMSDataContext db;

        private bool IsLive { get; set; }
        private string ServiceMode { get { return IsLive ? "LIVE" : "TEST"; } }

        public string GatewayType { get { return "BluePay"; } }

        public BluePayGateway(CMSDataContext db, bool testing)
        {
            this.db = db;
            IsLive = !(testing || db.Setting("GatewayTesting", "false").ToLower() == "true");
           
            _login = db.Setting("bluepay_accountId", "");
            _key = db.Setting("bluepay_secretKey", "");

            if (string.IsNullOrWhiteSpace(_login))
                throw new Exception("bluepay_accountId setting not found, which is required for BluePay.");
                        
            if (string.IsNullOrWhiteSpace(_key))
                throw new Exception("bluepay_secretKey setting not found, which is required for BluePay.");
          
        }
        //BluePay stores payment methods by simply using a past transaction's ID as a token. 
        //We will do an Auth for $0, and save the resulting TransactionID as a Token in the payment profile.
        //For now, we will use the TbnCardVaultId field, to avoid changing the DB schema.
        //(Future solution should involve creating a way to add new payment gateways without changing db schema)
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

            string first, last, addr, city, state, zip, phone, email, description;
            first = paymentInfo.FirstName ?? person.FirstName;
            last = paymentInfo.LastName ?? person.LastName;
            addr = paymentInfo.Address ?? person.PrimaryAddress;
            city = paymentInfo.City ?? person.PrimaryCity;
            state = paymentInfo.State ?? person.PrimaryState;
            zip = paymentInfo.Zip ?? person.PrimaryZip;
            phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone;
            email = person.EmailAddress;
            description = "Storing payment info in BluePay vault.";
            

            var gateway = createGateway();

            //TODO: Handle bank accounts too (not just credit cards)
            gateway.setupCCTransaction(peopleId, cardNumber, expires, description, null, cardCode, email, first, last, addr, city, state, zip, phone);

            if (paymentInfo.TbnCardVaultId == null)
                gateway.auth("0.00");
            else //send MasterId if available (this will allow for updating payment profile without changing cardnum)
                gateway.auth("0.00", paymentInfo.TbnCardVaultId.Value.ToString());

            gateway.Process();
            var response = getResponse(gateway);

            if (!response.Approved)
                throw new Exception(
                    "Payment Provider failed to update the credit card info for people id: {0}, message: {1}".Fmt(
                        person.PeopleId, response.Message));
            
            
            int tranId;
            if(int.TryParse(response.TransactionId, out tranId))
            {
                // use TbnCardVaultId for now to save TransactionID as Token for future vault payments
                paymentInfo.TbnCardVaultId  = tranId;
            }
            else
                throw new Exception("Vault failed to save for {0}. Invalid transaction ID: {1}".Fmt(peopleId, response.TransactionId));
            
            
            //TODO: Handle bank accounts too (not just credit cards)
         if (type == PaymentType.CreditCard)
            {
                paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
                paymentInfo.Ccv = cardCode;
                paymentInfo.Expires = expires;
            }
            else
                throw new ArgumentException("Type {0} not supported".Fmt(type), "type");

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

            if (paymentInfo.TbnCardVaultId.HasValue)
            {
                // clear out local record and save changes.
                //there is nothing to do on BluePay server since vault is really only a previous transaction ID.
                paymentInfo.TbnCardVaultId = null;
                paymentInfo.MaskedCard = null;
                paymentInfo.MaskedAccount = null;
                paymentInfo.Ccv = null;
                paymentInfo.Expires = null; //TODO: on the other gateways, this does not get cleared...
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
            gateway.refund(reference,amt.ToString("F2"));
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
            int tranid, string cardcode, string email, string first, string last, string addr, string city, string state,
            string zip, string phone)
        {
            var gateway = createGateway();
            gateway.setupCCTransaction(peopleId, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, city, state, zip, phone);
            
            gateway.auth(amt.ToString("F2"));
            
            gateway.Process();
            return getResponse(gateway);
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            var gateway = createGateway();
            gateway.setupCCTransaction(peopleId, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, city, state, zip, phone);

            gateway.sale(amt.ToString("F2"));
            
            gateway.Process();

            return getResponse(gateway);
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone)
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
            if (paymentInfo == null || !paymentInfo.TbnCardVaultId.HasValue)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };
            
            var masterId = paymentInfo.TbnCardVaultId.ToString();

            var gateway = createGateway();
            gateway.setupVaultTransaction(description, tranid);

            gateway.auth(amt.ToString("F2"),masterId);

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
               masterId = paymentInfo.TbnCardVaultId.ToString();
            else
                throw new ArgumentException("Type {0} not supported".Fmt(type), "type");

            var gateway = createGateway();
            gateway.setupVaultTransaction(description, tranid);

            gateway.auth(amt.ToString("F2"), masterId);

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
                        Name = "{0} {1}".Fmt(transaction.name1, transaction.name2),
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
                case "SALE": case "CAPTURE":
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

        public bool CanVoidRefund
        {
            get { return true; }
        }

        public bool CanGetSettlementDates
        {
            get { return true; }
        }

        public bool CanGetBounces
        {
            get { return false; }
        }

        public bool CanTakeBankAccounts
        {
            get { return false; }
        }

        
        private BluePayPayment createGateway()
        {
           return new BluePayPayment(_login, _key, ServiceMode);
        }

        private  TransactionResponse getResponse(BluePayPayment gateway)
        {
            return new TransactionResponse
            {
                Approved = gateway.getIsApproved(),
                AuthCode = gateway.getAuthCode(),
                Message = gateway.getMessage(),
                TransactionId = gateway.getTransID()
            };

        }

    }
}
