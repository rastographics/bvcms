using IntegrationTests.Support;
using System;

namespace IntegrationTests
{
    public class WebAppFixture : IDisposable
    {
        private IISExpress cmswebInstance;

        public WebAppFixture()
        {
            Settings.EnsureApplicationHostConfig();
            cmswebInstance = IISExpress.Start(Settings.ApplicationHostConfig, "CMSWeb");
        }

        public void Dispose()
        {
            cmswebInstance = null;
        }
    }
}
