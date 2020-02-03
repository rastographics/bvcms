using System.Web.Mvc;

namespace CmsWeb.Areas.CheckIn
{
    public class CheckInAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CheckIn";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("CheckIn_default", "CheckIn/{action}/{id}",
                new { controller = "CheckIn", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
