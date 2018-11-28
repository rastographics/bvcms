using CmsWeb.Lifecycle;
using SimpleInjector;

namespace CmsWeb
{
    public class DependencyConfig
    {
        public static void RegisterSimpleInjector(Container container)
        {
            container.Register<IRequestManager, RequestManager>(Lifestyle.Scoped);
        }
    }
}
