using System;
using System.Collections.Generic;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public void Output(StringBuilder sb, TimeSlots ts)
        {
            if (ts.list.Count == 0)
                return;
            AddValueCk(0, sb, "TimeSlotLockDays", ts.TimeSlotLockDays);
            AddValueNoCk(0, sb, "TimeSlots", "");
            foreach (var c in ts.list)
            {
                AddValueCk(1, sb, c.Description);
                AddValueCk(2, sb, "Time", c.Time.ToString2("t"));
                AddValueCk(2, sb, "DayOfWeek", c.DayOfWeek);
                AddValueCk(2, sb, "Limit", c.Limit);
            }
            sb.AppendLine();
        }
        public TimeSlots ParseTimeSlots()
        {
            var ts = new TimeSlots();
            ts.TimeSlotLockDays = GetNullInt();
            if (curr.indent == 0)
                return ts;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                var slot = new TimeSlots.TimeSlot();
                if (curr.kw != Parser.RegKeywords.None)
                    throw GetException("unexpected line in TimeSlots");
                slot.Description = GetLine();
                if (curr.indent <= startindent)
                {
                    ts.list.Add(slot);
                    continue;
                }
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    switch (curr.kw)
                    {
                        case Parser.RegKeywords.Time:
                            slot.Time = GetTime();
                            break;
                        case Parser.RegKeywords.DayOfWeek:
                            slot.DayOfWeek = GetInt();
                            break;
                        case Parser.RegKeywords.Limit:
                            slot.Limit = GetInt();
                            break;
                        default:
                            throw GetException("unexpected line in TimeSlot");
                    }
                }
                ts.list.Add(slot);
            }
            return ts;
        }
    }
}
