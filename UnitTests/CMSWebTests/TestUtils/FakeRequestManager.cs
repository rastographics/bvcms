using CmsData;
using ImageData;
using System;
using System.Security.Principal;
using System.Web;
using CmsWeb.Lifecycle;
using SharedTestFixtures;
using UtilityExtensions.Session;
using CMSShared.Session;
using UtilityExtensions.Extensions;

namespace CMSWebTests
{
    public class FakeRequestManager : IRequestManager, IDisposable
    {
        public Guid RequestId { get; }
        public IPrincipal CurrentUser { get; }
        public HttpContextBase CurrentHttpContext { get; }
        public CMSDataContext CurrentDatabase { get; private set; }
        public CMSImageDataContext CurrentImageDatabase { get; private set; }
        public ISessionProvider SessionProvider { get; }
        public string VisitorIpAddress { get; set; }

        public FakeRequestManager(bool isAuthenticated)
        {
            CurrentHttpContext = ContextTestUtils.CreateMockHttpContext(isAuthenticated).Object;
            CurrentDatabase = CMSDataContext.Create(DatabaseFixture.Host);
            CurrentImageDatabase = CMSImageDataContext.Create(DatabaseFixture.Host);
            CurrentUser = CurrentHttpContext.User;
            RequestId = Guid.NewGuid();
            SessionProvider = new CmsSessionProvider(CurrentDatabase);
            VisitorIpAddress = GetVisitorIpAddress();
        }

        public string GetVisitorIpAddress()
        {
            var ipAdd = CurrentHttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAdd))
                ipAdd = CurrentHttpContext.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ipAdd) || ipAdd == "::1")
                ipAdd = IpAddress.GetIpAddress();

            if (string.IsNullOrEmpty(ipAdd) || ipAdd == "::1")
            {
                ipAdd = "127.0.0.1";
            }

            return ipAdd;
        }

        public Elmah.ErrorLog GetErrorLog()
        {
            return Elmah.ErrorLog.GetDefault(CurrentHttpContext?.ApplicationInstance?.Context);
        }

        public static IRequestManager Create(bool isAuthenticated = true)
        {
            return new FakeRequestManager(isAuthenticated);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RequestManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
