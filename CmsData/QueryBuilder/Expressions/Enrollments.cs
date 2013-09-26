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
        internal static Expression AttendTypeIds(
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
                    where ids.Contains(m.MemberType.AttendanceTypeId ?? 0)
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
                    where ids.Contains(m.MemberType.AttendanceTypeId ?? 0)
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
    }
}
