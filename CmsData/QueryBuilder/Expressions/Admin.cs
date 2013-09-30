/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using CmsData.Codes;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression UserRole(
            ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Users.Any(u => u.UserRoles.Any(ur => ids.Contains(ur.RoleId))
                    || (!u.UserRoles.Any() && ids.Contains(0))
                );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsUser(
           ParameterExpression parm,
           CompareType op,
           bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.Users.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression CreatedBy(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            string name)
        {
            Expression<Func<Person, string>> pred = p =>
                Db.People.FirstOrDefault(u => u.Users.Any(u2 => u2.UserId == p.CreatedBy)).Name2;
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(name, typeof(string));
            return Compare(left, op, right);
        }
        internal static Expression RecentCreated(
            ParameterExpression parm,
            int days,
            CompareType op,
            bool tf)
        {
            var dt = DateTime.Today.AddDays(-days);
            Expression<Func<Person, bool>> pred = p => p.CreatedDate >= dt;
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsCurrentPerson(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.PeopleId == Db.CurrentPeopleId;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression DuplicateEmails(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    p.EmailAddress != null && p.EmailAddress != ""
                    && Db.People.Any(pp => pp.PeopleId != p.PeopleId && pp.EmailAddress == p.EmailAddress);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression DuplicateNames(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    Db.People.Any(pp => pp.PeopleId != p.PeopleId && pp.FirstName == p.FirstName && pp.LastName == p.LastName);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasLowerName(CMSDataContext Db,
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p =>
                    Db.StartsLower(p.FirstName).Value
                    || Db.StartsLower(p.LastName).Value;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression PeopleIds(
            ParameterExpression parm,
            CompareType op,
            int[] ids)
        {
            Expression<Func<Person, bool>> pred = p => ids.Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression MatchAnything(
            ParameterExpression parm)
        {
            Expression<Func<Person, bool>> pred = p => true;
            return Expression.Invoke(pred, parm);
        }
        internal static Expression HasEmailOptout(
            ParameterExpression parm,
            DateTime? to,
            CompareType op,
            string email)
        {
            Expression<Func<Person, bool>> pred = p =>
                (from oo in p.EmailOptOuts
                 where email == null || email == "" || oo.FromEmail == email
                 where @to == null || (oo.DateX >= @to)
                 select oo).Any();
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression MeetingId(
            ParameterExpression parm,
            CompareType op,
            int id)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Attends.Any(a =>
                    (a.AttendanceFlag == true)
                    && a.MeetingId == id
                    );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RegisteredForMeetingId(
            ParameterExpression parm,
            CompareType op,
            int id)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Attends.Any(a =>
                    (a.Commitment == AttendCommitmentCode.Attending
                    || a.Commitment == AttendCommitmentCode.Substitute
                    || a.Commitment == AttendCommitmentCode.FindSub)
                    && a.MeetingId == id
                    );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
