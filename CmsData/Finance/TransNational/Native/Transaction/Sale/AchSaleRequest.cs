using CmsData.Finance.TransNational.Native.Core;

namespace CmsData.Finance.TransNational.Native.Transaction.Sale
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
    }
}
