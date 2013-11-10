/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using CmsData.Codes;
using Community.CsharpSqlite;
using UtilityExtensions;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression RecentAttendMemberType(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p => (
                from a in p.Attends
                where a.AttendanceFlag
                where a.MeetingDate >= mindt
                where ids.Contains(a.MemberTypeId)
                where org == 0 || a.Meeting.OrganizationId == org
                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select a
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentAttendCount(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            int orgtype,
            int days,
            CompareType op,
            int cnt)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, int>> pred = null;
            if (progid > 0)
                pred = p => (
                                from a in p.Attends
                                where a.MeetingDate >= mindt
                                where a.AttendanceFlag
                                where orgtype == 0 || a.Meeting.Organization.OrganizationTypeId == orgtype
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                select a
                            ).Count();
            else
                pred = p => (
                                from a in p.Attends
                                where a.MeetingDate >= mindt
                                where a.AttendanceFlag
                                where orgtype == 0 || a.Meeting.Organization.OrganizationTypeId == orgtype
                                select a
                            ).Count();

            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression RecentAttendCountAttCred(
            ParameterExpression parm,
            CMSDataContext Db,
            int? progid,
            int? divid,
            int? org,
            int? attcred,
            int sched,
            int days,
            CompareType op,
            int cnt)
        {
            var mindt = Util.Now.AddDays(-days).Date;
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
                        where a.MeetingDate >= mindt
                        where attcred == 0 || a.Meeting.AttendCreditId == attcred
                        where a.MeetingDate.TimeOfDay == mtime.Value.TimeOfDay
                        where org == 0 || a.Meeting.OrganizationId == org
                        where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                        where progid == 0 || a.Organization.DivOrgs.Any( dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                        select a
                      ).Count();
            }
            else
            {
                pred = p =>
                    (
                        from a in p.Attends
                        where a.AttendanceFlag
                        where a.MeetingDate >= mindt
                        where attcred == 0 || a.Meeting.AttendCreditId == attcred
                        where org == 0 || a.Meeting.OrganizationId == org
                        where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                        where progid == 0 || a.Organization.DivOrgs.Any( dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                        select a
                     ).Count();

            }
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression VisitNumber(
            ParameterExpression parm, CMSDataContext Db,
            string number,
            CompareType op,
            DateTime? dt)
        {
            int n = number.ToInt2() ?? 1;
            var cdt = Db.Setting("DbConvertedDate", "1/1/1900").ToDate();

            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Greater:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate > dt);
                    break;
                case CompareType.Less:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate < dt);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate >= dt);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate <= dt);
                    break;
                case CompareType.Equal:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate.Date == dt);
                    break;
                case CompareType.NotEqual:
                case CompareType.IsNull:
                    pred = p => p.CreatedDate > cdt &&
                           !p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate.Date == dt);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n);
                    break;
            }
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }

	    internal static Expression CheckInByDate( ParameterExpression parm, CMSDataContext Db, DateTime? start, DateTime? end, CompareType op, int cnt )
	    {
		    Expression<Func<Person, int>> pred = null;
		    if( end != null ) end = end.Value.AddDays( 1 ).AddTicks( -1 );
		    else end = DateTime.Now.Date.AddDays( 1 ).AddTicks( -1 );

		    pred = p => p.CheckInTimes.Count(ct => ct.CheckInTimeX >= start && ct.CheckInTimeX <= end);

		    Expression left = Expression.Invoke( pred, parm );
			 var right = Expression.Convert(Expression.Constant(cnt), left.Type);
		    return Compare( left, op, right );
	    }

		internal static Expression CheckInByActivity(ParameterExpression parm, CMSDataContext Db, CompareType op, string[] values)
		{
			Expression<Func<Person, bool>> pred = p => p.CheckInTimes.Any( e => e.CheckInActivities.Any( a => values.Contains( a.Activity )));
			Expression expr = Expression.Invoke(pred, parm);

			if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
				expr = Expression.Not(expr);

			return expr;
		}

		internal static Expression RecentVisitNumber( ParameterExpression parm, CMSDataContext Db, string number, int days, CompareType op, bool tf )
        {
            int n = number.ToInt2() ?? 1;
            var dt = DateTime.Now.AddDays(-days);
            var cdt = Db.Setting("DbConvertedDate", "1/1/1900").ToDate();
            Expression<Func<Person, bool>> pred = p => p.CreatedDate > cdt &&
                p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate > dt);
            Expression expr = Expression.Invoke(pred, parm);

            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression RecentNewVisitCount(
            ParameterExpression parm,
            CMSDataContext Db,
            int? progid,
            int? divid,
            int? org,
            int orgtype,
            string days0,
            int days,
            CompareType op,
            int cnt)
        {
            var dt1 = DateTime.Today.AddDays(-(days0.ToInt2() ?? 365));
            var dt2 = DateTime.Today.AddDays(-days);

            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from p in Db.People
                        let g = from a in p.Attends
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                where a.AttendanceFlag
                                where a.MeetingDate >= dt1
                                select a
                        where !g.Any(aa => aa.MeetingDate < dt2)
                        where g.Count(aa => aa.MeetingDate > dt2) > cnt
                        select p.PeopleId;
                    break;
                case CompareType.GreaterEqual:
                    q = from p in Db.People
                        let g = from a in p.Attends
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                where a.AttendanceFlag
                                where a.MeetingDate >= dt1
                                select a
                        where !g.Any(aa => aa.MeetingDate < dt2)
                        where g.Count(aa => aa.MeetingDate > dt2) >= cnt
                        select p.PeopleId;
                    break;
                case CompareType.Less:
                    q = from p in Db.People
                        let g = from a in p.Attends
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                where a.AttendanceFlag
                                where a.MeetingDate >= dt1
                                select a
                        where !g.Any(aa => aa.MeetingDate < dt2)
                        where g.Count(aa => aa.MeetingDate > dt2) < cnt
                        select p.PeopleId;
                    break;
                case CompareType.LessEqual:
                    q = from p in Db.People
                        let g = from a in p.Attends
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                where a.AttendanceFlag
                                where a.MeetingDate >= dt1
                                select a
                        where !g.Any(aa => aa.MeetingDate < dt2)
                        where g.Count(aa => aa.MeetingDate > dt2) <= cnt
                        select p.PeopleId;
                    break;
                case CompareType.Equal:
                    q = from p in Db.People
                        let g = from a in p.Attends
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                where a.AttendanceFlag
                                where a.MeetingDate >= dt1
                                select a
                        where !g.Any(aa => aa.MeetingDate < dt2)
                        where g.Count(aa => aa.MeetingDate > dt2) == cnt
                        select p.PeopleId;
                    break;
                case CompareType.NotEqual:
                    q = from p in Db.People
                        let g = from a in p.Attends
                                where org == 0 || a.Meeting.OrganizationId == org
                                where divid == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                                where progid == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                                where a.AttendanceFlag
                                where a.MeetingDate >= dt1
                                select a
                        where !g.Any(aa => aa.MeetingDate < dt2)
                        where g.Count(aa => aa.MeetingDate > dt2) != cnt
                        select p.PeopleId;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression KidsRecentAttendCount(
            ParameterExpression parm,
            int days,
            CompareType op,
            int cnt)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, int>> pred = p =>
                p.Family.People.Any(k => k.PositionInFamilyId == 30 && k.Attends.Any())
                    ? p.Family.People.Where(k => k.PositionInFamilyId == 30).Max(k =>
                        k.Attends.Count(a => a.AttendanceFlag && a.MeetingDate >= mindt))
                    : 0;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression AttendPct(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            decimal pct)
        {
            Expression<Func<Person, decimal>> pred = p => (
                from om in p.OrganizationMembers
                where org == 0 || om.OrganizationId == org
                where divid == 0 || om.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || om.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select om
                ).Average(om => om.AttendPct).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(pct), left.Type);
            return Compare(left, op, right);
        }

        internal static Expression RecentAttendType(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            int orgtype,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p => (
                from a in p.Attends
                where a.MeetingDate >= mindt
                where a.AttendanceFlag || (ids.Length == 1 && ids[0] == AttendTypeCode.Offsite)
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
    }
}
