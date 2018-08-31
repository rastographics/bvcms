using System;

namespace CmsData
{
    public partial class OrgSchedule
    {
        public override string ToString()
        {
            var dayOfWeek = Enum.GetName(typeof(DayOfWeek), SchedDay.Value);
            var name = $"{dayOfWeek} {SchedTime.Value:H:mm tt}";

            return name;
        }
    }
}
