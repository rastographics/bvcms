
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction.Sale
{
    internal class AchSaleRequest : TransactionProcessingRequest
    {
        public AchSaleRequest(string id, string key, string originatorId, Ach ach, decimal amount)
            : base(id, key, "VIRTUAL_CHECK_PPD_SALE")
        {
            Data["C_ORIGINATOR_ID"] = originatorId;
            ach.SetAchData(Data);
            Data["T_AMT"] = amount.ToString("n2");
        }

        public AchSaleRequest(string id, string key, string originatorId, Ach ach, decimal amount, string orderNumber)
            : this(id, key, originatorId, ach, amount)
        {
            Data["T_ORDERNUM"] = orderNumber;
        }
    }
}
