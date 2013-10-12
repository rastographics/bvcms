using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Report
{
    public class DecisionsModel
    {
        public DateTime Sunday { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public DecisionsModel(DateTime? dt)
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
        public int QBDrillDownId(string CommandName, bool? NotAll, string CommandArgument)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);
            DateTime dt;
            DateTime? FromDt = null, dt2 = null;
            if (DateTime.TryParse(FromDate, out dt))
                FromDt = dt;
            if (DateTime.TryParse(ToDate, out dt))
                dt2 = dt;
            DateTime ToDt = (dt2 ?? FromDt).Value.AddDays(1);

            switch (CommandName)
            {
                case "ForDecisionType":
                    qb.AddNewClause(QueryType.DecisionDate, CompareType.GreaterEqual, FromDt);
                    qb.AddNewClause(QueryType.DecisionDate, CompareType.Less, ToDt);
                    if (NotAll == true)
                        qb.AddNewClause(QueryType.DecisionTypeId, CompareType.Equal, CommandArgument);
                    break;
                case "ForBaptismAge":
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, FromDt);
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.Less, ToDt);
                    if (NotAll == true)
                    {
                        var a = CommandArgument.ToString().Split('-');
                        if (a[0].StartsWith("Over "))
                        {
                            a = CommandArgument.ToString().Split(' ');
                            a[0] = (a[1].ToInt() + 1).ToString();
                            a[1] = "120";
                        }
                        qb.AddNewClause(QueryType.Age, CompareType.GreaterEqual, a[0].ToInt());
                        qb.AddNewClause(QueryType.Age, CompareType.LessEqual, a[1].ToInt());
                    }
                    break;
                case "ForBaptismType":
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.GreaterEqual, FromDt);
                    qb.AddNewClause(QueryType.BaptismDate, CompareType.Less, ToDt);
                    if (NotAll == true)
                        qb.AddNewClause(QueryType.BaptismTypeId, CompareType.Equal, CommandArgument);
                    break;
                case "ForNewMemberType":
                    qb.AddNewClause(QueryType.JoinDate, CompareType.GreaterEqual, FromDt);
                    qb.AddNewClause(QueryType.JoinDate, CompareType.Less, ToDt);
                    if (NotAll == true)
                        qb.AddNewClause(QueryType.JoinCodeId, CompareType.Equal, CommandArgument);
                    break;
                case "ForDropType":
                    qb.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, FromDt);
                    qb.AddNewClause(QueryType.DropDate, CompareType.Less, ToDt);
                    if (NotAll == true)
                        qb.AddNewClause(QueryType.DropCodeId, CompareType.Equal, CommandArgument);
                    qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                    break;
                case "DroppedForChurch":
                    qb.AddNewClause(QueryType.DropDate, CompareType.GreaterEqual, FromDt);
                    qb.AddNewClause(QueryType.DropDate, CompareType.Less, ToDt);
                    switch (CommandArgument)
                    {
                        case "Unknown":
                            qb.AddNewClause(QueryType.OtherNewChurch, CompareType.IsNull, "");
                            qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                            break;
                        case "Total":
                            break;
                        default:
                            qb.AddNewClause(QueryType.OtherNewChurch, CompareType.Equal, CommandArgument);
                            break;
                    }
                    qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return qb.QueryId;
        }
    }
}
