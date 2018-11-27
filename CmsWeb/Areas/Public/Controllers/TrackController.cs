using CmsWeb.Areas.Public.Models;
using CmsWeb.Lifecycle;
using System;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class TrackController : CMSBaseController
    {
        public TrackController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index(string id)
        {
            return Key(id);
        }
        public ActionResult Key(string id)
        {
            var b = Convert.FromBase64String("R0lGODlhAQABAIAAANvf7wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");

            var g = id.QuerystringToGuid();
            if (!g.HasValue)
            {
                return new FileContentResult(b, "image/gif");
            }

            try { CurrentDatabase.TrackOpen(g.Value); }
            catch (Exception) { }

            return new FileContentResult(b, "image/gif");
        }
        public FileContentResult Barcode(string id)
        {
            try
            {
                var code = new Code39BarCode(id) { BarCodePadding = 20 };
                return new FileContentResult(code.Generate(), "image/png");
            }
            catch (Exception)
            {
                var code = new Code39BarCode("N/A");
                return new FileContentResult(code.Generate(), "image/png");
            }
        }
    }
}
