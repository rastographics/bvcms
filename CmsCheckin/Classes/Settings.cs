using System;
using Newtonsoft.Json;
using System.Drawing;

namespace CmsCheckin
{
    class Settings
    {
        [JsonIgnore]
        public string user = "";
        [JsonIgnore]
        public string pass = "";
        [JsonIgnore]
        public string subdomain = "";
        [JsonIgnore]
        public bool ssl = true;
        public bool useTPSDB = true;

        public string printer = "";

        public bool advancedPageSize = false;
        public string printerWidth = "";
        public string printerHeight = "";

        public string printMode = "";
        public string printForKiosks = "";

        public string adminPIN = "";
        public string adminPINTimeout = "0";

        [JsonIgnore]
        public int adminPINTimeoutNumber = 0;

        public string campus = "";

        [JsonIgnore]
        public string campusID = "";

        [JsonIgnore]
        public int dayOfWeek = 0;

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
        public bool useOldDatamaxFormat = false;

        public bool buildingMode = false;
        public string building = "";

        public int popupForVersion = 0;

        public Uri createURI(string path)
        {
            return new Uri(new Uri(createURL()), path);
        }

        public string createURL()
        {
            string url = "";

            if (ssl)
            {
                if (useTPSDB)
                {
                    url = String.Format("https://{0}.tpsdb.com", subdomain);
                }
                else
                {
                    url = String.Format("https://{0}", subdomain);
                }
            }
            else
            {
                url = String.Format("http://{0}", subdomain);
            }

            return url;
        }

        public string createURL(string path)
        {
            string url = "";

            if (ssl)
            {
                if (useTPSDB)
                {
                    url = String.Format("https://{0}.tpsdb.com/{1}", subdomain, path);
                }
                else
                {
                    url = String.Format("https://{0}/{1}", subdomain, path);
                }
            }
            else
            {
                url = String.Format("http://{0}/{1}", subdomain, path);
            }

            return url;
        }

        public void setPopupForVersion(int version)
        {
            Settings1.Default.PopupForVersion = version;
            Settings1.Default.Save();
        }

        public void attendantPositionChanged(Point point)
        {
            Settings1.Default.AttendantLocX = point.X;
            Settings1.Default.AttendantLocY = point.Y;
            Settings1.Default.Save();
        }

        public Point attendantLastPosition()
        {
            return new Point(Settings1.Default.AttendantLocX, Settings1.Default.AttendantLocY);
        }

        public void basePositionChanged(Point point)
        {
            Settings1.Default.BaseFormLocX = point.X;
            Settings1.Default.BaseFormLocY = point.Y;
            Settings1.Default.Save();
        }

        public Point baseLastPosition()
        {
            return new Point(Settings1.Default.BaseFormLocX, Settings1.Default.BaseFormLocY);
        }

        public void load()
        {
            //Login
            ssl = Settings1.Default.UseSSL;
            subdomain = Settings1.Default.URL;
            user = Settings1.Default.username;

            // First Column
            campus = Settings1.Default.Campus;
            dayOfWeek = (int)DateTime.Now.DayOfWeek;
            earlyHours = Settings1.Default.LeadHours;
            lateMinutes = Settings1.Default.LateMinutes;

            buildingMode = Settings1.Default.BuildingMode;
            building = Settings1.Default.Building;

            // Second Column
            printMode = Settings1.Default.PrintMode;
            printForKiosks = Settings1.Default.Kiosks;
            printer = Settings1.Default.Printer;
            kioskName = Settings1.Default.KioskName;

            advancedPageSize = Settings1.Default.AdvancedPageSize;
            printerWidth = Settings1.Default.PrinterWidth;
            printerHeight = Settings1.Default.PrinterHeight;

            // Third Column
            disableLocationLabels = Settings1.Default.DisableLocationLabels;
            extraLabel = Settings1.Default.ExtraBlankLabel;
            securityLabelPerChild = Settings1.Default.SecurityLabelPerChild;
            useOldDatamaxFormat = Settings1.Default.OldLabels;

            askFriend = Settings1.Default.AskEmFriend;
            askChurch = Settings1.Default.AskChurch;
            askChurchName = Settings1.Default.AskChurchName;
            askGrade = Settings1.Default.AskGrade;

            //fullScreen = Settings1.Default.?;
            //hideCursor = Settings1.Default.?;
            //enableTimer = Settings1.Default.?;
            disableJoin = Settings1.Default.DisableJoin;

            // Fourth Column
            adminPIN = Settings1.Default.AdminPIN;
            adminPINTimeout = Settings1.Default.AdminPINTimeout;
            int.TryParse(adminPINTimeout, out adminPINTimeoutNumber);
        }

