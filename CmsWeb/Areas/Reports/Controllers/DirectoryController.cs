using CmsWeb.Areas.Main.Models.Directories;
using CmsWeb.Areas.Reports.Models;
using System;
using System.Web.Mvc;

namespace CmsWeb.Areas.Reports.Controllers
{
    public partial class ReportsController
    {
        [HttpGet]
        public ActionResult DocXDirectory(Guid id, string filename, string template)
        {
            return new DocXDirectoryResult(id, filename, template);
        }
        [HttpGet]
        public ActionResult CompactDir(Guid id)
        {
            return new CompactDir(id);
            //            return new DocXDirectoryResult(id, "compactdir", 
            //                Server.MapPath("/content/touchpoint/templates/compactdir.docx"));
        }

        [HttpGet]
        public ActionResult PictureDir(Guid id)
        {
            return new DocXDirectoryResult(id, "picturedir",
                Server.MapPath("/content/touchpoint/templates/picturedir24.docx"));
        }

        [HttpGet]
        public ActionResult FamilyPictureDir(Guid id)
        {
            return new DocXDirectoryResult(id, "familypicturedir",
                Server.MapPath("/content/touchpoint/templates/fampicturedir12.docx"));
        }

        [HttpGet]
        public ActionResult PictureDir4(Guid id)
        {
            return new DocXDirectoryResult(id, "picturedir4",
                Server.MapPath("/content/touchpoint/templates/picturedir4.docx"));
        }

    }
}
