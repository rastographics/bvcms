/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression RecentJoinChurch()
        {
            var tf = CodeIds == "1";
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.JoinDate >= mindt;
            Expression expr = Expression.Invoke(pred, parm);

            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentJoinChurchDaysRange()
        {
            var a = TextValue.SplitStr("-", 2);
            var maxdt = Util.Today.AddDays(-a[0].ToInt()).Date;
            var mindt = maxdt;
            if(a.Length > 1)
                mindt = Util.Now.AddDays(-a[1].ToInt()).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.JoinDate >= mindt && p.JoinDate < maxdt;
            Expression expr = Expression.Invoke(pred, parm);

            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentDecisionType()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.DecisionDate > mindt
                && CodeIntIds.Contains(p.DecisionTypeId.Value);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression CampusId()
        {
            var ids = CodeIntIds;
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

        private Expression JoinDateMonthsAgo()
        {
            var dt = DateTime.Parse("1/1/1900");
            var months = TextValue.ToInt();
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffMonth(p.JoinDate ?? dt, Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(months, typeof(int?));
            return Compare(parm, left, op, right);
        }
    }
}
