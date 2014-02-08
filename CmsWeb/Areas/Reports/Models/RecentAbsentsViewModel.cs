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
        public RecentAbsentsViewModel(int id, int? idfilter)
        {
            OrgId = id;
            OrgFilterId = idfilter;
            DefaultWorshipId = DbUtil.Db.Setting("WorshipId", "0").ToInt();
            var q = from o in DbUtil.Db.Organizations
                where o.OrganizationId == id
                let fo = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == idfilter)
                let wo = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == DefaultWorshipId)
                select new
                {
                    o.OrganizationName,
                    o.LeaderName,
                    LastMeeting = (from m in DbUtil.Db.Meetings
                        where m.OrganizationId == id
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
        public string OrganizationName { get; set; }
        public string OrganizationLeader { get; set; }
        public DateTime? LastMeeting { get; set; }
        public int ConsecutiveAbsentsThreshold { get; set; }
        public int? OrgFilterId { get; set; }
        public string OrgFilterName { get; set; }
        public string OrgFilterLeaderName { get; set; }
        public int? DefaultWorshipId { get; set; }
        public string DefaultWorshipName { get; set; }

        public IEnumerable<dynamic> FetchAbsents()
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            return cn.Query("RecentAbsentsSP", new { orgid = OrgId, orgidfilter = OrgFilterId },
                commandType: CommandType.StoredProcedure, commandTimeout: 600);
        }
    }
}