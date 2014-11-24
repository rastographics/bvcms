using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction.Auth
{
    internal class CreditCardAuthRequest : TransactionProcessingRequest
    {
        public CreditCardAuthRequest(string id, string key, CreditCard creditCard, decimal amount)
            : base(id, key, "BANKCARD_AUTHONLY")
        {
            creditCard.SetCreditCardData(Data);
            Data["T_CUSTOMER_NUMBER"] = string.Empty;
            Data["T_AMT"] = amount.ToString("n2");
        }

        public CreditCardAuthRequest(string id, string key, CreditCard creditCard, decimal amount, string orderNumber)
            : this(id, key, creditCard, amount)
        {
            Data["T_ORDERNUM"] = orderNumber;
        }

        public CreditCardAuthRequest(string id, string key, CreditCard creditCard, decimal amount, string orderNumber, string customerNumber)
            : this(id, key, creditCard, amount, orderNumber)
        {
            Data["T_CUSTOMER_NUMBER"] = customerNumber;
        }
    }
}
