using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "RegistrationSearch"), Route("{action}/{id?}")]
    public class RegistrationSearchController : CmsStaffController
    {
        [HttpGet, Route("~/RegistrationSearch")]
        public ActionResult Index()
        {
            Response.NoCache();
            var m = new RegistrationSearchModel();
            m.Pager.Set("/RegistrationSearch/Results");

            m.GetFromSession();
            return View(m);
        }
        [HttpPost, Route("Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, RegistrationSearchModel m)
        {
            m.Pager.Set("/RegistrationSearch/Results", page, size, sort, dir);
            m.SaveToSession();
            return View(m);
        }
        [HttpPost]
        public ActionResult Clear()
        {
            var m = new RegistrationSearchModel();
            m.ClearSession();
            return Redirect("/RegistrationSearch");
        }
    }
}
