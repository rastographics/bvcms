/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CmsData.API;
using UtilityExtensions;
using System.Configuration;
using System.Reflection;
using System.Collections;
using System.Data.Linq.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using CmsData.Codes;

namespace CmsData
{
    internal static partial class Expressions
    {
        internal static Expression RecentContributionCount( ParameterExpression parm, CMSDataContext Db,
            int days, int fund, CompareType op, int cnt)
        {
            return RecentContributionCount2(parm, Db, days, fund, op, cnt, taxnontax: false);
        }
        internal static Expression RecentContributionAmount( ParameterExpression parm, CMSDataContext Db,
            int days, int fund, CompareType op, decimal amt)
        {
            return RecentContributionAmount2(parm, Db, days, fund, op, amt, taxnontax: false);
        }
        internal static Expression RecentNonTaxDedCount( ParameterExpression parm, CMSDataContext Db,
            int days, int fund, CompareType op, int cnt)
        {
            return RecentContributionCount2(parm, Db, days, fund, op, cnt, taxnontax: true);
        }
        internal static Expression RecentNonTaxDedAmount( ParameterExpression parm, CMSDataContext Db,
            int days, int fund, CompareType op, decimal amt)
        {
            return RecentContributionAmount2(parm, Db, days, fund, op, amt, taxnontax: true);
        }
        private static Expression RecentContributionCount2(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            int cnt,
            bool taxnontax)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() > cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0 || cnt == 0
                        group c by c.CreditGiverId into g
                        where g.Count() >= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() < cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() <= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (cnt == 0) // This is a very special case, use different approach
                    {
                        q = from pid in Db.Contributions0(dt, now, fund, 0, false, taxnontax, true)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() == cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() != cnt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        private static Expression RecentContributionAmount2(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            decimal amt,
            bool taxnontax)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (amt == 0) // This is a very special case, use different approach
                    {
                        q = from pid in Db.Contributions0(dt, now, fund, 0, false, taxnontax, true)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentPledgeCount(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            int cnt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);

            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() > cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() >= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() < cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() <= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (cnt == 0) // special case, use different approach
                    {
                        q = from pid in Db.Pledges0(dt, now, fund, 0)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() == cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() != cnt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentPledgeAmount(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            decimal amt)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) < amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }

        internal static Expression ContributionAmount(
            ParameterExpression parm, CMSDataContext Db,
            DateTime? start,
            DateTime? end,
            int fund,
            CompareType op,
            decimal amt)
        {
            return ContributionAmount2(parm, Db, start, end, fund, op, amt, false);
        }
        internal static Expression NonTaxDedAmount(
            ParameterExpression parm, CMSDataContext Db,
            DateTime? start,
            DateTime? end,
            int fund,
            CompareType op,
            decimal amt)
        {
            return ContributionAmount2(parm, Db, start, end, fund, op, amt, true);
        }

        private static Expression ContributionAmount2(
            ParameterExpression parm, CMSDataContext Db,
            DateTime? start,
            DateTime? end,
            int fund,
            CompareType op,
            decimal amt,
            bool nontaxded)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.GreaterEqual:
                    q = from c in Db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Greater:
                    q = from c in Db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in Db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in Db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) < amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (amt == 0) // This is a very special case, use different approach
                    {
                        q = from pid in Db.Contributions0(start, end, fund, 0, false, nontaxded, true)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in Db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in Db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression ContributionChange(
            ParameterExpression parm, CMSDataContext Db,
            DateTime? dt1,
            DateTime? dt2,
            CompareType op,
            double pct)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);
            var q = Db.GivingCurrentPercentOfFormer(dt1, dt2,
                op == CompareType.Greater ? ">" :
                op == CompareType.GreaterEqual ? ">=" :
                op == CompareType.Less ? "<" :
                op == CompareType.LessEqual ? "<=" :
                op == CompareType.Equal ? "=" : "<>", pct);
            var tag = Db.PopulateTemporaryTag(q.Select(pp => pp.Pid));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentHasIndContributions(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            CompareType op,
            bool tf)
        {
            if(!Db.FromActiveRecords)
                if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse(parm);
            var now = DateTime.Now;
            var dt = now.AddDays(-days);
            Expression<Func<Person, bool>> pred = p =>
                           p.Contributions.Any(cc => cc.ContributionDate > dt && cc.ContributionAmount > 0 && !ContributionTypeCode.ReturnedReversedTypes.Contains(cc.ContributionTypeId));
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression RecentGivingAsPctOfPrevious(
            ParameterExpression parm, CMSDataContext Db,
            int Days,
            CompareType op,
            double pct)
        {
            var dt1 = DateTime.Today.AddDays(-Days * 2);
            var dt2 = DateTime.Today.AddDays(-Days);
            var q = Db.GivingCurrentPercentOfFormer(dt1, dt2, op == CompareType.Greater ? ">" : "<=", pct);
            var tag = Db.PopulateTemporaryTag(q.Select(pp => pp.Pid));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression RecentFirstTimeGiver(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            int fund,
            CompareType op,
            bool tf)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return AlwaysFalse(parm);

            var q = from f in Db.FirstTimeGivers(days, fund)
                    select f.PeopleId;

            var tag = Db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal static Expression IsTopGiver(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            string top,
            CompareType op,
            bool tf)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return Expressions.CompareConstant(parm, "PeopleId", CompareType.Equal, 0);

            var mindt = Util.Now.AddDays(-days).Date;
            var r = Db.TopGivers(top.ToInt(), mindt, DateTime.Now).ToList();
            var topgivers = r.Select(g => g.PeopleId).ToList();
            Expression<Func<Person, bool>> pred = p =>
                topgivers.Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression IsTopPledger(
            ParameterExpression parm, CMSDataContext Db,
            int days,
            string top,
            CompareType op,
            bool tf)
        {
            if (Db.CurrentUser == null || Db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return Expressions.CompareConstant(parm, "PeopleId", CompareType.Equal, 0);

            var mindt = Util.Now.AddDays(-days).Date;
            var r = Db.TopPledgers(top.ToInt(), mindt, DateTime.Now).ToList();
            var toppledgers = r.Select(g => g.PeopleId).ToList();
            Expression<Func<Person, bool>> pred = p =>
                toppledgers.Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal static Expression HasManagedGiving(
            ParameterExpression parm, CMSDataContext Db,
            CompareType op,
            bool tf)
        {
            Expression<Func<Person, bool>> pred = p => (from e in p.RecurringAmounts
                                                        where e.Amt > 0
                                                        select e).Any();

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
