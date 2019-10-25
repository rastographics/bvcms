using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class ContributionTests: IDisposable
    {
        [Fact]        
        public void Should_GetBundleHeader()
        {
            var contributionDate = new DateTime(2019,10,10);
            var depositDate = new DateTime(2019,10,01);

            using(var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var actual = Contribution.GetBundleHeader(db, contributionDate, DateTime.Now, depositDate: depositDate);

                actual.ContributionDate.ShouldBe(contributionDate);   
                actual.DepositDate.ShouldBe(depositDate);
           
                MockContributions.DeleteAllFromBundle(db, actual);
            }            
        }

        public void Dispose()
        {
        }
    }
}
