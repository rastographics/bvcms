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
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression HasTaskWithName()
        {
            var task = TextValue ?? "";
            Expression<Func<Person, bool>> pred = p =>
                p.TasksAboutPerson.Any(t => t.Description.Contains(task)
                    && t.StatusId != TaskStatusCode.Complete);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasTaskWithNotes()
        {
            var phrase = TextValue ?? "";
            Expression<Func<Person, bool>> pred = p =>
                p.TasksAboutPerson.Any(t => t.Notes.Contains(phrase)
                    && t.StatusId != TaskStatusCode.Complete);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasIncompleteTask()
        {
            var task = TextValue;
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
        internal Expression DaysSinceContact()
        {
            var days = TextValue.ToInt();
            Expression<Func<Person, bool>> hadcontact = p => p.contactsHad.Any();
            var dt = Util.Now.Date;
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(p.contactsHad.Max(cc => cc.contact.ContactDate), dt);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            var expr1 = Expression.Invoke(hadcontact, parm);
            return Expression.And(expr1, Compare(left, right));
        }
        internal Expression RecentContactMinistry()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && CodeIntIds.Contains(a.contact.MinistryId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentContactType()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && CodeIntIds.Contains(a.contact.ContactTypeId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentContactReason()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(a => a.contact.ContactDate >= mindt
                    && CodeIntIds.Contains(a.contact.ContactReasonId ?? 0));
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentEmailCount()
        {
            var cnt = TextValue.ToInt();
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, int>> pred = p =>
                p.EmailQueueTos.Count(e => e.Sent >= mindt);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression RecentEmailSentCount()
        {
            var cnt = TextValue.ToInt();
            var mindt = Util.Now.AddDays(-Days).Date;
            Expression<Func<Person, int>> pred = p =>
                p.EmailQueues.Count(e => e.Sent >= mindt && (e.Transactional ?? false) == false);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(cnt), left.Type);
            return Compare(left, right);
        }
        internal Expression EmailRecipient()
        {
            var id = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = p =>
                p.EmailQueueTos.Any(e => e.Id == id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression ContactRecipient()
        {
            var id = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = p =>
                p.contactsHad.Any(c => c.ContactId == id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression ContactMaker()
        {
            var id = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = p =>
                p.contactsMade.Any(c => c.ContactId == id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression MadeContactTypeAsOf()
        {
            //StartDate, EndDate, Program, CodeIntIds
            var to = (EndDate ?? StartDate ?? Util.Now).AddDays(1);

            Expression<Func<Person, bool>> pred = p => (
                from c in p.contactsMade
                where c.contact.MinistryId == MinistryInt || MinistryInt == 0
                where CodeIntIds.Contains(c.contact.ContactTypeId ?? 0) || CodeIntIds.Length == 0 || CodeIntIds[0] == 0
                where StartDate == null || StartDate <= c.contact.ContactDate
                where c.contact.ContactDate <= to
                select c
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasContacts()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p => p.contactsHad.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression FamilyHasContacts()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p => p.Family.People.Any(pp => pp.contactsHad.Any());
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
