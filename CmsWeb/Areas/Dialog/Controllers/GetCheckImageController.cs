using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ImageData;
using System.Drawing;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Models;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "GetCheckImage"), Route("{action}/{id?}")]
    public class GetCheckImageController : Controller
    {
        // GET: Dialog/GetcheckImage
        public ActionResult GetCheckImage(int id)
        {
            if (id.IsNull())
            {
                id = 0;                
            }

            int imgId = (from c in CurrentDatabase.Contributions
                where c.ContributionId == id
                select c.ImageID).FirstOrDefault();
            CheckImageModel chkImageModel = new CheckImageModel();
            if (imgId != 0)
            {
                chkImageModel = GetImageForContribution(imgId);
            }

            return PartialView("~/Areas/Dialog/Views/GetCheckImage/GetCheckImage.cshtml", chkImageModel);
        }

        public static CheckImageModel GetImageForContribution(int imgId)
        {
            System.Data.Linq.Binary iBinary = (from i in ImageData.CurrentDatabase.Others
                                               where i.Id == imgId
                                               select i.First).FirstOrDefault();

            CheckImageModel chkModel = new CheckImageModel
            {
                ImageId = imgId,
                checkImageBytes = iBinary.ToArray()
            };

            return chkModel;
        }
    }
}
