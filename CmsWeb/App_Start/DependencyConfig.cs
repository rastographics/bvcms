using CmsWeb.Lifecycle;
using CmsWeb.Services.MeetingCategory;
using SimpleInjector;
using System;
using System.Security.Principal;
using System.Web;

namespace CmsWeb
{
    public class DependencyConfig
    {
        public static void RegisterSimpleInjector(Container container)
        {
            container.Register<IRequestManager, RequestManager>(Lifestyle.Scoped);
            container.Register<IMeetingCategoryService, MeetingCategoryService>(Lifestyle.Scoped);
            container.Register(() => new Lazy<IPrincipal>(() => HttpContext.Current.User));
        }
    }
}
