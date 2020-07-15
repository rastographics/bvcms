using System;
using System.Collections.Generic;
using CmsData.Finance;

namespace CmsData
{
    public interface IGateway
    {
        bool CanGetBounces { get; }
        bool CanGetSettlementDates { get; }
        bool CanVoidRefund { get; }
        int GatewayAccountId { get; }
        string GatewayName { get; }
        string GatewayType { get; }
        string Identifier { get; }
        bool UseIdsForSettlementDates { get; }

        TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone, bool testing = false);
        [Obsolete]
        TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid);
        TransactionResponse AuthCreditCardVault(PaymentMethod paymentMethod, decimal amt, string description, int tranid, string lastName, string firstName, string address, string address2, string city, string state, string country, string zip, string phone, string emailAddress);
        void CheckBatchSettlements(DateTime start, DateTime end);
        void CheckBatchSettlements(List<string> transactionids);
        BatchResponse GetBatchDetails(DateTime start, DateTime end);
        ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end);
        TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string addr2, string city, string state, string country, string zip, string phone);
        TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone);
        TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type);
        TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "");
        TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "");
        void RemoveFromVault(int peopleId);
        void StoreInVault(PaymentMethod paymentMethod, string type, string cardNumber, string cvv, string bankAccountNum, string bankRoutingNum, int? expireMonth, int? expireYear, string address, string address2, string city, string state, string country,string zip, string phone, string emailAddress);
        void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving);
        string VaultId(int peopleId);
        TransactionResponse VoidCheckTransaction(string reference);
        TransactionResponse VoidCreditCardTransaction(string reference);
    }
}
