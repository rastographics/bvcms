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
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        private Expression RecentContributionCount2(int days, int fund, int cnt, bool taxnontax)
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var now = Util.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() > cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0 || cnt == 0
                        group c by c.CreditGiverId into g
                        where g.Count() >= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() < cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() <= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    //if (cnt == 0) // This is a very special case, use different approach
                    //{
                    //    q = from pid in db.Contributions0(dt, now, fund, 0, false, taxnontax, true)
                    //        select pid.PeopleId;
                    //    Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                    //    Expression expr0 = System.Linq.Expressions.Expression.Invoke(pred0, parm);
                    //    return expr0;
                    //}
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() == cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() != cnt
                        select g.Key ?? 0;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        public Expression IsRecentGiver()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();

            var tf = CodeIds == "1";
            var fundid = Quarters.ToInt();
            var q = fundid > 0
                ? db.RecentGiverFund(Days, fundid).Select(v => v.PeopleId.Value)
                : db.RecentGiver(Days).Select(v => v.PeopleId.Value);
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => op == CompareType.Equal && tf
                ? db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId)
                : !db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId);

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        private Expression RecentContributionAmount2(int days, int fund, decimal amt, bool taxnontax)
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var now = Util.Now;
            var dt = now.AddDays(-days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (amt == 0)
                        q = from pid in db.Contributions0(dt, now, fund, 0, false, taxnontax, true)
                            select pid.PeopleId.Value;
                    else
                        q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                            where fund == 0 || c.FundId == fund
                            where c.Amount > 0
                            group c by c.CreditGiverId into g
                            where g.Sum(cc => cc.Amount) == amt
                            select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.Contributions2(dt, now, 0, false, taxnontax, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        private Expression ContributionAmount2(DateTime? start, DateTime? end, int fund, decimal amt, bool nontaxded)
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.GreaterEqual:
                    q = from c in db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Greater:
                    q = from c in db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) < amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (amt == 0) // This is a special case, use different approach
                        q = from pid in db.Contributions0(start, end, fund, 0, false, nontaxded, true)
                            select pid.PeopleId.Value;
                    else
                        q = from c in db.Contributions2(start, end, 0, false, nontaxded, true)
                            where fund == 0 || c.FundId == fund
                            group c by c.CreditGiverId into g
                            where g.Sum(cc => cc.Amount) == amt
                            select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.Contributions2(start, end, 0, false, nontaxded, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.Amount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentContributionCount()
        {
            var fund = Quarters.ToInt();
            var cnt = TextValue.ToInt();
            return RecentContributionCount2(Days, fund, cnt, false);
        }
        internal Expression RecentContributionAmount()
        {
            var fund = Quarters.ToInt();
            var amt = decimal.Parse(TextValue);
            return RecentContributionAmount2(Days, fund, amt, false);
        }
        internal Expression RecentContributionAmountBothJoint()
        {
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            return ContributionAmountBothJoint(dt, now);
        }
        internal Expression ContributionAmountBothJointHistory()
        {
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            return ContributionAmountBothJoint(StartDate, enddt);

        }
        private Expression ContributionAmountBothJoint(DateTime? startdt, DateTime? enddt)
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var amt = decimal.Parse(TextValue);
            var fund = Quarters.ToInt2();

            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.GetContributionTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Amount > amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.GetContributionTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Amount >= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.Less:
                    q = from c in db.GetContributionTotalsBothIfJoint(StartDate, enddt, fund)
                        where c.Amount > 0
                        where c.Amount <= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.GetContributionTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Amount > 0
                        where c.Amount <= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.Equal:
                    if (amt == 0)
                        q = from pid in db.Contributions0(startdt, enddt, fund, 0, false, false, true)
                            select pid.PeopleId.Value;
                    else
                        q = from c in db.GetContributionTotalsBothIfJoint(startdt, enddt, fund)
                            where c.Amount > 0
                            where c.Amount == amt
                            select c.PeopleId.Value;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.GetContributionTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Amount > 0
                        where c.Amount != amt
                        select c.PeopleId.Value;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentPledgeAmountBothJoint()
        {
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            return PledgeAmountBothJoint(dt, now);
        }
        internal Expression PledgeAmountBothJointHistory()
        {
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            return PledgeAmountBothJoint(StartDate, enddt);

        }
        private Expression PledgeAmountBothJoint(DateTime? startdt, DateTime? enddt)
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var amt = decimal.Parse(TextValue);
            var fund = Quarters.ToInt2();

            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.PledgeAmount > amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.PledgeAmount.Value >= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.Less:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.PledgeAmount.Value > 0
                        where c.PledgeAmount.Value <= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.PledgeAmount.Value > 0
                        where c.PledgeAmount.Value <= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.Equal:
                    if (amt == 0)
                        q = from pid in db.Pledges0(startdt, enddt, fund, 0)
                            select pid.PeopleId;
                    else
                        q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                            where c.PledgeAmount.Value > 0
                            where c.PledgeAmount.Value == amt
                            select c.PeopleId.Value;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.PledgeAmount.Value > 0
                        where c.PledgeAmount.Value != amt
                        select c.PeopleId.Value;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentPledgeBalanceBothJoint()
        {
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            return PledgeBalanceBothJoint(dt, now);
        }
        internal Expression PledgeBalanceBothJointHistory()
        {
            DateTime? enddt = null;
            if (!EndDate.HasValue && StartDate.HasValue)
                    enddt = StartDate.Value.AddHours(24);
            if(EndDate.HasValue)
                enddt = EndDate.Value.AddHours(24);
            return PledgeBalanceBothJoint(StartDate, enddt);

        }
        private Expression PledgeBalanceBothJoint(DateTime? startdt, DateTime? enddt)
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var amt = decimal.Parse(TextValue);
            var fund = Quarters.ToInt2();

            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Balance > amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Balance >= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.Less:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Balance <= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Balance <= amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.Equal:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Balance == amt
                        select c.PeopleId.Value;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.GetPledgedTotalsBothIfJoint(startdt, enddt, fund)
                        where c.Balance != amt
                        select c.PeopleId.Value;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentNonTaxDedCount()
        {
            var fund = Quarters.ToInt();
            var cnt = TextValue.ToInt();
            return RecentContributionCount2(Days, fund, cnt, true);
        }
        internal Expression RecentNonTaxDedAmount()
        {
            var fund = Quarters.ToInt();
            var amt = decimal.Parse(TextValue);
            return RecentContributionAmount2(Days, fund, amt, true);
        }
        internal Expression RecentPledgeCount()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();

            var fund = Quarters.ToInt();
            var cnt = TextValue.ToInt();
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() > cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() >= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() < cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() <= cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    if (cnt == 0) // special case, use different approach
                    {
                        q = from pid in db.Pledges0(dt, now, fund, 0)
                            select pid.PeopleId;
                        Expression<Func<Person, bool>> pred0 = p => q.Contains(p.PeopleId);
                        Expression expr0 = Expression.Invoke(pred0, parm);
                        return expr0;
                    }
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() == cnt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.PledgeAmount > 0
                        group c by c.CreditGiverId into g
                        where g.Count() != cnt
                        select g.Key ?? 0;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentPledgeAmount()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var fund = Quarters.ToInt();
            var amt = decimal.Parse(TextValue);
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Greater:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) > amt
                        select g.Key ?? 0;
                    break;
                case CompareType.GreaterEqual:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) >= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Less:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) < amt
                        select g.Key ?? 0;
                    break;
                case CompareType.LessEqual:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) <= amt
                        select g.Key ?? 0;
                    break;
                case CompareType.Equal:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) == amt
                        select g.Key ?? 0;
                    break;
                case CompareType.NotEqual:
                    q = from c in db.Contributions2(dt, now, 0, true, false, true)
                        where fund == 0 || c.FundId == fund
                        where c.Amount > 0
                        group c by c.CreditGiverId into g
                        where g.Sum(cc => cc.PledgeAmount) != amt
                        select g.Key ?? 0;
                    break;
            }
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression ContributionAmount()
        {
            var fund = Quarters.ToInt();
            var amt = decimal.Parse(TextValue);
            return ContributionAmount2(StartDate, EndDate, fund, amt, false);
        }
        internal Expression NonTaxDedAmount()
        {
            var fund = Quarters.ToInt();
            var amt = decimal.Parse(TextValue);
            return ContributionAmount2(StartDate, EndDate, fund, amt, true);
        }
        internal Expression ContributionChange()
        {
            var pct = double.Parse(TextValue);
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var q = db.GivingCurrentPercentOfFormer(StartDate, EndDate,
                op == CompareType.Greater ? ">" :
                op == CompareType.GreaterEqual ? ">=" :
                op == CompareType.Less ? "<" :
                op == CompareType.LessEqual ? "<=" :
                op == CompareType.Equal ? "=" : "<>", pct);
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.Pid));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentHasIndContributions()
        {
            var tf = CodeIds == "1";
            if (!db.FromActiveRecords && !db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            Expression<Func<Person, bool>> pred = p =>
                p.Contributions.Any(cc => 
                            cc.ContributionDate > dt 
                            && cc.ContributionAmount > 0 
                            && cc.ContributionStatusId == ContributionStatusCode.Recorded
                            && !ContributionTypeCode.ReturnedReversedTypes.Contains(cc.ContributionTypeId));
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HadIndContributions()
        {
            var tf = CodeIds == "1";
            if (!db.FromActiveRecords && !db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            Expression<Func<Person, bool>> pred = p =>
                           p.Contributions.Any(cc => cc.ContributionDate > StartDate
                               && cc.ContributionDate <= EndDate
                               && cc.ContributionAmount > 0 
                               && !ContributionTypeCode.ReturnedReversedTypes.Contains(cc.ContributionTypeId));
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentHasFailedRecurringGiving()
        {
            var tf = CodeIds == "1";
            if (!db.FromActiveRecords && !db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            Expression<Func<Person, bool>> pred = p =>
                (from f in db.ViewFailedRecurringGivings
                 where f.Dt >= dt
                 select f.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentBundleType()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var now = Util.Now;
            var dt = now.AddDays(-Days);
            Expression<Func<Person, bool>> pred = p =>
            (
                from c in p.Contributions
                where c.ContributionDate > dt
                where c.ContributionAmount > 0
                where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                where c.BundleDetails.Any(cc => CodeIntIds.Contains(cc.BundleHeader.BundleHeaderTypeId))
                select c
            ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression GivingChange()
        {
            var days = Quarters.ToInt2() ?? 365;
            var pct = decimal.Parse(TextValue);
            var q = db.GivingChange(days);
            switch (op)
            {
                case CompareType.Greater:
                    q = q.Where(vv => vv.PctChange > pct);
                    break;
                case CompareType.GreaterEqual:
                    q = q.Where(vv => vv.PctChange >= pct);
                    break;
                case CompareType.Less:
                    q = q.Where(vv => vv.PctChange < pct);
                    break;
                case CompareType.LessEqual:
                    q = q.Where(vv => vv.PctChange <= pct);
                    break;
                case CompareType.Equal:
                    q = q.Where(vv => vv.PctChange == pct);
                    break;
                case CompareType.NotEqual:
                    q = q.Where(vv => vv.PctChange != pct);
                    break;
            }
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId));
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression IsFamilyGiver()
        {
            var tf = CodeIds == "1";
            var fundid = Quarters.ToInt2();
            var td = Util.Now;
            var fd = td.AddDays(Days == 0 ? -365 : -Days);
            var q = db.FamilyGiver(fd, td, fundid).Where(vv => vv.FamGive == true);
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId));

            Expression<Func<Person, bool>> pred = p => op == CompareType.Equal && tf
                ? db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId)
                : !db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId);

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression IsFamilyPledger()
        {
            var tf = CodeIds == "1";
            var fundid = Quarters.ToInt2();
            var td = Util.Now;
            var fd = td.AddDays(Days == 0 ? -365 : -Days);
            var q = db.FamilyGiver(fd, td, fundid).Where(vv => vv.FamPledge == true);
            var tag = db.PopulateTemporaryTag(q.Select(pp => pp.PeopleId));
            Expression<Func<Person, bool>> pred = p => op == CompareType.Equal && tf
                ? db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId)
                : !db.TagPeople.Where(vv => vv.Id == tag.Id).Select(vv => vv.PeopleId).Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression RecentFirstTimeGiver()
        {
            var tf = CodeIds == "1";
            var fund = Quarters.ToInt();

            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();

            var q = db.FirstTimeGivers(Days, fund).Select(p => p.PeopleId);

            //var tag = Db.PopulateTemporaryTag(q);
            //            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            //            Expression expr = Expression.Invoke(pred, parm);
            //            return expr;

            Expression<Func<Person, bool>> pred;
            if (op == CompareType.Equal ^ tf)
                pred = p => !q.Contains(p.PeopleId);
            else
                pred = p => q.Contains(p.PeopleId);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression IsTopGiver()
        {
            var top = Quarters.ToInt();
            var tf = CodeIds == "1";
            if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return CompareConstant(parm, "PeopleId", CompareType.Equal, 0);

            var mindt = Util.Now.AddDays(-Days).Date;
            var r = db.TopGivers(top, mindt, Util.Now).ToList();
            var topgivers = r.Select(g => g.PeopleId).ToList();
            Expression<Func<Person, bool>> pred = p =>
                topgivers.Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsTopPledger()
        {
            var tf = CodeIds == "1";
            var top = Quarters.ToInt();
            if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                return CompareConstant(parm, "PeopleId", CompareType.Equal, 0);

            var mindt = Util.Now.AddDays(-Days).Date;
            var r = db.TopPledgers(top, mindt, Util.Now).ToList();
            var toppledgers = r.Select(g => g.PeopleId).ToList();
            Expression<Func<Person, bool>> pred = p =>
                toppledgers.Contains(p.PeopleId);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression HasManagedGiving()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var tf = CodeIds == "1";
            var fundid = Quarters.ToInt2();
            Expression<Func<Person, bool>> pred = p => (from e in p.RecurringAmounts
                                                        where e.Amt > 0
                                                        where fundid == null || fundid == e.FundId
                                                        select e).Any();

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression ManagedGivingCreditCard()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p => (from e in p.RecurringAmounts
                                                        let pi = p.PaymentInfos.SingleOrDefault()
                                                        where e.Amt > 0
                                                        where pi.PreferredGivingType == "C"
                                                        select e).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (!(op == CompareType.Equal && tf))
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression CcExpiration()
        {
            if (!db.FromBatch)
                if (db.CurrentUser == null || db.CurrentUser.Roles.All(rr => rr != "Finance"))
                    return AlwaysFalse();
            var tf = CodeIds == "1";
            var cclist = (from e in db.PaymentInfos
                          join m in db.ManagedGivings on e.PeopleId equals m.PeopleId
                          where e.Expires.Length > 0
                          where e.MaskedCard.Length > 0
                          select new { e.Expires, e.PeopleId }).ToList();
            var pids = from i in cclist
                       let d = DbUtil.NormalizeExpires(i.Expires)
                       where d != null && d.Value.AddDays(-Days) <= DateTime.Today
                       select i.PeopleId;
            var tag = db.PopulateTempTag(pids);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression WantsElectronicStatement()
        {
            Expression<Func<Person, bool>> pred = p => p.ElectronicStatement == true;

            if (op == CompareType.Equal)
                pred = p => p.ElectronicStatement == (CodeIds == "1");

            else if (op == CompareType.NotEqual)
                if (CodeIds == "1")
                    pred = p => (p.ElectronicStatement ?? false) != true; // gets falses and nulls
                else if (CodeIds == "0")
                    pred = p => (p.ElectronicStatement ?? true); // get trues and nulls

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            return expr;
        }
    }
}
