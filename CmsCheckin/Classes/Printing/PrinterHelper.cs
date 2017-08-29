using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;
using ZXing;
using ZXing.Common;
using System.Windows.Forms;

namespace CmsCheckin.Classes
{
	class PrinterHelper
	{
		public const int TEST_HEIGHT = 200;

		public static string[] MAIN = { "Main" };
		public static string[] GUEST = { "Guest" };
		public static string[] NAMETAG = { "NameTag" };
		public static string[] SINGLE = { "Main" };

		private static int iPageHeight, iPageWidth, iPagePixelsX, iPagePixelsY;

		private static List<LabelPage> lPageList;

		private static Dictionary<string, string> dLabelFormats = new Dictionary<string, string>();

		/*
		 * When To Print Labels:
		 * 
		 * Main Label: Always printed for members and guests regardless of size
		 * 
		 * Security Label: Same for all label sizes only when needed, can have more than 1 printed via securitylabelsperchild
		 * 
		 * Guest Label: Gets put on the roles and has allergies (greater than or equal to 2 inch)
		 * Allergy Label (less than 2 inch): Try to not use this one
		 * 
		 * Location Label: For guest only
		 * 
		 * NameTag label: Prints if not security label is requested
		 * 
		 * If Member (2 inch): Only 1 Label if label
		 * If Guest (2 inch): Main, Guest, Security, Location (multiples of each can happen on request)
		 * 
		 * If Member (1 inch): Only 1 label unless more than 1 class
		 * If Guess (1 inch): Main, Guest, Security, Location (multiples
 		*/

		public static bool doPrinting(IEnumerable<LabelInfo> q, bool bPrintSingle = false)
		{
			LabelSet lsLabels = new LabelSet();
			int iLabelSize = PrinterHelper.getPageHeight(Program.settings.printer);

			// Adjust all records that are not members to have a "G" for Guest. Fix for iPad sending a "V" instead of a "G"
			foreach (var u in (from c in q where c.mv != "M" select c)) { u.mv = "G"; }

			var q2 = from c in q
						where c.n > 0
						orderby c.first ascending, c.hour descending
						group c by new { c.pid } into g
						select from c in g
								 select c;

			if (bPrintSingle) {
				string[] sFormats = PrinterHelper.SINGLE;

				foreach (var li in q2) {
					foreach (string sItem in sFormats) {
						lsLabels.addPages(PrinterHelper.fetchLabelFormat(sItem, iLabelSize), li.ToList<LabelInfo>());
					}
				}
			} else {
				var extras = from e in q
								 where e.n > 1
//								 where e.requiressecuritylabel == true
								 orderby e.first ascending, e.hour ascending
								 group e by new { e.pid, e.org } into eg
								 select from e in eg
										  select e;

				var locs = from c in q
							  where c.mv != "M"
							  where c.age < 13
							  where c.n > 0
							  orderby c.first ascending, c.hour ascending
							  group c by c.securitycode into g
							  select from c in g
										select c;

				foreach (var li in q2) {
					int iPersonSecurityCount = PrinterHelper.getSecurityCount(li);

					if (iPersonSecurityCount > 0 || li.First().age < 13) {
						string[] sFormats = PrinterHelper.MAIN;
						foreach (string sItem in sFormats) {
							lsLabels.addPages(PrinterHelper.fetchLabelFormat(sItem, iLabelSize), li.ToList<LabelInfo>());
						}

						var guestIn = from gi in li
										  where gi.mv != "M"
										  select gi;

						foreach (var guestLabel in guestIn) {
							string[] sGuestFormats = PrinterHelper.GUEST;
							IEnumerable<LabelInfo> iGuestLabels = new[] { guestLabel };

							foreach (string sItem in sGuestFormats) {
								lsLabels.addPages(PrinterHelper.fetchLabelFormat(sItem, iLabelSize), iGuestLabels.ToList<LabelInfo>());
							}
						}
					} else {

						string[] sFormats = PrinterHelper.NAMETAG;
						foreach (string sItem in sFormats) {
							lsLabels.addPages(PrinterHelper.fetchLabelFormat(sItem, iLabelSize), li.ToList<LabelInfo>());
						}
					}
				}

				foreach (var le in extras) {
					int iLabels = le.FirstOrDefault().n - 1;

					for (int iX = 0; iX < iLabels; iX++) {
						lsLabels.addPages(PrinterHelper.fetchLabelFormat("Extra", iLabelSize), le.ToList<LabelInfo>());
					}
				}

				if (lsLabels.getCount() > 0) {
					int iSecurityCount = PrinterHelper.getSecurityCount(q);

					if (iSecurityCount > 0) {
						var s = q2.First().Take(1).ToList<LabelInfo>();

						for (int iX = 0; iX < iSecurityCount; iX++) {
							lsLabels.addPages(PrinterHelper.fetchLabelFormat("Security", iLabelSize), s);
						}
					}

					if (Program.settings.disableLocationLabels == false) {
						foreach (var lc in locs) {
							lsLabels.addPages(PrinterHelper.fetchLabelFormat("Location", iLabelSize), lc.ToList<LabelInfo>());
						}
					}
				}
			}

            if (lsLabels.getCount() > 0)
            {
                if (Program.settings.extraLabel == true && q.Count() > 0) lsLabels.addBlank();

                PrinterHelper.printAllLabels(Program.settings.printer, lsLabels);
                return true;
            }
            else
            {
                return false;
            }
		}

