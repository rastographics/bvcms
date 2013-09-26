/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq.Expressions;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using System.Text.RegularExpressions;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression Birthday(
            ParameterExpression parm,
            CompareType op,
            string dob)
        {
            Expression<Func<Person, bool>> pred = p => false; // default
            DateTime dt;
            if (DateTime.TryParse(dob, out dt))
                if (Regex.IsMatch(dob, @"\d+/\d+/\d+"))
                    pred = p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month && p.BirthYear == dt.Year;
                else
                    pred = p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month;
            else
            {
                int y;
                if (int.TryParse(dob, out y))
                    if (y <= 12 && y > 0)
                        pred = p => p.BirthMonth == y;
                    else
                        pred = p => p.BirthYear == y;
            }
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression WeddingDate(
            ParameterExpression parm,
            CompareType op,
            string wed)
        {
            Expression<Func<Person, bool>> pred = p => false; // default
            DateTime dt;
            if (DateTime.TryParse(wed, out dt))
                if (Regex.IsMatch(wed, @"\d+/\d+/\d+"))
                    pred = p => p.WeddingDate == dt;
                else
                    pred = p => p.WeddingDate.Value.Day == dt.Day && p.WeddingDate.Value.Month == dt.Month;
            else
            {
                int y;
                if (int.TryParse(wed, out y))
                    if (y <= 12 && y > 0)
                        pred = p => p.WeddingDate.Value.Month == y;
                    else
                        pred = p => p.WeddingDate.Value.Year == y;
            }
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression WidowedDate(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            DateTime? date)
        {
            Expression<Func<Person, DateTime?>> pred = p =>
                Db.WidowedDate(p.PeopleId);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(date, typeof(DateTime?));
            return Compare(left, op, right);
        }
        internal static Expression DaysTillBirthday(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(Util.Now.Date, Db.NextBirthday(p.PeopleId));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression DaysTillAnniversary(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            int days)
        {
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(Util.Now.Date, Db.NextAnniversary(p.PeopleId));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, op, right);
        }
        internal static Expression HasPicture(ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p => p.PictureId != null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
