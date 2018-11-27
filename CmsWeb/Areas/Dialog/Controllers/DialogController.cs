using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "Dialog"), Route("{action}/{id?}")]
    public partial class DialogController : CMSBaseController
    {
        public class Options
        {
            public bool useMailFlags { get; set; }
        }

        public ActionResult ChooseFormat(string id)
        {
            var m = new Options() { useMailFlags = id == "useMailFlags" };
            return View(m);
        }
        public ActionResult TagAll()
        {
            return View();
        }

        public ActionResult GetExtraValue()
        {
            return View();
        }

    }
}