        public void save()
        {
            // Login
            Settings1.Default.UseSSL = ssl;
            Settings1.Default.URL = subdomain;
            Settings1.Default.username = user;

            // First Column
            Settings1.Default.Campus = campus;
            //Settings1.Default.? = dayOfWeek;
            Settings1.Default.LeadHours = earlyHours;
            Settings1.Default.LateMinutes = lateMinutes;

            Settings1.Default.BuildingMode = buildingMode;
            Settings1.Default.Building = building;

            // Second Column
            Settings1.Default.PrintMode = printMode;
            Settings1.Default.Kiosks = printForKiosks;
            Settings1.Default.Printer = printer;
            Settings1.Default.KioskName = kioskName;

            Settings1.Default.AdvancedPageSize = advancedPageSize;
            Settings1.Default.PrinterWidth = printerWidth;
            Settings1.Default.PrinterHeight = printerHeight;

            // Third Column
            Settings1.Default.DisableLocationLabels = disableLocationLabels;
            Settings1.Default.ExtraBlankLabel = extraLabel;
            Settings1.Default.SecurityLabelPerChild = securityLabelPerChild;
            Settings1.Default.OldLabels = useOldDatamaxFormat;

            Settings1.Default.AskEmFriend = askFriend;
            Settings1.Default.AskChurch = askChurch;
            Settings1.Default.AskChurchName = askChurchName;
            Settings1.Default.AskGrade = askGrade;

            //Settings1.Default.? = fullScreen;
            //Settings1.Default.? = hideCursor;
            //Settings1.Default.? = enableTimer;
            Settings1.Default.DisableJoin = disableJoin;

            // Fourth Column
            Settings1.Default.AdminPIN = adminPIN;
            Settings1.Default.AdminPINTimeout = adminPINTimeout;
            int.TryParse(adminPINTimeout, out adminPINTimeoutNumber);

            Settings1.Default.Save();
        }

        public void copy(Settings settings)
        {
            // Login
            //user = settings.user;
            //pass = settings.pass;
            //subdomain = settings.subdomain;
            //ssl = settings.ssl;

            // First Column
            //campus = settings.campus;
            //campusID = settings.campusID;
            //dayOfWeek = settings.dayOfWeek;
            earlyHours = settings.earlyHours;
            lateMinutes = settings.lateMinutes;

            buildingMode = settings.buildingMode;
            building = settings.building;

            // Second Column
            printMode = settings.printMode;
            printForKiosks = settings.printForKiosks;
            printer = settings.printer;
            kioskName = settings.kioskName;

            advancedPageSize = settings.advancedPageSize;
            printerWidth = settings.printerWidth;
            printerHeight = settings.printerHeight;

            // Third Column
            disableLocationLabels = settings.disableLocationLabels;
            extraLabel = settings.extraLabel;
            securityLabelPerChild = settings.securityLabelPerChild;
            useOldDatamaxFormat = settings.useOldDatamaxFormat;

            askFriend = settings.askFriend;
            askChurch = settings.askChurch;
            askChurchName = settings.askChurchName;
            askGrade = settings.askGrade;

            fullScreen = settings.fullScreen;
            hideCursor = settings.hideCursor;
            enableTimer = settings.enableTimer;
            disableJoin = settings.disableJoin;

            // Fourth Column
            adminPIN = settings.adminPIN;
            adminPINTimeout = settings.adminPINTimeout;
            int.TryParse(settings.adminPINTimeout, out adminPINTimeoutNumber);
        }
    }
}
