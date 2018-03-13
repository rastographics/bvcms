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
        internal Expression NumberOfFamilyMembers()
        {
            var cnt = TextValue.ToInt();
            Expression<Func<Person, int>> pred = p => p.Family.People.Count();
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression NumberOfPrimaryAdults()
        {
            var cnt = TextValue.ToInt();
            Expression<Func<Person, int>> pred = p => p.Family.People.Count(pp => pp.PositionInFamilyId == 10);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression HasParents()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.PositionInFamilyId == PositionInFamily.PrimaryAdult && p.PositionInFamilyId == PositionInFamily.Child);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamilyHasChildren()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => (m.Age ?? 0) <= 12 && m.PositionInFamilyId == PositionInFamily.Child);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamilyHasChildrenAged()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => (m.Age ?? 0) <= (Age ?? 0) && m.PositionInFamilyId == PositionInFamily.Child);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamilyHasChildrenAged2()
        {
            var range = Quarters.Split('-');
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => (m.Age ?? 0) >= range[0].ToInt() && (m.Age ?? 0) <= range[1].ToInt() && m.PositionInFamilyId == PositionInFamily.Child);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamilyHasChildrenAged3()
        {
            var range = Quarters?.Split('-');
            if (range == null || range.Length != 2)
                return AlwaysFalse();
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m =>
                    (m.Age ?? 0) >= range[0].ToInt()
                    && (m.Age ?? 0) <= range[1].ToInt()
                    && CodeIntIds.Contains(m.GenderId)
                    && m.PositionInFamilyId == PositionInFamily.Child
                );
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasRelatedFamily()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.RelatedFamilies1.Any()
                || p.Family.RelatedFamilies2.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RelatedFamilyMembers()
        {
            var familyid = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = p =>
                p.Family.RelatedFamilies1.Any(rr => rr.FamilyId == familyid || rr.RelatedFamilyId == familyid)
                || p.Family.RelatedFamilies2.Any(rr => rr.FamilyId == familyid || rr.RelatedFamilyId == familyid);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op != CompareType.Equal)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsHeadOfHousehold()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.HeadOfHouseholdId == p.PeopleId;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamHasPrimAdultChurchMemb()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m =>
                    m.PositionInFamilyId == PositionInFamily.PrimaryAdult
                    && m.MemberStatusId == 10 // church member
                    );
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamHasStatusFlag()
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
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Tags.Any(tt => codes.Contains(tt.Tag.Name) && tt.Tag.TypeId == DbUtil.TagTypeId_StatusFlags));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasFamilyPicture()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p => p.Family.PictureId != null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
