using System;

namespace CmsData.Finance
{
    public class ReturnedCheck
    {
        public int TransactionId { get; set; }

        public string Name { get; set; }

        public string RejectCode { get; set; }

        public decimal RejectAmount { get; set; }

        public DateTime RejectDate { get; set; }
      
    }
}