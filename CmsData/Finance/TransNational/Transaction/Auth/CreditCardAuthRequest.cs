using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Transaction.Auth
{
    internal class CreditCardAuthRequest : TransactRequest
    {
        public CreditCardAuthRequest(string userName, string password, CreditCard creditCard, decimal amount) 
            : base(userName, password)
        {
            Data["type"] = "auth";
            Data["payment"] = "creditcard";
            creditCard.SetCreditCardData(Data);
            Data["amount"] = amount.ToString("0.00");
        }

        public CreditCardAuthRequest(string userName, string password, CreditCard creditCard, decimal amount, string orderId)
            : this(userName, password, creditCard, amount)
        {
            Data["orderid"] = orderId;
        }

        public CreditCardAuthRequest(string userName, string password, CreditCard creditCard, decimal amount, string orderId, string orderDescription)
            : this(userName, password, creditCard, amount, orderId)
        {
            Data["orderdescription"] = orderDescription;
        }

        public CreditCardAuthRequest(string userName, string password, CreditCard creditCard, decimal amount, string orderId, string orderDescription, string poNumber)
            : this(userName, password, creditCard, amount, orderId, orderDescription)
        {
            Data["ponumber"] = poNumber;
        }
    }
}
