using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class BatchList : BaseResponse
    {
        public IEnumerable<Batch> Items { get; set; }
    }
}
