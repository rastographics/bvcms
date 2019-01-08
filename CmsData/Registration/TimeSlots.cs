using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;
using IronPython.Modules;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class TimeSlots
    {
        public string Help { get { return @"
This is help for TimeSlots
"; } }
        public List<TimeSlot> list { get; private set; }
        [Display(Name = "Lock Days")]
        public int? TimeSlotLockDays { get; set; }
        public bool HasValue { get { return list.Count > 0; } }

        public string FindDescription(DateTime dt) =>
                list.SingleOrDefault(vv => (DayOfWeek) vv.DayOfWeek == dt.DayOfWeek && vv.Time?.TimeOfDay == dt.TimeOfDay)?.Description;

        public TimeSlots()
        {
            list = new List<TimeSlot>();
        }
        public partial class TimeSlot
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public int DayOfWeek { get; set; }
            public int? Limit { get; set; }
            public string Name { get; set; }
            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:t}")]
            public DateTime? Time { get; set; }
            public DateTime Datetime()
            {
                var dt = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                return Time != null ?
                    dt.AddDays(DayOfWeek).Add(Time.Value.TimeOfDay)
                    : new DateTime();
            }

            public DateTime Datetime(DateTime dt)
            {
                return Time != null ?
                    dt.AddDays(DayOfWeek).Add(Time.Value.TimeOfDay)
                    : new DateTime();
            }
        }

        public void WriteXml(APIWriter w)
        {
            w.StartPending("TimeSlots");
            w.Attr("LockDays", TimeSlotLockDays);
            foreach (var c in list)
            {
                w.Start("Slot")
                    .Attr("Time", c.Time.ToString2("t"))
                    .Attr("DayOfWeek", c.DayOfWeek)
                    .Attr("Limit", c.Limit)
                    .AddText(c.Description)
                    .End();
            }
            w.EndPending();
        }
        public static TimeSlots ReadXml(XElement e)
        {
            var TimeSlots = new TimeSlots();
            TimeSlots.TimeSlotLockDays = e.Attribute("LockDays").ToInt2();
            foreach (var ele in e.Elements("Slot"))
                TimeSlots.list.Add(new TimeSlots.TimeSlot()
                {
                    Time = ele.Attribute("Time").ToDate(),
                    DayOfWeek = ele.Attribute("DayOfWeek").ToInt2() ?? 0,
                    Limit = ele.Attribute("Limit").ToInt2(),
                    Description = ele.Value,
                });
            return TimeSlots;
        }
    }
}
