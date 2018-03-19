/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression RecentAttendMemberType()
        {
            var q = db.RecentAttendMemberType(ProgramInt, DivisionInt, OrganizationInt, Days, string.Join(",", CodeIntIds));
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId.Value));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentAttendCount()
        {
            var cnt = TextValue.ToInt();
            var q = db.RecentAttendInDaysByCountDesc(ProgramInt, DivisionInt, OrganizationInt, OrgTypeInt ?? 0, Days, Quarters);
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
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentAttendCountAttCred()
        {
            var attcred = Quarters.ToInt();
            var cnt = TextValue.ToInt();
            var mindt = Util.Now.AddDays(-Days).Date;
            var sc = db.OrgSchedules.FirstOrDefault(cc => cc.ScheduleId == ScheduleInt);
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
                        where OrganizationInt == 0 || a.Meeting.OrganizationId == OrganizationInt
                        where DivisionInt == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                        where ProgramInt == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
                        where OrganizationInt == 0 || a.Meeting.OrganizationId == OrganizationInt
                        where DivisionInt == 0 || a.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                        where ProgramInt == 0 || a.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                        select a
                     ).Count();

            }
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression VisitNumber()
        {
            var n = Quarters.ToInt2() ?? 1;
            var cdt = db.Setting("DbConvertedDate", "1/1/1900").ToDate();

            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Greater:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate > DateValue);
                    break;
                case CompareType.Less:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate < DateValue);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate >= DateValue);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate <= DateValue);
                    break;
                case CompareType.Equal:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate.Date == DateValue);
                    break;
                case CompareType.NotEqual:
                case CompareType.IsNull:
                    pred = p => p.CreatedDate > cdt &&
                           !p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate.Date == DateValue);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.CreatedDate > cdt &&
                           p.Attends.Any(aa => aa.SeqNo == n);
                    break;
            }
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression CheckInByDate()
        {
            var cnt = TextValue.ToInt();
            Expression<Func<Person, int>> pred = null;
            var end = EndDate;
            if (end != null) end = end.Value.AddDays(1).AddTicks(-1);
            else end = Util.Now.Date.AddDays(1).AddTicks(-1);

            pred = p => p.CheckInTimes.Count(ct => ct.CheckInTimeX >= StartDate && ct.CheckInTimeX <= end);

            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression CheckInByActivity()
        {
            Expression<Func<Person, bool>> pred = p => p.CheckInTimes.Any(e => e.CheckInActivities.Any(a => CodeStrIds.Contains(a.Activity)));
            Expression expr = Expression.Invoke(pred, parm);

            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);

            return expr;
        }
        internal Expression RecentVisitNumber()
        {
            var tf = CodeIds == "1";
            int n = Quarters.ToInt2() ?? 1;
            var dt = Util.Now.AddDays(-Days);
            var cdt = db.Setting("DbConvertedDate", "1/1/1900").ToDate();
            Expression<Func<Person, bool>> pred = p => p.CreatedDate > cdt &&
                p.Attends.Any(aa => aa.SeqNo == n && aa.MeetingDate > dt);
            Expression expr = Expression.Invoke(pred, parm);

            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression HasRecentNewAttend()
        {
            var tf = CodeIds == "1";
            var days0 = Quarters.ToInt2();
            var tag = db.NewTemporaryTag();
            db.TagRecentStartAttend(ProgramInt ?? 0, DivisionInt ?? 0, OrganizationInt, OrgTypeInt ?? 0, days0 ?? 365, Days, tag.Id);
            Expression<Func<Person, bool>> pred = null;
            if (op == CompareType.Equal ^ tf)
                pred = p => p.Tags.All(t => t.Id != tag.Id);
            else
                pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression KidsRecentAttendCount()
        {
            var cnt = TextValue.ToInt();
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, int>> pred = p =>
                p.Family.People.Any(k => k.PositionInFamilyId == 30 && k.Attends.Any())
                    ? p.Family.People.Where(k => k.PositionInFamilyId == 30).Max(k =>
                        k.Attends.Count(a => a.AttendanceFlag && a.MeetingDate >= mindt))
                    : 0;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression AttendPct()
        {
            Expression<Func<Person, decimal>> pred = p => (
                from om in p.OrganizationMembers
                where OrganizationInt == 0 || om.OrganizationId == OrganizationInt
                where DivisionInt == 0 || om.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || om.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                where om.AttendPct > 0
                select om
                ).Average(om => om.AttendPct).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(decimal.Parse(TextValue)), left.Type);
            return Compare(left, right);
        }
        internal Expression RecentAttendType()
        {
            var q = db.RecentAttendType(ProgramInt, DivisionInt, OrganizationInt, Days, string.Join(",", CodeIntIds));
            Expression<Func<Person, bool>> pred = p => q.Select(c => c.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression NeedAttendance()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            var dow = Quarters.ToInt2();
            Expression<Func<Person, bool>> pred = p => (
                from m in p.OrganizationMembers
                where m.Organization.OrganizationStatusId == OrgStatusCode.Active
                let sc = m.Organization.OrgSchedules.FirstOrDefault()
                where sc != null
                where !dow.HasValue || sc.SchedDay == dow
                where CodeIntIds.Contains(m.MemberTypeId)
                where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                where !m.Organization.Meetings.Any(mm => mm.MeetingDate > mindt && (mm.HeadCount > 0 || mm.Attends.Any(aa => aa.AttendanceFlag)))
                select m
                ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentFirstFamilyVisit()
        {
            var tf = CodeIds == "1";
            var cdt = db.Setting("DbConvertedDate", "1/1/1900").ToDate();

            var mindt = Util.Now.AddDays(-Days).Date;

            var q = from fm in db.ViewFamilyFirstTimes
                    where fm.CreatedDate > cdt
                    where fm.FirstDate > mindt
                    select fm.FamilyId;

            Expression<Func<Person, bool>> pred = p => q.Contains(p.FamilyId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentFamilyAdultLastAttend()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            var tf = CodeIds == "1";
            var q = from m in db.LastFamilyOrgAttends(ProgramInt, DivisionInt, OrganizationInt, Codes.PositionInFamily.PrimaryAdult)
                where m.Lastattend > mindt
                select m.PeopleId;
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
