using CmsWeb.Lifecycle;
using CmsWeb.Services.MeetingCategory;
using SimpleInjector;

namespace CmsWeb
{
    public class DependencyConfig
    {
        public static void RegisterSimpleInjector(Container container)
        {
            container.Register<IRequestManager, RequestManager>(Lifestyle.Scoped);
            container.Register<IMeetingCategoryService, MeetingCategoryService>(Lifestyle.Scoped);
        }
    }
}
