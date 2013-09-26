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
using System.Data.Linq.SqlClient;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression HasVolunteerApplications(ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.Volunteers.Any(v => v.VolunteerForms.Any());
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression VolunteerApprovalCode(ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred =
                p => p.VoluteerApprovalIds.Any(vid => ids.Contains(vid.ApprovalId))
                     || (!p.VoluteerApprovalIds.Any() && ids.Contains(0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression VolAppStatusCode(ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Volunteers.Any(v => ids.Contains(v.StatusId ?? 0));
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression VolunteerProcessedDateMonthsAgo(ParameterExpression parm,
            CompareType op,
            int months)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffMonth(p.Volunteers.Max(v => v.ProcessedDate), Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(months, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression BackgroundCheckStatus(ParameterExpression parm,
            string labels,
            string usernameOrPeopleId,
            CompareType op,
            int[] ids)
        {
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
        //internal static Expression HasVolunteered(ParameterExpression parm,
        //	string View,
        //	CompareType op,
        //	bool tf)
        //{
        //	Expression<Func<Person, bool>> pred;
        //	if (View == "ns")
        //		pred = p => p.VolInterestInterestCodes.Count() > 0;
        //	else
        //	{
        //		var orgkeys = Person.OrgKeys(View);
        //		pred = p =>
        //			  p.VolInterestInterestCodes.Any(vi => orgkeys.Contains(vi.VolInterestCode.Org));
        //	}
        //	Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        //	if (!(op == CompareType.Equal && tf))
        //		expr = Expression.Not(expr);
        //	return expr;
        //}
    }
}
