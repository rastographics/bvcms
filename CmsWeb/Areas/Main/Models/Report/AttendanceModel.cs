﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using System.Collections;

namespace CMSWeb.Areas.Main.Models.Report
{
    public class AttendanceModel
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string Sort { get; set; }
        public string Dir { get; set; }

        public AttendanceModel(int orgId)
        {
            OrgId = orgId;
            var dt = UtilityExtensions.Util.Now;
            var q = from o in DbUtil.Db.Organizations
                    where o.OrganizationId == orgId
                    select new
                    {
                        o.OrganizationName,
                        MaxMeet = o.Meetings.Where(m => m.MeetingDate < dt).Max(m => m.MeetingDate),
                        MinMeet = o.Meetings.Min(m => m.MeetingDate)
                    };
            var i = q.SingleOrDefault();
            if (i == null)
                throw new Exception("organization not found");
            if (!i.MinMeet.HasValue)
                throw new Exception("no meetings");
            OrgName = i.OrganizationName;
            end = i.MaxMeet.Value;
            start = end.AddYears(-1);
            if (i.MinMeet > start)
                start = i.MinMeet.Value;
        }

        public IEnumerable<AttendInfo> Attendances()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.OrganizationId == OrgId
                    let attstr = GetAttendStr(om.PeopleId, om.OrganizationId)
                    let attpct = GetAttendPct(attstr)
                    select new AttendInfo
                    {
                        Name = om.Person.Name2,
                        PeopleId = om.PeopleId,
                        AttendPct = attpct,
                        AttendStr = attstr,
                        Age = om.Person.Age ?? 0,
                    };
            var list = q.ToList();
            if (Dir == "asc")
                switch (Sort)
                {
                    case "Name":
                        list = list.OrderBy(a => a.Name).ToList();
                        break;
                    case "Age":
                        list = list.OrderBy(a => a.Age).ToList();
                        break;
                    case "Percent":
                        list = list.OrderBy(a => a.AttendPct).ToList();
                        break;
                    case "Count":
                        list = list.OrderBy(a => a.AttendCount).ToList();
                        break;
                }
            else
                switch (Sort)
                {
                    case "Name":
                        list = list.OrderByDescending(a => a.Name).ToList();
                        break;
                    case "Age":
                        list = list.OrderByDescending(a => a.Age).ToList();
                        break;
                    case "Percent":
                        list = list.OrderByDescending(a => a.AttendPct).ToList();
                        break;
                    case "Count":
                        list = list.OrderByDescending(a => a.AttendCount).ToList();
                        break;
                }
            return list;
        }

        private string GetAttendStr(int pid, int orgid)
        {
            var q = from a in DbUtil.Db.Attends
                    where a.OrganizationId == orgid
                    where a.PeopleId == pid
                    where a.MeetingDate > start && a.MeetingDate <= end
                    where ((a.Organization.AttendTrkLevelId == 30 && a.Registered == true)
			        || a.Organization.AttendTrkLevelId == 20)
                    orderby a.MeetingDate.Date descending
                    select new
                    {
                        EffAttendFlag = a.EffAttendFlag,
                        AttendanceTypeId = a.AttendanceTypeId
                    };
            var attstr = new StringBuilder();
            foreach (var a in q)
            {
                char indicator;
                if (a.EffAttendFlag == null)
                    switch (a.AttendanceTypeId)
                    {
                        case (int)Attend.AttendTypeCode.Volunteer:
                            indicator = 'V';
                            break;
                        case (int)Attend.AttendTypeCode.InService:
                            indicator = 'I';
                            break;
                        case (int)Attend.AttendTypeCode.Group:
                            indicator = 'G';
                            break;
                        case (int)Attend.AttendTypeCode.Offsite:
                            indicator = 'O';
                            break;
                        default:
                            indicator = '*';
                            break;
                    }
                else if (a.EffAttendFlag == true)
                    indicator = 'P';
                else
                    indicator = '.';
                attstr.Insert(0, indicator);
            }
            return attstr.ToString();
        }
        private double GetAttendPct(string attstr)
        {
            int tcnt = 0, acnt = 0;
            foreach (var c in attstr)
            {
                if (c == 'P' || c == '.')
                    tcnt++;
                if (c == 'P')
                    acnt++;
            }
            if (tcnt > 0)
                return (Convert.ToDouble(acnt) / Convert.ToDouble(tcnt) * 100.0);
            else
                return 0.0;
        }
        public class AttendInfo
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int PeopleId { get; set; }
            public string AttendStr { get; set; }
            public double AttendPct { get; set; }
            public int AttendCount
            {
                get
                {
                    return AttendStr.ToCharArray().Count(c => c == 'P');
                }
            }
        }
    }
}
