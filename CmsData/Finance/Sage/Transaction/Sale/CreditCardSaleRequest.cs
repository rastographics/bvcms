using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction.Sale
{
    internal class CreditCardSaleRequest : TransactionProcessingRequest
    {
        public CreditCardSaleRequest(string id, string key, CreditCard creditCard, decimal amount)
            : base(id, key, "BANKCARD_SALE")
        {
            creditCard.SetCreditCardData(Data);
            Data["T_CUSTOMER_NUMBER"] = string.Empty;
            Data["T_AMT"] = amount.ToString("n2");
        }

        public CreditCardSaleRequest(string id, string key, CreditCard creditCard, decimal amount, string orderNumber)
            : this(id, key, creditCard, amount)
        {
            Data["T_ORDERNUM"] = orderNumber;
        }

        public CreditCardSaleRequest(string id, string key, CreditCard creditCard, decimal amount, string orderNumber, string customerNumber)
            : this(id, key, creditCard, amount, orderNumber)
        {
            Data["T_CUSTOMER_NUMBER"] = customerNumber;
        }
    }
}
