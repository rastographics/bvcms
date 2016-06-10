using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class OrgMemberInfo
    {
        public int OrgId { get; set; }
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string LeaderName { get; set; }
        public DateTime? MeetingTime { get; set; }
        public string MemberType { get; set; }
        public int? LeaderId { get; set; }
        public DateTime? EnrollDate { get; set; }
        public DateTime? DropDate { get; set; }
        public decimal? AttendPct { get; set; }
        public string DivisionName { get; set; }
        public string ProgramName { get; set; }
        public string OrgType { get; set; }
        public bool HasDirectory { get; set; }
        public bool IsLeaderAttendanceType { get; set; }

        public List<OrgMemberInfo> ChildOrgs { get; set; } 

        public string Schedule => $"{MeetingTime:ddd h:mm tt}";
        public string SchComma => MeetingTime.HasValue ? ", " : "";
        public string LocComma => Location.HasValue() ? ", " : "";

        private IEnumerable<OrganizationExtra> _extraFields; 
        public string GetColumnValue(InvolvementTableColumn column, bool inAccessRole, bool inOrgLeadersOnlyRole)
        {
            var field = column.Field.ToLower();

            switch (field)
            {
                case "orgid":
                    return OrgId.ToString();
                case "name":
                case "organization":
                    if (inAccessRole && 
                        (IsLeaderAttendanceType || !inOrgLeadersOnlyRole || !DbUtil.Db.Setting("UX-OrgLeadersOtherGroupsContentOnly", false)))
                    {
                        return $"<a href=\"{Util2.Org}/{OrgId}\">{Name}</a>";
                    }
                    else if (column.Page != "Current")
                    {
                        return $"<span title=\"{DivisionName}\">{Name}</span>";
                    }
                    else if (HasDirectory)
                    {
                        return $"<a title=\"{DivisionName}\" href=\"/MemberDirectory/{OrgId}\">{Name}</a>";
                    }
                    else
                    {
                        return $"<a title=\"{DivisionName}\" href=\"/OrgContent/{OrgId}\">{Name}</a>";
                    }
                case "enroll date":
                case "enrolldate":
                    return EnrollDate.FormatDate();
                case "drop date":
                case "dropdate":
                    return DropDate.FormatDate();
                case "location":
                    return Location;
                case "schedule":
                    return Schedule;
                case "leader":
                    if (inAccessRole)
                    {
                        return $"<a href=\" /Person2/{LeaderId}\">{LeaderName}</a>";
                    }
                    else
                    {
                        return LeaderName;
                    }
                case "attendpct":
                    return AttendPct > 0 ? AttendPct.Value.ToString("N1") : "";
                case "division":
                    return DivisionName;
                case "program":
                    return ProgramName;
                case "orgtype":
                    return OrgType;
                case "membertype":
                    if(column.Page == "Previous" && inAccessRole)
                        return $"<a target=\"_blank\" href=\"/TransactionHistory/{PeopleId}/{OrgId}\">{MemberType}</a>";
                    return
                        $"<a class=\"membertype\" href=\"/OrgMemberDialog/Member/{OrgId}/{PeopleId}\">{MemberType}</a>";
                case "leave":
                    if(column.Page == "Current")
                        return $"<button class=\"leave-org\" data-personid=\"{PeopleId}\" data-orgid=\"{OrgId}\">{column.Label}</button>";
                    else
                        return "";
                default:
                    if(_extraFields == null)
                        _extraFields = DbUtil.Db.LoadOrganizationById(OrgId)?.GetOrganizationExtras();
                    return _extraFields?.SingleOrDefault(x => x.Field.ToLower() == field)?.Data;
            }
        }
    }
}
