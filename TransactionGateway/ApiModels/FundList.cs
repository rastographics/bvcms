using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class FundList : BaseResponse
    {
        public IEnumerable<Fund> Items { get; set; }
    }
}
