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
        internal Expression PeopleExtra()
        {
            Expression<Func<Person, bool>> pred = p =>
                p.PeopleExtras.Any(e => CodeStrIds.Contains(e.FieldValue));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasPeopleExtraField()
        {
            var NameTypes = ExtraValue.Views.GetViewableNameTypes("People", nocache: true);
            if (!NameTypes.Any(nn => nn.Name == TextValue && nn.CanView))
                return AlwaysFalse();
            Expression<Func<Person, bool>> pred = p => p.PeopleExtras.Any(e => e.Field == TextValue);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression PeopleExtraData()
        {
            var field = Quarters;
            var NameTypes = ExtraValue.Views.GetViewableNameTypes("People", nocache: true);
            if (!NameTypes.Any(nn => nn.Name == field && nn.CanView))
                return AlwaysFalse();
            Expression<Func<Person, string>> pred = p =>
                p.PeopleExtras.Where(ff => ff.Field == field).Select(ff => ff.Data).SingleOrDefault();
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(TextValue, typeof(string));
            return Compare(left, right);
        }
        internal Expression PeopleExtraInt()
        {
            var field = Quarters;
            var NameTypes = ExtraValue.Views.GetViewableNameTypes("People", nocache: true);
            if (!NameTypes.Any(nn => nn.Name == field && nn.CanView))
                return AlwaysFalse();
            var value = TextValue.ToInt2();
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
                        return AlwaysFalse();
                }
            }

            Expression<Func<Person, int>> pred = p =>
                p.PeopleExtras.Single(e =>
                    e.Field == field).IntValue ?? 0;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(value), left.Type);
            return Compare(left, right);
        }
        internal Expression PeopleExtraDate()
        {
            var field = Quarters;
            var NameTypes = ExtraValue.Views.GetViewableNameTypes("People", nocache: true);
            if (!NameTypes.Any(nn => nn.Name == field && nn.CanView))
                return AlwaysFalse();
            if (!DateValue.HasValue)
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
                        return AlwaysFalse();
                }
            }
            else
            {
                Expression<Func<Person, DateTime>> pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue.Value;
                Expression left = Expression.Invoke(pred, parm);
                var right = Expression.Convert(Expression.Constant(DateValue), left.Type);
                return Compare(left, right);
            }
        }
    }
}
