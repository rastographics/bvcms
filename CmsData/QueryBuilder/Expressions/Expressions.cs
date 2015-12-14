/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UtilityExtensions;

namespace CmsData
{
    public  partial class Condition
    {
        private static Expression Compare(ParameterExpression parm, Expression left, CompareType op, Expression right)
        {
            Expression expr = null;
            switch (op)
            {
                case CompareType.NotEqual:
                case CompareType.IsNotNull:
                    expr = Expression.NotEqual(left, right);
                    break;
                case CompareType.Equal:
                case CompareType.IsNull:
                    expr = Expression.Equal(left, right);
                    break;
                case CompareType.Greater:
                    expr = Expression.GreaterThan(left, right);
                    break;
                case CompareType.GreaterEqual:
                    expr = Expression.GreaterThanOrEqual(left, right);
                    break;
                case CompareType.Less:
                    expr = Expression.LessThan(left, right);
                    break;
                case CompareType.LessEqual:
                    expr = Expression.LessThanOrEqual(left, right);
                    break;
                case CompareType.DoesNotStartWith:
                case CompareType.StartsWith:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), right);
                    break;
                case CompareType.DoesNotEndWith:
                case CompareType.EndsWith:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), right);
                    break;
                case CompareType.DoesNotContain:
                case CompareType.Contains:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) }), right);
                    break;
                case CompareType.After:
                case CompareType.AfterOrSame:
                case CompareType.Before:
                case CompareType.BeforeOrSame:
                    expr = Expression.Call(left,
                        typeof(string).GetMethod("CompareTo", new[] { typeof(string) }), right);
                    break;
            }
            switch (op)
            {
                // now reverse the logic if necessary
                case CompareType.DoesNotEndWith:
                case CompareType.DoesNotContain:
                case CompareType.DoesNotStartWith:
                    expr = Expression.Not(expr);
                    break;
                case CompareType.After:
                    expr = Expression.GreaterThan(expr, Expression.Constant(0));
                    break;
                case CompareType.AfterOrSame:
                    expr = Expression.GreaterThanOrEqual(expr, Expression.Constant(0));
                    break;
                case CompareType.Before:
                    expr = Expression.LessThan(expr, Expression.Constant(0));
                    break;
                case CompareType.BeforeOrSame:
                    expr = Expression.LessThanOrEqual(expr, Expression.Constant(0));
                    break;
            }
            return expr;
        }
        private static MethodInfo EnumerableContains;
        private static Expression CompareContains(ParameterExpression parm, string prop, CompareType op, object a, Type arrayType, Type itemType)
        {
            var left = Expression.Constant(a, arrayType);
            var right = Expression.Convert(Expression.Property(parm, prop), itemType);
            if (EnumerableContains == null)
                EnumerableContains = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Contains");
            var method = EnumerableContains.MakeGenericMethod(itemType);
            Expression expr = Expression.Call(method, new Expression[] { left, right });
            if (op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression CompareConstant(ParameterExpression parm, string prop, CompareType op, object value)
        {
            if (value != null)
                if (value.GetType() == typeof(int[])) // use isarray?
                    return CompareContains(parm, prop, op, value, typeof(int[]), typeof(int));
                else if (value.GetType() == typeof(string[]))
                    return CompareContains(parm, prop, op, value, typeof(string[]), typeof(string));
            var left = Expression.Property(parm, prop);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(parm, left, op, right);
        }
        internal Expression Compare(Expression left, Expression right)
        {
            return Compare(parm, left, op, right);
        }
        internal Expression CompareDateConstant(string prop, DateTime? dt)
        {
            if (!dt.HasValue)
            {
                if (op == CompareType.NotEqual)
                    return CompareConstant(parm, prop, CompareType.IsNotNull, null);
                return CompareConstant(parm, prop, CompareType.IsNull, null);
            }

            Expression left = Expression.Property(parm, prop);
            if (dt.Value.Date == dt) // 12:00:00 AM?
            {
                left = Expression.MakeMemberAccess(left, typeof(DateTime?).GetProperty("Value"));
                left = Expression.MakeMemberAccess(left, typeof(DateTime).GetProperty("Date"));
            }
            var right = Expression.Convert(Expression.Constant(dt), typeof(DateTime));
            return Compare(left, right);
        }
        internal Expression CompareIntConstant(string prop, string value)
        {
            var left = Expression.Property(parm, prop);
            int? i = value.ToInt2();
            var right = Expression.Convert(Expression.Constant(i), left.Type);
            return Compare(left, right);
        }
        internal Expression CompareConstant(string prop, object value)
        {
            return CompareConstant(parm, prop, op, value);
        }
        internal Expression CompareCodeConstant(string prop, object value)
        {
            if (value != null)
                if (value.GetType() == typeof(int[])) // use isarray?
                    return CompareContains(parm, prop, op, value, typeof(int[]), typeof(int));
            var left = Expression.Coalesce(Expression.Property(parm, prop), Expression.Constant(0));
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, right);
        }
        internal Expression CompareStringConstant(string prop, object value)
        {
            if (value != null)
                if (value.GetType() == typeof(int[])) // use isarray?
                    return CompareContains(parm, prop, op, value, typeof(int[]), typeof(int));
                else if (value.GetType() == typeof(string[]))
                    return CompareContains(parm, prop, op, value, typeof(string[]), typeof(string));

            var left = Expression.Call(Expression.Coalesce(Expression.Property(parm, prop), Expression.Constant("")), "Trim", null);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, right);
        }
        internal Expression CompareProperty(string prop, string prop2)
        {
            var left = Expression.Property(parm, prop);
            var right = Expression.Property(parm, prop2);
            return Compare(left, right);
        }
        internal Expression AlwaysFalse(ParameterExpression parm0 = null)
        {
            Expression<Func<Person, bool>> pred = p => false;
            Expression expr = Expression.Invoke(pred, parm0 ?? parm);
            return expr;
        }

    }
}
