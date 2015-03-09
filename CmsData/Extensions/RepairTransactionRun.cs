using System;

namespace CmsData
{
    public partial class RepairTransactionsRun
    {
        public TimeSpan? Elapsed
        {
            get
            {
                if(Completed.HasValue && Started.HasValue)
                    return Completed.Value.Subtract(Started.Value);
                return null;
            }
        }

        public int? NumberOfRecords
        {
            get { return Count; }
        }
    }
}