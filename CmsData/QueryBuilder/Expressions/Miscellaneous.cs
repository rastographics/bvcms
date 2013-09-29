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

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression IncludeDeceased(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = null;

            bool include = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
            if (include)
                pred = p => p.DeceasedDate == null || p.DeceasedDate != null;
            else
                pred = p => p.DeceasedDate == null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
        internal static Expression ParentsOf(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = null;

            bool include = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
            pred = p => p.PeopleId > 0;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
        internal static Expression StatusFlag(
            ParameterExpression parm,
            CompareType op,
            params string[] codes)
        {
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(tt => codes.Contains(tt.Tag.Name) && tt.Tag.TypeId == 100);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasCurrentTag(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.Tags.Any(t => t.Tag.Name == Db.CurrentTagName && t.Tag.PeopleId == Db.CurrentTagOwnerId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasMyTag(ParameterExpression parm,
            string tag,
            CompareType op,
            bool tf)
        {
            var a = (tag ?? "").Split(';').Select(s => s.Split(',')[0].ToInt()).ToArray();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => a.Contains(t.Id));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasMemberDocs(ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.MemberDocForms.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression SavedQuery(ParameterExpression parm,
            CMSDataContext Db,
            string QueryIdDesc,
            CompareType op,
            bool tf)
        {
            var a = QueryIdDesc.Split(":".ToCharArray(), 2);
            var QueryId = a[0].ToInt();
            var q2 = Db.PeopleQuery(QueryId);
            if (q2 == null)
                return AlwaysFalse(parm);
            var tag = Db.PopulateTemporaryTag(q2.Select(pp => pp.PeopleId));

            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression SavedQuery2(ParameterExpression parm,
            CMSDataContext Db,
            string QueryIdDesc,
            CompareType op,
            bool tf)
        {
            var a = QueryIdDesc.Split(":".ToCharArray(), 2);
            Guid QueryId;
            Guid.TryParse(a[0], out QueryId);
            var q2 = Db.PeopleQuery(QueryId);
            if (q2 == null)
                return AlwaysFalse(parm);
            var tag = Db.PopulateTemporaryTag(q2.Select(pp => pp.PeopleId));

            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression SavedQueryPlus(ParameterExpression parm,
            CMSDataContext Db,
            string QueryIdDesc,
            CompareType op,
            int[] ids)
        {
            var a = QueryIdDesc.SplitStr(":", 2);
            var savedquery = Db.QueryBuilderClauses.SingleOrDefault(q =>
                q.SavedBy == a[0] && q.Description == a[1]);
            var pred = savedquery.Predicate(Db);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecActiveOtherChurch(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.ActiveInAnotherChurch == true)
                    && p.RecRegs.Any();
            Expression expr1 = Expression.Convert(Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr2 = Expression.Not(expr2);
            return Expression.And(expr1, expr2);
        }
        internal static Expression RecInterestedCoaching(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.Coaching == true)
                    && p.RecRegs.Any();
            Expression expr1 = Expression.Convert(Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr2 = Expression.Not(expr2);
            return Expression.And(expr1, expr2);
        }
        internal static Expression InOneOfMyOrgs(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            bool tf)
        {
            var uid = Util.UserPeopleId;
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    Db.OrganizationMembers.Any(um =>
                        um.OrganizationId == m.OrganizationId && um.PeopleId == uid)
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression CheckInVisits(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            int visits)
        {
            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Greater:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() > visits
                                 select g).Any();
                    break;
                case CompareType.Less:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() < visits
                                 select g).Any();
                    break;
                case CompareType.GreaterEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() >= visits
                                 select g).Any();
                    break;
                case CompareType.LessEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() <= visits
                                 select g).Any();
                    break;
                case CompareType.Equal:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() == visits
                                 select g).Any();
                    break;
                case CompareType.NotEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() != visits
                                 select g).Any();
                    break;
            }

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression MedicalLength(
            ParameterExpression parm,
            CompareType op,
            int len)
        {
            if (len == 0 && op == CompareType.Equal)
            {
                Expression<Func<Person, bool>> pp = p =>
                    !p.RecRegs.Any() || p.RecRegs.First().MedicalDescription == null ||
                    p.RecRegs.First().MedicalDescription.Length == 0;
                return Expression.Invoke(pp, parm);
            }
            Expression<Func<Person, int>> pred = p => p.RecRegs.Sum(rr => rr.MedicalDescription.Length);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(len), left.Type);
            return Compare(left, op, right);
        }
    }
}
