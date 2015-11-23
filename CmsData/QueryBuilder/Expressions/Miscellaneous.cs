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
    public partial class Condition
    {
        internal Expression StatusFlag()
        {
            var codes0 = CodeValues.Split(',').Select(ff => ff.Trim()).ToList();
            var q = db.ViewStatusFlagLists.ToList();
            if (!db.FromBatch)
                q = (from f in q
                     where f.RoleName == null || db.CurrentRoles().Contains(f.RoleName)
                     select f).ToList();
            var codes = (from f in q
                         join c in codes0 on f.Flag equals c into j
                         from c in j
                         select c).ToList();
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(tt => codes.Contains(tt.Tag.Name) && tt.Tag.TypeId == DbUtil.TagTypeId_StatusFlags);
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression HasCurrentTag()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.Tags.Any(t => t.Tag.Name == db.CurrentTagName && t.Tag.PeopleId == db.CurrentTagOwnerId);
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression HasMyTag()
        {
            var tf = CodeIds == "1";
            var a = (Tags ?? "").Split(';').Select(s => s.Split(',')[0].ToInt()).ToArray();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => a.Contains(t.Id));
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression HasMemberDocs()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.MemberDocForms.Any();
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
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
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression RecActiveOtherChurch()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.ActiveInAnotherChurch == true)
                    && p.RecRegs.Any();
            Expression expr1 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr2 = System.Linq.Expressions.Expression.Not(expr2);
            return System.Linq.Expressions.Expression.And(expr1, expr2);
        }
        internal Expression RecInterestedCoaching()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.Coaching == true)
                    && p.RecRegs.Any();
            Expression expr1 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr2 = System.Linq.Expressions.Expression.Not(expr2);
            return System.Linq.Expressions.Expression.And(expr1, expr2);
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
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(tf), left.Type);
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

            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
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
                return System.Linq.Expressions.Expression.Invoke(pp, parm);
            }
            Expression<Func<Person, int>> pred = p => p.RecRegs.Sum(rr => rr.MedicalDescription.Length);
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(len), left.Type);
            return Compare(left, right);
        }
        internal Expression HasFailedEmails()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                db.EmailQueueToFails.Any(f => f.PeopleId == p.PeopleId && (f.Time >= StartDate || StartDate == null));
            Expression expr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }

        internal Expression RecentFlagAdded()
        {
            var codes0 = CodeValues.Split(',').Select(ff => ff.Trim()).ToList();
            var q = db.ViewStatusFlagLists.ToList();
            if (!db.FromBatch)
                q = (from f in q
                     where f.RoleName == null || db.CurrentRoles().Contains(f.RoleName)
                     select f).ToList();
            var codes = (from f in q
                         join c in codes0 on f.Flag equals c into j
                         from c in j
                         select c).ToList();
            var now = DateTime.Now;
            var dt = now.AddDays(-Days).Date;

            Expression<Func<Person, bool>> pred = p => (
                    from t in p.Tags
                    where codes.Contains(t.Tag.Name)
                    where t.Tag.TypeId == DbUtil.TagTypeId_StatusFlags
                    where t.DateCreated >= dt
                    select t
                    ).Any();
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
    }
}
