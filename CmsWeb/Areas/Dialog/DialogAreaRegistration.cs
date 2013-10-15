using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog
{
    public class DialogAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Dialog";
            }
       } 

        public override void RegisterArea(AreaRegistrationContext context)
        {
            AddRoute(context, "SearchAdd");
            AddRoute(context, "AddOrganization");
            AddRoute(context, "SearchUsers");
            AddRoute(context, "SearchOrgs");
            AddRoute(context, "SearchDivisions");
            AddRoute(context, "OrgMemberDialog");
            AddRoute(context, "AddToOrgFromTag");
            AddRoute(context, "AddAttendeesFromTag");
            AddRoute(context, "RepairTransactions");
            AddRoute(context, "DeleteMeeting");
            AddRoute(context, "TransactionHistory");
            AddRoute(context, "OrgMembersDialog");
           context.MapRoute( "DeleteAllTranHistory",
                "TransactionHistory/DeleteAll/{orgid}/{peopleid}",
                new { controller = "TransactionHistory", action = "DeleteAll", orgid="", peopleid="" }
            );
            context.MapRoute(
                "Dialog_default",
                "Dialog/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
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
