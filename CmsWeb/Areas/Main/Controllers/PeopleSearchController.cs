using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix="PeopleSearch"), Route("{action}")]
    public class PeopleSearchController : CmsController
    {
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
                    m.m = i;
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
