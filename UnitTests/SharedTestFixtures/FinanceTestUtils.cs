using CmsData;

namespace SharedTestFixtures
{
    public class FinanceTestUtils : FinanceTestBase
    {
        private CMSDataContext db;

        public FinanceTestUtils(CMSDataContext db)
        {
            this.db = db;
        }

        public BundleHeader BundleHeader
        {
            get
            {
                return CreateBundle(db, null);
            }
        }
    }
}
