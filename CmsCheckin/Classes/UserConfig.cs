using Newtonsoft.Json;
using System;
using System.IO;

namespace CmsCheckin
{
    public class UserConfig
    {
        public static UserConfig Load()
        {
            UserConfig config = null;
            try
            {
                var configFileName = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\CMSCheckin.config");
                if (File.Exists(configFileName))
                {
                    config = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(configFileName));
                }
            }
            catch { }
            return config ?? new UserConfig();
        }

        public string Printer { get; set; }
        public string Campus { get; set; }
        public int? LeadHours { get; set; }
        public int? LateMinutes { get; set; }
        public bool? AskChurch { get; set; }
        public bool? AskGrade { get; set; }
        public string KioskName { get; set; }
        public bool? AskEmFriend { get; set; }
        public string URL { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool? AskChurchName { get; set; }
        public bool? UseSSL { get; set; }
        public bool? TwoInchLabel { get; set; }
        public bool? DisableJoin { get; set; }
        public string Kiosks { get; set; }
        public string PrintMode { get; set; }
        public bool? SecurityLabelPerChild { get; set; }
        public bool? BuildingMode { get; set; }
        public string Building { get; set; }
        public int? BaseFormLocX { get; set; }
        public int? BaseFormLocY { get; set; }
        public int? AttendantLocX { get; set; }
        public int? AttendantLocY { get; set; }
        public string PrinterWidth { get; set; }
        public string PrinterHeight { get; set; }
        public bool? AdvancedPageSize { get; set; }
        public bool? DisableLocationLabels { get; set; }
        public bool? ExtraBlankLabel { get; set; }
        public bool? OldLabels { get; set; }
        public string AdminPIN { get; set; }
        public string AdminPINTimeout { get; set; }
        public int? PopupForVersion { get; set; }

    }
}
