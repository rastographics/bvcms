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
        internal static Expression HasInvalidEmailAddress(CMSDataContext Db, ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                (Db.IsValidEmail(p.EmailAddress ?? "") == false
                || Db.IsValidEmail(p.EmailAddress2 ?? "") == false);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression SpouseHasEmail(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(pp => pp.PeopleId == p.SpouseId && pp.EmailAddress.Contains("@"));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
