/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using Community.CsharpSqlite;
using DDay.iCal;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression IncludeDeceased()
        {
            setIncludeDeceased();
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = null;

            bool include = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
            if (include)
                pred = p => p.DeceasedDate == null || p.DeceasedDate != null;
            else
                pred = p => p.DeceasedDate == null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
        internal Expression ExprParentsOf()
        {
            var tf = CodeIds == "1";
            setParentsOf(op, tf);
            Expression<Func<Person, bool>> pred = null;

            bool include = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
            pred = p => p.PeopleId > 0;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
        internal Expression StatusFlag()
        {
            var codes0 = CodeValues.Split(',');
            var codes = (from f in db.ViewStatusFlagLists.ToList()
                         where f.RoleName == null || db.CurrentRoles().Contains(f.RoleName)
                         join c in codes0 on f.Flag equals c into j
                         from c in j
                         select c).ToList();
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(tt => codes.Contains(tt.Tag.Name) && tt.Tag.TypeId == 100);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasCurrentTag()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.Tags.Any(t => t.Tag.Name == db.CurrentTagName && t.Tag.PeopleId == db.CurrentTagOwnerId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasMyTag()
        {
            var tf = CodeIds == "1";
            var a = (Tags ?? "").Split(';').Select(s => s.Split(',')[0].ToInt()).ToArray();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => a.Contains(t.Id));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasMemberDocs()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.MemberDocForms.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression SavedQuery2()
        {
            var tf = CodeIds == "1";
            var a = SavedQuery.Split(":".ToCharArray(), 2);
            Guid QueryId;
            Guid.TryParse(a[0], out QueryId);
            var q2 = db.PeopleQuery(QueryId);
            if (q2 == null)
                return AlwaysFalse();
            var tag = db.PopulateTemporaryTag(q2.Select(pp => pp.PeopleId));

            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecActiveOtherChurch()
        {
            var tf = CodeIds == "1";
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
        internal Expression RecInterestedCoaching()
        {
            var tf = CodeIds == "1";
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
        internal Expression InOneOfMyOrgs()
        {
            var tf = CodeIds == "1";
            var uid = Util.UserPeopleId;
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    db.OrganizationMembers.Any(um =>
                        um.OrganizationId == m.OrganizationId && um.PeopleId == uid)
                );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, right);
        }
        internal Expression CheckInVisits()
        {
            var visits = TextValue.ToInt();
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
        internal Expression MedicalLength()
        {
            var len = TextValue.ToInt();
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
            return Compare(left, right);
        }
    }
}
