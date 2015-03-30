using System;

namespace CmsData
{
    public partial class DeleteMeetingRun
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

        public int? NumberOfAttends
        {
            get { return Count; }
        }
    }
}