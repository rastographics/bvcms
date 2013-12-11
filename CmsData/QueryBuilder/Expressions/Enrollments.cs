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
using CmsData.Codes;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression IsMemberOf()
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId != MemberTypeCode.InActive
                    && m.MemberTypeId != MemberTypeCode.Prospect
                    && (m.Pending ?? false) == false
                    && (Organization == 0 || m.OrganizationId == Organization)
                    && (Division == 0 || m.Organization.DivOrgs.Any(t => t.DivId == Division))
                    && (Program == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == Program)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && CodeIds == "1"))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression AttendTypeIds()
        {
            Expression<Func<Person, bool>> pred;
            if (Schedule == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberType.AttendanceTypeId ?? 0)
                    where (Campus == 0
                            || m.Organization.CampusId == Campus
                            || (Campus == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgType ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgType
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Any();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberType.AttendanceTypeId ?? 0)
                    where (Campus == 0
                            || m.Organization.CampusId == Campus
                            || (Campus == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgType ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgType
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                    where Schedule == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == Schedule)
                    select m
                    ).Any();
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                return Expression.Not(Expression.Invoke(pred, parm));
            else
                return Expression.Invoke(pred, parm);
        }
        internal Expression MemberTypeIds()
        {
            Expression<Func<Person, bool>> pred;
            if (Schedule == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberTypeId)
                    where (Campus == 0
                            || m.Organization.CampusId == Campus
                            || (Campus == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgType ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgType
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Any();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where CodeIntIds.Contains(m.MemberTypeId)
                    where (Campus == 0
                            || m.Organization.CampusId == Campus
                            || (Campus == -1 && m.Organization.CampusId == null)
                            )
                    where (OrgType ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgType
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                    where Schedule == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == Schedule)
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
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
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
                where (OrgType ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgType
                where Organization == 0 || m.OrganizationId == Organization
                where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
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
            if (!(op == CompareType.Equal && tf))
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
                where (OrgType ?? 0) == 0 || m.Organization.OrganizationTypeId == OrgType
                where Organization == 0 || m.OrganizationId == Organization
                where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
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
                            where Organization == 0 || m.OrganizationId == Organization
                            where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                            where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                            select m
                        ).Any();
                        break;
                    case CompareType.StartsWith:
                        pred = p => (
                            from m in p.OrganizationMembers
                            where m.OrgMemMemTags.Any(mt => mt.MemberTag.Name.StartsWith(smallgroup))
                            where Organization == 0 || m.OrganizationId == Organization
                            where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                            where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
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
                            where Organization == 0 || m.OrganizationId == Organization
                            where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                            where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
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
            if (Schedule == -1)
                pred = p => (
                    from m in p.OrganizationMembers
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                    where (m.Pending ?? false) == false
                    where !m.Organization.OrgSchedules.Any()
                    select m
                    ).Count();
            else
                pred = p => (
                    from m in p.OrganizationMembers
                    where Organization == 0 || m.OrganizationId == Organization
                    where Division == 0 || m.Organization.DivOrgs.Any(dg => dg.DivId == Division)
                    where Program == 0 || m.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == Program))
                    where (m.Pending ?? false) == false
                    where Schedule == 0 || m.Organization.OrgSchedules.Any(os => os.ScheduleId == Schedule)
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
                    (m.Pending ?? false) == true
                    && (Organization == 0 || m.OrganizationId == Organization)
                    && (Division == 0 || m.Organization.DivOrgs.Any(t => t.DivId == Division))
                    && (Program == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == Program)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsInactiveMemberOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId == MemberTypeCode.InActive
                    && (Organization == 0 || m.OrganizationId == Organization)
                    && (Division == 0 || m.Organization.DivOrgs.Any(t => t.DivId == Division))
                    && (Program == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == Program)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsProspectOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.OrganizationMembers.Any(m =>
                    m.MemberTypeId == MemberTypeCode.Prospect
                    && (Organization == 0 || m.OrganizationId == Organization)
                    && (Division == 0 || m.Organization.DivOrgs.Any(t => t.DivId == Division))
                    && (Program == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == Program)))
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
