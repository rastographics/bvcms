using CmsData;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class SupportLinkTests
    {
        [Fact]
        public void Should_Return_Anonymous_Replacement_Link()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var link = EmailReplacements.SupportLinkAnonymousReplacement(db, "1", "1");
                String[] spearator = { "//", "/" };
                var url = link.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in url)
                {
                    item.ShouldNotBe("");
                }
            }
        }
    }
}
