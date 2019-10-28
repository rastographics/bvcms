using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class OrganizationTests : IDisposable
    {
        [Fact]
        public void Should_CopySettings2()
        {
            var org = new Organization
            {
                OrganizationName = "Org",
                Description = "TestDescription",
                BirthDayStart = new DateTime(1990, 6, 22),
                BirthDayEnd = new DateTime(2000, 6, 22),
                RegSetting = "This are reg settings"
            };

            var org2 = new Organization
            {
                OrganizationName = "Org Copied",
            };

            Organization.CopySettings2(org, org2);

            org2.Description.ShouldBe(org.Description);
            org2.BirthDayStart.ShouldBe(org.BirthDayStart);
            org2.BirthDayEnd.ShouldBe(org.BirthDayEnd);
            org2.RegSetting.ShouldBe(org.RegSetting);
        }

        public void Dispose()
        {
        }
    }
}
