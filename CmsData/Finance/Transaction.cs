using System;
using System.Linq;
using CmsData.View;
using UtilityExtensions;

namespace CmsData
{
    public partial class Transaction
    {
        public bool CanCredit(CMSDataContext db)
        {
            return db.Gateway().CanVoidRefund
                && Approved == true
                && Voided != true
                && Credited != true
                && (Coupon ?? false) == false
                && TransactionId.HasValue()
                && Batchtyp == "eft" || Batchtyp == "bankcard"
                && Amt > 0;
        }
        public bool CanVoid(CMSDataContext db)
        {
            return db.Gateway().CanVoidRefund
                && Approved == true
                && !CanCredit(db)
                && Voided != true
                && Credited != true
                && (Coupon ?? false) == false
                && TransactionId.HasValue()
                && Amt > 0;
        }
        public Transaction OriginalTrans => OriginalTransaction ?? this;

        public int FirstTransactionPeopleId()
        {
            return OriginalTrans.TransactionPeople.Select(pp => pp.PeopleId).FirstOrDefault();
        }

        public static string FullName(Transaction t)
        {
            return FullName(t.First, t.Last, t.MiddleInitial, t.Suffix, t.Name);
        }
        public static string FullName(TransactionList t)
        {
            return FullName(t.First, t.Last, t.MiddleInitial, t.Suffix, t.Name);
        }
        private static string FullName(string first, string last, string mi, string suffix, string name)
        {
            var s = "";
            if (!last.HasValue())
                return name;
            s = mi.HasValue()
                ? $"{first} {mi} {last}"
                : $"{first} {last}";
            if (suffix.HasValue())
                s = s + ", " + suffix;
            return s;
        }

        private int? timeOut;
        public int TimeOut
        {
            get
            {
                if (!timeOut.HasValue)
                    timeOut = Util.IsDebug() ? 16000000 : 180000;
                return timeOut.Value;
            }
        }
        public static void ResolvePrevDaysVirtualCheckRejects(CMSDataContext db, DateTime start, DateTime end)
        {
            var gateway = db.Gateway();
            var response = gateway.GetReturnedChecks(start, end);


            //var ds = gw.VirtualCheckRejects(dt);
            //var items = from r in ds.Tables[0].AsEnumerable()
            //            let rejectdt = r["reject_date"].ToDate() ?? DateTime.MinValue
            //            where rejectdt > DateTime.MinValue
            //            select new
            //            {
            //                rejectdt,
            //                trantype = r["trantype"],
            //                amt = r["rejedt_amount"].ToString().ToDecimal(),
            //                tranid = r["customer_number"].ToInt(),
            //                rejectcode = r["reject_code"].ToString(),
            //                message = r["correction_info"].ToString(),
            //            };
            /*
             * Create a new transaction to reverse the original
             * If the transaction was for online giving or recurring giving, then reverse the contribution.
             * If the transaction contained an extra donation, then reverse that contribution.
             * Send an email to the payor.
             * Send an email notification to the online notify list for the associated organization
             */
        }
    }
}
