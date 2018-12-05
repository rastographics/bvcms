/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class WeeklyAttendanceModel : OrgSearchModel
    {
        public WeeklyAttendanceModel() { }
        public IEnumerable<AttendInfo> Attendances()
        {
            var Orgs = FetchOrgs();
            var dt = Util.Now;
            var yearago = dt.AddYears(-1);
            var orgs = string.Join(",", Orgs.Select(oo => oo.OrganizationId));
            var people = (from p in DbUtil.Db.MembersWhoAttendedOrgs(orgs, yearago)
                select new
                {
                    p.PeopleId,
                    p.Name2,
                    p.Age,
                    p.BibleFellowshipClassId,
                    p.OrganizationName,
                    p.LeaderName
                }).ToDictionary(pp => pp.PeopleId, pp => pp);

            return from w in DbUtil.Db.WeeklyAttendsForOrgs(orgs, yearago).ToList()
                group w by w.PeopleId into g
                where g.Count(aa => aa.Attended) > 0
                let p = people[g.Key]
                orderby p.Name2
                select new AttendInfo()
                {
                    PeopleId = g.Key,
                    Name = p.Name2,
                    Age = p.Age ?? 0,
                    MainFellowship = p.OrganizationName,
                    OrgId = p.BibleFellowshipClassId,
                    Teacher = p.LeaderName,
                    AttendPct = g.Count(aa => aa.Attended)*100.0/g.Count(),
                    AttendStr = string.Join("", g.OrderBy(aa => aa.Sunday).Select(aa => aa.Attended ? "P" : ".")),
                    Count = g.Count(aa => aa.Attended),
                    FirstDate = g.Min(aa => aa.Sunday),
                    LastDate = g.Max(aa => aa.Sunday)
                };
        }

        public class AttendInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string MainFellowship { get; set; }
            public int? OrgId { get; set; }
            public string Teacher { get; set; }
            public string AttendStr { get; set; }
            public double AttendPct { get; set; }
            public int Count { get; set; }
            public DateTime FirstDate { get; set; }
            public DateTime LastDate { get; set; }
        }
    }
}

