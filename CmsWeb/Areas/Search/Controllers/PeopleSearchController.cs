using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Search", AreaPrefix = "PeopleSearch"), Route("{action}")]
    public class PeopleSearchController : CmsController
    {
        public PeopleSearchController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/PeopleSearch/{name?}")]
        public ActionResult Index(string name)
        {
            var m = new PeopleSearchModel();
            if (name.HasValue())
            {
                m.m.name = name;
            }
            else
            {
                var i = Session["FindPeopleInfo"] as PeopleSearchInfo;
                if (i != null)
                {
                    m.m = i;
                }
            }

            return View(m);
        }
        [HttpPost]
        public ActionResult Results(PeopleSearchModel m)
        {
            UpdateModel(m.m);
            Session["FindPeopleInfo"] = m.m;
            return View(m);
        }
        [HttpPost]
        public ActionResult ConvertToQuery(PeopleSearchModel m)
        {
            UpdateModel(m.m);
            Session["FindPeopleInfo"] = m.m;
            return Content(m.ConvertToSearch());
        }
    }
}
