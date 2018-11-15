using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "GetCheckImage"), Route("{action}/{id?}")]
    public class GetCheckImageController : CMSBaseController
    {
        public GetCheckImageController(IRequestManager requestManager) : base(requestManager)
        {
        }

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

        public CheckImageModel GetImageForContribution(int imgId)
        {
            System.Data.Linq.Binary iBinary = (from i in CurrentImageDatabase.Others
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
