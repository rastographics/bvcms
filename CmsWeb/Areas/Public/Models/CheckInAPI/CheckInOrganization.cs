using System;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.CheckInAPI
{
    public class CheckInOrganization
    {
        public int id;
        public string name;
        public string leader;
        public DateTime? hour;

        public double leadTime = 0;

        public string location;

        public DateTime? birthdayStart;
        public DateTime? birthdayEnd;

        public void adjustLeadTime(int day, int tzOffset)
        {
            if (hour.HasValue)
            {
                var theirTime = DateTime.Now.AddHours(tzOffset);

                if (DateTime.Now.DayOfWeek.ToInt() != day)
                {
                    int dayDiff = day - DateTime.Now.DayOfWeek.ToInt();

                    if (dayDiff < 0)
                        theirTime = theirTime.AddDays(7 + dayDiff);
                    else
                        theirTime = theirTime.AddDays(dayDiff);
                }

                leadTime = hour.Value.Subtract(theirTime).TotalHours;
            }
        }
    }
}