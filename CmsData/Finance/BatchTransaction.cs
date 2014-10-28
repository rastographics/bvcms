using System;

namespace CmsData.Finance
{
    public class BatchTransaction
    {
        /// <summary>
        /// This is our transaction id.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// This is the gateway's transaction id.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// This is the gateway's batch id.
        /// </summary>
        public string BatchReference { get; set; }

        public TransactionType TransactionType { get; set; }

        public BatchType BatchType { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public bool Approved { get; set; }

        public string Message { get; set; } 

        public DateTime TransactionDate { get; set; }
        
        public DateTime SettledDate { get; set; }
    }
}