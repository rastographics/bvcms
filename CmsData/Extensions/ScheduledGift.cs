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
                        next = from.AddDays(7);
                        break;
                    case ScheduledGiftTypeCode.BiWeekly:
                        next = from.AddDays(14);
                        break;
                    case ScheduledGiftTypeCode.SemiMonthly:
                        next = from.Day.IsBetween(1, 14)
                            ? new DateTime(from.Year, from.Month, 15)
                            : new DateTime(from.Year, from.Month + 1, 1);
                        break;
                    case ScheduledGiftTypeCode.Monthly:
                        next = from.AddMonths(1);
                        break;
                    case ScheduledGiftTypeCode.Quarterly:
                        next = from.AddMonths(3);
                        if (from.IsLastDayOfMonth())
                        {
                            next = next.Value.LastDayOfMonth();
                        }
                        break;
                    case ScheduledGiftTypeCode.Annually:
                        next = from.AddYears(1);
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
