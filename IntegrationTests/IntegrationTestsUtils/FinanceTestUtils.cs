using CmsData;
using CmsDataTests.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.IntegrationTestsUtils
{
    [Collection("Database collection")]
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
