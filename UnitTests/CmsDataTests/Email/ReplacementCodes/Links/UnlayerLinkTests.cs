using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System.Linq;
using System.Reflection;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests.Email.ReplacementCodes.Links
{
    [Collection(Collections.Database)]
    public class UnlayerLinkTests: DatabaseTestBase
    {
        [Fact]
        public void RsvpLink_Should_Have_regrets_Parameter()
        {
            using(var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                EmailReplacements email = new EmailReplacements(db, "", new System.Net.Mail.MailAddress("info@touchpointsoftware.com"));
                EmailQueueTo queue = new EmailQueueTo();

                queue.PeopleId = 1;
                queue.Id = 1;
                queue.GoerSupportId = 1;

                var dynMethod = email.GetType().GetMethod("UnlayerLinkReplacement", BindingFlags.NonPublic | BindingFlags.Instance);
                string url = dynMethod.Invoke(email, new object[] { "https://regretslink/?meeting=370&amp;confirm=true", queue }).ToString();

                url.ShouldContain("&regrets=true");
            }
        }
    }
}
