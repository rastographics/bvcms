using System.Web.Mvc;
using CmsWeb.Models;

namespace CmsWeb.Controllers
{
	public class SupportController : CmsController
	{
        [HttpPost, Route("Support/SendRequest")]
        [ValidateInput(false)]
		public ActionResult SendSupportRequest(SupportRequestModel m)
        {
			if (!SupportRequestModel.CanSupport) 
                return Content("Support not available!");
            m.SendSupportRequest();
			return Content("OK");
		}

        [HttpPost, Route("Support/MyDataSendRequest")]
        [ValidateInput(false)]
		public ActionResult MyDataSendSupportRequest(SupportRequestModel m)
        {
			if (!SupportRequestModel.CanSupport) 
                return Content("Support not available!");
            m.MyDataSendSupportRequest();
			return Content("OK");
		}
	}
}