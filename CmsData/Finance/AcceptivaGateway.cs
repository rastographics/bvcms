using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Finance
{
    public class AcceptivaGateway : IGateway
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly CMSDataContext db;

        public string GatewayType => "Acceptiva";

        public bool CanVoidRefund => throw new NotImplementedException();

        public bool CanGetSettlementDates => throw new NotImplementedException();

        public bool UseIdsForSettlementDates => throw new NotImplementedException();

        public bool CanGetBounces => throw new NotImplementedException();

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
            throw new NotImplementedException();
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}
