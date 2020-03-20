using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace CmsDataTests.Finance
{
    [Collection(Collections.Database)]
    public class MultipleGatewayUtilsTests
    {
        [Theory]
        [InlineData("Online Giving", 8, 1)]       
        [InlineData("OrgEx", 1, 3)]
        public void Should_Return_ProcessType(string description, int processType, int result)
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var org = new Organization
                {
                    OrganizationName = DatabaseTestBase.RandomString(),
                    RegistrationTypeId = processType,
                    Description = description,
                    CreatedDate = DateTime.Now,
                    OrganizationStatusId = 30,
                };

                try
                {
                    db.Organizations.InsertOnSubmit(org);
                    db.SubmitChanges();
                }
                catch(Exception e)
                {
                    var i = e;
                }

                var paymentProcessType = MultipleGatewayUtils.ProcessByTransactionDescription(db, org.Description);
                var pp = (int)paymentProcessType;
                pp.ShouldBe(result);
            }
        }

        [Fact]        
        public void Should_Return_RegistrationProcessType()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var randomDescription = DatabaseTestBase.RandomString();
                var paymentProcessType = MultipleGatewayUtils.ProcessByTransactionDescription(db, randomDescription);
                var pp = (int)paymentProcessType;
                pp.ShouldBe(3);
            }
        }
    }
}
