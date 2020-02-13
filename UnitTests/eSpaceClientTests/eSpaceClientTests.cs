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
            var client = CreateClient();
            var list = client.Event.List(new NameValueCollection { { "endDate", "2020-01-31" } });
            list.ShouldNotBeNull();
            list.Count().ShouldBe(3);
        }

        [Fact]
        public void EventServiceOccurrencesTest()
        {
            NetworkFixture.MockResponse("/event/occurrences\\?endDate=2020-01-31&eventId=874491", new MockHttpResponse
            {
                Headers = NetworkFixture.JsonHeaders,
                ResponseBody = Encoding.Default.GetString(Resources.EventServiceOccurrencesTestResponse)
            });
            var client = CreateClient();
            var list = client.Event.Occurrences(874491, new NameValueCollection { { "endDate", "2020-01-31" } });
            list.ShouldNotBeNull();
            list.Count().ShouldBe(5);
        }

        private static eSpaceClient CreateClient()
        {
            return new eSpaceClient
            {
                BaseUrl = NetworkFixture.ProxyUrl,
                Username = "username@example.com",
                Password = "thisisapassword"
            };
        }
    }
}
