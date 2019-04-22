using CmsWeb.Lifecycle;
using CmsWeb.Membership;
using CmsWeb.Services.MeetingCategory;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;

namespace CmsWeb
{
    public class DependencyConfig
    {
        public static void RegisterSimpleInjector()
        {
            // bootstrapping for simpleinjector
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.Register<IRequestManager, RequestManager>();
            container.Register<IMeetingCategoryService, MeetingCategoryService>();
            container.Register(() => new Lazy<IPrincipal>(() => HttpContext.Current.User));
            container.Register(() => new Lazy<MembershipProvider>(() => CMSMembershipProvider.provider));
            container.Register(() => new Lazy<RoleProvider>(() => CMSRoleProvider.provider));

            container.RegisterInitializer<IRequestManager>(instance => {
                CMSMembershipProvider.provider.RequestManager = instance;
                CMSRoleProvider.provider.RequestManager = instance;
            });

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            //container.Verify();
        }
    }
}
