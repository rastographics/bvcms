using CmsData;

namespace SharedTestFixtures
{
    public class MockGivingPage
    {
        public static GivingPage CreateGivingPage(CMSDataContext db, string pageName = null, int? fundId = null, int pageTypeId = 7)
        {
            if (pageName == null)
            {
                pageName = DatabaseTestBase.RandomString();
            }
            if (fundId == null)
            {
                fundId = DatabaseTestBase.RandomNumber();
            }
            var givingPage = new GivingPage
            {
                GivingPageId = DatabaseTestBase.RandomNumber(),
                PageName = pageName,
                PageUrl = DatabaseTestBase.RandomString(),
                PageType = pageTypeId,
                FundId = (int)fundId,
                Enabled = true,
                DisabledRedirect = DatabaseTestBase.RandomString()
            };
            db.GivingPages.InsertOnSubmit(givingPage);
            db.SubmitChanges();

            return givingPage;
        }
        public static void DeleteGivingPage(CMSDataContext db, GivingPage givingPage)
        {
            db.GivingPages.DeleteOnSubmit(givingPage);
            db.SubmitChanges();
        }

        public static GivingPageFund CreateGivingPageFund(CMSDataContext db, int givingPageId, int fundId)
        {
            var givingPageFund = new GivingPageFund
            {
                GivingPageId = givingPageId,
                FundId = fundId
            };
            db.GivingPageFunds.InsertOnSubmit(givingPageFund);
            db.SubmitChanges();

            return givingPageFund;
        }
        public static void DeleteGivingPageFund(CMSDataContext db, GivingPageFund givingPageFund)
        {
            db.GivingPageFunds.DeleteOnSubmit(givingPageFund);
            db.SubmitChanges();
        }
    }
}
