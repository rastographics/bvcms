using CmsData;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using CMSWebTests.Support;
using Shouldly;
using Xunit;
using SharedTestFixtures;

namespace CMSWebTests.Areas.Public.Models.MobileAPIv2
{
    [Collection(Collections.Database)]
    public class MobileAccountTests : ControllerTestBase
    {
        [Fact]
        public void Should_getMobileResponse()
        {
            string username = RandomString();
            string password = RandomString();
            string msgInstance = RandomString();
            string msgKey = RandomString();
            bool useMobileMessages = true;
            User user = CreateUser(username, password);

            MobileMessage message = new MobileMessage
            {
                device = (int)MobileMessage.Device.ANDROID,
                argString = $"/Person2/{user.PeopleId}/Resources",
                instance = msgInstance,
                key = msgKey
            };

            MobileAccount account = new MobileAccount(db);
            account.setDeepLinkFields(message.device, message.instance, message.key, message.argString);
            account.sendDeepLink();

            MobileMessage response = account.getMobileResponse(useMobileMessages);
            response.ShouldNotBe(null);
        }
    }
}
