/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression PeopleExtra(
            ParameterExpression parm,
            CompareType op,
            string[] values)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.PeopleExtras.Any(e =>
                    values.Contains(e.FieldValue));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasPeopleExtraField(
            ParameterExpression parm,
            CompareType op,
            string field)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.PeopleExtras.Any(e =>
                    e.Field == field);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression PeopleExtraData(
            ParameterExpression parm,
            string field,
            CompareType op,
            string value)
        {
            Expression<Func<Person, string>> pred = p =>
                p.PeopleExtras.Where(ff => ff.Field == field).Select(ff => ff.Data).SingleOrDefault();
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(value, typeof(string));
            return Compare(left, op, right);
        }
        internal static Expression PeopleExtraInt(
            ParameterExpression parm,
            string field,
            CompareType op,
            int? value)
        {
            if (!value.HasValue)
            {
                Expression<Func<Person, bool>> predint = null;
                switch (op)
                {
                    case CompareType.Equal:
                        predint = p => !p.PeopleExtras.Any(e => e.Field == field)
                                    || p.PeopleExtras.SingleOrDefault(e => e.Field == field).IntValue == null;
                        return Expression.Invoke(predint, parm);
                    case CompareType.NotEqual:
                        predint = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).IntValue != null;
                        return Expression.Invoke(predint, parm);
                    default:
                        return AlwaysFalse(parm);
                }
            }

            Expression<Func<Person, int>> pred = p =>
                p.PeopleExtras.Single(e =>
                    e.Field == field).IntValue ?? 0;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression PeopleExtraDate(
            ParameterExpression parm,
            string field,
            CompareType op,
            DateTime? value)
        {
            if (!value.HasValue)
            {
                Expression<Func<Person, bool>> pred = null;
                switch (op)
                {
                    case CompareType.Equal:
                        pred = p => !p.PeopleExtras.Any(e => e.Field == field)
                              || p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue == null;
                        return Expression.Invoke(pred, parm);
                    case CompareType.NotEqual:
                        pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue != null;
                        return Expression.Invoke(pred, parm);
                    default:
                        return AlwaysFalse(parm);
                }
            }
            else
            {
                Expression<Func<Person, DateTime>> pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue.Value;
                Expression left = Expression.Invoke(pred, parm);
                var right = Expression.Convert(Expression.Constant(value), left.Type);
                return Compare(left, op, right);
            }
        }
    }
}
