using System;
using System.Collections.Generic;

namespace CmsData.Finance
{
    public class BatchResponse
    {
        public IList<Batch> Batches { get; private set; }

        public BatchResponse(IList<Batch> batchList)
        {
            Batches = batchList;
        }
    }

    public class Batch
    {
        public DateTime SettledDate { get; set; }
        public string Type { get; set; }
        public string BatchId { get; set; }

        public IList<BatchTransaction> Transactions { get; private set; }

        public Batch()
        {
            Transactions = new List<BatchTransaction>();
        }
    }

    public class BatchTransaction
    {
        public int TransactionId { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}