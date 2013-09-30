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
        internal static Expression RecentJoinChurch(
            ParameterExpression parm,
            int days,
            CompareType op,
            bool tf)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.JoinDate > mindt;
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual
                || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;

        }

        internal static Expression RecentDecisionType(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.DecisionDate > mindt
                && ids.Contains(p.DecisionTypeId.Value);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression CampusId(ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = null;
            if (op == CompareType.IsNull)
                pred = p => p.CampusId == null;
            else if (op == CompareType.IsNotNull)
                pred = p => p.CampusId != null;
            else if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                pred = p => !ids.Contains(p.CampusId ?? 0);
            else
                pred = p => ids.Contains(p.CampusId ?? 0);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p

            return expr;
        }

    }
}