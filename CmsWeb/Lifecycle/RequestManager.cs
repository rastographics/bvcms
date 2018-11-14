using CmsData;
using ImageData;
using System;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

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
        string CurrentHost { get; }
        string CurrentConnectionString { get; }
        CMSDataContext CurrentDatabase { get; }
        CMSImageDataContext CurrentImageDatabase { get; }
    }

    public class RequestManager : IRequestManager
    {
        private readonly CMSConfigurationManager _cmsConfigurationManager;

        public Guid RequestId { get; }
        public HttpContext CurrentHttpContext { get; }
        public string CurrentHost { get; private set; }
        public string CurrentConnectionString { get; private set; }
        public CMSDataContext CurrentDatabase { get; private set; }
        public CMSImageDataContext CurrentImageDatabase { get; private set; }

        public RequestManager(HttpContext httpContext, CMSConfigurationManager configurationManager)
        {
            CurrentHttpContext = httpContext;
            _cmsConfigurationManager = configurationManager;

            CurrentHost = GetHost(CurrentHttpContext);
            CurrentConnectionString = GetConnectionString(CurrentHttpContext);
            RequestId = Guid.NewGuid();

            CurrentDatabase = CmsData.DbUtil.Create(CurrentConnectionString, CurrentHost);
            CurrentImageDatabase = ImageData.DbUtil.Db;
        }

        private string GetHost(HttpContext context)
        {
            if (_cmsConfigurationManager.HasConfiguredValue(CMSConfigurationManager.HostSetting))
            {
                CurrentHost = _cmsConfigurationManager.GetConfiguredValue(CMSConfigurationManager.HostSetting);
            }
            else
            {
                try
                {
                    CurrentHost = context.Request.Url.Authority.SplitStr(".:")[0];
                }
                catch (Exception)
                {
                    CurrentHost = string.Empty;
                }
            }

            return CurrentHost;
        }

        private string GetConnectionString(HttpContext context)
        {
            return string.Empty;
        }
    }
}
