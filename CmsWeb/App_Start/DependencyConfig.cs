using CmsWeb.Lifecycle;
using CmsWeb.Membership;
using CmsWeb.Services.MeetingCategory;
using SimpleInjector;
using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace CmsWeb
{
    public class DependencyConfig
    {
        public static void RegisterSimpleInjector(Container container)
        {
            container.Register<IRequestManager, RequestManager>(Lifestyle.Scoped);
            container.Register<IMeetingCategoryService, MeetingCategoryService>(Lifestyle.Scoped);
            container.Register(() => new Lazy<IPrincipal>(() => HttpContext.Current.User));
            container.Register(() => new Lazy<MembershipProvider>(() => CMSMembershipProvider.provider));
            container.Register(() => new Lazy<RoleProvider>(() => CMSRoleProvider.provider));
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterInitializer<IRequestManager>(instance => {
                CMSMembershipProvider.provider.RequestManager = instance;
                CMSRoleProvider.provider.RequestManager = instance;
            });
        }
    }
}
