using CmsData;
using ImageData;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.OData;
using UtilityExtensions;

namespace CmsWeb.Lifecycle
{
    public class CMSBaseModel
    {
        protected IRequestManager RequestManager { get; }
        protected HttpContextBase CurrentHttpContext => RequestManager.CurrentHttpContext;
        protected CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;
        protected CMSImageDataContext CurrentImageDatabase => RequestManager.CurrentImageDatabase;

        public CMSBaseModel(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }
    }

    public class CMSBaseService
    {
        protected IRequestManager RequestManager { get; }
        protected HttpContextBase CurrentHttpContext => RequestManager.CurrentHttpContext;
        protected CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;
        protected CMSImageDataContext CurrentImageDatabase => RequestManager.CurrentImageDatabase;

        public CMSBaseService(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }
    }

    public class CMSBaseController : Controller
    {
        protected IRequestManager RequestManager { get; }
        protected HttpContextBase CurrentHttpContext => RequestManager.CurrentHttpContext;
        protected CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;
        protected CMSImageDataContext CurrentImageDatabase => RequestManager.CurrentImageDatabase;
        protected IPrincipal CurrentUser => RequestManager.CurrentUser;

        public CMSBaseController(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }
    }

    public class CMSBaseODataController : ODataController
    {
        protected IRequestManager RequestManager { get; }
        protected HttpContextBase CurrentHttpContext => RequestManager.CurrentHttpContext;
        protected CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;
        protected CMSImageDataContext CurrentImageDatabase => RequestManager.CurrentImageDatabase;

        public CMSBaseODataController(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }
    }


    public class CMSConfigurationManager
    {
        public const string HostSetting = "Host";

        public bool HasConfiguredValue(string key)
        {
            return false;
        }

        public string GetConfiguredValue(string key)
        {
            return string.Empty;
        }
    }

    public interface IRequestManager
    {
        Guid RequestId { get; }
        HttpContextBase CurrentHttpContext { get; }
        IPrincipal CurrentUser { get; }
        CMSDataContext CurrentDatabase { get; }
        CMSImageDataContext CurrentImageDatabase { get; }
    }

    public class RequestManager : IRequestManager, IDisposable
    {
        public Guid RequestId { get; }
        public IPrincipal CurrentUser { get; }
        public HttpContextBase CurrentHttpContext { get; }
        public CMSDataContext CurrentDatabase { get; private set; }
        public CMSImageDataContext CurrentImageDatabase { get; private set; }

        public RequestManager()
        {
            CurrentHttpContext = HttpContextFactory.Current;
            RequestId = Guid.NewGuid();
            CurrentUser = CurrentHttpContext.User;
            CurrentDatabase = CMSDataContext.Create(CurrentHttpContext);
            CurrentImageDatabase = CMSImageDataContext.Create(CurrentHttpContext);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CurrentDatabase.Dispose();
                    CurrentImageDatabase.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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
