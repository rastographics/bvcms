using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Controllers
{
    public class DialogController : Controller
    {
        [HttpGet, Route("Dialog/ChooseFormat")]
        public ActionResult ChooseFormat()
        {
            return View();
        }

        [HttpGet, Route("Dialog/TagAll")]
        public ActionResult TagAll()
        {
            return View();
        }

        [HttpGet, Route("Dialog/DeleteStandardExtra")]
        public ActionResult DeleteStandardExtra()
        {
            return View();
        }

        [HttpGet, Route("Dialog/GetExtraValue")]
        public ActionResult GetExtraValue()
        {
            return View();
        }

    }
}
