using CmsData;
using CmsData.API;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace CmsDataTests.API
{
    [Collection(Collections.Database)]
    public class APIContributionSearchModelTests : IDisposable
    {
        [Fact]
        public void Should_APIContributionSearchModel()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                ContributionSearchInfo SearchInfo = new ContributionSearchInfo
                {
                    Name = "TestName"
                };
                var api = new APIContributionSearchModel(db, SearchInfo);
                api.model.Name.ShouldBe("TestName");
            }
        }

        public void Dispose()
        {
        }
    }
}
