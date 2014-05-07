using CmsData.Registration;
using CmsData.View;
using UtilityExtensions;
using System.Linq;

namespace CmsData
{
    public partial class Transaction
    {
    	public bool CanCredit(CMSDataContext db)
    	{
				if (!Util.IsSage.HasValue)
					Util.IsSage = db.Setting("TransactionGateway", "").ToLower() == "sage";
    			return Approved == true 
    			       && Util.IsSage.Value
    			       && Voided != true
    			       && Credited != true
    			       && (Coupon ?? false) == false
    			       && TransactionId.HasValue()
					   && Batchtyp == "eft" || Batchtyp == "bankcard"
					   && Amt > 0;
    	}
    	public bool CanVoid(CMSDataContext db)
    	{
				if (!Util.IsSage.HasValue)
					Util.IsSage = db.Setting("TransactionGateway", "").ToLower() == "sage";
    			return Approved == true 
					 && !CanCredit(db)
    			       && Util.IsSage.Value
    			       && Voided != true
    			       && Credited != true
    			       && (Coupon ?? false) == false
    			       && TransactionId.HasValue()
					   && Amt > 0;
    	}
		public int FirstTransactionPeopleId()
		{
			return OriginalTransaction.TransactionPeople.Select(pp => pp.PeopleId).FirstOrDefault();
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
                ? "{0} {1} {2}".Fmt(first, mi, last) 
                : "{0} {1}".Fmt(first, last);
            if (suffix.HasValue())
                s = s + ", " + suffix;
            return s;
        }

        private bool? usebootstrap;
        public bool UseBootstrap(CMSDataContext db)
        {
            if (usebootstrap.HasValue)
                return usebootstrap.Value;
            var org = db.LoadOrganizationById(OrgId);
            return (usebootstrap = org.UseBootstrap) ?? false;
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