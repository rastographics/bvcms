using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class WeeklyDecisionsModel
    {
        public DateTime Sunday { get; set; }
        public int? Campus { get; set; }

        public WeeklyDecisionsModel() { }
        public WeeklyDecisionsModel(DateTime? dt)
        {
            Sunday = dt ?? MostRecentAttendedSunday();
        }
        public static DateTime MostRecentAttendedSunday()
        {
            var q = from m in DbUtil.Db.Meetings
                    where m.MeetingDate.Value.Date.DayOfWeek == 0
                    where m.MaxCount > 0
                    where m.MeetingDate < Util.Now
                    orderby m.MeetingDate descending
                    select m.MeetingDate.Value.Date;
            var dt = q.FirstOrDefault();
            if (dt == DateTime.MinValue) //Sunday Date equal/before today
                dt = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return dt;
        }
        public class NameCount
        {
            public int? Count { get; set; }
            public string Name { get; set; }
        }

        public NameCount TotalBaptisms;
        public NameCount TotalDecisions;
        public IEnumerable<NameCount> Baptisms()
        {
            var dt1 = Sunday.AddDays(-4);
            var dt2 = Sunday.AddDays(2);
            var q3 = from p in DbUtil.Db.People
                     where p.BaptismDate >= dt1 && p.BaptismDate <= dt2
                     where (Campus ?? 0) == 0 || p.CampusId == Campus
                     group p by p.BaptismType.Description into g
                     orderby g.Key
                     select new NameCount
                     {
                         Name = g.Key,
                         Count = g.Count()
                     };
            var list = q3.ToList();
            TotalBaptisms = new NameCount
            {
                Name = "Total",
                Count = q3.Sum(i => i.Count)
            };
            return list;
        }

        public IEnumerable<NameCount> Decisions()
        {
            var dt1 = Sunday.AddDays(-4);
            var dt2 = Sunday.AddDays(2);
            var q3 = from p in DbUtil.Db.People
                     where p.DecisionDate >= dt1 && p.DecisionDate < dt2.AddDays(1)
                     where (Campus ?? 0) == 0 || p.CampusId == Campus
                     group p by p.DecisionType.Description into g
                     orderby g.Key
                     select new NameCount
                     {
                         Name = g.Key,
                         Count = g.Count()
                     };
            var list = q3.ToList();
            TotalDecisions = new NameCount
            {
                Name = "Total",
                Count = q3.Sum(i => i.Count)
            };
            return list;
        }
        public SelectList CampusList()
        {
            return new CodeValueModel().AllCampuses0().ToSelect();
        }
    }
    public class TypeCountInfo
    {
        public string Id { get; set; }
        public string Desc { get; set; }
        public int? Count { get; set; }
        public string CssClass { get { return Desc == "Total" ? "TotalLine" : ""; } }
    }
    public class PersonInfo2
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

    public class DecisionSummaryModel
    {
        public DecisionSummaryModel(DateTime? dt1, DateTime? dt2)
        {
            this.dt1 = dt1;
            this.dt2 = dt2;
        }
        public DateTime? dt1 { get; set; }
        public DateTime? dt2 { get; set; }
        public int? Campus { get; set; }

        private DateTime dt2v
        {
            get
            {
                var dt = dt2 ?? dt1;
                if (dt.HasValue)
                    dt = dt.Value.AddDays(1);
                else
                    dt = DateTime.MaxValue;
                return dt.Value;
            }
        }

        IEnumerable<TypeCountInfo> Total(int? count)
        {
            if (!count.HasValue || count.Value == 0)
                return new TypeCountInfo[] { };
            return new TypeCountInfo[]  
            { 
                new TypeCountInfo() { Id="All", Desc = "Total", Count = count ?? 0 } 
            };
        }
        private int[] decisionTypes = new int[] 
        { 
            DecisionCode.Unknown,
            DecisionCode.ProfessionNotForMembership,
            DecisionCode.Cancelled,
        };
        public int DecisionsCount { get; set; }
        public IEnumerable<TypeCountInfo> DecisionsByType()
        {
            if (!dt1.HasValue)
                return null;
            // member decisions
            var q = from p in DbUtil.Db.People
                    where p.DecisionDate >= dt1 && p.DecisionDate < dt2v
                    where p.DecisionTypeId != null
                    where !decisionTypes.Contains(p.DecisionTypeId.Value)
                    where (Campus ?? 0) == 0 || p.CampusId == Campus
                    group p by p.DecisionTypeId + "," + p.DecisionType.Code
                        into g
                        orderby g.Key
                        select new TypeCountInfo
                        {
                            Id = g.Key,
                            Desc = g.First().DecisionType.Description,
                            Count = g.Count(),
                        };
            DecisionsCount = q.Sum(t => t.Count) ?? 0;
            return q.ToList();
        }
        public IEnumerable<TypeCountInfo> DecisionsByType2()
        {
            if (!dt1.HasValue)
                return null;
            // non member decisions
            var q = from p in DbUtil.Db.People
                    where p.DecisionDate >= dt1 && p.DecisionDate < dt2v
                    where (Campus ?? 0) == 0 || p.CampusId == Campus
                    where p.DecisionTypeId == null || decisionTypes.Contains(p.DecisionTypeId.Value)
                    select p;
            var q2 = from p in q
                     group p by p.DecisionTypeId + "," + p.DecisionType.Code into g
                     orderby g.Key
                     select new TypeCountInfo
                     {
                         Id = g.Key,
                         Desc = g.First().DecisionType.Description,
                         Count = g.Count(),
                     };
            return q2;
        }

        public int BaptismsCount { get; set; }
        public IEnumerable<TypeCountInfo> BaptismsByAge()
        {
            if (!dt1.HasValue)
                return null;
            var q = from p in DbUtil.Db.People
                    let agerange = DbUtil.Db.BaptismAgeRange(p.Age)
                    where p.BaptismDate >= dt1 && p.BaptismDate < dt2v
                    where (Campus ?? 0) == 0 || p.CampusId == Campus
                    group p by agerange into g
                    orderby g.Key
                    select new TypeCountInfo
                    {
                        Id = g.Key,
                        Desc = g.Key,
                        Count = g.Count(),
                    };
            BaptismsCount = q.Sum(t => t.Count) ?? 0;
            return q.ToList();
        }
        public IEnumerable<TypeCountInfo> BaptismsByType()
        {
            if (!dt1.HasValue)
                return null;
            var q = from p in DbUtil.Db.People
                    where p.BaptismDate >= dt1 && p.BaptismDate < dt2v
                    where (Campus ?? 0) == 0 || p.CampusId == Campus
                    group p by p.BaptismTypeId + "," + p.BaptismType.Code into g
                    orderby g.Key
                    select new TypeCountInfo
                    {
                        Id = g.Key,
                        Desc = g.First().BaptismType.Description,
                        Count = g.Count(),
                    };
            BaptismsCount = q.Sum(t => t.Count) ?? 0;
            return q.ToList();
        }

        public int NewMembersCount { get; set; }
        public IEnumerable<TypeCountInfo> NewMemberByType()
        {
            if (!dt1.HasValue)
                return null;
            var q = from p in DbUtil.Db.People
                    where p.JoinDate >= dt1 && p.JoinDate < dt2v
                    where (Campus ?? 0) == 0 || p.CampusId == Campus
                    group p by p.JoinCodeId + "," + p.JoinType.Code into g
                    orderby g.Key
                    select new TypeCountInfo
                    {
                        Id = g.Key,
                        Desc = g.First().JoinType.Description,
                        Count = g.Count(),
                    };
            NewMembersCount = q.Sum(t => t.Count) ?? 0;
            return q.ToList();
        }

        public int DroppedMembersCount { get; set; }
        public IEnumerable<TypeCountInfo> DroppedMemberByType()
        {
            if (!dt1.HasValue)
                return null;
            var q = from p in DbUtil.Db.People
                    where p.DropDate >= dt1 && p.DropDate < dt2v
                    where (Campus ?? 0) == 0 || p.CampusId == Campus
                    group p by p.DropCodeId + "," + p.DropType.Code into g
                    orderby g.Key
                    select new TypeCountInfo
                    {
                        Id = g.Key,
                        Desc = g.First().DropType.Description,
                        Count = g.Count(),
                    };
            DroppedMembersCount = q.Sum(t => t.Count) ?? 0;
            return q.ToList();
        }

        public IEnumerable<TypeCountInfo> DroppedMemberByChurch()
        {
            if (!dt1.HasValue)
                return null;
            var q0 = from p in DbUtil.Db.People
                     where p.DropDate >= dt1 && p.DropDate < dt2v
                     where (Campus ?? 0) == 0 || p.CampusId == Campus
                     select p;
            DroppedMembersCount = q0.Count();
            var q1 = from p in q0
                     group p by p.OtherNewChurch into g
                     select new TypeCountInfo
                     {
                         Desc = g.Key,
                         Count = g.Count()
                     };
            var q2 = from g in q1
                     let c = (g.Desc == "" || g.Desc == null) ? "Unknown" : g.Desc
                     group g by c into gg
                     select new TypeCountInfo
                     {
                         Desc = gg.Key,
                         Count = gg.Sum(t => t.Count)
                     };
            return q2.OrderByDescending(t => t.Count).ToList();
        }


        public string ConvertToSearch(string command, string key)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();

            bool NotAll = key != "All";

            switch (command)
            {
                case "ForDecisionType":
                    cc.AddNewClause(QueryType.DecisionDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.DecisionDate, CompareType.Less, dt2v);
                    if ((Campus ?? 0) > 0)
                        cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                    if (NotAll)
                        cc.AddNewClause(QueryType.DecisionTypeId, CompareType.Equal, key);
                    break;
                case "ForBaptismAge":
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.Less, dt2v);
                    if (NotAll)
                    {
                        if (key == " NA")
                        {
                            cc.AddNewClause(QueryType.BDate, CompareType.Equal, null);
                            if ((Campus ?? 0) > 0)
                                cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                        }
                        else
                        {
                            var a = key.Split('-');
                            if (a[0].StartsWith("Over "))
                            {
                                a = key.Split(' ');
                                a[0] = (a[1].ToInt() + 1).ToString();
                                a[1] = "120";
                            }
                            cc.AddNewClause(QueryType.Age, CompareType.GreaterEqual, a[0].ToInt());
                            cc.AddNewClause(QueryType.Age, CompareType.LessEqual, a[1].ToInt());
                            if ((Campus ?? 0) > 0)
                                cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                        }
                    }
                    break;
                case "ForBaptismType":
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.Less, dt2v);
                    if (NotAll)
                        cc.AddNewClause(QueryType.BaptismTypeId, CompareType.Equal, key);
                    if ((Campus ?? 0) > 0)
                        cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                    break;
                case "ForNewMemberType":
                    cc.AddNewClause(QueryType.JoinDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.JoinDate, CompareType.Less, dt2v);
                    if (NotAll)
                        cc.AddNewClause(QueryType.JoinCodeId, CompareType.Equal, key);
                    if ((Campus ?? 0) > 0)
                        cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                    break;
                case "ForDropType":
                    cc.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.DropDate, CompareType.Less, dt2v);
                    if (NotAll)
                        cc.AddNewClause(QueryType.DropCodeId, CompareType.Equal, key);
                    cc.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,True");
                    if ((Campus ?? 0) > 0)
                        cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                    break;
                case "DroppedForChurch":
                    cc.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.DropDate, CompareType.Less, dt2v);
                    switch (key)
                    {
                        case "Unknown":
                            cc.AddNewClause(QueryType.OtherNewChurch, CompareType.Equal, "");
                            cc.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,True");
                            break;
                        case "All":
                            break;
                        default:
                            cc.AddNewClause(QueryType.OtherNewChurch, CompareType.Equal, key);
                            break;
                    }
                    cc.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,True");
                    if ((Campus ?? 0) > 0)
                        cc.AddNewClause(QueryType.CampusId, CompareType.Equal, Campus);
                    break;
            }
            cc.Save(DbUtil.Db);
            return "/Query/" + cc.Id;
        }

        public SelectList CampusList()
        {
            return new CodeValueModel().AllCampuses0().ToSelect();
        }
    }
}
