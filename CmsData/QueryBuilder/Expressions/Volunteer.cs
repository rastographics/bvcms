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
        internal Expression HasVolunteerApplications()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.Volunteers.Any(v => v.VolunteerForms.Any());
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression VolunteerApprovalCode()
        {
            Expression<Func<Person, bool>> pred =
                p => p.VoluteerApprovalIds.Any(vid => CodeIntIds.Contains(vid.ApprovalId))
                     || (!p.VoluteerApprovalIds.Any() && CodeIntIds.Contains(0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression VolAppStatusCode()
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Volunteers.Any(v => CodeIntIds.Contains(v.StatusId ?? 0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression VolunteerProcessedDateMonthsAgo()
        {
            var dt = DateTime.Parse("1/1/1900");
            var months = TextValue.ToInt();
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffMonth(p.Volunteers.Max(v => v.ProcessedDate) ?? dt, Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(months, typeof(int?));
            return Compare(parm, left, op, right);
        }
        internal Expression MVRStatusCode()
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Volunteers.Any(v => CodeIntIds.Contains(v.MVRStatusId ?? 0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression MVRProcessedDateMonthsAgo()
        {
            var dt = DateTime.Parse("1/1/1900");
            var months = TextValue.ToInt();
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffMonth(p.Volunteers.Max(v => v.MVRProcessedDate) ?? dt, Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(months, typeof(int?));
            return Compare(parm, left, op, right);
        }
        internal Expression BackgroundCheckStatus()
        {
            var labels = Tags; 
            var usernameOrPeopleId = Quarters;
            var ids = CodeIntIds;
            int[] lab = (labels ?? "99").Split(',').Select(vv => vv.ToInt()).ToArray();
            var pid = usernameOrPeopleId.ToInt();
            var user = "";
            if (usernameOrPeopleId.HasValue())
                if (pid == 0)
                    user = usernameOrPeopleId ?? "";

            Expression<Func<Person, bool>> pred = p =>
              p.BackgroundChecks.Any(bc =>
                  ids.Contains(bc.StatusID)
                  && (user == "" || bc.User.Users.Any(uu => uu.Username == user))
                  && (pid == 0 || bc.UserID == pid)
                  && (lab.Contains(bc.ReportLabelID) || lab.Length == 0 || lab[0] == 99)
                  );
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
