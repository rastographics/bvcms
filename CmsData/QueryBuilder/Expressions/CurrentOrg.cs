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
        internal Expression HasBalanceInCurrentOrg()
        {
            var tf = CodeIds == "1";
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgMember(db.CurrentOrgId0, GroupSelectCode.Member, co.First(), co.Last(), co.SgFilter, co.ShowHidden)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId)
                && (from t in db.ViewTransactionSummaries
                       where t.PeopleId == p.PeopleId
                       where t.OrganizationId == db.CurrentOrgId0
                       orderby t.RegId descending
                    select t.IndDue).FirstOrDefault() > 0;

            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression InCurrentOrgNew()
        {
            var tf = CodeIds == "1";
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgPeopleIds(db.CurrentOrgId0, co.GroupSelect, co.First(), co.Last(), 
                        co.SgFilter, co.ShowHidden, Util2.CurrentTag, Util2.CurrentTagOwnerId,
                        co.FilterIndividuals, co.FilterTag, null, Util.UserPeopleId)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId);

            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));

            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression InCurrentOrg()
        {
            if (Util2.UseNewOrg)
                return InCurrentOrgNew();

            var tf = CodeIds == "1";
            var cg = db.CurrentGroups.ToArray();
            var cgmode = db.CurrentGroupsMode;
            Expression<Func<Person, bool>> pred = p => (
                from m in p.OrganizationMembers
                where m.OrganizationId == db.CurrentOrgId0
                let gc = m.OrgMemMemTags.Count(mt => cg.Contains(mt.MemberTagId))
                // for Match Any
                where gc > 0 || cg[0] <= 0 || cgmode == 1
                // for Match All
                where gc == cg.Length || cg[0] <= 0 || cgmode == 0
                // for Match No SmallGroup assigned
                where !m.OrgMemMemTags.Any() || cg[0] != -1
                where m.MemberTypeId != MemberTypeCode.InActive
                where m.MemberTypeId != MemberTypeCode.Prospect
                where (m.Pending ?? false) == false
                select m
                ).Any();

            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (db.CurrentGroupsPrefix.HasValue())
            {
                var aa = db.CurrentGroupsPrefix.Split(',');
                foreach (var sg in aa)
                {
                    Expression<Func<Person, bool>> pred2 = null;
                    if (sg.StartsWith("-"))
                    {
                        var g = sg.Substring(1);
                        pred2 = p => (from om in p.OrganizationMembers
                                      where om.OrganizationId == db.CurrentOrgId0
                                      where om.OrgMemMemTags.All(
                                          mm => !mm.MemberTag.Name.StartsWith(g))
                                      select om).Any();
                    }
                    else
                    {
                        pred2 = p => (from om in p.OrganizationMembers
                                      where om.OrganizationId == db.CurrentOrgId0
                                      where om.OrgMemMemTags.Any(
                                          mm => mm.MemberTag.Name.StartsWith(sg))
                                      select om).Any();
                    }
                    var expr1 = System.Linq.Expressions.Expression.Invoke(pred2, parm);
                    expr = System.Linq.Expressions.Expression.And(expr1, expr);
                }
            }

            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression LeadersUnderCurrentOrg()
        {
            var tf = CodeIds == "1";
            var oids = db.GetParentChildOrgIds(db.CurrentOrgId0);
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    p.OrganizationMembers.Any(mm => oids.Contains(mm.OrganizationId) && mm.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                );
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(tf), left.Type);
            return Compare(left, right);
        }
        internal Expression MembersUnderCurrentOrg()
        {
            var tf = CodeIds == "1";
            var oids = db.GetParentChildOrgIds(db.CurrentOrgId0);
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    p.OrganizationMembers.Any(mm => oids.Contains(mm.OrganizationId))
                );
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(tf), left.Type);
            return Compare(left, right);
        }
		internal Expression InactiveCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentOrgId0
                    && m.MemberTypeId == MemberTypeCode.InActive);
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }

        internal Expression ProspectCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentOrgId0
                    && m.MemberTypeId == MemberTypeCode.Prospect);
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression PendingCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentOrgId0
                    && (m.Pending ?? false) == true);
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression PreviousCurrentOrg()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.EnrollmentTransactions.Any(m =>
                    m.OrganizationId == db.CurrentOrgId0
                    && m.TransactionTypeId > 3
                    && m.MemberTypeId != MemberTypeCode.Prospect
                    && m.TransactionStatus == false
                    && (m.Pending ?? false) == false)
                && !p.OrganizationMembers.Any(m =>
                    m.OrganizationId == db.CurrentOrgId0
                    && (m.Pending ?? false) == false);
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression VisitedCurrentOrg()
        {
            var tf = CodeIds == "1";
            var mindt = Util.Now.AddDays(-db.VisitLookbackDays).Date;
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
                where a.MeetingDate > db.LastDrop(a.OrganizationId, a.PeopleId)
                where ids.Contains(a.AttendanceTypeId.Value)
                where a.OrganizationId == db.CurrentOrgId0
                select a
                ).Any() && !(
                    from m in p.OrganizationMembers
                    where m.OrganizationId == db.CurrentOrgId0
                    where (m.Pending ?? false) == false
                    where (m.MemberTypeId != MemberTypeCode.Prospect)
                    select m
                    ).Any();
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
    }
}