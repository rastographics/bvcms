using System.Web.Mvc;

namespace CmsWeb.Areas.Figures
{
    public class FiguresAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Figures";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
          
            context.MapRoute(
                "Figures_default",
                "Figures/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
        private void AddRoute(AreaRegistrationContext context, string controller)
        {
            context.MapRoute(controller, controller + "/{action}/{id}",
                new { controller = controller, action = "Index", id = UrlParameter.Optional });
        }
        private static void AddRoute(AreaRegistrationContext context, string name, string controller, string path)
        {
            context.MapRoute(name, path + "/{action}/{id}",
                new { controller = controller, action = "Index", id = UrlParameter.Optional });
        }
        private static void AddRoute(AreaRegistrationContext context, string name, string controller, string path, string action)
        {
            context.MapRoute(name, path,
                new { controller = controller, action = action, id = UrlParameter.Optional });
        }
    }
}
