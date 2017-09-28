using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CmsCheckin.Classes;

namespace CmsCheckin
{
	class DoPrinting
	{
		public DateTime time { get; set; }
		public int LabelsPrinted { get; set; }
		public int RequiresSecurityLabels { get; set; }

		public DoPrinting()
		{
			time = DateTime.Now;
		}

		public void PrintLabels(MemoryStream ms, IEnumerable<LabelInfo> q)
		{
			foreach (var li in q) {
				LabelsPrinted += ms.Label(li, li.n, Program.SecurityCode);
				LabelsPrinted += ms.AllergyLabel(li);
			}
			foreach (var li in q)
				LabelsPrinted += ms.LocationLabel(li);
			RequiresSecurityLabels = q.Count(li => li.requiressecuritylabel == true && li.n > 0);
		}

		public void PrintLabels2(MemoryStream ms, IEnumerable<LabelInfo> q)
		{
			var q2 = from c in q
						group c by c.pid into g
						select from c in g
								 select c;

			foreach (var li in q2)
				LabelsPrinted += ms.Label2(li, li.Max(ll => ll.n), Program.SecurityCode);
			LabelsPrinted += ms.LocationLabel2(q2);
			RequiresSecurityLabels = q.Count(li => li.requiressecuritylabel == true && li.n > 0);
		}

		public void FinishUp(MemoryStream ms)
		{
			if (LabelsPrinted > 0) {
				if (RequiresSecurityLabels > 0) {
					int iLabelSize = PrinterHelper.getPageHeight(Program.settings.printer);

					var n = RequiresSecurityLabels;
					if (iLabelSize >= 170 && iLabelSize <= 230)
						LabelsPrinted += ms.SecurityLabel2(time, Program.SecurityCode, n);
					else
						LabelsPrinted += ms.SecurityLabel(time, Program.SecurityCode, n);
				}
				ms.BlankLabel(LabelsPrinted == 1); // force blank if only 1
			}
			PrintRawHelper.SendDocToPrinter(Program.settings.printer, ms);
		}
	}
}
