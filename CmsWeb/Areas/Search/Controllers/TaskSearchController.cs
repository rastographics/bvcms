using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "TaskSearch"), Route("{action}/{id?}")]
    public class TaskSearchController : CmsStaffController
    {
        [HttpGet, Route("~/TaskSearch")]
        public ActionResult Index()
        {
            Response.NoCache();
            var m = new TaskSearchModel();

            m.GetPreference();
            m.SearchParameters.ExcludeCompleted = true;
            return View(m);
        }

        [HttpPost]
        public ActionResult Results(TaskSearchModel m)
        {
            m.SavePreference();
            return View(m);
        }

        [HttpPost]
        public ActionResult Clear()
        {
            var m = new TaskSearchModel();
            m.ClearPreference();
            return Redirect("/TaskSearch");
        }
    }
}
