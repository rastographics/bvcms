using System.Collections.Generic;

namespace CmsData.Finance
{
    public class BatchResponse
    {
        public IEnumerable<BatchTransaction> BatchTransactions { get; private set; }

        public BatchResponse(IEnumerable<BatchTransaction> batchTransactions)
        {
            BatchTransactions = batchTransactions;
        }
    }
}