using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Public
{
    public class PublicAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            AddRoute(context, "APIMeta");
            
            AddRoute(context, "logininfo", "APIPerson", "API/LoginInfo/{id}", "LoginInfo");
            AddRoute(context, "apiorgmembers", "APIOrg", "API/OrgMembers/{id}", "OrgMembers");

            AddRoute(context, "APITest");
            AddRoute(context, "APICheckin");
            AddRoute(context, "APICheckin2");
            AddRoute(context, "APIiPhone");
            AddRoute(context, "APIPerson");
            AddRoute(context, "Checkin2", "APICheckin2", "Checkin2");
            AddRoute(context, "Checkin", "APICheckin", "Checkin");
            AddRoute(context, "iPhone", "APIiPhone", "iPhone");

            AddRoute(context, "MOBS");
            AddRoute(context, "SGMap");
            AddRoute(context, "OrgContentStart", "OrgContent", "OrgContent/{id}", "Index");
            AddRoute(context, "OrgContent");
            AddRoute(context, "OptOut");
            AddRoute(context, "Track");
            AddRoute(context, "StepClass");
            AddRoute(context, "Event");
            AddRoute(context, "VolunteerConfirm", "Volunteer", "Volunteer/Confirm", "confirm");
            AddRoute(context, "VolunteerPicklist", "Volunteer", "Volunteer/Picklist2", "Picklist2");
            AddRoute(context, "VolunteerStart", "Volunteer", "Volunteer/{id}", "Start");
            AddRoute(context, "Volunteer");

            AddRoute(context, "SmallGroupFinder");

            context.MapRoute(
                "Public_default",
                "Public/{controller}/{action}/{id}",
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
