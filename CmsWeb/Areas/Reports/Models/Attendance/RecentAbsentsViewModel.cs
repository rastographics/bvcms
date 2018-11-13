using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CmsData;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class RecentAbsentsViewModel
    {
        public RecentAbsentsViewModel(int id, Guid queryid, int? otherorgfilterid)
        {
            QueryId = queryid;
            OrgId = id;
            OtherOrgFilterId = otherorgfilterid;
            DefaultWorshipId = CurrentDatabase.Setting("WorshipId", "0").ToInt();
            var q = from o in CurrentDatabase.Organizations
                where o.OrganizationId == OrgId
                let fo = CurrentDatabase.Organizations.Single(oo => oo.OrganizationId == otherorgfilterid)
                let wo = CurrentDatabase.Organizations.Single(oo => oo.OrganizationId == DefaultWorshipId)
                select new
                {
                    o.OrganizationName,
                    o.LeaderName,
                    LastMeeting = (from m in CurrentDatabase.Meetings
                        where m.OrganizationId == OrgId
                        where m.Attends.Any(aa => aa.AttendanceFlag)
                        orderby m.MeetingDate descending
                        select m.MeetingDate).First(),
                    ConsecutiveAbsentsThreshold = o.ConsecutiveAbsentsThreshold ?? 2,
                    FilterName = fo != null ? fo.OrganizationName : "",
                    FilterLeader = fo != null ? fo.LeaderName : "",
                    WorshipName = wo != null ? wo.OrganizationName : "",
                };
            var i = q.Single();
            OrganizationName = i.OrganizationName;
            OrganizationLeader = i.LeaderName;
            LastMeeting = i.LastMeeting;
            ConsecutiveAbsentsThreshold = i.ConsecutiveAbsentsThreshold;
            OrgFilterName = i.FilterName;
            OrgFilterLeaderName = i.FilterLeader;
            DefaultWorshipName = i.WorshipName;
        }
        public int OrgId { get; set; }
        public Guid? QueryId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationLeader { get; set; }
        public DateTime? LastMeeting { get; set; }
        public int ConsecutiveAbsentsThreshold { get; set; }
        public int? OtherOrgFilterId { get; set; }
        public string OrgFilterName { get; set; }
        public string OrgFilterLeaderName { get; set; }
        public int? DefaultWorshipId { get; set; }
        public string DefaultWorshipName { get; set; }

        public IEnumerable<dynamic> FetchAbsents()
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            return cn.Query("RecentAbsents1", new { OrgId, OtherOrgFilterId, QueryId },
                commandType: CommandType.StoredProcedure, commandTimeout: 600);
        }
    }
}