using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    public class PeopleSearchController : CmsController
    {
        [HttpGet]
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
        public ActionResult Results()
        {
            var m = new PeopleSearchModel();
            UpdateModel(m);
            UpdateModel(m.m);
            Session["FindPeopleInfo"] = m.m;
            return View(m);
        }
        [HttpPost]
        public ActionResult ConvertToQuery()
        {
            var m = new PeopleSearchModel();
            UpdateModel(m);
            UpdateModel(m.m);
            Session["FindPeopleInfo"] = m.m;
            return Content(m.ConvertToSearch());
        }
    }
}
