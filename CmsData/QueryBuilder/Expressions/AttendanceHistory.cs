/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CmsData.API;
using UtilityExtensions;
using System.Configuration;
using System.Reflection;
using System.Collections;
using System.Data.Linq.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using CmsData.Codes;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression AttendMemberTypeAsOf(CMSDataContext Db,
            ParameterExpression parm,
            DateTime? from,
            DateTime? to,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            string ids)
        {
            to = to.HasValue ? to.Value.AddDays(1) : from.Value.AddDays(1);
            Expression<Func<Person, bool>> pred = p =>
                Db.AttendMemberTypeAsOf(from, to, progid, divid, org,
                    op == CompareType.NotEqual || op == CompareType.NotOneOf,
                    ids).Select(a => a.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression AttendanceTypeAsOf(
            ParameterExpression parm,
            DateTime? from,
            DateTime? to,
            int? progid,
            int? divid,
            int? org,
            int orgtype,
            CompareType op,
            params int[] ids)
        {
            to = to.HasValue ? to.Value.AddDays(1) : from.Value.AddDays(1);
            Expression<Func<Person, bool>> pred = p => (
                from a in p.Attends
                where a.MeetingDate >= @from
                where a.MeetingDate < to
                where (a.AttendanceFlag
                        || (ids.Length == 1 && ids[0] == AttendTypeCode.Offsite))
                where ids.Contains(a.AttendanceTypeId ?? 0)
                where orgtype == 0 || a.Meeting.Organization.OrganizationTypeId == orgtype
                where org == 0 || a.Meeting.OrganizationId == org
                where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select a
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression AttendedAsOf(
            ParameterExpression parm,
            DateTime? from,
            DateTime? to,
            int? progid,
            int? divid,
            CompareType op,
            bool tf,
            bool guestonly)
        {
            Expression<Func<Person, bool>> pred = p => (
                from a in p.Attends
                where a.MeetingDate >= @from
                where a.MeetingDate <= to
                where guestonly == false || a.AttendanceTypeId == 50 || a.AttendanceTypeId == 60
                where a.AttendanceFlag
                where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select a
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression AttendPctHistory(
           ParameterExpression parm, CMSDataContext Db,
           int? progid,
           int? divid,
           int? org,
           DateTime? start,
           DateTime? end,
           CompareType op,
           double pct)
        {
            if (!end.HasValue)
                end = start.Value;
            end = end.Value.AddDays(1);
            // note: this only works for members because visitors do not have att%
            var now = DateTime.Now;

            var q = from p in Db.People
                    let m = from et in p.EnrollmentTransactions
                            where et.TransactionTypeId <= 3 // things that start a change
                            where et.TransactionStatus == false
                            where et.TransactionDate < end // transaction starts <= looked for end
                            where (et.Pending ?? false) == false
                            where (et.NextTranChangeDate ?? now) >= start // transaction ends >= looked for start
                            where org == 0 || et.OrganizationId == org
                            where divid == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                            where progid == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                            select et
                    where m.Any()
                    select p;
            IQueryable<int> q2 = null;

            switch (op)
            {
                case CompareType.Greater:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= start
                                 where a.MeetingDate < end
                                 where org == 0 || a.Meeting.OrganizationId == org
                                 where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                 where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) > pct
                         select p.PeopleId;
                    break;
                case CompareType.GreaterEqual:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= start
                                 where a.MeetingDate < end
                                 where org == 0 || a.Meeting.OrganizationId == org
                                 where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                 where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) >= pct
                         select p.PeopleId;
                    break;
                case CompareType.Less:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= start
                                 where a.MeetingDate < end
                                 where org == 0 || a.Meeting.OrganizationId == org
                                 where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                 where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) < pct
                         select p.PeopleId;
                    break;
                case CompareType.LessEqual:
                    q2 = from p in q
                         let g = from a in p.Attends
                                 where a.MeetingDate >= start
                                 where a.MeetingDate < end
                                 where org == 0 || a.Meeting.OrganizationId == org
                                 where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                 where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                 select a
                         let n = g.Count(aa => aa.EffAttendFlag == true)
                         let d = g.Count(aa => aa.EffAttendFlag != null)
                         where (d == 0 ? 0d : n * 100.0 / d) <= pct
                         select p.PeopleId;
                    break;
                case CompareType.NotEqual:
                case CompareType.Equal:
                    return AlwaysFalse(parm);
            }

            var tag = Db.PopulateTemporaryTag(q2);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression AttendCntHistory(
            ParameterExpression parm, CMSDataContext Db,
            int? progid,
            int? divid,
            int? org,
            int sched,
            DateTime? start,
            DateTime? end,
            CompareType op,
            int cnt)
        {
            if (!end.HasValue)
                end = start.Value;
            end = end.Value.AddDays(1);
            var sc = Db.OrgSchedules.FirstOrDefault(cc => cc.ScheduleId == sched);
            DateTime? mtime = null;
            if (sc != null)
                mtime = sc.SchedTime;
            Expression<Func<Person, int>> pred = null;
            if (mtime != null)
            {
                pred = p =>
                    (
                    from a in p.Attends
                    where a.AttendanceFlag
                    where a.MeetingDate >= start
                    where a.MeetingDate < end
                        where a.MeetingDate.TimeOfDay == mtime.Value.TimeOfDay
                        where org == 0 || a.Meeting.OrganizationId == org
                        where divid == 0 || a.Meeting.Organization.DivOrgs.Any( dg => dg.DivId == divid)
                        where progid == 0 || a.Meeting.Organization.DivOrgs.Any( dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                        select a 
                     ).Count();
            }
            else
            {
                pred = p =>
                    (
                        from a in p.Attends
                        where a.AttendanceFlag
                        where a.MeetingDate >= start
                        where a.MeetingDate < end
                    where org == 0 || a.Meeting.OrganizationId == org
                    where divid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                    where progid == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                    select a
                ).Count();
            }
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression DaysBetween12Attendance(
            ParameterExpression parm, CMSDataContext Db,
            int? lookback,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int>> pred = p =>
                    Db.DaysBetween12Attend(p.PeopleId, progid, divid, org, lookback).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(days), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression DaysAfterNthVisitDateRange(
            ParameterExpression parm, CMSDataContext db,
            DateTime? from,
            DateTime? to,
            int nthvisit,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int>> pred = p =>
                 db.AttendDaysAfterNthVisitInDateRange(p.PeopleId, progid, divid, org,
                                                       from, to, nthvisit).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(days), left.Type);
            return Compare(left, op, right);
        }
    }
}
