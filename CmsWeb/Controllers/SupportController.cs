using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Controllers
{
    public class SupportController : CmsController
    {
        public SupportController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("Support/SendRequest")]
        [ValidateInput(false)]
        public ActionResult SendSupportRequest(SupportRequestModel m)
        {
            if (!SupportRequestModel.CanSupport)
            {
                return Content("Support not available!");
            }

            m.SendSupportRequest();
            return Content("OK");
        }

        [HttpPost, Route("Support/MyDataSendRequest")]
        [ValidateInput(false)]
        public ActionResult MyDataSendSupportRequest(SupportRequestModel m)
        {
            if (!SupportRequestModel.CanSupport)
            {
                return Content("Support not available!");
            }

            m.MyDataSendSupportRequest();
            return Content("OK");
        }
    }
}
