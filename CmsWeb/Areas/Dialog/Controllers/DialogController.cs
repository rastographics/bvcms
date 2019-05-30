using CmsWeb.Lifecycle;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using System;

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
        public ActionResult TagAll(Guid? id)
        {           
            ViewBag.Title = "QueryBuilder";
            ViewBag.OrigQueryId = id;
            var m = new QueryModel(id, CurrentDatabase);
            ViewBag.ForceAutoRun = TempData["autorun"];
            return View(m);
        }

        public ActionResult GetExtraValue()
        {
            return View();
        }

    }
}
