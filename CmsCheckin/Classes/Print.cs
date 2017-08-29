using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace CmsCheckin
{
	public static class Print
	{
		public static void BlankLabel(this MemoryStream ms, bool datamaxonly)
		{
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("Datamax")) {
				sw.WriteLine("\x02L");
				sw.WriteLine("H07");
				sw.WriteLine("D11");
				sw.WriteLine("E");
			} else if (!datamaxonly && Program.settings.printer.Contains("ZDesigner")) {
				sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				sw.WriteLine("^XA");
				sw.WriteLine("^MMT");
				sw.WriteLine("^PW609");
				sw.WriteLine("^LL0406");
				sw.WriteLine("^LS0");
				sw.WriteLine("^PQ1,0,1,Y^XZ");
			}
			sw.Flush();
		}

		public static void LabelKiosk(this MemoryStream ms, LabelInfo li)
		{
			if (!Program.settings.printer.HasValue())
				return;
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("ZDesigner")) {
				//sw.WriteLine("CT~~CD,~CC^~CT~");
				sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				sw.WriteLine("^XA");
				sw.WriteLine("^MMT");
				sw.WriteLine("^PW609");
				sw.WriteLine("^LL0203");
				sw.WriteLine("^LS0");
				sw.WriteLine(string.Format(@"^FT592,122^A0I,79,79^FH\^FD{0}^FS", li.first));
				sw.WriteLine(string.Format(@"^FT583,69^A0I,34,33^FH\^FD{0}^FS", li.last));
				var al = "na";
				if (li.allergies.HasValue())
					al = li.allergies;
				sw.WriteLine(string.Format(@"^FT585,26^A0I,23,24^FH\^FD{0}, ({1})^FS", li.org, al));
				sw.WriteLine(string.Format(@"^^FT133,65^A0I,51,50^FH\^FD{0}^FS", li.location));
				sw.WriteLine(string.Format("^PQ{0},0,1,Y^XZ", li.n));
			} else if (Program.settings.printer.Contains("Datamax")) {
				sw.WriteLine("\x02n");
				sw.WriteLine("\x02M0500");
				sw.WriteLine("\x02O0220");
				sw.WriteLine("\x02V0");
				sw.WriteLine("\x02SG");
				sw.WriteLine("\x02d");
				sw.WriteLine("\x01D");
				sw.WriteLine("\x02L");
				sw.WriteLine("D11");
				sw.WriteLine("PG");
				sw.WriteLine("pC");
				sw.WriteLine("SG");
				sw.WriteLine("ySPM");
				sw.WriteLine("A2");
				sw.WriteLine("1911A2400490006" + li.first);
				sw.WriteLine("1911A1200280008" + li.last);
				sw.WriteLine(string.Format("1911A0800050007{0}, ({1})", li.org, li.allergies));
				sw.WriteLine("1911A2400280190" + li.location);
				sw.WriteLine("Q" + li.n.ToString("0000"));
				sw.WriteLine("E");
			} else {
				sw.WriteLine(li.first);
				sw.WriteLine(li.last);
				sw.WriteLine(string.Format("{0}, ({1})", li.org, li.allergies));
				sw.WriteLine(li.location);
			}

			sw.Flush();
			ms.BlankLabel(true);
		}

		public static void LabelKiosk2(this MemoryStream ms, LabelInfo li)
		{
			if (!Program.settings.printer.HasValue())
				return;
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("Godex")) {
				StartGodexLabel(sw, 1);
				sw.WriteLine("AH,18,0,1,1,0,0," + li.first);
				sw.WriteLine("AE,18,84,1,1,0,0," + li.last);
				var al = li.allergies;
				if (!al.HasValue())
					al = "na";
				sw.WriteLine("AC,18,133,1,1,0,0,(" + al + ")");
				sw.WriteLine("AC,19,191,1,1,0,0" + li.org);
				sw.WriteLine("AC,19,227,1,1,0,0," + li.location);
				sw.WriteLine("E");
			}
			sw.Flush();
		}

		public static int SecurityLabel(this MemoryStream ms, DateTime time, string code, int n)
		{
			if (!Program.settings.printer.HasValue())
				return 0;
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("ZDesigner")) {
				sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				sw.WriteLine("^XA");
				sw.WriteLine("^MMT");
				sw.WriteLine("^PW609");
				sw.WriteLine("^LL0406");
				sw.WriteLine("^LS0");
				sw.WriteLine(string.Format(@"^FT81,115^A0N,73,72^FH\^FD{0}^FS", code));
				sw.WriteLine(string.Format(@"^FT82,170^A0N,34,33^FH\^FD{0:d}^FS", time));
				sw.WriteLine(string.Format(@"^FT395,170^A0N,34,33^FH\^FD{0:d}^FS", time));
				sw.WriteLine(string.Format(@"^FT397,115^A0N,73,72^FH\^FD{0}^FS", code));
				sw.WriteLine(@"^FO310,15^GB0,175,2^FS");
				sw.WriteLine("^PQ1,0,1,Y^XZ");
			} else if (Program.settings.printer.Contains("Datamax")) {
				sw.WriteLine("\x02n");
				sw.WriteLine("\x02M0500");
				sw.WriteLine("\x02O0220");
				sw.WriteLine("\x02V0");
				sw.WriteLine("\x02SG");
				sw.WriteLine("\x02d");
				sw.WriteLine("\x01D");
				sw.WriteLine("\x02ICAFgfx0");
				sw.WriteLine("0000FF98");
				sw.WriteLine("8001E0");
				sw.WriteLine("FFFF");
				sw.WriteLine("\x02L");
				sw.WriteLine("D11");
				sw.WriteLine("PG");
				sw.WriteLine("pC");
				sw.WriteLine("SG");
				sw.WriteLine("ySPM");
				sw.WriteLine("A2");
				sw.WriteLine("1911A2400360035" + code);
				sw.WriteLine("1911A1200050036" + time.ToString("d"));
				sw.WriteLine("1911A1200050190" + time.ToString("d"));
				sw.WriteLine("1911A2400360191" + code);
				sw.WriteLine("1Y1100000020148gfx0");
				sw.WriteLine("Q0001");
				sw.WriteLine("E");
			}

			sw.Flush();
			return 1;
		}

		public static int SecurityLabel2(this MemoryStream ms, DateTime time, string code, int n)
		{
			if (!Program.settings.printer.HasValue())
				return 0;
			var sw = new StreamWriter(ms);

			if (Program.settings.printer.Contains("ZDesigner")) {
				StartZDesignerLabel(sw);
				sw.WriteLine(string.Format(@"^FT68,151^A0N,102,100^FH\^FD{0}^FS", code));
				sw.WriteLine(string.Format(@"^FT87,225^A0N,34,33^FH\^FD{0:d}^FS", time));
				sw.WriteLine(string.Format(@"^FT410,227^A0N,34,33^FH\^FD{0:t}^FS", time));
				sw.WriteLine(string.Format(@"^FT386,152^A0N,102,100^FH\^FD{0}^FS", code));
				sw.WriteLine(@"^FO310,29^GB0,350,2^FS");
				sw.WriteLine("^PQ{0},0,1,Y^XZ".Fmt(n));
			} else if (Program.settings.printer.Contains("Godex")) {
				StartGodexLabel(sw, n);
				sw.WriteLine("AH,68,78,1,1,0,0," + code);
				sw.WriteLine("AH,376,78,1,1,0,0," + code);
				sw.WriteLine("Lo,296,36,303,379");
				sw.WriteLine("AE,74,174,1,1,0,0,{0:d}".Fmt(time));
				sw.WriteLine("AE,380,174,1,1,0,0,{0:t}".Fmt(time));
				sw.WriteLine("E");
			}
			sw.Flush();
			return 1;
		}

		public static int Label(this MemoryStream ms, LabelInfo li, int nlabels, string code)
		{
			//MainLabel
			if (nlabels <= 0 || !Program.settings.printer.HasValue())
				return 0;
			var sw = new StreamWriter(ms);

			string lorg = "";
			if (li.location.HasValue())
				if (!li.org.StartsWith(li.location))
					lorg = li.location + ", ";
			lorg += li.org;

			if (Program.settings.printer.Contains("ZDesigner")) {
				sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				sw.WriteLine("^XA");
				sw.WriteLine("^MMT");
				sw.WriteLine("^PW609");
				sw.WriteLine("^LL0406");
				sw.WriteLine("^LS0");
				sw.WriteLine(@"^FT29,75^A0N,62,62^FH\^FD{0}^FS".Fmt(li.first));
				sw.WriteLine(@"^FT29,118^A0N,28,28^FH\^FD{0}^FS".Fmt(li.last));
				sw.WriteLine(@"^FT29,153^A0N,28,28^FH\^FD{0} |{1}{2}{3}^FS".Fmt(li.mv.Replace("V", "G"),
					li.allergies.HasValue() ? " A |" : "",
					li.transport ? " T |" : "",
					li.custody ? " C |" : ""));
				sw.WriteLine(@"^FT29,185^A0N,28,28^FH\^FD{0}^FS".Fmt(lorg));
				sw.WriteLine(@"^FT473,48^A0N,28,28^FH\^FD{0:d}^FS".Fmt(li.hour));
				sw.WriteLine(@"^FT474,77^A0N,28,28^FH\^FD{0:t}^FS".Fmt(li.hour));
				sw.WriteLine(@"^FT470,152^A0N,73,72^FH\^FD{0}^FS".Fmt(code));
				sw.WriteLine("^PQ{0},0,1,Y^XZ".Fmt(nlabels));
			} else if (Program.settings.printer.Contains("Datamax")) {
				sw.WriteLine("\x02n");
				sw.WriteLine("\x02M0500");
				sw.WriteLine("\x02O0220");
				sw.WriteLine("\x02V0");
				sw.WriteLine("\x02SG");
				sw.WriteLine("\x02d");
				sw.WriteLine("\x01D");
				sw.WriteLine("\x02L");
				sw.WriteLine("D11");
				sw.WriteLine("PG");
				sw.WriteLine("pC");
				sw.WriteLine("SG");
				sw.WriteLine("ySPM");
				sw.WriteLine("A2");
				sw.WriteLine("1911A1800500010" + li.first);
				sw.WriteLine("1911A1000350011" + li.last);
				sw.WriteLine("1911A1000190011{0} |{1}{2}{3}".Fmt(li.mv.Replace("V", "G"),
					li.allergies.HasValue() ? " A |" : "",
					li.transport ? " T |" : "",
					li.custody ? " C |" : ""));
				sw.WriteLine("1911A1000020011" + lorg);
				sw.WriteLine("1911A1000610222{0:d}".Fmt(li.hour));
				sw.WriteLine("1911A1000470222{0:t}".Fmt(li.hour));
				sw.WriteLine("1911A2400140219" + code);

				sw.WriteLine("Q" + nlabels.ToString("0000"));
				sw.WriteLine("E");
			}
			sw.Flush();
			return nlabels;
		}

		public static int Label2(this MemoryStream ms, IEnumerable<LabelInfo> li, int nlabels, string code)
		{
			if (nlabels <= 0 || !Program.settings.printer.HasValue())
				return 0;
			var sw = new StreamWriter(ms);

			var list = new List<string>();
			if (li.First().allergies.HasValue())
				list.Add(" A ");
			if (li.First().transport)
				list.Add(" T ");
			if (li.First().custody)
				list.Add(" C ");
			var s = string.Join("|", list.ToArray());

			if (Program.settings.printer.Contains("ZDesigner")) {
				StartZDesignerLabel(sw);
				sw.WriteLine(@"^FT27,87^A0N,79,79^FH\^FD{0}^FS".Fmt(li.First().first));
				sw.WriteLine(@"^FT26,133^A0N,34,33^FH\^FD{0}^FS".Fmt(li.First().last));
				sw.WriteLine(@"^FT459,150^A0N,28,28^FH\^FD{0:d}^FS".Fmt(li.First().hour));
				sw.WriteLine(@"^FT444,223^A0N,79,79^FH\^FD{0}^FS".Fmt(code));

				sw.WriteLine(@"^FO16,138^GB206,54,8^FS");
				sw.WriteLine(@"^FT27,174^A0N,28,28^FH\^FDWear this label^FS");
				sw.WriteLine(@"^FT241,178^A0N,28,28^FH\^FD{0}^FS".Fmt(s));

				var vertpos = 352;
				foreach (var i in li.OrderByDescending(ll => ll.hour)) {
					var loc = i.location;
					if (!loc.HasValue())
						loc = "n/a";

					sw.WriteLine(@"^FT28,{0}^A0N,28,28^FH\^FD{1}^FS".Fmt(vertpos, loc));
					sw.WriteLine(@"^FT112,{0}^A0N,28,28^FH\^FD{1}^FS".Fmt(vertpos,
						"{0:h:mm t}".Fmt(i.hour).PadLeft(7, '~').Replace("~", "  ")));
					sw.WriteLine(@"^FT42,{0}^A0N,23,24^FH\^FD{1} ({2})^FS".Fmt(vertpos + 28, i.org, i.mv.Replace("V", "G")));

					vertpos -= 57;
				}
				sw.WriteLine(@"^PQ{0},0,1,Y^XZ".Fmt(nlabels));

				sw.Flush();
				foreach (var i in li)
					ms.AllergyLabel2(i, code);
				// Do the Extra labels here
			} else if (Program.settings.printer.Contains("Godex")) {
				StartGodexLabel(sw, 1);
				var item = li.First();
				if (!item.requiressecuritylabel) {
					sw.WriteLine("AH,36,40,1,1,0,0," + li.First().first);
					sw.WriteLine("AH,36,148,1,1,0,0," + li.First().last);
				} else {
					sw.WriteLine("AH,18,0,1,1,0,0," + li.First().first);
					sw.WriteLine("AE,18,84,1,1,0,0," + li.First().last);
					sw.WriteLine("AD,433,115,1,1,0,0,{0:d}".Fmt(li.First().hour));
					sw.WriteLine("AH,428,136,1,1,0,0," + code);
					sw.WriteLine("R14,140,239,181,1,1");
					sw.WriteLine("AD,22,140,1,1,0,0,Wear this label  " + s);
				}

				var vertpos = 338;
				foreach (var i in li.OrderByDescending(ll => ll.hour)) {
					var loc = i.location;
					if (!loc.HasValue())
						loc = "n/a";
					if (i.org.StartsWith(loc) && i.org.Length > loc.Length + 2)
						i.org = i.org.Substring(loc.Length + 2);
					if (loc.Length > 5)
						loc = loc.Substring(0, 5);
					sw.WriteLine("AC,18,{0},1,1,0,0,{1}".Fmt(vertpos, loc));
					sw.WriteLine("AC,120,{0},1,1,0,0,{1}".Fmt(vertpos,
						"{0:h:mm t}".Fmt(i.hour).PadLeft(7, '~').Replace("~", "  ")));
					sw.WriteLine("AB,36,{0},1,1,0,0,{1} ({2})".Fmt(vertpos + 31, i.org, i.mv.Replace("V", "G")));
					vertpos -= 68;
				}
				sw.WriteLine("E");
				sw.Flush();

				foreach (var i in li)
					AllergyLabel2(ms, i, code);
				foreach (var i in li.Where(ll => ll.n > 1)) {
					StartGodexLabel(sw, i.n - 1);
					sw.WriteLine("AH,18,0,1,1,0,0," + li.First().first);
					sw.WriteLine("AE,18,84,1,1,0,0," + li.First().last);
					sw.WriteLine("AD,286,161,1,1,0,0,{0:d}".Fmt(li.First().hour));
					sw.WriteLine("AH,422,129,1,1,0,0," + code);
					sw.WriteLine("AD,19,129,1,1,0,0,Extra | {0}{1}{2}".Fmt(
					li.First().allergies.HasValue() ? " A |" : "",
					li.First().transport ? " T |" : "",
					li.First().custody ? " C |" : ""));

					var loc = i.location;
					if (!loc.HasValue())
						loc = "n/a";
					if (i.org.StartsWith(loc))
						i.org = i.org.Substring(loc.Length + 2);
					if (loc.Length > 5)
						loc = loc.Substring(0, 5);
					sw.WriteLine("AC,17,202,1,1,0,0," + loc);
					sw.WriteLine("AC,120,202,1,1,0,0," +
						"{0:h:mm t}".Fmt(i.hour).PadLeft(7, '~').Replace("~", "  "));
					sw.WriteLine("AB,36,233,1,1,0,0,{0} ({1})".Fmt(i.org, i.mv.Replace("V", "G")));
					vertpos += 68;
					sw.WriteLine("E");
				}
			}
			sw.Flush();
			return nlabels;
		}

		public static int AllergyLabel(this MemoryStream ms, LabelInfo li)
		{
			//GuestLabel
			if (li.n == 0 || !Program.settings.printer.HasValue())
				return 0;
			if (li.mv != "G" && li.mv != "V")
				return 0;
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("ZDesigner")) {
				sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				sw.WriteLine("^XA");
				sw.WriteLine("^MMT");
				sw.WriteLine("^PW609");
				sw.WriteLine("^LL0406");
				sw.WriteLine("^LS0");
				sw.WriteLine(@"^FT29,83^A0N,62,62^FH\^FD{0}^FS".Fmt(li.first));
				sw.WriteLine(@"^^FT30,134^A0N,34,33^FH\^FD{0}^FS".Fmt(li.last));
				sw.WriteLine(@"^FT30,183^A0N,28,28^FH\^FDGuest{0}^FS".Fmt(li.allergies.HasValue() ? " (" + li.allergies + ")" : ""));
				sw.WriteLine("^PQ1,0,1,Y^XZ");
			} else if (Program.settings.printer.Contains("Datamax")) {
				sw.WriteLine("\x02n");
				sw.WriteLine("\x02M0500");
				sw.WriteLine("\x02O0220");
				sw.WriteLine("\x02V0");
				sw.WriteLine("\x02SG");
				sw.WriteLine("\x02d");
				sw.WriteLine("\x01D");
				sw.WriteLine("\x02L");
				sw.WriteLine("D11");
				sw.WriteLine("PG");
				sw.WriteLine("pC");
				sw.WriteLine("SG");
				sw.WriteLine("ySPM");
				sw.WriteLine("A2");
				sw.WriteLine("1911A1800500010" + li.first);
				sw.WriteLine("1911A1200320010" + li.last);
				sw.WriteLine("1911A1000060011Guest" + (li.allergies.HasValue() ? " (" + li.allergies + ")" : ""));
				sw.WriteLine("Q0001");
				sw.WriteLine("E");
			}
			sw.Flush();
			return 1;
		}

		public static int AllergyLabel2(this MemoryStream ms, LabelInfo li, string code)
		{
			if (li.n == 0 || !Program.settings.printer.HasValue())
				return 0;
			if (li.mv != "G" && li.mv != "V")
				return 0;
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("ZDesigner")) {
				StartZDesignerLabel(sw);
				sw.WriteLine(@"^FT27,87^A0N,79,79^FH\^FD{0}^FS".Fmt(li.first));
				sw.WriteLine(@"^FT26,138^A0N,39,38^FH\^FD{0}^FS".Fmt(li.last));
				sw.WriteLine(@"^FT26,173^A0N,28,28^FH\^FDGuest {0}^FS".Fmt(li.allergies));
				sw.WriteLine(@"^FT26,217^A0N,23,24^FH\^FD{0} ({1})^FS".Fmt(li.org, li.mv.Replace("V", "G")));
				sw.WriteLine(@"^FT27,263^A0N,28,28^FH\^FD{0}^FS".Fmt(li.location));
				sw.WriteLine(@"^FT143,263^A0N,28,28^FH\^FD{0:d}^FS".Fmt(li.hour));
				sw.WriteLine(@"^FT252,263^A0N,28,28^FH\^FD{0:t}^FS".Fmt(li.hour));
				sw.WriteLine(@"^FT434,265^A0N,79,79^FH\^FD{0}^FS".Fmt(code));
				sw.WriteLine(@"^BY3,2,87^FT55,386^B3N,N,,N,N");
				sw.WriteLine(@"^FD10210217^FS".Fmt(li.pid));
				sw.WriteLine("^PQ1,0,1,Y^XZ");
			} else if (Program.settings.printer.Contains("Godex")) {
				var loc = li.location;
				if (!loc.HasValue())
					loc = "n/a";
				var allergies = li.allergies;
				if (allergies.HasValue())
					allergies = "(" + allergies + ")";
				StartGodexLabel(sw, 1);
				sw.WriteLine("AG,16,0,1,1,0,0," + li.first);
				sw.WriteLine("AE,18,66,1,1,0,0," + li.last);
				sw.WriteLine("AD,18,108,1,1,0,0,Guest " + allergies);
				sw.WriteLine("AB,18,152,1,1,0,0,{0} ({1})".Fmt(li.org, li.mv.Replace("V", "G")));
				sw.WriteLine("AD,18,195,1,1,0,0," + loc);
				sw.WriteLine("AD,158,195,1,1,0,0,{0:d}".Fmt(li.hour));
				sw.WriteLine("AD,303,195,1,1,0,0,{0:t}".Fmt(li.hour));
				sw.WriteLine("AH,433,162,1,1,0,0," + code);
				sw.WriteLine("BA,57,241,3,7,100,0,2," + li.pid);
				sw.WriteLine("E");
			}
			sw.Flush();
			return 1;
		}

		public static int LocationLabel(this MemoryStream ms, LabelInfo li)
		{
			if (li.n == 0 || !Program.settings.printer.HasValue())
				return 0;
			if (!li.mv.Replace("V", "G").Contains("G") || !li.location.HasValue())
				return 0;
			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("ZDesigner")) {
				sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				sw.WriteLine("^XA");
				sw.WriteLine("^MMT");
				sw.WriteLine("^PW609");
				sw.WriteLine("^LL0406");
				sw.WriteLine("^LS0");
				sw.WriteLine(@"^FT29,83^A0N,62,62^FH\^FD{0}^FS".Fmt(li.first));
				sw.WriteLine(@"^FT30,155^A0N,45,45^FH\^FDLocation/time: {0}, {1:h:mm tt}^FS".Fmt(li.location, li.hour));
				sw.WriteLine("^PQ1,0,1,Y^XZ");
			} else if (Program.settings.printer.Contains("Datamax")) {
				sw.WriteLine("\x02n");
				sw.WriteLine("\x02M0500");
				sw.WriteLine("\x02O0220");
				sw.WriteLine("\x02V0");
				sw.WriteLine("\x02SG");
				sw.WriteLine("\x02d");
				sw.WriteLine("\x01D");
				sw.WriteLine("\x02L");
				sw.WriteLine("D11");
				sw.WriteLine("PG");
				sw.WriteLine("pC");
				sw.WriteLine("SG");
				sw.WriteLine("ySPM");
				sw.WriteLine("A2");
				sw.WriteLine("1911A1800450014" + li.first);
				sw.WriteLine("1911A1400120013Location: {0}, {1:h:mm tt}".Fmt(li.location, li.hour));
				sw.WriteLine("Q0001");
				sw.WriteLine("E");
			}
			sw.Flush();
			return 1;
		}

		public static int LocationLabel2(this MemoryStream ms, IEnumerable<IEnumerable<LabelInfo>> list)
		{
			if (!Program.settings.printer.HasValue())
				return 0;
			if (!list.Any(li => li.Any(i => i.mv.Replace("V", "G").Contains("G") && i.location.HasValue())))
				return 0;
			if (list.Sum(li => li.Sum(i => i.n)) == 0)
				return 0;

			var sw = new StreamWriter(ms);
			if (Program.settings.printer.Contains("ZDesigner")) {
				//sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
				//sw.WriteLine("^XA");
				//sw.WriteLine("^MMT");
				//sw.WriteLine("^PW609");
				//sw.WriteLine("^LL0406");
				//sw.WriteLine("^LS0");
				//sw.WriteLine(@"^FT29,83^A0N,62,62^FH\^FD{0}^FS".Fmt(li.first));
				//sw.WriteLine(@"^FT30,155^A0N,45,45^FH\^FDLocation/time: {0}, {1:h:mm tt}^FS"
				//	.Fmt(li.location, li.hour));
				//sw.WriteLine("^PQ1,0,1,Y^XZ");
			} else if (Program.settings.printer.Contains("Godex")) {
				StartGodexLabel(sw, 1);
				var vertpos = 6;
				foreach (var li in list) {
					foreach (var i in li) {
						if (vertpos > 340) {
							sw.WriteLine("E");
							StartGodexLabel(sw, 1);
							vertpos = 6;
						}
						var loc = i.location;
						if (!loc.HasValue())
							loc = "n/a";
						sw.WriteLine("AE,16,{0},1,1,0,0,{1}".Fmt(vertpos, i.first));
						sw.WriteLine("AC,394,{0},1,1,0,0,{1}".Fmt(vertpos + 5, loc));
						sw.WriteLine("AC,500,{0},1,1,0,0,{1}".Fmt(vertpos + 5, "{0:h:mm t}".Fmt(i.hour).PadLeft(7, '~').Replace("~", "  ")));
						vertpos += 42;
						sw.WriteLine("AB,16,{0},1,1,0,0,{1} ({2})".Fmt(vertpos + 4, i.org, i.mv.Replace("V", "G")));
						vertpos += 42;
					}
					sw.WriteLine("Lo,13,{0},584,{1}".Fmt(vertpos + 3, vertpos + 4));
				}
				sw.WriteLine("E");
			}
			sw.Flush();
			return 1;
		}

		private static void StartGodexLabel(StreamWriter sw, int nlabels)
		{
			sw.WriteLine("^Q51,3");
			sw.WriteLine("^W76");
			sw.WriteLine("^H10");
			sw.WriteLine("^P" + nlabels);
			sw.WriteLine("^S4");
			sw.WriteLine("^AD");
			sw.WriteLine("^C1");
			sw.WriteLine("^R0");
			sw.WriteLine("~Q+0");
			sw.WriteLine("^O0");
			sw.WriteLine("^D0");
			sw.WriteLine("^E12");
			sw.WriteLine("~R200");
			sw.WriteLine("^L");
			sw.WriteLine("Dy2-me-dd");
			sw.WriteLine("Th:m:s");
		}

		private static void StartZDesignerLabel(StreamWriter sw)
		{
			sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
			sw.WriteLine("^XA");
			sw.WriteLine("^MMT");
			sw.WriteLine("^PW609");
			sw.WriteLine("^LL0406");
			sw.WriteLine("^LS0");
		}
	}
}
