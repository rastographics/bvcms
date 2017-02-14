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
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression AttendMemberTypeAsOf()
        {
            var ids = string.Join(",", CodeStrIds);
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            var q = db.AttendMemberTypeAsOf(StartDate, enddt, ProgramInt, DivisionInt, OrganizationInt, ids, null)
                  .Select(a => a.PeopleId ?? 0).Distinct();
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => op == CompareType.NotEqual || op == CompareType.NotOneOf
                ? !db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId)
                : db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId);

            return Expression.Invoke(pred, parm);
        }
        internal Expression AttendanceTypeAsOf()
        {
            var ids = string.Join(",", CodeStrIds);
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            var q = db.AttendanceTypeAsOf(StartDate, enddt, ProgramInt, DivisionInt, OrganizationInt, OrgTypeInt ?? 0, ids)
                      .Select(a => a.PeopleId);

            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => op == CompareType.NotEqual || op == CompareType.NotOneOf
                ? !db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId)
                : db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId);

            return Expression.Invoke(pred, parm);
        }
        internal Expression GuestAttendedAsOf()
        {
            return AttendedAsOf(true);
        }
        internal Expression MemberAttendedAsOf()
        {
            return AttendedAsOf(false);
        }
        private Expression AttendedAsOf(bool guestonly)
        {
            var tf = CodeIds == "1";
            var enddt = EndDate;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(enddt.HasValue && enddt.Value.TimeOfDay.Ticks == 0)
                enddt = enddt.Value.AddHours(24);

            IQueryable<int> q;
            if(op == CompareType.Equal ^ tf)
                q = db.NotAttendedAsOf(ProgramInt, DivisionInt, OrganizationInt, StartDate, enddt, guestonly).Select(p => p.PeopleId);
            else
                q = db.AttendedAsOf(ProgramInt, DivisionInt, OrganizationInt, StartDate, enddt, guestonly).Select(p => p.PeopleId);
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            return Expression.Invoke(pred, parm);
        }
        internal Expression AttendPctHistory()
        {
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            var pct = double.Parse(TextValue);
            // note: this only works for members because visitors do not have att%
            var now = Util.Now;

            var q = from p in db.People
                    let m = from et in p.EnrollmentTransactions
                            where et.TransactionTypeId <= 3 // things that start a change
                            where et.TransactionStatus == false
                            where et.TransactionDate < enddt // transaction starts <= looked for end
                            where (et.Pending ?? false) == false
                            where (et.NextTranChangeDate ?? now) >= StartDate // transaction ends >= looked for start
                            where OrganizationInt == 0 || et.OrganizationId == OrganizationInt
                            where DivisionInt == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                            where ProgramInt == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
                                 where a.MeetingDate < enddt
                                 where OrganizationInt == 0 || a.Meeting.OrganizationId == OrganizationInt
                                 where DivisionInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                                 where ProgramInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
                                 where a.MeetingDate < enddt
                                 where OrganizationInt == 0 || a.Meeting.OrganizationId == OrganizationInt
                                 where DivisionInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                                 where ProgramInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
                                 where a.MeetingDate < enddt
                                 where OrganizationInt == 0 || a.Meeting.OrganizationId == OrganizationInt
                                 where DivisionInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                                 where ProgramInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
                                 where a.MeetingDate < enddt
                                 where OrganizationInt == 0 || a.Meeting.OrganizationId == OrganizationInt
                                 where DivisionInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                                 where ProgramInt == 0 || a.Meeting.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            var cnt = TextValue.ToInt();

            var q = db.AttendCntHistory(ProgramInt, DivisionInt, OrganizationInt, ScheduleInt, StartDate, enddt);
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
        internal Expression DaysBetween12Attendance()
        {
            Expression<Func<Person, int>> pred = p =>
                    db.DaysBetween12Attend(p.PeopleId, ProgramInt, DivisionInt, OrganizationInt, Days).Value;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(TextValue.ToInt()), left.Type);
            return Compare(left, right);
        }
        internal Expression DaysAfterNthVisitAsOf()
        {
            var tf = CodeIds == "1";
            var q = db.AttendDaysAfterNthVisitAsOf(ProgramInt, DivisionInt, OrganizationInt, StartDate, EndDate, Quarters.ToInt(), Days).Select(p => p.PeopleId);

            Expression<Func<Person, bool>> pred;
            if (op == CompareType.Equal ^ tf)
                pred = p => !q.Contains(p.PeopleId);
            else
                pred = p => q.Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;

        }
        internal Expression MeetingId()
        {
            var meetingid = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = p =>
                p.Attends.Any(a =>
                    a.AttendanceFlag
                    && a.MeetingId == meetingid
                    );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RegisteredForMeetingId()
        {
            var meetingid = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = p =>
                p.Attends.Any(a => AttendCommitmentCode.committed.Contains(a.Commitment ?? 0)
                    && a.MeetingId == meetingid
                    );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression CommitmentForMeetingId()
        {
            var meetingid = Quarters.ToInt();
            Expression<Func<Person, bool>> pred = p => p.Attends.Any(a =>
                a.MeetingId == meetingid && CodeIntIds.Contains(a.Commitment ?? 99)); 
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasCommitmentForMeetingId()
        {
            var tf = CodeIds == "1";
            var meetingid = Quarters.ToInt();

            Expression<Func<Person, bool>> pred = null;
            if (op == CompareType.Equal ^ tf) // true means not committed
                pred = p => p.Attends.Any(a => a.MeetingId == meetingid && a.Commitment == null); // not committed 
            else
                pred = p => p.Attends.Any(a => a.MeetingId == meetingid && a.Commitment != null); // committed

            Expression expr = Expression.Invoke(pred, parm);
            return expr;

            /* Truth Table
             * 
             * 1 ^ 1 = 0    EQ ^ T = F = Committed
             * 0 ^ 1 = 1    EQ ^ F = T = NotCommitted
             * 1 ^ 0 = 1    NE ^ T = T = NotCommitted
             * 0 ^ 0 = 0    NE ^ F = F = Committed
             */
        }
        internal Expression FirstFamilyVisitAsOf()
        {
            var cdt = db.Setting("DbConvertedDate", "1/1/1900").ToDate();

            if (!StartDate.HasValue)
                return AlwaysFalse();
            var enddt = EndDate?.AddHours(24) ?? Util.Today.AddHours(24);

            var q = from fm in db.ViewFamilyFirstTimes
                    where fm.CreatedDate > cdt
                    where fm.FirstDate >= StartDate
                    where fm.FirstDate < enddt
                    select fm.FamilyId;

            Expression<Func<Person, bool>> pred = p => q.Contains(p.FamilyId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
