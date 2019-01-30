using System;
using System.Text;

namespace UtilityExtensions
{
    /// <summary>
    /// Class to format a start and end date/time.
    /// </summary>
    /// 
    public partial class DateTimeRangeFormatter
    {
        /// <summary>
        /// Sets or gets whether full month names are used.
        /// </summary>
        public bool UseFullMonthName { get; set; }

        /// <summary>
        /// Sets or gets whether day, month format is used.
        /// Otherwise, month, day format is used.
        /// </summary>
        public bool UseDayMonthFormat { get; set; }

        /// <summary>
        /// Sets or gets whether the day of the week is displayed.
        /// </summary>
        public bool ShowDayOfWeek { get; set; }

        /// <summary>
        /// Sets or gets whether the time of day is displayed.
        /// </summary>
        public bool ShowTime { get; set; }

        /// <summary>
        /// If true, and the end time is 12:00 AM of a date that is after the
        /// start date, a day is subtracted from the end date so that midnight
        /// is not counted as another day.
        /// </summary>
        public bool TreatMidnightAsSameDay { get; set; }

        public DateTimeRangeFormatter()
        {
            // Set defaults
            UseFullMonthName = true;
            UseDayMonthFormat = false;
            ShowDayOfWeek = false;
            ShowTime = false;
            TreatMidnightAsSameDay = false;
        }

        /// <summary>
        /// Formats a date range using the current settings.
        /// </summary>
        /// <param name="startDate">Start date/time</param>
        /// <param name="endDate">End date/time</param>
        public string FormatDateRange(DateTime startDate, DateTime endDate)
        {
            // Ensure that start date is not after the end date
            if (endDate < startDate)
            {
                endDate = startDate;
            }

            // Special handling for events that end at midnight
            if (TreatMidnightAsSameDay && endDate.TimeOfDay.Ticks == 0 && startDate.Date < endDate.Date)
            {
                endDate = endDate.AddDays(-1);
            }

            string result;
            if (startDate.Date == endDate.Date)
            {
                result = FormatDateTime(startDate, DateTimeParts.Day | DateTimeParts.Month | DateTimeParts.Year);
                if (ShowTime)
                {
                    result += " {0} - {1}";
                }
            }
            else
            {
                result = String.Format(ShowTime ?
                    "{0} {{0}} - {1} {{1}}" :
                    "{0} - {1}",
                    FormatDateTime(startDate,
                        startDate.Year == endDate.Year ?
                        DateTimeParts.Day | DateTimeParts.Month :
                        DateTimeParts.Day | DateTimeParts.Month | DateTimeParts.Year),
                    FormatDateTime(endDate,
                        startDate.Year == endDate.Year && startDate.Month == endDate.Month ?
                        DateTimeParts.Day | DateTimeParts.Year :
                        DateTimeParts.Day | DateTimeParts.Month | DateTimeParts.Year));
            }
            return (ShowTime) ?
                String.Format(result, startDate.ToShortTimeString(), endDate.ToShortTimeString()) :
                result;
        }

        /// <summary>
        /// Used by FormatDateTime to specify which date/time parts
        /// are to be displayed.
        /// </summary>
        [Flags]
        private enum DateTimeParts
        {
            Day = 0x01,
            Month = 0x02,
            Year = 0x04,
        }

        /// <summary>
        /// Formats a single date/time value using the current settings.
        /// </summary>
        private string FormatDateTime(DateTime dt, DateTimeParts parts)
        {
            StringBuilder sb = new StringBuilder();

            if (parts.HasFlag(DateTimeParts.Day) && ShowDayOfWeek)
            {
                sb.Append(parts.HasFlag(DateTimeParts.Month) ? "dddd, " : "dddd ");
            }
            if (parts.HasFlag(DateTimeParts.Day))
            {
                if (parts.HasFlag(DateTimeParts.Month))
                {
                    if (UseDayMonthFormat)
                    {
                        sb.Append(UseFullMonthName ? "d MMMM" : "d MMM");
                    }
                    else
                    {
                        sb.Append(UseFullMonthName ? "MMMM d" : "MMM d");
                    }
                }
                else
                {
                    sb.Append("%d");
                }
            }
            if (parts.HasFlag(DateTimeParts.Year))
            {
                sb.Append(UseDayMonthFormat ? " yyyy" : ", yyyy");
            }
            return dt.ToString(sb.ToString());
        }
    }
}
