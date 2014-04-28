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
        public string FullName
        {
            get
            {
                var s = "";
                if (Last.HasValue())
                {
                    if (MiddleInitial.HasValue())
                        s = "{0} {1} {2}".Fmt(First, MiddleInitial, Last);
                    else
                        s = "{0} {1}".Fmt(First, Last);
                    if (Suffix.HasValue())
                        s = s + ", " + Suffix;
                    return s;
                }
                return Name;
            }
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