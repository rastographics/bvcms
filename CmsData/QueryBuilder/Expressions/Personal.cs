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
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression Birthday()
        {
            var dob = TextValue;
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
        internal Expression WeddingDate()
        {
            Expression<Func<Person, bool>> pred = p => false; // default
            if (!TextValue.HasValue())
            {
                pred = p => p.WeddingDate == null;
            }
            else
            {
                DateTime weddingdate;
                if (DateTime.TryParse(TextValue, out weddingdate))
                    if (Regex.IsMatch(TextValue, @"\d+/\d+/\d+"))
                        pred = p => p.WeddingDate == weddingdate;
                    else
                        pred =
                            p =>
                                p.WeddingDate.Value.Day == weddingdate.Day &&
                                p.WeddingDate.Value.Month == weddingdate.Month;
                else
                {
                    int y;
                    if (int.TryParse(TextValue, out y))
                        if (y <= 12 && y > 0)
                            pred = p => p.WeddingDate.Value.Month == y;
                        else
                            pred = p => p.WeddingDate.Value.Year == y;
                }
            }
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression WidowedDate()
        {
            Expression<Func<Person, DateTime?>> pred = p =>
                db.WidowedDate(p.PeopleId);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(DateValue, typeof(DateTime?));
            return Compare(left, right);
        }
        internal Expression DaysTillBirthday()
        {
            var days = TextValue.ToInt();
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(Util.Now.Date, db.NextBirthday(p.PeopleId));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, right);
        }
        internal Expression DaysTillAnniversary()
        {
            var days = TextValue.ToInt();
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(Util.Now.Date, db.NextAnniversary(p.PeopleId));
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, right);
        }
        internal Expression HasPicture()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p => p.PictureId != null;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
