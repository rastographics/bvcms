using CmsData;
using ImageData;
using System;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Lifecycle
{
    public class CMSBaseService
    {
        protected IRequestManager RequestManager { get; }
        protected HttpContext CurrentHttpContext => RequestManager.CurrentHttpContext;
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
        protected HttpContext CurrentHttpContext => RequestManager.CurrentHttpContext;
        protected CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;
        protected CMSImageDataContext CurrentImageDatabase => RequestManager.CurrentImageDatabase;

        public CMSBaseController(IRequestManager requestManager)
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
        HttpContext CurrentHttpContext { get; }
        CMSDataContext CurrentDatabase { get; }
        CMSImageDataContext CurrentImageDatabase { get; }
    }

    public class RequestManager : IRequestManager
    {
        public Guid RequestId { get; }
        public HttpContext CurrentHttpContext { get; }
        public CMSDataContext CurrentDatabase { get; private set; }
        public CMSImageDataContext CurrentImageDatabase { get; private set; }

        public RequestManager()
        {
            CurrentHttpContext = HttpContext.Current;
            RequestId = Guid.NewGuid();

            CurrentDatabase = CmsData.DbUtil.Db;
            CurrentImageDatabase = ImageData.DbUtil.Db;
        }
    }
}
