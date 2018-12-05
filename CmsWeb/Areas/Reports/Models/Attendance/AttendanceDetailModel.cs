using CmsData;
using CmsWeb.Areas.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Reports.Models
{
    public class AttendanceDetailModel
    {
        private readonly DateTime dt1;
        private DateTime? dt2;
        private OrgSearchModel Model;

        public AttendanceDetailModel() { }
        public AttendanceDetailModel(DateTime dt1, DateTime? dt2, OrgSearchModel model)
        {
            if (dt2.HasValue)
            {
                if (dt2.Value.TimeOfDay == TimeSpan.Zero)
                {
                    dt2 = dt2.Value.AddDays(1);
                }
            }
            else
            {
                dt2 = dt1.AddDays(1);
            }

            this.dt1 = dt1;
            this.dt2 = dt2;
            Model = model;
        }
        public List<MeetingRow> FetchMeetings()
        {
            var orgs = Model.FetchOrgs();
            var q = from m in DbUtil.Db.Meetings
                    join o in orgs on m.OrganizationId equals o.OrganizationId
                    where m.MeetingDate >= dt1
                    where m.MeetingDate <= dt2
                    where Model.ScheduleId == 0 || m.ScheduleId == Model.ScheduleId
                    orderby o.OrganizationName, o.OrganizationId, m.MeetingDate descending
                    select new
                    {
                        MeetingId = m.MeetingId,
                        OrgName = m.Organization.OrganizationName,
                        Leader = m.Organization.LeaderName,
                        location = m.Organization.Location,
                        date = m.MeetingDate.Value,
                        OrgId = m.OrganizationId,
                        Present = m.MaxCount ?? 0,
                        Visitors = m.NumNewVisit + m.NumRepeatVst,
                        OutTowners = m.NumOutTown ?? 0
                    };

            var q2 = from m in q
                     select new MeetingRow
                     {
                         OrgName = m.OrgName,
                         Leader = m.Leader,
                         DateTime = m.date,
                         Present = m.Present,
                         Visitors = m.Visitors,
                         OutTowners = m.OutTowners
                     };
            var list = q2.ToList();
            var t = new MeetingRow
            {
                OrgName = "Total",
                Leader = string.Empty,
                Present = list.Sum(i => i.Present),
                Visitors = list.Sum(i => i.Visitors),
                OutTowners = list.Sum(i => i.OutTowners)
            };
            list.Add(t);
            return list;
        }
        public class MeetingRow
        {
            //            public int MeetingId { get; set; }
            public string OrgName { get; set; }
            public string Leader { get; set; }
            public DateTime DateTime { get; set; }
            //            public string OrgId { get; set; }
            public int Present { get; set; }
            public int Visitors { get; set; }
            public int OutTowners { get; set; }
        }
    }
}
