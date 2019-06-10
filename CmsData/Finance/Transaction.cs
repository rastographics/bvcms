using System;
using System.Linq;
using CmsData.View;
using UtilityExtensions;

namespace CmsData
{
    public partial class Transaction
    {
        private IGateway gateway;
        private IGateway GetGateway(CMSDataContext db)
        {
            return gateway ?? (gateway = db.Gateway(name: TransactionGateway));
        }

        public bool CanCredit(CMSDataContext db)
        {
            return GetGateway(db).CanVoidRefund
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
            return GetGateway(db).CanVoidRefund
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
    }
}