		public static void setPaperSize(PrintDocument pd)
		{
			string sPrinter = pd.PrinterSettings.PrinterName;

			if (sPrinter == "PDFCreator") {
				PaperSize paperSize = new PaperSize("CustomLabelSize", getPageHeight(sPrinter), getPageWidth(sPrinter));
				paperSize.RawKind = (int)PaperKind.Custom;
				pd.DefaultPageSettings.PaperSize = paperSize;
				pd.DefaultPageSettings.Landscape = true;
			} else {
				if (pd.DefaultPageSettings.Landscape) {
					PaperSize paperSize = new PaperSize("CustomLabelSize", getPageHeight(sPrinter), getPageWidth(sPrinter));
					paperSize.RawKind = (int)PaperKind.Custom;
					pd.DefaultPageSettings.PaperSize = paperSize;
				} else {
					PaperSize paperSize = new PaperSize("CustomLabelSize", getPageWidth(sPrinter), getPageHeight(sPrinter));
					paperSize.RawKind = (int)PaperKind.Custom;
					pd.DefaultPageSettings.PaperSize = paperSize;
				}
			}
		}

		public static void printAllLabels(string sPrinter, LabelSet lsLabels)
		{
			if (sPrinter == null) return;
			if (sPrinter.Length == 0 || lsLabels.getCount() == 0) return;

			lPageList = lsLabels.lPages;

			PrintDocument pd = new PrintDocument();
			pd.PrintController = new StandardPrintController();
			pd.PrinterSettings.PrinterName = sPrinter;

			setPaperSize(pd);
			setPageInfo(pd);

			pd.PrintPage += new PrintPageEventHandler(printLabelEvent);
			pd.Print();
		}

		public static void printTestLabel(string sPrinter, string sLabelFormat)
		{
			LabelSet lsLabels = new LabelSet();
			lsLabels.addPages(sLabelFormat, null);

			lPageList = lsLabels.lPages;

			PrintDocument pd = new PrintDocument();
			pd.PrinterSettings.PrinterName = sPrinter;

			setPaperSize(pd);
			setPageInfo(pd);

			pd.PrintPage += new PrintPageEventHandler(printLabelEvent);
			pd.Print();
		}

		public static void printLabelEvent(object sender, PrintPageEventArgs e)
		{
			printLabel(e.Graphics, lPageList.ElementAt(0));
			lPageList.RemoveAt(0);

			e.HasMorePages = (lPageList.Count() > 0);
		}

