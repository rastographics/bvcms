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
        internal Expression IsMemberOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId != MemberTypeCode.InActive
                    && m.MemberTypeId != MemberTypeCode.Prospect
                    && (m.Pending ?? false) == false
                    && (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsMemberOfDirectory()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.Organization.PublishDirectory > 0
                    && (m.MemberTypeId != MemberTypeCode.InActive 
                        || m.Organization.OrganizationExtras.Any(ev => 
                            ev.Field == "IncludeInactiveInDirectory" && ev.BitValue == true))
                    && (m.MemberTypeId != MemberTypeCode.Prospect
                        || m.Organization.OrganizationExtras.Any(ev => 
                            ev.Field == "IncludeProspectInDirectory" && ev.BitValue == true))
                    && m.OrgMemMemTags.All(vv => vv.MemberTag.Name != "DoNotPublish")
                    && (m.Pending ?? false) == false
                    && (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression AttendTypeIds()
        {
            Expression<Func<Person, bool>> pred;
            if (ScheduleInt == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberType.AttendanceTypeId ?? 0)
                    where ((CampusInt ?? 0) == 0
                            || m.Organization.CampusId == (CampusInt ?? 0)
                            || (CampusInt == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgTypeInt ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgTypeInt
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Any();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberType.AttendanceTypeId ?? 0)
                    where ((CampusInt ?? 0) == 0
                            || m.Organization.CampusId == (CampusInt ?? 0)
                            || (CampusInt == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgTypeInt ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgTypeInt
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where ScheduleInt == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == ScheduleInt)
                    select m
                    ).Any();
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                return Expression.Not(Expression.Invoke(pred, parm));
            return Expression.Invoke(pred, parm);
        }
        internal Expression MemberTypeIds()
        {
            Expression<Func<Person, bool>> pred;
            if (ScheduleInt == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberTypeId)
                    where ((CampusInt ?? 0) == 0
                            || m.Organization.CampusId == (CampusInt ?? 0)
                            || (CampusInt == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgTypeInt ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgTypeInt
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Any();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberTypeId)
                    where ((CampusInt ?? 0) == 0
                            || m.Organization.CampusId == (CampusInt ?? 0)
                            || (CampusInt == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgTypeInt ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgTypeInt
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where ScheduleInt == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == ScheduleInt)
                    select m
                    ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression MembOfOrgWithSched()
        {
            Expression<Func<Person, bool>> pred = p => (
                    from m in p.OrganizationMembers
                    where m.MemberTypeId != MemberTypeCode.InActive
                    where m.MemberTypeId != MemberTypeCode.Prospect
                    where (m.Pending ?? false) == false
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where m.Organization.OrgSchedules.Any(os => CodeIntIds.Contains(os.ScheduleId ?? 0))
                    select m
                    ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression MembOfOrgWithCampus()
        {
            Expression<Func<Person, bool>> pred = p => (
                from m in p.OrganizationMembers
                where m.MemberTypeId != MemberTypeCode.InActive
                where m.MemberTypeId != MemberTypeCode.Prospect
                where (m.Pending ?? false) == false
                where (OrgTypeInt ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgTypeInt
                where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                where CodeIntIds.Contains(m.Organization.CampusId ?? 0)
                    || (CodeIntIds[0] == -1 && m.Organization.CampusId == null)
                select m
                ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression InBFClass()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.Organization.IsBibleFellowshipOrg == true
                    && m.MemberTypeId != MemberTypeCode.InActive
                    && m.MemberTypeId != MemberTypeCode.Prospect
                    && (m.Pending ?? false) == false);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentRegistrationType()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p => (
                from m in p.EnrollmentTransactions
                where m.TransactionTypeId == 1
                where m.EnrollmentDate >= mindt
                where CodeIntIds[0] == 99 || CodeIntIds.Contains(m.Organization.RegistrationTypeId ?? 0)
                where (OrgTypeInt ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgTypeInt
                where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                select m
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression SmallGroup()
        {
            var smallgroup = TextValue;
            if (smallgroup.HasValue())
            {
                Expression<Func<Person, bool>> pred = null;
                switch (op)
                {
                    case CompareType.Equal:
                    case CompareType.NotEqual:
                        pred = p => (
                            from m in p.OrganizationMembers
                            where m.OrgMemMemTags.Any(mt => mt.MemberTag.Name == smallgroup)
                            where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                            where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                            where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                            select m
                        ).Any();
                        break;
                    case CompareType.StartsWith:
                        pred = p => (
                            from m in p.OrganizationMembers
                            where m.OrgMemMemTags.Any(mt => mt.MemberTag.Name.StartsWith(smallgroup))
                            where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                            where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                            where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
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
                            where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                            where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                            where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                            select m
                        ).Any();
                var expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
                if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                    expr = Expression.Not(expr);
                return expr;
            }
        }
        internal Expression NumberOfMemberships()
        {
            var cnt = TextValue.ToInt();
            Expression<Func<Person, int>> pred;
            if (ScheduleInt == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where (m.Pending ?? false) == false
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Count();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where OrganizationInt == 0 || m.OrganizationId == OrganizationInt
                    where DivisionInt == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                    where ProgramInt == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                    where (m.Pending ?? false) == false
                    where ScheduleInt == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == ScheduleInt)
                    select m
                    ).Count();
            var left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression IsPendingMemberOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    (m.Pending ?? false)
                    && (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsInactiveMemberOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId == MemberTypeCode.InActive
                    && (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsProspectOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId == MemberTypeCode.Prospect
                    && (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression OrgSearchMember()
        {
            var q = db.PeopleIdsFromOrgSearch(OrgName, ProgramInt, DivisionInt, OrgType2, CampusInt, ScheduleInt, OrgStatusInt,
                OnlineRegInt, null, null);
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentIncompleteRegistrations()
        {
            var tf = CodeIds == "1";
            var mindt = Util.Now.AddDays(-Days).Date;
            var q = db.HasIncompleteRegistrations(ProgramInt, DivisionInt, OrganizationInt, mindt, Util.Now);
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId ?? 0));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasBalance()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    && (from ts in db.ViewTransactionSummaries
                           where ts.PeopleId == p.PeopleId
                           where ts.OrganizationId == m.OrganizationId
                           orderby ts.RegId descending
                        select ts.IndDue).FirstOrDefault() > 0);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
