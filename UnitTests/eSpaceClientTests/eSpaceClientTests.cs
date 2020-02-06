using Xunit;
using SharedTestFixtures;
using System.Collections.Specialized;
using Shouldly;
using SharedTestFixtures.Network;
using eSpaceClientTests.Properties;
using System.Text;
using System.Linq;

namespace eSpace.eSpaceClientTests
{
    [Collection(Collections.Network)]
    public class eSpaceClientTests
    {
        [Fact]
        public void EventServiceListTest()
        {
            NetworkFixture.MockResponse("/event/list\\?endDate=2020-01-31", new MockHttpResponse
            {
                Headers = NetworkFixture.JsonHeaders,
                ResponseBody = Encoding.Default.GetString(Resources.EventServiceListTestResponse)
            });
            var client = new eSpaceClient
            {
                BaseUrl = NetworkFixture.ProxyUrl,
                Username = "username@example.com",
                Password = "thisisapassword"
            };
            var list = client.Event.List(new NameValueCollection { { "endDate", "2020-01-31" } });
            list.ShouldNotBeNull();
            list.Count().ShouldBe(3);
        }
    }
}
