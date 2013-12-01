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
                where et.MemberTypeId != MemberTypeCode.Prospect
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
    }
}
