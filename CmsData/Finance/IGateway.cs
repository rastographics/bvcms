using System;
using CmsData.Finance;

namespace CmsData
{
    public interface IGateway
    {
        string GatewayType { get; }

        bool CanVoidRefund { get;}
        bool CanGetSettlementDates { get;}
        bool CanGetBounces { get;}

        void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving);
        void RemoveFromVault(int peopleId);
        TransactionResponse VoidCreditCardTransaction(string reference);
        TransactionResponse VoidCheckTransaction(string reference);
        TransactionResponse RefundCreditCard(string reference, Decimal amt, string lastDigits = "");
        TransactionResponse RefundCheck(string reference, Decimal amt, string lastDigits = "");
        TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone);
        TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone);
        TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone);
        TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid);
        TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type);
        BatchResponse GetBatchDetails(DateTime start, DateTime end);
        ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end);
    }
}
