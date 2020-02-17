using CmsData;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
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
                String[] separator = { "//", "/" };
                var url = link.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var linkId = url[4];
                var guid = linkId.ToGuid();

                var ot = db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                ot.ShouldNotBeNull();
            }
        }
    }
}
