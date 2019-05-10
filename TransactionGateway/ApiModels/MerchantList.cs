using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class MerchantList : BaseResponse 
    {
        public IEnumerable<Merchant> Items { get; set; }
    }
}
