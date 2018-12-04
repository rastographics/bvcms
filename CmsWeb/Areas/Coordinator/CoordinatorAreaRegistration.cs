using System.Web.Mvc;

namespace CmsWeb.Areas.Coordinator
{
    public class CoordinatorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Coordinator";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute("Coordinator_default", "CheckinCoordinator/{controller}/{action}/{id}",
                new { controller = "CheckinCoordinator", action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}
