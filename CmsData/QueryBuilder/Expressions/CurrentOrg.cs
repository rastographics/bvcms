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
        internal Expression HasBalanceInCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentSessionOrgId
                    && m.MemberTypeId != MemberTypeCode.InActive
                    && m.MemberTypeId != MemberTypeCode.Prospect)
                && (from t in db.ViewTransactionSummaries
                       where t.PeopleId == p.PeopleId
                       where t.OrganizationId == db.CurrentSessionOrgId
                       orderby t.RegId descending
                    select t.IndDue).FirstOrDefault() > 0;

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression LeadersUnderCurrentOrg()
        {
            var tf = CodeIds == "1";
            var oids = db.GetParentChildOrgIds(db.CurrentSessionOrgId);
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    p.OrganizationMembers.Any(mm => oids.Contains(mm.OrganizationId) && mm.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, right);
        }
        internal Expression MembersUnderCurrentOrg()
        {
            var tf = CodeIds == "1";
            var oids = db.GetParentChildOrgIds(db.CurrentSessionOrgId);
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    p.OrganizationMembers.Any(mm => oids.Contains(mm.OrganizationId))
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, right);
        }
		internal Expression InactiveCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentSessionOrgId
                    && m.MemberTypeId == MemberTypeCode.InActive);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression ProspectCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentSessionOrgId
                    && m.MemberTypeId == MemberTypeCode.Prospect);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression PendingCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentSessionOrgId
                    && (m.Pending ?? false));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression PreviousCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.EnrollmentTransactions.Any(m =>
                    m.OrganizationId == db.CurrentSessionOrgId
                    && m.TransactionTypeId > 3
                    && m.MemberTypeId != MemberTypeCode.Prospect
                    && m.TransactionStatus == false
                    && (m.Pending ?? false) == false)
                && !p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentSessionOrgId
                    && (m.Pending ?? false) == false);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression VisitedCurrentOrg()
        {
            var tf = CodeIds == "1";
            var mindt = Util.Now.AddDays(-db.VisitLookbackDays).Date;
            var ids = new[]
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
                where a.MeetingDate > db.LastDrop(a.OrganizationId, a.PeopleId)
                where ids.Contains(a.AttendanceTypeId.Value)
                where a.OrganizationId == db.CurrentSessionOrgId
                select a
                ).Any() && !(
                    from m in p.OrganizationMembers
                    where m.OrganizationId == db.CurrentSessionOrgId
                    where (m.Pending ?? false) == false
                    where (m.MemberTypeId != MemberTypeCode.Prospect)
                    select m
                    ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression OrgFilter()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                db.OrgFilterIds(ParentId).Select(vv => vv.PeopleId).Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));

            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
