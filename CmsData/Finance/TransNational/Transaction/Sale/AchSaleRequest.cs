using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Transaction.Sale
{
    internal class AchSaleRequest : TransactRequest
    {
        public AchSaleRequest(string userName, string password, Ach ach, decimal amount) 
            : base(userName, password)
        {
            Data["type"] = "sale";
            Data["payment"] = "check";
            ach.SetAchData(Data);
            Data["amount"] = amount.ToString("0.00");
        }

        public AchSaleRequest(string userName, string password, Ach ach, decimal amount, string orderId)
            : this(userName, password, ach, amount)
        {
            Data["orderid"] = orderId;
        }

        public AchSaleRequest(string userName, string password, Ach ach, decimal amount, string orderId, string orderDescription)
            : this(userName, password, ach, amount, orderId)
        {
            Data["orderdescription"] = orderDescription;
        }

        public AchSaleRequest(string userName, string password, Ach ach, decimal amount, string orderId, string orderDescription, string customerId)
            : this(userName, password, ach, amount, orderId, orderDescription)
        {
            Data["customer_id"] = customerId;
        }
    }
}
