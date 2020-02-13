using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests
{
    public class ContextTests
    {
        [Fact]
        public void Should_SubmitChanges_In_OneTimeLinks()
        {
            Guid id;
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                OneTimeLink otl = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = DatabaseTestBase.RandomString(),
                    Used = false,
                    Expires = DateTime.Now.AddDays(1)
                };
                db.OneTimeLinks.InsertOnSubmit(otl);
                db.SubmitChanges();
                id = otl.Id;
                var result = db.OneTimeLinks.SingleOrDefault(o => o.Id == id);
                result.ShouldNotBeNull();
            }
        }
    }
}
