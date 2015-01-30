using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Public.Models;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Public.Controllers
{
    public class TrackController : Controller
    {
        public ActionResult Index(string id)
        {
            return Key(id);
        }
        public ActionResult Key(string id)
        {
            var b = Convert.FromBase64String("R0lGODlhAQABAIAAANvf7wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");

            var g = id.QuerystringToGuid();
            if (!g.HasValue)
                return new FileContentResult(b, "image/gif");

            try { DbUtil.Db.TrackOpen(g.Value); }
            catch (Exception ex) { }

            return new FileContentResult(b, "image/gif");
        }
        public FileContentResult Barcode(string id)
        {
            id = id.GetDigits();
            if (!id.HasValue())
                id = "0";
            var code = new Code39BarCode(id);
            return new FileContentResult(code.Generate(), "image/png");
        }
    }
}
