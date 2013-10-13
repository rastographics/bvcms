using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;

namespace CmsWeb.Controllers
{
    public class DialogController : Controller
    {
        [GET("Dialog/ChooseFormat")]
        public ActionResult ChooseFormat()
        {
            return View();
        }

        [GET("Dialog/TagAll")]
        public ActionResult TagAll()
        {
            return View();
        }

        [GET("Dialog/DeleteStandardExtra")]
        public ActionResult DeleteStandardExtra()
        {
            return View();
        }

        [GET("Dialog/GetExtraValue")]
        public ActionResult GetExtraValue()
        {
            return View();
        }

    }
}
