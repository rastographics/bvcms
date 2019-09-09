using CmsData;
using ImageData;
using System;
using System.Security.Principal;
using System.Web;
using CmsWeb.Lifecycle;
using UtilityExtensions;
using SharedTestFixtures;

namespace CMSWebTests
{
    public class FakeRequestManager : IRequestManager, IDisposable
    {
        public Guid RequestId { get; }
        public IPrincipal CurrentUser { get; }
        public HttpContextBase CurrentHttpContext { get; }
        public CMSDataContext CurrentDatabase { get; private set; }
        public CMSImageDataContext CurrentImageDatabase { get; private set; }

        public FakeRequestManager()
        {
            CurrentHttpContext = ContextTestUtils.CreateMockHttpContext().Object;
            CurrentDatabase = CMSDataContext.Create(DatabaseFixture.Host);
            CurrentImageDatabase = CMSImageDataContext.Create(DatabaseFixture.Host);
            CurrentUser = CurrentHttpContext.User;
            RequestId = Guid.NewGuid();
        }

        public Elmah.ErrorLog GetErrorLog()
        {
            return Elmah.ErrorLog.GetDefault(CurrentHttpContext?.ApplicationInstance?.Context);
        }

        public static IRequestManager Create()
        {
            return new FakeRequestManager();
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
