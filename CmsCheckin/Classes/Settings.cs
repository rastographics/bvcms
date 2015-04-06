using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace CmsCheckin
{
	class Settings
	{
		public string user = "";
		public string subdomain = "";
		public bool ssl = true;

		[JsonIgnore]
		public string pass = "";

		public string printer = "";

		public bool advancedPageSize = false;
		public string printerWidth = "";
		public string printerHeight = "";

		public string printMode = "";
		public string printForKiosks = "";

		public string adminPIN = "";
		public int adminPINTimeout = 0;

		public string campus = "";

		public int earlyHours = 0;
		public int lateMinutes = 0;

		public bool hideCursor = true;
		public bool enableTimer = true;
		public bool disableJoin = false;
		public bool fullScreen = true;
		public string kioskName = "";

		public bool askFriend = false;
		public bool askGrade = false;
		public bool askChurch = false;
		public bool askChurchName = false;

		public bool disableLocationLabels = false;
		public bool securityLabelPerChild = false;
		public bool extraLabel = false;

		public bool buildingMode = false;
		public string building = "";

		public Uri createURI(string path)
		{
			return new Uri(new Uri(createURL()), path);
		}

		public string createURL()
		{
			string url = "";

			if (ssl) {
				url = String.Format("https://{0}.tpsdb.com", subdomain);
			} else {
				url = String.Format("http://{0}", subdomain);
			}

			return url;
		}

		public string createURL(string path)
		{
			string url = "";

			if (ssl) {
				url = String.Format("https://{0}.tpsdb.com/{1}", subdomain, path);
			} else {
				url = String.Format("http://{0}/{1}", subdomain, path);
			}

			return url;
		}

		public void load()
		{

		}

		public void save()
		{
			Settings1.Default.UseSSL = ssl;
			Settings1.Default.URL = subdomain;
			Settings1.Default.username = user;

			Settings1.Default.Printer = printer;

			Settings1.Default.AdvancedPageSize = advancedPageSize;
			Settings1.Default.PrinterWidth = printerWidth;
			Settings1.Default.PrinterHeight = printerHeight;

			Settings1.Default.PrintMode = printMode;
			Settings1.Default.Kiosks = printForKiosks;

			Settings1.Default.KioskName = kioskName;

			Settings1.Default.BuildingMode = buildingMode;
			Settings1.Default.Building = building;

			Settings1.Default.AdminPIN = adminPIN;
			Settings1.Default.AdminPINTimeout = adminPINTimeout.ToString();

			Settings1.Default.Campus = campus;

			Settings1.Default.LeadHours = earlyHours;
			Settings1.Default.LateMinutes = lateMinutes;

			Settings1.Default.AskChurch = askChurch;
			Settings1.Default.AskChurchName = askChurchName;
			Settings1.Default.AskEmFriend = askFriend;
			Settings1.Default.AskGrade = askGrade;

			Settings1.Default.DisableLocationLabels = disableLocationLabels;			
			Settings1.Default.DisableJoin = disableJoin;
			Settings1.Default.ExtraBlankLabel = extraLabel;

			Settings1.Default.Save();
		}
	}
}
