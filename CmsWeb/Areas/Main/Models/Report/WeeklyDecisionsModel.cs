using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Report
{
    public class WeeklyDecisionsModel
    {
        public DateTime Sunday { get; set; }

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
                     where p.DecisionDate >= dt1 && p.DecisionDate <= dt2
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
                where p.DecisionDate >= dt1 && p.DecisionDate < (dt2 ?? dt1).Value.AddDays(1)
                where p.DecisionTypeId != null
                where !decisionTypes.Contains(p.DecisionTypeId.Value)
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
            var q2 = from p in DbUtil.Db.People
                     where p.DecisionDate >= dt1 && p.DecisionDate <= (dt2 ?? dt1).Value.AddDays(1)
                     where p.DecisionTypeId == null ||
                         decisionTypes.Contains(p.DecisionTypeId.Value)
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
                    let agerange = DbUtil.Db.BaptismAgeRange(p.Age ?? 0)
                    where p.BaptismDate >= dt1 && p.BaptismDate <= (dt2 ?? dt1).Value.AddDays(1)
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
                    where p.BaptismDate >= dt1 && p.BaptismDate <= (dt2 ?? dt1).Value.AddDays(1)
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
                    where p.JoinDate >= dt1 && p.JoinDate <= (dt2 ?? dt1).Value.AddDays(1)
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
                    where p.DropDate >= dt1 && p.DropDate <= (dt2 ?? dt1).Value
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
                     where p.DropDate >= dt1 && p.DropDate <= (dt2 ?? dt1).Value
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
            if (Fingerprint.UseNewLook())
                return ConvertToQuery(command, key);
            return ConvertToSearchBuilder(command, key);
        }
        private string ConvertToSearchBuilder(string command, string key)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);

            bool NotAll = key != "All";

            switch (command)
            {
                case "ForDecisionType":
                    qb.AddNewClause(QueryType.DecisionDate, CompareType.GreaterEqual, dt1);
                    qb.AddNewClause(QueryType.DecisionDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        qb.AddNewClause(QueryType.DecisionTypeId, CompareType.Equal, key);
                    break;
                case "ForBaptismAge":
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, dt1);
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                    {
                        var a = key.Split('-');
                        if (a[0].StartsWith("Over "))
                        {
                            a = key.Split(' ');
                            a[0] = (a[1].ToInt() + 1).ToString();
                            a[1] = "120";
                        }
                        qb.AddNewClause(QueryType.Age, CompareType.GreaterEqual, a[0].ToInt());
                        qb.AddNewClause(QueryType.Age, CompareType.LessEqual, a[1].ToInt());
                    }
                    break;
                case "ForBaptismType":
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, dt1);
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        qb.AddNewClause(QueryType.BaptismTypeId, CompareType.Equal, key);
                    break;
                case "ForNewMemberType":
                    qb.AddNewClause(QueryType.JoinDate, CompareType.GreaterEqual, dt1);
                    qb.AddNewClause(QueryType.JoinDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        qb.AddNewClause(QueryType.JoinCodeId, CompareType.Equal, key);
                    break;
                case "ForDropType":
                    qb.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, dt1);
                    qb.AddNewClause(QueryType.DropDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        qb.AddNewClause(QueryType.DropCodeId, CompareType.Equal, key);
                    qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                    break;
                case "DroppedForChurch":
                    qb.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, dt1);
                    qb.AddNewClause(QueryType.DropDate, CompareType.LessEqual, dt2);
                    switch (key)
                    {
                        case "Unknown":
                            qb.AddNewClause(QueryType.OtherNewChurch, CompareType.IsNull, "");
                            qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                            break;
                        case "Total":
                            break;
                        default:
                            qb.AddNewClause(QueryType.OtherNewChurch, CompareType.Equal, key);
                            break;
                    }
                    qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return "/QueryBuilder/Main/" + qb.QueryId;
        }
        private string ConvertToQuery(string command, string key)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset(DbUtil.Db);

            bool NotAll = key != "All";

            switch (command)
            {
                case "ForDecisionType":
                    cc.AddNewClause(QueryType.DecisionDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.DecisionDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        cc.AddNewClause(QueryType.DecisionTypeId, CompareType.Equal, key);
                    break;
                case "ForBaptismAge":
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.LessEqual, dt2);
                    if (NotAll)
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
                    }
                    break;
                case "ForBaptismType":
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.BaptismDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        cc.AddNewClause(QueryType.BaptismTypeId, CompareType.Equal, key);
                    break;
                case "ForNewMemberType":
                    cc.AddNewClause(QueryType.JoinDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.JoinDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        cc.AddNewClause(QueryType.JoinCodeId, CompareType.Equal, key);
                    break;
                case "ForDropType":
                    cc.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.DropDate, CompareType.LessEqual, dt2);
                    if (NotAll)
                        cc.AddNewClause(QueryType.DropCodeId, CompareType.Equal, key);
                    cc.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                    break;
                case "DroppedForChurch":
                    cc.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, dt1);
                    cc.AddNewClause(QueryType.DropDate, CompareType.LessEqual, dt2);
                    switch (key)
                    {
                        case "Unknown":
                            cc.AddNewClause(QueryType.OtherNewChurch, CompareType.IsNull, "");
                            cc.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                            break;
                        case "Total":
                            break;
                        default:
                            cc.AddNewClause(QueryType.OtherNewChurch, CompareType.Equal, key);
                            break;
                    }
                    cc.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                    break;
            }
            cc.Save(DbUtil.Db);
            return "/QueryBuilder/Main/" + cc.Id;
        }
    }
}
