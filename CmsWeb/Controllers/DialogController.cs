using System.Web.Mvc;

namespace CmsWeb.Controllers
{
    public class DialogController : Controller
    {
        public class Options
        {
            public bool useMailFlags { get; set; }
        }
        public ActionResult ChooseFormat(string id)
        {
            var m = new Options() {useMailFlags = id == "useMailFlags"};
            return View(m);
        }

        public ActionResult TagAll()
        {
            return View();
        }

        public ActionResult DeleteStandardExtra()
        {
            return View();
        }

        public ActionResult GetExtraValue()
        {
            return View();
        }

    }
}
