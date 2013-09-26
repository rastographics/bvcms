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
using CmsData.Codes;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression HasTaskWithName(
            ParameterExpression parm,
            CompareType op,
            string task)
        {
            if (task == null)
                task = "";
            Expression<Func<Person, bool>> pred = p =>
                p.TasksAboutPerson.Any(t => t.Description.Contains(task)
                    && t.StatusId != TaskStatusCode.Complete);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasIncompleteTask(
            ParameterExpression parm,
            CompareType op,
            string task)
        {
            var empty = !task.HasValue();
            Expression<Func<Person, bool>> pred = p => (
                from t in p.TasksCoOwned
                where empty || t.Description.Contains(task) || t.Owner.Name.Contains(task)
                where t.StatusId != TaskStatusCode.Complete
                select t).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression DaysSinceContact(
            ParameterExpression parm,
            CompareType op,
            int days)
        {
            Expression<Func<Person, bool>> hadcontact = p => p.contactsHad.Count() > 0;
            var dt = Util.Now.Date;
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(p.contactsHad.Max(cc => cc.contact.ContactDate), dt);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            var expr1 = Expression.Invoke(hadcontact, parm);
            return Expression.And(expr1, Compare(left, op, right));
        }
        internal static Expression RecentContactMinistry(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && ids.Contains(a.contact.MinistryId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentContactType(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && ids.Contains(a.contact.ContactTypeId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentContactReason(
            ParameterExpression parm,
            int days,
            CompareType op,
            int[] ids)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && ids.Contains(a.contact.ContactReasonId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentEmailCount(
            ParameterExpression parm,
            int days,
            CompareType op,
            int cnt)
        {
            var mindt = Util.Now.AddDays(-days).Date;
            Expression<Func<Person, int>> pred = p =>
                p.EmailQueueTos.Count(e => e.Sent >= mindt);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, op, right);
        }
        internal static Expression EmailRecipient(
            ParameterExpression parm,
            CompareType op,
            int id)
        {
            Expression<Func<Person, bool>> pred = p =>
                p.EmailQueueTos.Any(e => e.Id == id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression MadeContactTypeAsOf(
            ParameterExpression parm,
            DateTime? from,
            DateTime? to,
            int? ministryid,
            CompareType op,
            params int[] ids)
        {
            to = to.HasValue ? to.Value.AddDays(1) : from.Value.AddDays(1);

            Expression<Func<Person, bool>> pred = p => (
                from c in p.contactsMade
                where c.contact.MinistryId == ministryid || ministryid == 0
                where ids.Contains(c.contact.ContactTypeId ?? 0) || ids.Length == 0 || ids[0] == 0
                where @from <= c.contact.ContactDate // where it ends
                where c.contact.ContactDate <= to // where it begins
                select c
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasContacts(
            ParameterExpression parm,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p => p.contactsHad.Count() > 0;
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
