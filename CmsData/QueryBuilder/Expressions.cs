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
    internal static class Expressions
    {
        internal static Expression MatchAnything(
            ParameterExpression parm)
        {
            Expression<Func<Person, bool>> pred = p => true;
            return Expression.Invoke(pred, parm);
        }
        internal static Expression BackgroundCheckStatus(ParameterExpression parm,
            string labels,
            string usernameOrPeopleId,
            CompareType op,
            int[] ids)
        {
            int[] lab = (labels ?? "99").Split(',').Select(vv => vv.ToInt()).ToArray();
            var pid = usernameOrPeopleId.ToInt();
            var user = "";
            if (usernameOrPeopleId.HasValue())
                if (pid == 0)
                    user = usernameOrPeopleId ?? "";

            Expression<Func<Person, bool>> pred = p =>
              p.BackgroundChecks.Any(bc =>
                  ids.Contains(bc.StatusID)
                  && (user == "" || bc.User.Users.Any(uu => uu.Username == user))
                  && (pid == 0 || bc.UserID == pid)
                  && (lab.Contains(bc.ReportLabelID) || lab.Length == 0 || lab[0] == 99)
                  );
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression MemberTypeIds(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int org,
            int orgtype,
            int sched,
            int campus,
            CompareType op,
            params int[] ids)
        {
            Expression<Func<Person, bool>> pred;
            if (sched == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where ids.Contains(m.MemberTypeId)
                    where (campus == 0
                            || m.Organization.CampusId == campus
                            || (campus == -1 && m.Organization.CampusId == null)
                            )
                    where orgtype == 0 || m.Organization.OrganizationTypeId == orgtype
                    where org == 0 || m.OrganizationId == org
                    where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Any();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where ids.Contains(m.MemberTypeId)
                    where (campus == 0
                            || m.Organization.CampusId == campus
                            || (campus == -1 && m.Organization.CampusId == null)
                            )
                    where orgtype == 0 || m.Organization.OrganizationTypeId == orgtype
                    where org == 0 || m.OrganizationId == org
                    where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                    where sched == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == sched)
                    select m
                    ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression StatusFlag(
            ParameterExpression parm,
            CompareType op,
            params string[] codes)
        {
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(tt => codes.Contains(tt.Tag.Name) && tt.Tag.TypeId == 100);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression MembOfOrgWithSched(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int org,
            CompareType op,
            params int[] ids)
        {
            Expression<Func<Person, bool>> pred = p => (
                    from m in p.OrganizationMembers
                    where org == 0 || m.OrganizationId == org
                    where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                    where m.Organization.OrgSchedules.Any(os => ids.Contains(os.ScheduleId ?? 0))
                    select m
                    ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression MembOfOrgWithCampus(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int org,
            int orgtype,
            CompareType op,
            params int[] ids)
        {
            Expression<Func<Person, bool>> pred = p => (
                from m in p.OrganizationMembers
                where orgtype == 0 || m.Organization.OrganizationTypeId == orgtype
                where org == 0 || m.OrganizationId == org
                where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                where ids.Contains(m.Organization.CampusId ?? 0)
                    || (ids[0] == -1 && m.Organization.CampusId == null)
                select m
                ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression WasMemberAsOf(
            ParameterExpression parm,
            DateTime? startdt,
            DateTime? enddt,
            int? progid,
            int? divid,
            int? orgid,
            int orgtype,
            CompareType op,
            bool tf)
        {
            var now = DateTime.Now;
            enddt = enddt.HasValue ? enddt.Value.AddDays(1) : startdt.Value.AddDays(1);
            Expression<Func<Person, bool>> pred = p => (
                from et in p.EnrollmentTransactions
                where et.TransactionTypeId <= 3 // things that start a change
                where et.TransactionStatus == false
                where et.TransactionDate <= enddt // transaction starts <= looked for end
                where (et.Pending ?? false) == false
                where (et.NextTranChangeDate ?? now) >= startdt // transaction ends >= looked for start
                where orgtype == 0 || et.Organization.OrganizationTypeId == orgtype
                where orgid == 0 || et.OrganizationId == orgid
                where divid == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select et
                ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression MemberTypeAsOf(
            ParameterExpression parm,
            DateTime? frdt,
            DateTime? todt,
            int? progid,
            int? divid,
            int? org,
            int orgtype,
            CompareType op,
            params int[] ids)
        {
            todt = todt.HasValue ? todt.Value.AddDays(1) : frdt.Value.AddDays(1);

            Expression<Func<Person, bool>> pred = p => (
                from et in p.EnrollmentTransactions
                where et.TransactionTypeId <= 3 // things that start a change
                where et.TransactionStatus == false
                where frdt <= (et.NextTranChangeDate ?? Util.Now) // where it ends
                where et.TransactionDate <= todt // where it begins
                where (et.Pending ?? false) == false
                where ids.Contains(et.MemberTypeId)  // what it's type was during that time
                where orgtype == 0 || et.Organization.OrganizationTypeId == orgtype
                where org == 0 || et.OrganizationId == org
                where divid == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select et
                ).Any();
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
        internal static Expression RecentJoinChurch(
            ParameterExpression parm,
            int days,
            CompareType op,
            bool tf)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.JoinDate > mindt;
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual
                || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;

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
        internal static Expression RecentRegistrationType(
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
                from m in p.EnrollmentTransactions
                where m.TransactionTypeId == 1
                where m.EnrollmentDate >= mindt
                where ids[0] == 99 || ids.Contains(m.Organization.RegistrationTypeId ?? 0)
                where orgtype == 0 || m.Organization.OrganizationTypeId == orgtype
                where org == 0 || m.OrganizationId == org
                where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                select m
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression PeopleIds(
            ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = p => ids.Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasTaskWithName(
            ParameterExpression parm,
            CompareType op,
            string task)
        {
            if (task == null)
                task = "";
            Expression<Func<Person, bool>> pred = p =>
                p.TasksAboutPerson.Any(t => t.Description.Contains(task)
                    && t.StatusId != TaskStatusCode.Complete);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasIncompleteTask(
            ParameterExpression parm,
            CompareType op,
            string task)
        {
            var empty = !task.HasValue();
            Expression<Func<Person, bool>> pred = p => (
                from t in p.TasksCoOwned
                where empty || t.Description.Contains(task) || t.Owner.Name.Contains(task)
                where t.StatusId != TaskStatusCode.Complete
                select t).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression MadeContactTypeAsOf(
            ParameterExpression parm,
            DateTime? from,
            DateTime? to,
            int? ministryid,
            CompareType op,
            params int[] ids)
        {
            to = to.HasValue ? to.Value.AddDays(1) : from.Value.AddDays(1);

            Expression<Func<Person, bool>> pred = p => (
                from c in p.contactsMade
                where c.contact.MinistryId == ministryid || ministryid == 0
                where ids.Contains(c.contact.ContactTypeId ?? 0) || ids.Length == 0 || ids[0] == 0
                where @from <= c.contact.ContactDate // where it ends
                where c.contact.ContactDate <= to // where it begins
                select c
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentContactType(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && ids.Contains(a.contact.ContactTypeId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentContactReason(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && ids.Contains(a.contact.ContactReasonId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression RecentContactMinistry(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && ids.Contains(a.contact.MinistryId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression MeetingId(
            ParameterExpression parm,
            CompareType op,
            int id)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Attends.Any(a =>
                    (a.AttendanceFlag == true)
                    && a.MeetingId == id
                    );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RegisteredForMeetingId(
            ParameterExpression parm,
            CompareType op,
            int id)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Attends.Any(a =>
                    (a.Commitment == AttendCommitmentCode.Attending
                    || a.Commitment == AttendCommitmentCode.Substitute
                    || a.Commitment == AttendCommitmentCode.FindSub)
                    && a.MeetingId == id
                    );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression PeopleExtra(
            ParameterExpression parm,
            CompareType op,
            string[] values)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.PeopleExtras.Any(e =>
                    values.Contains(e.FieldValue));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasPeopleExtraField(
            ParameterExpression parm,
            CompareType op,
            string field)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.PeopleExtras.Any(e =>
                    e.Field == field);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression PeopleExtraData(
            ParameterExpression parm,
            string field,
            CompareType op,
            string value)
        {
            Expression<Func<Person, string>> pred = p =>
                p.PeopleExtras.Where(ff => ff.Field == field).Select(ff => ff.Data).SingleOrDefault();
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(value, typeof(string));
            return Compare(left, op, right);
        }
        internal static Expression PeopleExtraInt(
            ParameterExpression parm,
            string field,
            CompareType op,
            int? value)
        {
            if (!value.HasValue)
            {
                Expression<Func<Person, bool>> predint = null;
                switch (op)
                {
                    case CompareType.Equal:
                        predint = p => !p.PeopleExtras.Any(e => e.Field == field)
                                    || p.PeopleExtras.SingleOrDefault(e => e.Field == field).IntValue == null;
                        return Expression.Invoke(predint, parm);
                    case CompareType.NotEqual:
                        predint = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).IntValue != null;
                        return Expression.Invoke(predint, parm);
                    default:
                        return AlwaysFalse(parm);
                }
            }

            Expression<Func<Person, int>> pred = p =>
                p.PeopleExtras.Single(e =>
                    e.Field == field).IntValue ?? 0;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression PeopleExtraDate(
            ParameterExpression parm,
            string field,
            CompareType op,
            DateTime? value)
        {
            if (!value.HasValue)
            {
                Expression<Func<Person, bool>> pred = null;
                switch (op)
                {
                    case CompareType.Equal:
                        pred = p => !p.PeopleExtras.Any(e => e.Field == field)
                              || p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue == null;
                        return Expression.Invoke(pred, parm);
                    case CompareType.NotEqual:
                        pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue != null;
                        return Expression.Invoke(pred, parm);
                    default:
                        return AlwaysFalse(parm);
                }
            }
            else
            {
                Expression<Func<Person, DateTime>> pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue.Value;
                Expression left = Expression.Invoke(pred, parm);
                var right = Expression.Convert(Expression.Constant(value), left.Type);
                return Compare(left, op, right);
            }
        }
        internal static Expression HasEmailOptout(
            ParameterExpression parm,
            DateTime? to,
            CompareType op,
            string email)
        {
            Expression<Func<Person, bool>> pred = p =>
                (from oo in p.EmailOptOuts
                 where email == null || email == "" || oo.FromEmail == email
                 where @to == null || (oo.DateX >= @to)
                 select oo).Any();
            Expression expr = Expression.Invoke(pred, parm);
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
        internal static Expression RecentVisitNumber(
            ParameterExpression parm, CMSDataContext Db,
            string number,
            int days,
            CompareType op,
            bool tf)
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
                p.Family.People.Where(k => k.PositionInFamilyId == 30).Max(k =>
                    k.Attends.Count(a => a.AttendanceFlag == true && a.MeetingDate >= mindt));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression RecentDecisionType(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.DecisionDate > mindt
                && ids.Contains(p.DecisionTypeId.Value);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression EmailRecipient(
            ParameterExpression parm,
            CompareType op,
            int id)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.EmailQueueTos.Any(e => e.Id == id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentEmailCount(
            ParameterExpression parm,
            int days,
            CompareType op,
            int cnt)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, int>> pred = p =>
                p.EmailQueueTos.Count(e => e.Sent >= mindt);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression AlwaysFalse(ParameterExpression parm)
        {
            Expression<Func<Person, bool>> pred = p => false;
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }

        internal static Expression RecentPledgeCount(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            int cnt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);

            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() > cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() >= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() < cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() <= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (cnt == 0) // special case, use different approach
                    {
                        q = from pid in Db.Pledges0(dt, now, fund, 0)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() == cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() != cnt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentPledgeAmount(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            decimal amt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) < amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentFirstTimeGiver(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            bool tf)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);

            var q = from f in Db.FirstTimeGivers(days, fund)
                    select f.PeopleId;

            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentHasIndContributions(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            CompareType op,
            bool tf)
        {
            if(!Db.FromActiveRecords)
                if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            Expression<Func<Person, bool>> pred = p =>
                           p.Contributions.Any(cc => cc.ContributionDate > dt && cc.ContributionAmount > 0 && !ContributionTypeCode.ReturnedReversedTypes.Contains(cc.ContributionTypeId));
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentContributionCount(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            int cnt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() > cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0 || cnt == 0
                        group c by c.CreditGiverId into g
                        where g.Count() >= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() < cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() <= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (cnt == 0) // This is a very special case, use different approach
                    {
                        q = from pid in Db.Contributions0(dt, now, fund, 0)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() == cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() != cnt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentContributionAmount(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            decimal amt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression ContributionAmount2(
            ParameterExpression parm, CMSDataContext Db,
            DateTime? start,
            DateTime? end,
            int fund,
            CompareType op,
            decimal amt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(start, end, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Greater:
                    q = from c in Db.Contributions2(start, end, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(start, end, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(start, end, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) < amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    q = from c in Db.Contributions2(start, end, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(start, end, 0, false, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression ContributionChange(
            ParameterExpression parm, CMSDataContext Db,
            DateTime? dt1,
            DateTime? dt2,
            CompareType op,
            double pct)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var q = Db.GivingCurrentPercentOfFormer(dt1, dt2,
                op == CompareType.Greater ? ">" :
                op == CompareType.GreaterEqual ? ">=" :
                op == CompareType.Less ? "<" :
                op == CompareType.LessEqual ? "<=" :
                op == CompareType.Equal ? "=" : "<>", pct);
            var tag = Db.PopulateTemporaryTag(q.Select(pp => pp.Pid));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentGivingAsPctOfPrevious(
            ParameterExpression parm, CMSDataContext Db,
            int Days,
            CompareType op,
            double pct)
        {
            var dt1 = DateTime.Today.AddDays(-Days * 2);
            var dt2 = DateTime.Today.AddDays(-Days);
            var q = Db.GivingCurrentPercentOfFormer(dt1, dt2, op == CompareType.Greater ? ">" : "<=", pct);
            var tag = Db.PopulateTemporaryTag(q.Select(pp => pp.Pid));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression IsTopGiver(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            string top,
            CompareType op,
            bool tf)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return Expressions.CompareConstant(parm, "PeopleId", CompareType.Equal, 0);

            var mindt = Util.Now.AddDays(-days).Date;
            var r = Db.TopGivers(top.ToInt(), mindt, DateTime.Now).ToList();
            var topgivers = r.Select(g => g.PeopleId).ToList();
            Expression<Func<Person, bool>> pred = p =>
                topgivers.Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsTopPledger(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            string top,
            CompareType op,
            bool tf)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return Expressions.CompareConstant(parm, "PeopleId", CompareType.Equal, 0);

            var mindt = Util.Now.AddDays(-days).Date;
            var r = Db.TopPledgers(top.ToInt(), mindt, DateTime.Now).ToList();
            var toppledgers = r.Select(g => g.PeopleId).ToList();
            Expression<Func<Person, bool>> pred = p =>
                toppledgers.Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression NumberOfFamilyMembers(
            ParameterExpression parm,
            CompareType op,
            int cnt)
        {
            Expression<Func<Person, int>> pred = p => p.Family.People.Count();
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression NumberOfMemberships(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            int? sched,
            CompareType op,
            int cnt)
        {
            Expression<Func<Person, int>> pred;
            if (sched == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where org == 0 || m.OrganizationId == org
                    where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                    where (m.Pending ?? false) == false
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Count();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where org == 0 || m.OrganizationId == org
                    where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                    where (m.Pending ?? false) == false
                    where sched == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == sched)
                    select m
                    ).Count();
            var left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression VisitedCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            var mindt = Util.Now.AddDays(-Db.VisitLookbackDays).Date;
            var ids = new int[] 
            { 
                AttendTypeCode.NewVisitor, 
                AttendTypeCode.RecentVisitor, 
                AttendTypeCode.VisitingMember 
            };
            Expression<Func<Person, bool>> pred = p => (
                    from a in p.Attends
                    where a.AttendanceFlag
                    where a.MeetingDate >= mindt
                    let dt = a.Meeting.Organization.FirstMeetingDate
                    where dt == null || a.MeetingDate >= dt
                    where a.MeetingDate > Db.LastDrop(a.OrganizationId, a.PeopleId)
                    where ids.Contains(a.AttendanceTypeId.Value)
                    where a.OrganizationId == Db.CurrentOrgId
                    select a
                ).Any() && !(
                    from m in p.OrganizationMembers
                    where m.OrganizationId == Db.CurrentOrgId
                    where (m.Pending ?? false) == false
                    select m
                ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
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

        internal static Expression CreatedBy(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            string name)
        {
            Expression<Func<Person, string>> pred = p =>
                Db.People.FirstOrDefault(u => u.Users.Any(u2 => u2.UserId == p.CreatedBy)).Name2;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(name, typeof(string));
            return Compare(left, op, right);
        }
        internal static Expression SmallGroup(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            string name)
        {
            if (name.HasValue())
            {
                Expression<Func<Person, bool>> pred = null;
                switch (op)
                {
                    case CompareType.Equal:
                    case CompareType.NotEqual:
                        pred = p => (
                            from m in p.OrganizationMembers
                            where m.OrgMemMemTags.Any(mt => mt.MemberTag.Name == name)
                            where org == 0 || m.OrganizationId == org
                            where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                            where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                            select m
                        ).Any();
                        break;
                    case CompareType.StartsWith:
                        pred = p => (
                            from m in p.OrganizationMembers
                            where m.OrgMemMemTags.Any(mt => mt.MemberTag.Name.StartsWith(name))
                            where org == 0 || m.OrganizationId == org
                            where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                            where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                            select m
                        ).Any();
                        break;
                }
                var expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
                if (op == CompareType.NotEqual)
                    expr = Expression.Not(expr);
                return expr;
            }
            else
            {
                Expression<Func<Person, bool>> pred = p => (
                            from m in p.OrganizationMembers
                            where !m.OrgMemMemTags.Any()
                            where org == 0 || m.OrganizationId == org
                            where divid == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == divid)
                            where progid == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == progid))
                            select m
                        ).Any();
                var expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
                if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                    expr = Expression.Not(expr);
                return expr;
            }
        }

        internal static Expression OrgJoinDateCompare(
           ParameterExpression parm,
           int? progid,
           int? divid,
           int? org,
           CompareType op,
           string prop2)
        {
            Expression<Func<Person, DateTime?>> pred = p =>
                p.OrganizationMembers.Where(m =>
                    (org == 0 || m.OrganizationId == org)
                    && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                    && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                    ).Min(m => m.EnrollmentDate);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Property(parm, prop2);
            return Compare(left, op, right);
        }

        internal static Expression OrgJoinDate(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            DateTime? date)
        {
            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Equal:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate.Value.Date == date);
                    break;
                case CompareType.Greater:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate.Value.Date > date);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate.Value.Date >= date);
                    break;
                case CompareType.Less:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate.Value.Date < date);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate.Value.Date <= date);
                    break;
                case CompareType.NotEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate.Value.Date != date);
                    break;
                case CompareType.IsNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate == null);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.EnrollmentDate != null);
                    break;
            }
            return Expression.Invoke(pred, parm);
        }
        internal static Expression OrgJoinDateDaysAgo(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(p.OrganizationMembers.Where(m =>
                    (org == 0 || m.OrganizationId == org)
                    && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                    && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                    ).Min(m => m.EnrollmentDate), Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression RecentCreated(
            ParameterExpression parm,
            int days,
            CompareType op,
            bool tf)
        {
            var dt = DateTime.Today.AddDays(-days);
            Expression<Func<Person, bool>> pred = p => p.CreatedDate >= dt;
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression Birthday(
            ParameterExpression parm,
            CompareType op,
            string dob)
        {
            Expression<Func<Person, bool>> pred = p => false; // default
            DateTime dt;
            if (DateTime.TryParse(dob, out dt))
                if (Regex.IsMatch(dob, @"\d+/\d+/\d+"))
                    pred = p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month && p.BirthYear == dt.Year;
                else
                    pred = p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month;
            else
            {
                int y;
                if (int.TryParse(dob, out y))
                    if (y <= 12 && y > 0)
                        pred = p => p.BirthMonth == y;
                    else
                        pred = p => p.BirthYear == y;
            }
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression WeddingDate(
            ParameterExpression parm,
            CompareType op,
            string wed)
        {
            Expression<Func<Person, bool>> pred = p => false; // default
            DateTime dt;
            if (DateTime.TryParse(wed, out dt))
                if (Regex.IsMatch(wed, @"\d+/\d+/\d+"))
                    pred = p => p.WeddingDate == dt;
                else
                    pred = p => p.WeddingDate.Value.Day == dt.Day && p.WeddingDate.Value.Month == dt.Month;
            else
            {
                int y;
                if (int.TryParse(wed, out y))
                    if (y <= 12 && y > 0)
                        pred = p => p.WeddingDate.Value.Month == y;
                    else
                        pred = p => p.WeddingDate.Value.Year == y;
            }
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression OrgInactiveDate(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            DateTime? date)
        {
            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Equal:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate.Value.Date == date);
                    break;
                case CompareType.Greater:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate.Value.Date > date);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate.Value.Date >= date);
                    break;
                case CompareType.Less:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate.Value.Date < date);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate.Value.Date <= date);
                    break;
                case CompareType.NotEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate.Value.Date != date);
                    break;
                case CompareType.IsNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate == null);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (org == 0 || m.OrganizationId == org)
                        && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                        && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                        && m.InactiveDate != null);
                    break;
            }
            return Expression.Invoke(pred, parm);
        }
        internal static Expression WidowedDate(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            DateTime? date)
        {
            Expression<Func<Person, DateTime?>> pred = p =>
                Db.WidowedDate(p.PeopleId);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(date, typeof(DateTime?));
            return Compare(left, op, right);
        }
        internal static Expression DaysTillBirthday(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(Util.Now.Date, Db.NextBirthday(p.PeopleId));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression DaysTillAnniversary(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(Util.Now.Date, Db.NextAnniversary(p.PeopleId));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression DaysSinceContact(
            ParameterExpression parm,
            CompareType op,
            int days)
        {
            Expression<Func<Person, bool>> hadcontact = p => p.contactsHad.Count() > 0;
            var dt = Util.Now.Date;
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(p.contactsHad.Max(cc => cc.contact.ContactDate), dt);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            var expr1 = Expression.Invoke(hadcontact, parm);
            return Expression.And(expr1, Compare(left, op, right));
        }
        internal static Expression HasContacts(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p => p.contactsHad.Count() > 0;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
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

        internal static Expression IsMemberOf(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId != MemberTypeCode.InActive
                    && (m.Pending ?? false) == false
                    && (org == 0 || m.OrganizationId == org)
                    && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                    && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsInactiveMemberOf(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId == MemberTypeCode.InActive
                    && (org == 0 || m.OrganizationId == org)
                    && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                    && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsPendingMemberOf(
            ParameterExpression parm,
            int? progid,
            int? divid,
            int? org,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    (m.Pending ?? false) == true
                    && (org == 0 || m.OrganizationId == org)
                    && (divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid))
                    && (progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression InBFClass(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.Organization.IsBibleFellowshipOrg == true
                    && (m.Pending ?? false) == false);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression DuplicateEmails(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.EmailAddress != null && p.EmailAddress != ""
                    && Db.People.Any(pp => pp.PeopleId != p.PeopleId && pp.EmailAddress == p.EmailAddress);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression DuplicateNames(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    Db.People.Any(pp => pp.PeopleId != p.PeopleId && pp.FirstName == p.FirstName && pp.LastName == p.LastName);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecActiveOtherChurch(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.ActiveInAnotherChurch == true)
                    && p.RecRegs.Any();
            Expression expr1 = Expression.Convert(Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr2 = Expression.Not(expr2);
            return Expression.And(expr1, expr2);
        }
        internal static Expression RecInterestedCoaching(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.Coaching == true)
                    && p.RecRegs.Any();
            Expression expr1 = Expression.Convert(Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr2 = Expression.Not(expr2);
            return Expression.And(expr1, expr2);
        }
        internal static Expression MedicalLength(
            ParameterExpression parm,
            CompareType op,
            int len)
        {
            if (len == 0 && op == CompareType.Equal)
            {
                Expression<Func<Person, bool>> pp = p =>
                    !p.RecRegs.Any() || p.RecRegs.First().MedicalDescription == null ||
                    p.RecRegs.First().MedicalDescription.Length == 0;
                return Expression.Invoke(pp, parm);
            }
            Expression<Func<Person, int>> pred = p => p.RecRegs.Sum(rr => rr.MedicalDescription.Length);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(len), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression HasParents(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.PositionInFamilyId == 10 && p.PositionInFamilyId == 30);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildren(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Age <= 12);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildrenAged(
            ParameterExpression parm,
            int age,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Age <= age);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildrenAged2(
            ParameterExpression parm,
            string range,
            CompareType op,
            bool tf)
        {
            var a = range.Split('-');
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Age >= a[0].ToInt() && m.Age <= a[1].ToInt());
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildrenAged3(
            ParameterExpression parm,
            string range,
            CompareType op,
            int[] ids)
        {
            var a = range.Split('-');
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m =>
                    m.Age >= a[0].ToInt()
                    && m.Age <= a[1].ToInt()
                    && ids.Contains(m.GenderId)
                );
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasRelatedFamily(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.RelatedFamilies1.Any()
                || p.Family.RelatedFamilies2.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsHeadOfHousehold(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.HeadOfHouseholdId == p.PeopleId;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasLowerName(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    Db.StartsLower(p.FirstName).Value
                    || Db.StartsLower(p.LastName).Value;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsUser(
           ParameterExpression parm,
           CompareType op,
           bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.Users.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasPicture(ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p => p.PictureId != null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasCurrentTag(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.Tags.Any(t => t.Tag.Name == Db.CurrentTagName && t.Tag.PeopleId == Db.CurrentTagOwnerId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasBalanceInCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            var cg = Db.CurrentGroups.ToArray();
            var cgmode = Db.CurrentGroupsMode;
            Expression<Func<Person, bool>> pred = p => (
                from m in p.OrganizationMembers
                where m.OrganizationId == Db.CurrentOrgId
                let gc = m.OrgMemMemTags.Count(mt => cg.Contains(mt.MemberTagId))
                // for Match Any
                where gc > 0 || cg[0] <= 0 || cgmode == 1
                // for Match All
                where gc == cg.Length || cg[0] <= 0 || cgmode == 0
                // for Match No SmallGroup assigned
                where !m.OrgMemMemTags.Any() || cg[0] != -1
                where m.MemberTypeId != MemberTypeCode.InActive
                where (m.Pending ?? false) == false
                where (from t in Db.Transactions
                       where t.OriginalTransaction.TransactionPeople.Any(pp => pp.PeopleId == p.PeopleId)
                       where t.OriginalTransaction.OrgId == Db.CurrentOrgId
                       orderby t.Id descending
                       select t.Amtdue).FirstOrDefault() > 0
                select m).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression InCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            var cg = Db.CurrentGroups.ToArray();
            var cgmode = Db.CurrentGroupsMode;
            Expression<Func<Person, bool>> pred = p => (
                    from m in p.OrganizationMembers
                    where m.OrganizationId == Db.CurrentOrgId
                    let gc = m.OrgMemMemTags.Count(mt => cg.Contains(mt.MemberTagId))
                    // for Match Any
                    where gc > 0 || cg[0] <= 0 || cgmode == 1
                    // for Match All
                    where gc == cg.Length || cg[0] <= 0 || cgmode == 0
                    // for Match No SmallGroup assigned
                    where !m.OrgMemMemTags.Any() || cg[0] != -1
                    where m.MemberTypeId != MemberTypeCode.InActive
                    where (m.Pending ?? false) == false
                    select m
                    ).Any();

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (Db.CurrentGroupsPrefix.HasValue())
            {
                var aa = Db.CurrentGroupsPrefix.Split(',');
                foreach (var sg in aa)
                {
                    Expression<Func<Person, bool>> pred2 = null;
                    if (sg.StartsWith("-"))
                    {
                        var g = sg.Substring(1);
                        pred2 = p => (from om in p.OrganizationMembers
                                      where om.OrganizationId == Db.CurrentOrgId
                                      where om.OrgMemMemTags.All(
                                          mm => !mm.MemberTag.Name.StartsWith(g))
                                      select om).Any();
                    }
                    else
                    {
                        pred2 = p => (from om in p.OrganizationMembers
                                      where om.OrganizationId == Db.CurrentOrgId
                                      where om.OrgMemMemTags.Any(
                                          mm => mm.MemberTag.Name.StartsWith(sg))
                                      select om).Any();
                    }
                    var expr1 = Expression.Invoke(pred2, parm);
                    expr = Expression.And(expr1, expr);
                }
            }

            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression InactiveCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                        m.OrganizationId == Db.CurrentOrgId
                        && m.MemberTypeId == MemberTypeCode.InActive);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression PendingCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                        m.OrganizationId == Db.CurrentOrgId
                        && (m.Pending ?? false) == true);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression PreviousCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.EnrollmentTransactions.Any(m =>
                        m.OrganizationId == Db.CurrentOrgId
                        && m.TransactionTypeId > 3
                        && m.TransactionStatus == false
                        && (m.Pending ?? false) == false)
                    && !p.OrganizationMembers.Any(m =>
                        m.OrganizationId == Db.CurrentOrgId
                        && (m.Pending ?? false) == false);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsCurrentPerson(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.PeopleId == Db.CurrentPeopleId;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasMyTag(ParameterExpression parm,
            string tag,
            CompareType op,
            bool tf)
        {
            var a = tag.Split(';').Select(s => s.Split(',')[0].ToInt()).ToArray();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => a.Contains(t.Id));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        //internal static Expression HasVolunteered(ParameterExpression parm,
        //	string View,
        //	CompareType op,
        //	bool tf)
        //{
        //	Expression<Func<Person, bool>> pred;
        //	if (View == "ns")
        //		pred = p => p.VolInterestInterestCodes.Count() > 0;
        //	else
        //	{
        //		var orgkeys = Person.OrgKeys(View);
        //		pred = p =>
        //			  p.VolInterestInterestCodes.Any(vi => orgkeys.Contains(vi.VolInterestCode.Org));
        //	}
        //	Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        //	if (!(op == CompareType.Equal && tf))
        //		expr = Expression.Not(expr);
        //	return expr;
        //}
        internal static Expression VolunteerApprovalCode(ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred =
                p => p.VoluteerApprovalIds.Any(vid => ids.Contains(vid.ApprovalId))
                     || (!p.VoluteerApprovalIds.Any() && ids.Contains(0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression CampusId(ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = null;
            if (op == CompareType.IsNull)
                pred = p => p.CampusId == null;
            else if (op == CompareType.IsNotNull)
                pred = p => p.CampusId != null;
            else if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                pred = p => !ids.Contains(p.CampusId ?? 0);
            else
                pred = p => ids.Contains(p.CampusId ?? 0);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p

            return expr;
        }
        internal static Expression VolAppStatusCode(ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Volunteers.Any(v => ids.Contains(v.StatusId ?? 0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression VolunteerProcessedDateMonthsAgo(ParameterExpression parm,
            CompareType op,
            int months)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffMonth(p.Volunteers.Max(v => v.ProcessedDate), Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(months, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression HasVolunteerApplications(ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.Volunteers.Any(v => v.VolunteerForms.Any());
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasMemberDocs(ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.MemberDocForms.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasInvalidEmailAddress(CMSDataContext Db, ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                (Db.IsValidEmail(p.EmailAddress ?? "") == false
                || Db.IsValidEmail(p.EmailAddress2 ?? "") == false);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IncludeDeceased(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = null;

            bool include = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
            if (include)
                pred = p => p.DeceasedDate == null || p.DeceasedDate != null;
            else
                pred = p => p.DeceasedDate == null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
        internal static Expression ParentsOf(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = null;

            bool include = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
            pred = p => p.PeopleId > 0;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
        internal static Expression UserRole(
            ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Users.Any(u => u.UserRoles.Any(ur => ids.Contains(ur.RoleId))
                    || (!u.UserRoles.Any() && ids.Contains(0))
                );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamHasPrimAdultChurchMemb(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m =>
                    m.PositionInFamilyId == PositionInFamily.PrimaryAdult
                    && m.MemberStatusId == 10 // church member
                    //&& m.PeopleId != p.PeopleId // someone else in family
                    );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression LeadersUnderCurrentOrg(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            bool tf)
        {
            var oids = Db.GetParentChildOrgIds(Db.CurrentOrgId);
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    p.OrganizationMembers.Any(mm => oids.Contains(mm.OrganizationId) && mm.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression MembersUnderCurrentOrg(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            bool tf)
        {
            var oids = Db.GetParentChildOrgIds(Db.CurrentOrgId);
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    p.OrganizationMembers.Any(mm => oids.Contains(mm.OrganizationId))
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression InOneOfMyOrgs(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            bool tf)
        {
            var uid = Util.UserPeopleId;
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    Db.OrganizationMembers.Any(um =>
                        um.OrganizationId == m.OrganizationId && um.PeopleId == uid)
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression SavedQuery(ParameterExpression parm,
            CMSDataContext Db,
            string QueryIdDesc,
            CompareType op,
            bool tf)
        {
            var a = QueryIdDesc.SplitStr(":", 2);
            var qid = a[0].ToInt();
            var savedquery = Db.QueryBuilderClauses.SingleOrDefault(q =>
                q.QueryId == qid);
            if (savedquery == null)
                return AlwaysFalse(parm);

            var q2 = Db.PeopleQuery(qid).Select(pp => pp.PeopleId);
            var tag = Db.PopulateTemporaryTag(q2);

            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression SavedQueryPlus(ParameterExpression parm,
            CMSDataContext Db,
            string QueryIdDesc,
            CompareType op,
            int[] ids)
        {
            var a = QueryIdDesc.SplitStr(":", 2);
            var savedquery = Db.QueryBuilderClauses.SingleOrDefault(q =>
                q.SavedBy == a[0] && q.Description == a[1]);
            var pred = savedquery.Predicate(Db);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        private static MethodInfo EnumerableContains = null;
        private static Expression CompareContains(ParameterExpression parm,
            string prop,
            CompareType op,
            object a,
            Type arrayType,
            Type itemType)
        {
            var left = Expression.Constant(a, arrayType);
            var right = Expression.Convert(Expression.Property(parm, prop), itemType);
            if (EnumerableContains == null)
                EnumerableContains = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Contains");
            var method = EnumerableContains.MakeGenericMethod(itemType);
            Expression expr = Expression.Call(method, new Expression[] { left, right });
            if (op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression CompareDateConstant(
            ParameterExpression parm,
            string prop,
            CompareType op,
            DateTime? dt)
        {
            if (!dt.HasValue)
            {
                if (op == CompareType.NotEqual)
                    return CompareConstant(parm, prop, CompareType.IsNotNull, null);
                return CompareConstant(parm, prop, CompareType.IsNull, null);
            }

            Expression left = Expression.Property(parm, prop);
            if (dt.Value.Date == dt) // 12:00:00 AM?
            {
                left = Expression.MakeMemberAccess(left, typeof(DateTime?).GetProperty("Value"));
                left = Expression.MakeMemberAccess(left, typeof(DateTime).GetProperty("Date"));
            }
            var right = Expression.Convert(Expression.Constant(dt), typeof(DateTime));
            return Compare(left, op, right);
        }
        internal static Expression CompareIntConstant(
            ParameterExpression parm,
            string prop,
            CompareType op,
            string value)
        {
            var left = Expression.Property(parm, prop);
            int? i = value.ToInt2();
            var right = Expression.Convert(Expression.Constant(i), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression CompareConstant(
            ParameterExpression parm,
            string prop,
            CompareType op,
            object value)
        {
            if (value != null)
                if (value.GetType() == typeof(int[])) // use isarray?
                    return CompareContains(parm, prop, op, value, typeof(int[]), typeof(int));
                else if (value.GetType() == typeof(string[]))
                    return CompareContains(parm, prop, op, value, typeof(string[]), typeof(string));
            var left = Expression.Property(parm, prop);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression CompareCodeConstant(
            ParameterExpression parm,
            string prop,
            CompareType op,
            object value)
        {
            if (value != null)
                if (value.GetType() == typeof(int[])) // use isarray?
                    return CompareContains(parm, prop, op, value, typeof(int[]), typeof(int));
            var left = Expression.Coalesce(Expression.Property(parm, prop), Expression.Constant(0));
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression CompareStringConstant(
            ParameterExpression parm,
            string prop,
            CompareType op,
            object value)
        {
            if (value != null)
                if (value.GetType() == typeof(int[])) // use isarray?
                    return CompareContains(parm, prop, op, value, typeof(int[]), typeof(int));
                else if (value.GetType() == typeof(string[]))
                    return CompareContains(parm, prop, op, value, typeof(string[]), typeof(string));

            var left = Expression.Call(Expression.Coalesce(Expression.Property(parm, prop), Expression.Constant("")), "Trim", null);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, op, right);
        }

        internal static Expression CompareProperty(
            ParameterExpression parm,
            string prop,
            CompareType op,
            string prop2)
        {
            var left = Expression.Property(parm, prop);
            var right = Expression.Property(parm, prop2);
            return Compare(left, op, right);
        }

        private static Expression Compare(
            Expression left,
            CompareType op,
            Expression right)
        {
            Expression expr = null;
            switch (op)
            {
                case CompareType.NotEqual:
                case CompareType.IsNotNull:
                    expr = Expression.NotEqual(left, right);
                    break;
                case CompareType.Equal:
                case CompareType.IsNull:
                    expr = Expression.Equal(left, right);
                    break;
                case CompareType.Greater:
                    expr = Expression.GreaterThan(left, right);
                    break;
                case CompareType.GreaterEqual:
                    expr = Expression.GreaterThanOrEqual(left, right);
                    break;
                case CompareType.Less:
                    expr = Expression.LessThan(left, right);
                    break;
                case CompareType.LessEqual:
                    expr = Expression.LessThanOrEqual(left, right);
                    break;
                case CompareType.DoesNotStartWith:
                case CompareType.StartsWith:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                        new[] { right });
                    break;
                case CompareType.DoesNotEndWith:
                case CompareType.EndsWith:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                        new[] { right });
                    break;
                case CompareType.DoesNotContain:
                case CompareType.Contains:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                        new[] { right });
                    break;
                case CompareType.After:
                case CompareType.AfterOrSame:
                case CompareType.Before:
                case CompareType.BeforeOrSame:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("CompareTo", new[] { typeof(string) }),
                        new[] { right });
                    break;
            }
            switch (op)
            {
                // now reverse the logic if necessary
                case CompareType.DoesNotEndWith:
                case CompareType.DoesNotContain:
                case CompareType.DoesNotStartWith:
                    expr = Expression.Not(expr);
                    break;
                case CompareType.After:
                    expr = Expression.GreaterThan(expr, Expression.Constant(0));
                    break;
                case CompareType.AfterOrSame:
                    expr = Expression.GreaterThanOrEqual(expr, Expression.Constant(0));
                    break;
                case CompareType.Before:
                    expr = Expression.LessThan(expr, Expression.Constant(0));
                    break;
                case CompareType.BeforeOrSame:
                    expr = Expression.LessThanOrEqual(expr, Expression.Constant(0));
                    break;
            }
            return expr;
        }

        internal static Expression CheckInVisits(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            int visits)
        {
            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Greater:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() > visits
                                 select g).Any();
                    break;
                case CompareType.Less:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() < visits
                                 select g).Any();
                    break;
                case CompareType.GreaterEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() >= visits
                                 select g).Any();
                    break;
                case CompareType.LessEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() <= visits
                                 select g).Any();
                    break;
                case CompareType.Equal:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() == visits
                                 select g).Any();
                    break;
                case CompareType.NotEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() != visits
                                 select g).Any();
                    break;
            }

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }

        internal static Expression HasManagedGiving(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p => (from e in p.RecurringAmounts
                                                        where e.Amt > 0
                                                        select e).Any();

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
