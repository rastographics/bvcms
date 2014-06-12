using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.View;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class TransactionsModel
    {
        public string description { get; set; }
        public string name { get; set; }
        public string Submit { get; set; }
        public decimal? gtamount { get; set; }
        public decimal? ltamount { get; set; }
        public DateTime? startdt { get; set; }
        public DateTime? enddt { get; set; }
        public bool testtransactions { get; set; }
        public bool apprtransactions { get; set; }
        public bool nocoupons { get; set; }
        public string batchref { get; set; }
        public bool usebatchdates { get; set; }
        public PagerModel2 Pager { get; set; }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchTransactions().Count();
            return _count.Value;
        }
        public bool isSage { get; set; }
        public bool finance { get; set; }
        public bool admin { get; set; }
        public TransactionsModel(int? tranid)
            : this()
        {
            this.name = tranid.ToString();
        }
        public TransactionsModel()
        {
            Pager = new PagerModel2(Count);
            Pager.Sort = "Date";
            Pager.Direction = "desc";
            finance = HttpContext.Current.User.IsInRole("Finance");
            isSage = OnlineRegModel.GetTransactionGateway() == "sage";
            admin = HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("ManageTransactions");
        }
        public IEnumerable<TransactionList> Transactions()
        {
            var q0 = ApplySort();
            q0 = q0.Skip(Pager.StartRow).Take(Pager.PageSize);
            return q0;
        }

        public class TotalTransaction
        {
            public int Count { get; set; }
            public decimal Amt { get; set; }
            public decimal Amtdue { get; set; }
            public decimal Donate { get; set; }
        }

        public TotalTransaction TotalTransactions()
        {
            var q0 = FetchTransactions();
            var q = from t in q0
                    group t by 1 into g
                    select new TotalTransaction()
                    {
                        Amt = g.Sum(tt => tt.Amt ?? 0),
                        Amtdue = g.Sum(tt => tt.Amtdue ?? 0),
                        Donate = g.Sum(tt => tt.Donate ?? 0),
                        Count = g.Count()
                    };
            return q.FirstOrDefault();
        }

        private IQueryable<TransactionList> _transactions;
        private IQueryable<TransactionList> FetchTransactions()
        {
            if (_transactions != null)
                return _transactions;
            if (!name.HasValue())
                name = null;
            string first, last;
            Util.NameSplit(name, out first, out last);
            var hasfirst = first.HasValue();
            var nameid = name.ToInt();
            _transactions
               = from t in DbUtil.Db.ViewTransactionLists
                 let donate = t.Donate ?? 0
                 where t.Amt > gtamount || gtamount == null
                 where t.Amt <= ltamount || ltamount == null
                 where description == null || t.Description.Contains(description)
                 where nameid > 0 || ((t.Testing ?? false) == testtransactions)
                 where apprtransactions == (t.Moneytran == true) || !apprtransactions
                 where (nocoupons && !t.TransactionId.Contains("Coupon")) || !nocoupons
                 where (t.Financeonly ?? false) == false || finance
                 select t;
            if (name != null)
                _transactions = from t in _transactions
                                where
                                    (
                                        (t.Last.StartsWith(last) || t.Last.StartsWith(name))
                                        && (!hasfirst || t.First.StartsWith(first) || t.Last.StartsWith(name))
                                    )
                                    || t.Batchref == name || t.TransactionId == name || t.OriginalId == nameid || t.Id == nameid
                                select t;
            if (!HttpContext.Current.User.IsInRole("Finance"))
                _transactions = _transactions.Where(tt => (tt.Financeonly ?? false) == false);

            var edt = enddt;
            if (!edt.HasValue && startdt.HasValue)
                edt = startdt;
            if (edt.HasValue)
                edt = edt.Value.AddHours(24);
            if (usebatchdates && startdt.HasValue)
            {
                CheckBatchDates(startdt.Value, edt.Value);
                _transactions = from t in _transactions
                                where t.Batch >= startdt || startdt == null
                                where t.Batch <= edt || edt == null
                                where t.Moneytran == true
                                select t;
            }
            else
                _transactions = from t in _transactions
                                where t.TransactionDate >= startdt || startdt == null
                                where t.TransactionDate <= edt || edt == null
                                select t;
            //			var q0 = _transactions.ToList();
            //            foreach(var t in q0)
            //                Debug.WriteLine("\"{0}\"\t{1}\t{2}", t.Description, t.Id, t.Amt);
            return _transactions;
        }

        public class BatchTranGroup
        {
            public int count { get; set; }
            public DateTime? batchdate { get; set; }
            public string BatchRef { get; set; }
            public string BatchType { get; set; }
            public decimal Total { get; set; }
        }

        public IQueryable<BatchTranGroup> FetchBatchTransactions()
        {
            var q = from t in FetchTransactions()
                    group t by t.Batchref into g
                    orderby g.First().Batch descending
                    select new BatchTranGroup()
                    {
                        count = g.Count(),
                        batchdate = g.Max(gg => gg.Batch),
                        BatchRef = g.Key,
                        BatchType = g.First().Batchtyp,
                        Total = g.Sum(gg => gg.Amt ?? 0)
                    };
            return q;
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
        public IEnumerable<DescriptionGroup> FetchTransactionsByDescription()
        {
            var q0 = FetchTransactions();
            var q = from t in q0
                    group t by t.Description into g
                    orderby g.First().Batch descending
                    select new DescriptionGroup()
                    {
                        count = g.Count(),
                        Description = g.Key,
                        Total = g.Sum(gg => gg.Amt ?? 0)
                    };
            return q;
        }
        public IQueryable<BatchDescriptionGroup> FetchTransactionsByBatchDescription()
        {
            var q = from t in FetchTransactions()
                    group t by new { t.Batchref, t.Description } into g
                    let f = g.First()
                    orderby f.Batch, f.Description descending
                    select new BatchDescriptionGroup()
                    {
                        count = g.Count(),
                        batchdate = f.Batch,
                        BatchRef = f.Batchref,
                        BatchType = f.Batchtyp,
                        Description = f.Description,
                        Total = g.Sum(gg => gg.Amt ?? 0)
                    };
            return q;
        }

        private void CheckBatchDates(DateTime start, DateTime end)
        {
            if (OnlineRegModel.GetTransactionGateway() != "sage")
                return;
            var sage = new SagePayments(DbUtil.Db, false);
            var bds = sage.SettledBatchSummary(start, end, true, true);
            var batches = from batch in bds.Tables[0].AsEnumerable()
                          select new
                          {
                              date = batch["date"].ToDate().Value.AddHours(4),
                              reference = batch["reference"].ToString(),
                              type = batch["type"].ToString()
                          };
            foreach (var batch in batches)
            {
                if (DbUtil.Db.CheckedBatches.Any(tt => tt.BatchRef == batch.reference))
                    continue;

                var ds = sage.SettledBatchListing(batch.reference, batch.type);

                var items = from r in ds.Tables[0].AsEnumerable()
                            select new
                            {
                                settled = r["settle_date"].ToDate().Value.AddHours(4),
                                tranid = r["order_number"],
                                reference = r["reference"].ToString(),
                                approved = r["approved"].ToString().ToBool(),
                                name = r["name"].ToString(),
                                message = r["message"].ToString(),
                                amount = r["total_amount"].ToString(),
                                date = r["date"].ToDate(),
                                type = r["transaction_code"].ToInt()
                            };
                var settlelist = items.ToDictionary(ii => ii.reference, ii => ii);

                var q = from t in DbUtil.Db.Transactions
                        where settlelist.Keys.Contains(t.TransactionId)
                        where t.Approved == true
                        select t;
                var tlist = q.ToDictionary(ii => ii.TransactionId, ii => ii); // transactions that are found in setteled list;
                var q2 = from st in settlelist
                         where !tlist.Keys.Contains(st.Key)
                         select st.Value;
                var notbefore = DateTime.Parse("6/1/12");
                foreach (var st in q2)
                {
                    var t = DbUtil.Db.Transactions.SingleOrDefault(j => j.TransactionId == st.reference && st.date >= notbefore);
                    string first, last;
                    Util.NameSplit(st.name, out first, out last);
                    var tt = new Transaction
                    {
                        Name = st.name,
                        First = first,
                        Last = last,
                        TransactionId = st.reference,
                        Amt = st.amount.ToDecimal(),
                        Approved = st.approved,
                        Message = st.message,
                        TransactionDate = st.date,
                        TransactionGateway = "sage",
                        Settled = st.settled,
                        Batch = batch.date,
                        Batchref = batch.reference,
                        Batchtyp = batch.type,
                        OriginalId = t != null ? (t.OriginalId ?? t.Id) : (int?)null,
                        Fromsage = true,
                        Description = t != null ? t.Description : "no description from sage, id=" + st.tranid,
                    };
                    if (st.type == 6) // credit transaction
                        tt.Amt = -tt.Amt;
                    DbUtil.Db.Transactions.InsertOnSubmit(tt);
                }

                foreach (var t in q)
                {
                    if (!settlelist.ContainsKey(t.TransactionId))
                        continue;
                    t.Batch = batch.date;
                    t.Batchref = batch.reference;
                    t.Batchtyp = batch.type;
                    t.Settled = settlelist[t.TransactionId].settled;
                }
                var cb = DbUtil.Db.CheckedBatches.SingleOrDefault(bb => bb.BatchRef == batch.reference);
                if (cb == null)
                {
                    DbUtil.Db.CheckedBatches.InsertOnSubmit(
                        new CheckedBatch()
                        {
                            BatchRef = batch.reference,
                            CheckedX = DateTime.Now
                        });
                }
                else
                    cb.CheckedX = DateTime.Now;
                DbUtil.Db.SubmitChanges();
            }
        }
        public IQueryable<TransactionList> ApplySort()
        {
            var q = FetchTransactions();
            if (Pager.Direction == "asc")
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
            else
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

            return q;
        }
        public IQueryable ExportTransactions()
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
                     RegAmt = t.Amt ?? 0,
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
            return q2;
        }
    }
}
