using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class TimeSlots
	{
        public string Help { get { return @"
This is help for TimeSlots
"; } }
		public List<TimeSlot> list { get; private set; }
        [Display(Name="Lock Days")]
		public int? TimeSlotLockDays { get; set; }
		public bool HasValue { get { return list.Count > 0; } }

		public TimeSlots()
		{
			list = new List<TimeSlot>();
		}
		public void Output(StringBuilder sb)
		{
			if (list.Count == 0)
				return;
			Settings.AddValueCk(0, sb, "TimeSlotLockDays", TimeSlotLockDays);
			Settings.AddValueNoCk(0, sb, "TimeSlots", "");
			foreach (var c in list)
				c.Output(sb);
			sb.AppendLine();
		}

		public static TimeSlots Parse(Parser parser)
		{
			var ts = new TimeSlots();
			ts.TimeSlotLockDays = parser.GetNullInt();
			ts.list = new List<TimeSlot>();
			if (parser.curr.indent == 0)
				return ts;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var s = TimeSlot.Parse(parser, startindent);
				ts.list.Add(s);
			}
			return ts;
		}

		public class TimeSlot
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
			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, Description);
				Settings.AddValueCk(2, sb, "Time", Time.ToString2("t"));
				Settings.AddValueCk(2, sb, "DayOfWeek", DayOfWeek);
				Settings.AddValueCk(2, sb, "Limit", Limit);
			}
			public static TimeSlot Parse(Parser parser, int startindent)
			{
				var timeslot = new TimeSlot();
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line in TimeSlots");
				timeslot.Description = parser.GetLine();
				if (parser.curr.indent <= startindent)
					return timeslot;
				var ind = parser.curr.indent;
				while (parser.curr.indent == ind)
				{
					switch (parser.curr.kw)
					{
						case Parser.RegKeywords.Time:
							timeslot.Time = parser.GetTime();
							break;
						case Parser.RegKeywords.DayOfWeek:
							timeslot.DayOfWeek = parser.GetInt();
							break;
						case Parser.RegKeywords.Limit:
							timeslot.Limit = parser.GetInt();
							break;
						default:
							throw parser.GetException("unexpected line in TimeSlot");
					}
				}
				return timeslot;
			}
		}

        public void WriteXml(APIWriter w)
        {
			w.StartPending("TimeSlots");
			w.Attr("LockDays", TimeSlotLockDays);
			foreach (var c in list)
			{
			    w.Start("Slot");
			    w.Attr("Time", c.Time.ToString2("t"));
			    w.Attr("DayOfWeek", c.DayOfWeek);
			    w.Attr("Limit", c.Limit);
			    w.AddText(c.Description);
			    w.End();
			}
            w.EndPending();
        }
	}
}
