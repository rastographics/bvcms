using CmsData;
using CmsData.View;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class TransactionsModel
    {
        private int? _count;
        private IQueryable<TransactionList> _transactions;
        public int nameid;

        public TransactionsModel(int? tranid, string reference = "", string desc = "")
            : this()
        {
            name = tranid.ToString();
            if (!tranid.HasValue)
            {
                GoerId = null;
            }

            if (!string.IsNullOrWhiteSpace(reference))
            {
                name = reference;
            }
            if (!string.IsNullOrWhiteSpace(desc))
            {
                description = desc;
            }
        }

        public TransactionsModel()
        {
            Pager = new PagerModel2(Count);
            Pager.Sort = "Date";
            Pager.Direction = "desc";
            finance = HttpContextFactory.Current.User.IsInRole("Finance");
            admin = HttpContextFactory.Current.User.IsInRole("Admin") || HttpContextFactory.Current.User.IsInRole("ManageTransactions");
        }

        public string description { get; set; }
        public string name { get; set; }
        public string Submit { get; set; }
        public decimal? gtamount { get; set; }
        public decimal? ltamount { get; set; }
        public DateTime? startdt { get; set; }
        public DateTime? enddt { get; set; }
        public bool testtransactions { get; set; }
        public bool apprtransactions { get; set; }
        public bool includesadditionaldonation { get; set; }
        public bool nocoupons { get; set; }
        public string batchref { get; set; }
        public bool usebatchdates { get; set; }
        public PagerModel2 Pager { get; set; }
        public bool finance { get; set; }
        public bool admin { get; set; }
        public int? GoerId { get; set; } // for mission trip supporters of this goer
        public int? SenderId { get; set; } // for mission trip goers of this supporter

        public int Count()
        {
            if (!_count.HasValue)
            {
                _count = FetchTransactions().Count();
            }

            return _count.Value;
        }

        public IEnumerable<TransactionList> Transactions()
        {
            var q0 = ApplySort();
            q0 = q0.Skip(Pager.StartRow).Take(Pager.PageSize);
            return q0;
        }

        public TotalTransaction TotalTransactions()
        {
            var q0 = FetchTransactions();
            var q = from t in q0
                    group t by 1
                    into g
                    select new TotalTransaction
                    {
                        Amt = g.Sum(tt => tt.Amt ?? 0),
                        Amtdue = g.Sum(tt => tt.Amtdue ?? 0),
                        Donate = g.Sum(tt => tt.Donate ?? 0),
                        Count = g.Count()
                    };
            return q.FirstOrDefault();
        }

        private IQueryable<TransactionList> FetchTransactions()
        {
            if (_transactions != null)
            {
                return _transactions;
            }

            if (!name.HasValue())
            {
                name = null;
            }

            string first, last;
            Util.NameSplit(name, out first, out last);
            var hasfirst = first.HasValue();
            var roles = DbUtil.Db.CurrentRoles();
            nameid = name.ToInt();
            _transactions
                = from t in DbUtil.Db.ViewTransactionLists
                  join org in DbUtil.Db.Organizations on t.OrgId equals org.OrganizationId
                  let donate = t.Donate ?? 0
                  where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                  where t.Amt >= gtamount || gtamount == null
                  where t.Amt <= ltamount || ltamount == null
                  where description == null || t.Description.Contains(description)
                  where nameid > 0 || ((t.Testing ?? false) == testtransactions)
                  where apprtransactions == (t.Moneytran == true) || !apprtransactions
                  where (nocoupons && !t.TransactionId.Contains("Coupon")) || !nocoupons
                  where (t.Financeonly ?? false) == false || finance
                  select t;
            if (name != null)
            {
                if (name == "0")
                {
                    // special case, return no transactions, all we are interested in is the Senders on a Mission Trip
                    _transactions = from t in _transactions
                                    where t.OriginalId == nameid
                                    select t;
                }
                else
                {
                    _transactions = from t in _transactions
                                    where
                                        (
                                            (t.Last.StartsWith(last) || t.Last.StartsWith(name))
                                            && (!hasfirst || t.First.StartsWith(first) || t.Last.StartsWith(name))
                                            )
                                        || t.Batchref == name || t.TransactionId == name || t.OriginalId == nameid || t.Id == nameid
                                    select t;
                }
            }

            if (!HttpContextFactory.Current.User.IsInRole("Finance"))
            {
                _transactions = _transactions.Where(tt => (tt.Financeonly ?? false) == false);
            }

            var edt = enddt;
            if (!edt.HasValue && startdt.HasValue)
            {
                edt = startdt;
            }

            edt = edt?.AddHours(24);
            if (usebatchdates && startdt.HasValue && edt.HasValue)
            {
                // Apply an offset to the startdate to get those records that occurred prior to the batch date and haven't been batched at present
                CheckBatchDates(startdt.Value.AddDays(-7), edt.Value);
                _transactions = from t in _transactions
                                where t.Batch >= startdt || startdt == null
                                where t.Batch <= edt || edt == null
                                where t.Moneytran == true
                                select t;
            }
            else
            {
                _transactions = from t in _transactions
                                where t.TransactionDate >= startdt || startdt == null
                                where t.TransactionDate <= edt || edt == null
                                select t;
            }

            if (includesadditionaldonation)
            {
                _transactions = _transactions.Where(t => t.Donate > 0.00m);
            }
            //			var q0 = _transactions.ToList();
            //            foreach(var t in q0)
            //                Debug.WriteLine("\"{0}\"\t{1}\t{2}", t.Description, t.Id, t.Amt);
            return _transactions;
        }

        public IQueryable<BatchTranGroup> FetchBatchTransactions()
        {
            var q = from t in FetchTransactions()
                    group t by t.Batchref
                    into g
                    orderby g.First().Batch descending
                    select new BatchTranGroup
                    {
                        count = g.Count(),
                        batchdate = g.Max(gg => gg.Batch),
                        BatchRef = g.Key,
                        BatchType = g.First().Batchtyp,
                        Total = g.Sum(gg => gg.Amt ?? 0)
                    };
            return q;
        }

        public IEnumerable<DescriptionGroup> FetchTransactionsByDescription()
        {
            var q0 = FetchTransactions();
            var q = from t in q0
                    group t by t.Description
                    into g
                    orderby g.First().Batch descending
                    select new DescriptionGroup
                    {
                        count = g.Count(),
                        Description = g.Key,
                        Total = g.Sum(gg => (gg.Amt ?? 0) - (gg.Donate ?? 0))
                    };
            return q;
        }

        public IQueryable<BatchDescriptionGroup> FetchTransactionsByBatchDescription()
        {
            var q = from t in FetchTransactions()
                    group t by new { t.Batchref, t.Description }
                    into g
                    let f = g.First()
                    orderby f.Batch, f.Description descending
                    select new BatchDescriptionGroup
                    {
                        count = g.Count(),
                        batchdate = f.Batch,
                        BatchRef = f.Batchref,
                        BatchType = f.Batchtyp,
                        Description = f.Description,
                        Total = g.Sum(gg => (gg.Amt ?? 0) - (gg.Donate ?? 0))
                    };
            return q;
        }

        private void CheckBatchDates(DateTime start, DateTime end)
        {
            var gateway = DbUtil.Db.Gateway();
            if (!gateway.CanGetSettlementDates)
            {
                return;
            }

            if (gateway.UseIdsForSettlementDates)
            {
                var tranids = (from t in _transactions
                               where t.TransactionDate >= start
                               where t.TransactionDate <= end
                               where t.Settled == null
                               where t.Moneytran == true
                               select t.TransactionId).ToList();
                gateway.CheckBatchSettlements(tranids);
            }
            else
            {
                gateway.CheckBatchSettlements(start, end);
            }
        }

        public IQueryable<TransactionList> ApplySort()
        {
            var q = FetchTransactions();
            if (Pager.Direction == "asc")
            {
                switch (Pager.Sort)
                {
                    case "Id":
                        q = from t in q
                            orderby (t.OriginalId ?? t.Id), t.TransactionDate
                            select t;
                        break;
                    case "Tran Id":
                        q = from t in q
                            orderby t.TransactionId
                            select t;
                        break;
                    case "Appr":
                        q = from t in q
                            orderby t.Approved, t.TransactionDate descending
                            select t;
                        break;
                    case "Date":
                        q = from t in q
                            orderby t.TransactionDate
                            select t;
                        break;
                    case "Description":
                        q = from t in q
                            orderby t.Description, t.TransactionDate descending
                            select t;
                        break;
                    case "Name":
                        q = from t in q
                            orderby t.Name, t.First, t.Last, t.TransactionDate descending
                            select t;
                        break;
                    case "Amount":
                        q = from t in q
                            orderby t.Amt, t.TransactionDate descending
                            select t;
                        break;
                    case "Due":
                        q = from t in q
                            orderby t.TotDue, t.TransactionDate descending
                            select t;
                        break;
                }
            }
            else
            {
                switch (Pager.Sort)
                {
                    case "Id":
                        q = from t in q
                            orderby (t.OriginalId ?? t.Id) descending, t.TransactionDate descending
                            select t;
                        break;
                    case "Tran Id":
                        q = from t in q
                            orderby t.TransactionId descending
                            select t;
                        break;
                    case "Appr":
                        q = from t in q
                            orderby t.Approved descending, t.TransactionDate
                            select t;
                        break;
                    case "Date":
                        q = from t in q
                            orderby t.TransactionDate descending
                            select t;
                        break;
                    case "Description":
                        q = from t in q
                            orderby t.Description descending, t.TransactionDate
                            select t;
                        break;
                    case "Name":
                        q = from t in q
                            orderby t.Name descending, t.First descending, t.Last descending, t.TransactionDate
                            select t;
                        break;
                    case "Amount":
                        q = from t in q
                            orderby t.Amt descending, t.TransactionDate
                            select t;
                        break;
                    case "Due":
                        q = from t in q
                            orderby t.TotDue descending, t.TransactionDate
                            select t;
                        break;
                }
            }

            return q;
        }

        public DataTable ExportTransactions()
        {
            var q = FetchTransactions();

            var q2 = from t in q
                     select new
                     {
                         t.Id,
                         t.TransactionId,
                         t.Approved,
                         TranDate = t.TransactionDate.FormatDate(),
                         BatchDate = t.Batch.FormatDate(),
                         t.Batchtyp,
                         t.Batchref,
                         t.People,
                         RegAmt = (t.Amt ?? 0) - (t.Donate ?? 0),
                         Donate = t.Donate ?? 0,
                         TotalAmt = t.Amt ?? 0,
                         Amtdue = t.TotDue ?? 0,
                         t.Description,
                         t.Message,
                         FullName = Transaction.FullName(t),
                         t.Address,
                         t.City,
                         t.State,
                         t.Zip,
                         t.Fund
                     };
            return q2.ToDataTable();
        }

        public IQueryable<SupporterInfo> Supporters()
        {
            return from gs in DbUtil.Db.GoerSenderAmounts
                   where gs.GoerId == GoerId
                   where gs.SupporterId != gs.GoerId
                   let sp = DbUtil.Db.People.Single(ss => ss.PeopleId == gs.SupporterId)
                   let gp = DbUtil.Db.People.Single(ss => ss.PeopleId == gs.GoerId)
                   let o = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == gs.OrgId)
                   orderby gs.Created descending
                   select new SupporterInfo
                   {
                       gs = gs,
                       SupporterName = sp.Name,
                       SupporterId = sp.PeopleId,
                       GoerName = gp.Name,
                       GoerId = gp.PeopleId,
                       OrgId = o.OrganizationId,
                       TripName = o.OrganizationName
                   };
        }

        public List<MissionTripBalanceInfo> MissionTripBalances()
        {
            var q = from gs in DbUtil.Db.GoerSenderAmounts
                    where gs.GoerId == GoerId
                    group gs by new { gs.GoerId, gs.OrgId, gs.Organization.OrganizationName } into g
                    select new MissionTripBalanceInfo
                    {
                        GoerId = g.Key.GoerId ?? 0,
                        OrgId = g.Key.OrgId,
                        TripName = g.Key.OrganizationName,
                        Balance = OrganizationMember.AmountDue(DbUtil.Db, g.Key.OrgId, g.Key.GoerId ?? 0)
                    };
            return q.ToList().Where(vv => vv.Balance > 0).ToList();
        }

        public IQueryable<SupporterInfo> SelfSupports()
        {
            return from gs in DbUtil.Db.GoerSenderAmounts
                   where gs.GoerId == GoerId
                   where gs.SupporterId == gs.GoerId
                   let sp = DbUtil.Db.People.Single(ss => ss.PeopleId == gs.SupporterId)
                   let gp = DbUtil.Db.People.Single(ss => ss.PeopleId == gs.GoerId)
                   let o = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == gs.OrgId)
                   orderby gs.Created descending
                   select new SupporterInfo
                   {
                       gs = gs,
                       SupporterName = sp.Name,
                       SupporterId = sp.PeopleId,
                       GoerName = gp.Name,
                       GoerId = gp.PeopleId,
                       OrgId = o.OrganizationId,
                       TripName = o.OrganizationName
                   };
        }

        public IQueryable<SupporterInfo> SupportOthers()
        {
            return from gs in DbUtil.Db.GoerSenderAmounts
                   where gs.SupporterId == SenderId
                   where gs.SupporterId != (gs.GoerId ?? 0)
                   let gp = DbUtil.Db.People.SingleOrDefault(ss => ss.PeopleId == gs.GoerId)
                   let sp = DbUtil.Db.People.Single(ss => ss.PeopleId == gs.SupporterId)
                   let o = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == gs.OrgId)
                   orderby gs.Created descending
                   select new SupporterInfo
                   {
                       gs = gs,
                       SupporterName = sp.Name,
                       SupporterId = sp.PeopleId,
                       GoerName = gp.Name,
                       GoerId = gp.PeopleId,
                       OrgId = o.OrganizationId,
                       TripName = o.OrganizationName
                   };
        }

        public EpplusResult ToExcel()
        {
            return ExportTransactions().ToExcel("Transactions.xlsx");
        }

        public class TotalTransaction
        {
            public int Count { get; set; }
            public decimal Amt { get; set; }
            public decimal Amtdue { get; set; }
            public decimal Donate { get; set; }
        }

        public class BatchTranGroup
        {
            public int count { get; set; }
            public DateTime? batchdate { get; set; }
            public string BatchRef { get; set; }
            public string BatchType { get; set; }
            public decimal Total { get; set; }
        }

        public class DescriptionGroup
        {
            public int count { get; set; }
            public string Description { get; set; }
            public decimal Total { get; set; }
        }

        public class BatchDescriptionGroup
        {
            public int count { get; set; }
            public DateTime? batchdate { get; set; }
            public string BatchRef { get; set; }
            public string BatchType { get; set; }
            public string Description { get; set; }
            public decimal Total { get; set; }
        }

        public class SupporterInfo
        {
            public GoerSenderAmount gs { get; set; }
            public string GoerName { get; set; }
            public string SupporterName { get; set; }
            public int? GoerId { get; set; }
            public int? SupporterId { get; set; }
            public int? OrgId { get; set; }
            public string TripName { get; set; }
        }

        public class MissionTripBalanceInfo
        {
            public int GoerId { get; set; }
            public int OrgId { get; set; }
            public string TripName { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
