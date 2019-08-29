using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "GetCheckImage"), Route("{action}/{id?}")]
    [Authorize(Roles = "Finance,FinanceDataEntry")]
    public class GetCheckImageController : CMSBaseController
    {
        public GetCheckImageController(IRequestManager requestManager) : base(requestManager)
        {
        }

        // GET: Dialog/GetcheckImage
        public ActionResult GetCheckImage(int id)
        {
            CheckImageModel chkImageModel = GetImageForContribution(id);

            return PartialView("~/Areas/Dialog/Views/GetCheckImage/GetCheckImage.cshtml", chkImageModel);
        }

        // GET: Dialog/RawCheckImage
        public ActionResult RawCheckImage(int id)
        {
            CheckImageModel chkImageModel = GetImageForContribution(id);
            return new FileContentResult(chkImageModel.checkImageBytes, "image/png");
        }

        public CheckImageModel GetImageForContribution(int contributionId)
        {
            int imgId = (from c in CurrentDatabase.Contributions
                         where c.ContributionId == contributionId
                         select c.ImageID).FirstOrDefault();

            CheckImageModel chkModel = null;
            if (imgId != 0)
            {
                System.Data.Linq.Binary iBinary = (from i in CurrentImageDatabase.Others
                                                   where i.Id == imgId
                                                   select i.First).FirstOrDefault();

                chkModel = new CheckImageModel
                {
                    ImageId = imgId,
                    checkImageBytes = iBinary.ToArray()
                };
            }
            return chkModel ?? new CheckImageModel();
        }
    }
}
