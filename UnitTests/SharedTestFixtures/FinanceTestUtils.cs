using CmsData;
using System.Linq;
using UtilityExtensions;

namespace SharedTestFixtures
{
    public class FinanceTestUtils
    {
        private CMSDataContext db;

        public static GatewayAccount CreateMockPaymentProcessor(CMSDataContext db, PaymentProcessTypes processType, GatewayTypes gatewayType)
        {
            GatewayAccount account = null;
            var paymentProcess = db.PaymentProcess.Single(x => x.ProcessId == (int)processType);
            if (paymentProcess.GatewayAccountId.HasValue)
            {
                account = db.GatewayAccount.First(a => a.GatewayAccountId == paymentProcess.GatewayAccountId);
            }
            if (account == null)
            {
                account = db.GatewayAccount.First(a => a.GatewayAccountId == (int)gatewayType);
                paymentProcess.GatewayAccountId = account.GatewayAccountId;
                var details = db.GatewayDetails.Where(d => d.GatewayAccountId == account.GatewayAccountId).ToList();
                foreach (var d in details)
                {
                    if (d.GatewayDetailName == "GatewayTesting")
                    {
                        d.GatewayDetailValue = "true";
                    }
                    else if (!d.IsBoolean && !d.GatewayDetailValue.HasValue())
                    {
                        d.GatewayDetailValue = DatabaseTestBase.RandomString();
                    }
                }
                db.SubmitChanges();
            }
            return account;
        }

        public FinanceTestUtils(CMSDataContext db)
        {
            this.db = db;
        }

        public BundleHeader BundleHeader
        {
            get
            {
                return MockContributions.CreateSaveBundle(db, null);
            }
        }
    }
}
