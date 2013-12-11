/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression AttendMemberTypeAsOf()
        {
            var ids = string.Join(",", CodeStrIds);
            EndDate = EndDate.HasValue ? EndDate.Value.AddDays(1) : StartDate.Value.AddDays(1);
            Expression<Func<Person, bool>> pred = p =>
                db.AttendMemberTypeAsOf(StartDate, EndDate, Program, Division, Organization, ids)
                .Select(a => a.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression AttendanceTypeAsOf()
        {
            var ids = string.Join(",", CodeStrIds);
            EndDate = EndDate.HasValue ? EndDate.Value.AddDays(1) : StartDate.Value.AddDays(1);
            Expression<Func<Person, bool>> pred = p =>
                db.AttendanceTypeAsOf(StartDate, EndDate, Program, Division, Organization, OrgType ?? 0, ids)
                .Select(a => a.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression GuestAttendedAsOf()
        {
            return AttendedAsOf(guestonly: true);
        }
        internal Expression MemberAttendedAsOf()
        {
            return AttendedAsOf(guestonly: false);
        }
        private Expression AttendedAsOf(bool guestonly)
        {
            var tf = CodeIds == "1"; 
            var q = db.AttendedAsOf(Program, Division, null, StartDate, EndDate, guestonly).Select(p => p.PeopleId);
            Expression<Func<Person, bool>> pred;
            if (op == CompareType.Equal ^ tf)
                pred = p => !q.Contains(p.PeopleId);
            else
                pred = p => q.Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression AttendPctHistory()
        {
            var end = EndDate;
            if (!end.HasValue)
                end = StartDate.Value;
            end = end.Value.AddDays(1);
            var pct = double.Parse(TextValue);
            // note: this only works for members because visitors do not have att%
            var now = DateTime.Now;

            var q = from p in db.People
                    let m = from et in p.EnrollmentTransactions
                            where et.TransactionTypeId <= 3 // things that start a change
                            where et.TransactionStatus == false
                            where et.TransactionDate < end // transaction starts <= looked for end
                            where (et.Pending ?? false) == false
                            where (et.NextTranChangeDate ?? now) >= StartDate // transaction ends >= looked for start
                            where Organization == 0 || et.OrganizationId == Organization
                            where Division == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                            where Program == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                            select et
                    where m.Any()
                    select p;
            IQueryable<int> q2 = null;

            switch (op)
            {
                case CompareType.Greater:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= StartDate
                                 where a.MeetingDate < end
                                 where Organization == 0 || a.Meeting.OrganizationId == Organization
                                 where Division == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                                 where Program == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) > pct
                         select p.PeopleId;
                    break;
                case CompareType.GreaterEqual:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= StartDate
                                 where a.MeetingDate < end
                                 where Organization == 0 || a.Meeting.OrganizationId == Organization
                                 where Division == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                                 where Program == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) >= pct
                         select p.PeopleId;
                    break;
                case CompareType.Less:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= StartDate
                                 where a.MeetingDate < end
                                 where Organization == 0 || a.Meeting.OrganizationId == Organization
                                 where Division == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                                 where Program == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) < pct
                         select p.PeopleId;
                    break;
                case CompareType.LessEqual:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= StartDate
                                 where a.MeetingDate < end
                                 where Organization == 0 || a.Meeting.OrganizationId == Organization
                                 where Division == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                                 where Program == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) <= pct
                         select p.PeopleId;
                    break;
                case CompareType.NotEqual:
                case CompareType.Equal:
                    return AlwaysFalse();
                default:
                    break;
            }

            var tag = db.PopulateTemporaryTag(q2);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression AttendCntHistory()
        {
            var end = EndDate;
            if (!end.HasValue)
                end = StartDate.Value;
            end = end.Value.AddDays(1);
            var cnt = TextValue.ToInt();

            var q = db.AttendCntHistory(Program, Division, Organization, Schedule, StartDate, end);
            switch (op)
            {
                case CompareType.Greater:
                    q = q.Where(cc => cc.Cnt > cnt); break;
                case CompareType.GreaterEqual:
                    q = q.Where(cc => cc.Cnt >= cnt); break;
                case CompareType.Less:
                    q = q.Where(cc => cc.Cnt < cnt); break;
                case CompareType.LessEqual:
                    q = q.Where(cc => cc.Cnt <= cnt); break;
                case CompareType.Equal:
                    q = q.Where(cc => cc.Cnt == cnt); break;
                case CompareType.NotEqual:
                    q = q.Where(cc => cc.Cnt != cnt); break;
            }
            Expression<Func<Person, bool>> pred = p => q.Select(c => c.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression DaysBetween12Attendance()
        {
            var lookback = TextValue.ToInt();
            Expression<Func<Person, int>> pred = p =>
                    db.DaysBetween12Attend(p.PeopleId, Program, Division, Organization, lookback).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(Days), left.Type);
            return Compare(left, right);
        }
        internal Expression DaysAfterNthVisitDateRange()
        {
            var nthvisit = TextValue.ToInt();
            Expression<Func<Person, int>> pred = p =>
                 db.AttendDaysAfterNthVisitInDateRange(p.PeopleId, Program, Division, Organization,
                                                       StartDate, EndDate, nthvisit).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(Quarters.ToInt()), left.Type);
            return Compare(left, right);
        }
    }
}