		public static void printLabel(Graphics gPage, LabelPage lpPage)
		{
			foreach (LabelEntry leItem in lpPage.leEntries) {
				switch (leItem.iType) {
					case LabelEntry.TYPE_STRING:
					case LabelEntry.TYPE_LABEL:
						drawString(gPage, leItem.sFontName, leItem.fSize, leItem.sText, iPageWidth * leItem.fStartX, iPageHeight * leItem.fStartY, leItem.iAlignX, leItem.iAlignY);
						break;

					case LabelEntry.TYPE_LINE:
						Pen pLine = new Pen(Brushes.Black, leItem.fSize);
						gPage.DrawLine(pLine, iPageWidth * leItem.fStartX, iPageHeight * leItem.fStartY, iPageWidth * leItem.fEndX, iPageHeight * leItem.fEndY);
						break;

					case LabelEntry.TYPE_BARCODE:
						float offsetX = 0, offsetY = 0;

						BarcodeWriter bw = new BarcodeWriter();
						bw.Format = BarcodeFormat.CODE_128;
						bw.Options = new EncodingOptions { Width = leItem.iWidth, Height = leItem.iHeight, PureBarcode = true };

						Image ibc = (Image)bw.Write(leItem.sText);

						switch (leItem.iAlignX) {
							case 1: break;
							case 2: offsetX = leItem.iWidth * 0.5f; break;
							case 3: offsetX = leItem.iWidth; break;
						}

						switch (leItem.iAlignY) {
							case 1: break;
							case 2: offsetY = leItem.iHeight * 0.5f; break;
							case 3: offsetY = leItem.iHeight; break;
						}

						gPage.DrawImage(ibc, (iPageWidth * leItem.fStartX) - offsetX, (iPageHeight * leItem.fStartY) - offsetY);
						break;
				}
			}
		}

		public static void drawString(Graphics g, string sFont, float fSize, string sText, float fX, float fY, int iAlignX, int iAlignY)
		{
			Font drawFont = new Font(sFont, fSize);
			SolidBrush drawBrush = new SolidBrush(Color.Black);

			SizeF sfSize = g.MeasureString(sText, drawFont);

			switch (iAlignX) {
				case 1: break;
				case 2: fX = fX - (sfSize.Width * 0.5f); break;
				case 3: fX = fX - sfSize.Width; break;
			}

			switch (iAlignY) {
				case 1: break;
				case 2: fY = fY - (sfSize.Height * 0.5f); break;
				case 3: fY = fY - sfSize.Height; break;
			}

			g.DrawString(sText, drawFont, drawBrush, fX, fY);
		}

		public static void setPageInfo(PrintDocument pd)
		{
			iPageHeight = pd.DefaultPageSettings.Landscape ? pd.DefaultPageSettings.PaperSize.Width : pd.DefaultPageSettings.PaperSize.Height;
			iPageWidth = pd.DefaultPageSettings.Landscape ? pd.DefaultPageSettings.PaperSize.Height : pd.DefaultPageSettings.PaperSize.Width;

			iPagePixelsX = (int)((iPageWidth / 100f) * (pd.DefaultPageSettings.PrinterResolution.X));
			iPagePixelsY = (int)((iPageHeight / 100f) * (pd.DefaultPageSettings.PrinterResolution.Y));
		}

		public static int getSecurityCount(IEnumerable<LabelInfo> liList)
		{
			if (Program.settings.securityLabelPerChild) {
				return liList.Count(li => li.requiressecuritylabel == true && li.n > 0);
			} else {
				if (liList.Count(li => li.requiressecuritylabel == true && li.n > 0) > 0) return 1;
				else return 0;
			}
		}

		public static int getPageWidth(string sPrinter)
		{
			int pageWidth = 0;

			if (Program.settings.printerWidth.HasValue() && int.TryParse(Program.settings.printerWidth, out pageWidth)) {
				return pageWidth;
			} else {
				return getPrinterWidth(sPrinter);
			}
		}

