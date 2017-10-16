using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class PromoteInfo
    {
        public bool IsSelected { get; set; }
        public string Checked => IsSelected ? "checked='checked'" : "";
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public int CurrClassId { get; set; }
        public string CurrClassName => Organization.FormatOrgName(CurrOrgName, CurrLeader, CurrLoc) + ", " + CurrSchedule;
        public string CurrOrgName { get; set; }
        public string CurrLeader { get; set; }
        public string CurrLoc { get; set; }
        public string CurrSchedule { get; set; }
        public int? PendingClassId { get; set; }
        public string PendingClassName => Organization.FormatOrgName(PendingOrgName, PendingLeader, PendingLoc) + ", " + PendingSchedule;
        public string PendingOrgName { get; set; }
        public string PendingLeader { get; set; }
        public string PendingLoc { get; set; }
        public string PendingSchedule { get; set; }
        public decimal? AttendPct { get; set; }
        public string AttendIndicator => AttendPct > 80 ? "Hi" : AttendPct > 40 ? "Med" : "Lo";
        public string Gender { get; set; }
        public int? BDay { get; set; }
        public int? BMon { get; set; }
        public int? BYear { get; set; }
        public string Birthday => Person.FormatBirthday(BYear, BMon, BDay, PeopleId);
        public int Hash { get; set; }
    }
}
