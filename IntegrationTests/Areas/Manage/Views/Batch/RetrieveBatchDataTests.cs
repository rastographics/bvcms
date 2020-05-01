using CmsData;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;

namespace IntegrationTests.Areas.Manage.Views.Batch
{
    [Collection(Collections.Webapp)]
    public class RetrieveBatchDataTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Should_RetrieveBatchDates_With_No_Date()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.OnlineRegistration, GatewayTypes.Acceptiva);
            Login();
            Wait(3);
            OrgId = CreateOrgWithFee();
            SettingUtils.UpdateSetting("UseRecaptcha", "false");
            SettingUtils.UpdateSetting("AutomaticSettle", "true");
            SettingUtils.UpdateSetting("AutoSyncBatchDates", "true");
            SettingUtils.UpdateSetting("AutoSyncBatchDatesWindow", "10");
            PayRegistration(OrgId);

            db.RetrieveBatchData(testing: true) ;
            var transaction = db.Transactions.FirstOrDefault(t => t.OrgId == OrgId);
            transaction.Settled.ShouldNotBeNull();
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
