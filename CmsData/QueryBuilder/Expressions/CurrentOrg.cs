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
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
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

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
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
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }

        internal static Expression ProspectCurrentOrg(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == Db.CurrentOrgId
                    && m.MemberTypeId == MemberTypeCode.Prospect);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
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
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
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
                    && m.MemberTypeId != MemberTypeCode.Prospect
                    && m.TransactionStatus == false
                    && (m.Pending ?? false) == false)
                && !p.OrganizationMembers.Any(m =>
                    m.OrganizationId == Db.CurrentOrgId
                    && (m.Pending ?? false) == false);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
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
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof (bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
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
    }
}