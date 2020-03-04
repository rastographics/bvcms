using UtilityExtensions;

namespace CmsData
{
    public partial class ActivityLog
    {
        public string HowLongAgo
        {
            get
            {
                var delta = Util.Now.Subtract(ActivityDate.Value);
                if (delta.Days > 7)
                    return ActivityDate.Value.ToShortDateString();
                if (delta.Days > 1)
                    return $"{delta.Days} days ago";
                if (delta.Days == 1)
                    return "Yesterday";
                if (delta.Hours > 1)
                    return $"{delta.Hours} hours ago";
                if (delta.Hours == 1)
                    return "1 hour ago";
                if (delta.Minutes > 1)
                    return $"{delta.Minutes} minutes ago";
                if (delta.Minutes == 1)
                    return "1 minute ago";
                return $"{delta.Seconds} seconds ago";
            }
        }
    }
}
