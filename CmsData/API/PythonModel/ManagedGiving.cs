using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void EnableFund(int peopleId, int fundId)
        {
            using (var db2 = NewDataContext())
            {
                RecurringAmount ra = (from m in db2.RecurringAmounts
                                      where m.PeopleId == peopleId
                                      where m.FundId == fundId
                                      select m).SingleOrDefault();
                if (ra != null)
                {
                    ra.Disabled = false;
                    db2.SubmitChanges();
                }
            }
        }

        public void DisableFund(int peopleId, int fundId)
        {
            using (var db2 = NewDataContext())
            {
                RecurringAmount ra = (from m in db2.RecurringAmounts
                                      where m.PeopleId == peopleId
                                      where m.FundId == fundId
                                      select m).SingleOrDefault();
                if (ra != null)
                {
                    ra.Disabled = true;
                    db2.SubmitChanges();
                }
            }
        }
    }
}
