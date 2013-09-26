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
        internal static Expression NumberOfFamilyMembers(
            ParameterExpression parm,
            CompareType op,
            int cnt)
        {
            Expression<Func<Person, int>> pred = p => p.Family.People.Count();
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression HasParents(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.PositionInFamilyId == 10 && p.PositionInFamilyId == 30);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildren(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Age <= 12);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildrenAged(
            ParameterExpression parm,
            int age,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Age <= age);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildrenAged2(
            ParameterExpression parm,
            string range,
            CompareType op,
            bool tf)
        {
            var a = range.Split('-');
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m => m.Age >= a[0].ToInt() && m.Age <= a[1].ToInt());
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamilyHasChildrenAged3(
            ParameterExpression parm,
            string range,
            CompareType op,
            int[] ids)
        {
            var a = range.Split('-');
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m =>
                    m.Age >= a[0].ToInt()
                    && m.Age <= a[1].ToInt()
                    && ids.Contains(m.GenderId)
                );
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasRelatedFamily(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.RelatedFamilies1.Any()
                || p.Family.RelatedFamilies2.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsHeadOfHousehold(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.HeadOfHouseholdId == p.PeopleId;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression FamHasPrimAdultChurchMemb(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(m =>
                    m.PositionInFamilyId == PositionInFamily.PrimaryAdult
                    && m.MemberStatusId == 10 // church member
                    //&& m.PeopleId != p.PeopleId // someone else in family
                    );
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(tf), left.Type);
            return Compare(left, op, right);
        }
    }
}
