using CmsData.Codes;
using System;
using UtilityExtensions;

namespace CmsData
{
    public partial class ScheduledGift
    {
        public DateTime? Next(DateTime? fromDate = null)
        {
            var start = StartDate == null ? Util.Now : StartDate;
            var from = (fromDate ?? LastProcessed ?? start).Date;
            DateTime? next = null;

            if (!EndDate.HasValue || EndDate > from)
            {
                switch (ScheduledGiftTypeId)
                {
                    case ScheduledGiftTypeCode.Weekly:
                        next = from.AddDays(Interval * 7);
                        break;
                    case ScheduledGiftTypeCode.BiWeekly:
                        next = from.AddDays(Interval * 14);
                        break;
                    case ScheduledGiftTypeCode.SemiMonthly:
                        int day1 = Day1.GetValueOrDefault(1);
                        int day2 = Day2.GetValueOrDefault(15);
                        next = from.Day.IsBetween(day1, day2 - 1)
                            ? new DateTime(from.Year, from.Month, day2)
                            : new DateTime(from.Year, from.Month + Interval, day1);
                        break;
                    case ScheduledGiftTypeCode.Monthly:
                        next = from.AddMonths(Interval);
                        if (Day1 < next.Value.Day || (Day1 > next.Value.Day && Day1 <= next.Value.DaysInMonth()))
                        {
                            next = new DateTime(next.Value.Year, next.Value.Month, Day1.Value);
                        }
                        break;
                    case ScheduledGiftTypeCode.Quarterly:
                        next = from.AddMonths(Interval * 3);
                        if (Day1 < next.Value.Day || (Day1 > next.Value.Day && Day1 <= next.Value.DaysInMonth()))
                        {
                            next = new DateTime(next.Value.Year, next.Value.Month, Day1.Value);
                        }
                        break;
                    case ScheduledGiftTypeCode.Annually:
                        next = from.AddYears(Interval);
                        if (from.IsLastDayOfMonth())
                        {
                            next = next.Value.LastDayOfMonth();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"The value of ScheduledGiftTypeId was not recognized {ScheduledGiftTypeId }");
                }
                if (EndDate.HasValue && next > EndDate)
                {
                    next = null;
                }
            }
            return next;
        }
    }
}
