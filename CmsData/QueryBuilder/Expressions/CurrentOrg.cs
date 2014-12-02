/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using DevDefined.OAuth.Consumer;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression HasBalanceInCurrentOrg()
        {
            var tf = CodeIds == "1";
            string first, last;
            Util.NameSplit(db.CurrentOrg.NameFilter, out first, out last);
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgMember(db.CurrentOrgId0, GroupSelectCode.Member, first, last, co.SgFilter, co.ShowHidden)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId)
                && (from t in db.ViewTransactionSummaries
                    where t.PeopleId == p.PeopleId
                    where t.OrganizationId == db.CurrentOrgId0
                    orderby t.RegId descending
                    select t.IndDue).FirstOrDefault() > 0;

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression InCurrentOrg()
        {
            var tf = CodeIds == "1";
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgMember(db.CurrentOrgId0, GroupSelectCode.Member, co.First(), co.Last(), co.SgFilter, co.ShowHidden)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));

            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression InactiveCurrentOrg()
        {
            var tf = CodeIds == "1";
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgMember(db.CurrentOrgId0, GroupSelectCode.Inactive, co.First(), co.Last(), co.SgFilter, co.ShowHidden)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression ProspectCurrentOrg()
        {
            var tf = CodeIds == "1";
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgMember(db.CurrentOrgId0, GroupSelectCode.Prospect, co.First(), co.Last(), co.SgFilter, co.ShowHidden)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression PendingCurrentOrg()
        {
            var tf = CodeIds == "1";
            var co = db.CurrentOrg;
            Expression<Func<Person, bool>> pred = p =>
                db.OrgMember(db.CurrentOrgId0, GroupSelectCode.Pending, co.First(), co.Last(), co.SgFilter, co.ShowHidden)
                    .Select(gg => gg.PeopleId)
                    .Contains(p.PeopleId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
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
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression VisitedCurrentOrg()
        {
            var tf = CodeIds == "1";
            var mindt = Util.Now.AddDays(-db.VisitLookbackDays).Date;

            var q = db.GuestList(db.CurrentOrgId0, mindt, db.CurrentOrg.ShowHidden, null, null);
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
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
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
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
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, right);
        }
    }
}