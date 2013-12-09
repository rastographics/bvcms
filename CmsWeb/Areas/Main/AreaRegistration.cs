using System.Web.Mvc;

namespace CmsWeb.Areas.Main
{
    public class MainAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Main";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            AddRoute(context, "Volunteering");
            AddRoute(context, "Coupon");
            AddRoute(context, "Tags");
            AddRoute(context, "PeopleSearch");
            AddRoute(context, "MemberDocs");
            AddRoute(context, "UserPref");
            AddRoute(context, "Email");
            AddRoute(context, "SMS");
            AddRoute(context, "RegSetting");
            AddRoute(context, "Meeting");
            AddRoute(context, "OrgSearch");
            AddRoute(context, "MemberDirectory");
            AddRoute(context, "ContactSearch");
            AddRoute(context, "QuickSearch");
            AddRoute(context, "Contact");
            AddRoute(context, "TaskList", "Task", "Task/{action}/{id}", "List");
            AddRoute(context, "Task");
            AddRoute(context, "Organization");
            AddRoute(context, "OrgGroups");
            AddRoute(context, "OrgChildren");
            AddRoute(context, "SavedQuery");
            AddRoute(context, "SavedQuery2");
            AddRoute(context, "Reports");
            AddRoute(context, "Person");
            AddRoute(context, "Family");
            AddRoute(context, "TaskDetail", "Task", "Task/Detail/{id}/Row/{rowid}", "Detail");
            AddRoute(context, "Statement", "Person", "Person/ContributionStatement/{id}/{fr}/{to}", "ContributionStatement");
            AddRoute(context, "QueryBuilderMain", "QueryBuilder", "QueryBuilder/{action}/{id}", "Main");
            AddRoute(context, "QueryBuilder");
            AddRoute(context, "QueryBuilderMain2", "QueryBuilder2", "QueryBuilder2/{action}/{id}", "Main");
            AddRoute(context, "QueryBuilder2");
            AddRoute(context, "CurrentRegs", "Person", "CurrentRegistrations", "CurrentRegistrations");

            //context.MapRoute(
            //    "QuickSearch",
            //    "QuickSearch/{action}/{q}",
            //    new { controller = "QuickSearch", action = "Index", q = "" }
            //);


            context.MapRoute(
                "Main_default",
                "Main/{controller}/{action}/{id}",
                new { action = "Index", id = "" }
            );
        }
        private void AddRoute(AreaRegistrationContext context, string controller)
        {
            context.MapRoute(controller, controller + "/{action}/{id}",
                new { controller = controller, action = "Index", id = "" });
        }
        private static void AddRoute(AreaRegistrationContext context, string name, string controller, string path, string action)
        {
            context.MapRoute(name, path,
                new { controller = controller, action = action, id = "" });
        }
    }
}
