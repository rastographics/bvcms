using CmsData;
using System;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Lifecycle
{
    public class CMSBaseController : Controller
    {
        protected RequestManager RequestManager { get; }
        protected CMSDataContext Db => RequestManager.Database;
        protected Guid CurrentRequestId => RequestManager.RequestId;

        public CMSBaseController(RequestManager requestManager)
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

    public class RequestManager
    {
        private readonly HttpContext _httpContext;
        private readonly CMSConfigurationManager _cmsConfigurationManager;

        public Guid RequestId { get; }
        public HttpContext CurrentHttpContext => _httpContext;
        public string Host { get; private set; }
        public string ConnectionString { get; private set; }
        public CMSDataContext Database { get; private set; }

        public RequestManager(HttpContext httpContext, CMSConfigurationManager configurationManager)
        {
            _httpContext = httpContext;
            _cmsConfigurationManager = configurationManager;

            Host = GetHost(_httpContext);
            ConnectionString = GetConnectionString(_httpContext);
            RequestId = Guid.NewGuid();

            Database = DbUtil.Create(ConnectionString, Host);
        }

        private string GetHost(HttpContext context)
        {
            if (_cmsConfigurationManager.HasConfiguredValue(CMSConfigurationManager.HostSetting))
            {
                Host = _cmsConfigurationManager.GetConfiguredValue(CMSConfigurationManager.HostSetting);
            }
            else
            {
                try
                {
                    Host = context.Request.Url.Authority.SplitStr(".:")[0];
                }
                catch (Exception)
                {
                    Host = string.Empty;
                }
            }

            return Host;
        }

        private string GetConnectionString(HttpContext context)
        {
            return string.Empty;
        }
    }
}
