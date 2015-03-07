using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org2.Models;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    [RouteArea("Organization", AreaPrefix="Dialog"), Route("{action}/{id?}")]
    public partial class DialogController : Controller
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
