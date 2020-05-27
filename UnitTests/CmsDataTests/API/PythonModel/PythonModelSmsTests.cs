using SharedTestFixtures;
using SharedTestFixtures.Network;
using Shouldly;
using System;
using Xunit;

namespace CmsDataTests.API.PythonModel
{
    [Collection(Collections.Network)]
    public class PythonModelSmsTests : IDisposable
    {
        public PythonModelSmsTests()
        {
            MockAppSettings.Apply(
                ("UrlShortenerService", NetworkFixture.ProxyUrl + "Create"),
                ("UrlShortenerServiceToken", "abcd1234")
            );
        }

        [Fact]
        public void CreateTinyUrlTest()
        {
            NetworkFixture.MockResponse("/Create", new MockHttpResponse
            {
                Headers = NetworkFixture.PlainText,
                ResponseBody = "tpsdb.co/short"
            });
            var shorturl = CmsData.PythonModel.CreateTinyUrl("https://status.touchpointsoftware.com");
            shorturl.ShouldBe("tpsdb.co/short");
        }

        public void Dispose()
        {
            MockAppSettings.Remove("UrlShortenerService", "UrlShortenerServiceToken");
        }
    }
}