		public static int getPageHeight(string sPrinter)
		{
			int pageHeight = 0;

			if (Program.settings.printerHeight.HasValue() && int.TryParse(Program.settings.printerHeight, out pageHeight)) {
				return pageHeight;
			} else {
				return getPrinterHeight(sPrinter);
			}
		}

		public static int getPrinterWidth(string sPrinter)
		{
			int iSize = 0;

			PrintDocument pd = new PrintDocument();
			pd.PrinterSettings.PrinterName = sPrinter;

			if (pd.DefaultPageSettings.Landscape) iSize = pd.DefaultPageSettings.PaperSize.Height;
			else iSize = pd.DefaultPageSettings.PaperSize.Width;

			pd.Dispose();
			return iSize;
		}

		public static int getPrinterHeight(string sPrinter)
		{
			int iSize = 0;

			PrintDocument pd = new PrintDocument();
			pd.PrinterSettings.PrinterName = sPrinter;

			if (pd.DefaultPageSettings.Landscape) iSize = pd.DefaultPageSettings.PaperSize.Width;
			else iSize = pd.DefaultPageSettings.PaperSize.Height;

			pd.Dispose();
			return iSize;
		}

		public static string fetchLabelFormat(string sName, int iSize)
		{
			string sKey = createLabelName(sName, iSize);

			if (dLabelFormats.ContainsKey(sKey)) {
				Debug.Print("Using cached label format for: " + sKey);
				return dLabelFormats[sKey];
			} else {
				try {
					Debug.Print("Fetching label format for: " + sKey);

					var c = new NameValueCollection();
					c.Add("sName", sName);
					c.Add("iSize", iSize.ToString());

					var path = "Checkin2/FetchLabelFormat/";
					var url = Program.settings.createURI(path);

					var wc = Util.CreateWebClient();
					var resp = wc.UploadValues(url, "POST", c);

					string ret = Encoding.ASCII.GetString(resp);

					dLabelFormats.Add(sKey, ret);

					return ret;
				} catch (Exception) { }
			}

			return "";
		}

		public static int saveLabelFormat(string sName, string sSize, string sFormat)
		{
			string sKey = createLabelName(sName, sSize);

			try {
				var c = new NameValueCollection();
				c.Add("sName", sName);
				c.Add("iSize", sSize);
				c.Add("sFormat", sFormat);

				var path = "Checkin2/SaveLabelFormat/";
				var url = Program.settings.createURI(path);

				var wc = Util.CreateWebClient();
				var resp = wc.UploadValues(url, "POST", c);

				int ret = int.Parse(Encoding.ASCII.GetString(resp));

				if (dLabelFormats.ContainsKey(sKey)) dLabelFormats[sKey] = sFormat;
				else dLabelFormats.Add(sKey, sFormat);

				return ret;
			} catch (Exception) { }

			return 1;
		}

		public static string[] fetchLabelList()
		{
			try {
				var path = "Checkin2/FetchLabelList/";
				var url = Program.settings.createURI(path);

				var wc = Util.CreateWebClient();
				var resp = wc.DownloadData(url);

				string ret = Encoding.ASCII.GetString(resp);

				return ret.Split(new char[] { ',' });
			} catch (Exception) { }

			return null;
		}

		public static string createLabelName(string sName, int iSize)
		{
			return sName + "~" + iSize;
		}

		public static string createLabelName(string sName, string sSize)
		{
			return sName + "~" + sSize;
		}

		/*
		public static PrinterSettings.StringCollection fetchPrinterList()
		{
			PrintDocument pd = new PrintDocument();
			return PrinterSettings.InstalledPrinters;
		}

		public static PageSettings fetchPrinterDefaults( string printer )
		{
			PrintDocument pd = new PrintDocument();
			pd.PrinterSettings.PrinterName = printer;
			return pd.DefaultPageSettings;
		}
		*/
	}
}
