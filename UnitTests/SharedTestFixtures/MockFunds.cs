using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockFunds
    {
        public static ContributionFund CreateSaveFund(CMSDataContext db, bool isPledge)
        {
            int fundId;
            Random random = new Random();
            bool fundIdExists = false;
            do
            {
                fundId = random.Next(1000);
                fundIdExists = db.ContributionFunds.Any(f => f.FundId == fundId);
            } while (fundIdExists);

            var fund = new ContributionFund
            {
                FundId = fundId,
                FundName = DatabaseTestBase.RandomString(),          
                FundPledgeFlag = isPledge,
                CreatedBy = 1,
                CreatedDate = DateTime.Now
            };
            db.ContributionFunds.InsertOnSubmit(fund);
            db.SubmitChanges();
            return fund;
        }

        public static void DeleteFund(CMSDataContext db, int fundId)
        {
            var fund = db.ContributionFunds.Where(b => b.FundId == fundId);
            db.ContributionFunds.DeleteAllOnSubmit(fund);
            db.SubmitChanges();
        }
    }
}
