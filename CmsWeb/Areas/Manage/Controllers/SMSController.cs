using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Manage.Models.SMSMessages;
using CmsWeb.Lifecycle;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage")]
    public class SmsController : CmsStaffController
    {
        public SmsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Sms/List")]
        public ActionResult Index(SmsMessagesModel m)
        {
            UpdateModel(m.Pager);
            return View(m);
        }

        [Route("~/Sms/Details/{id:int}")]
        public ActionResult Details( int id )
        {
            var m = new SmsListModel(CurrentDatabase, id);
            return View(m);
        }

    }
}
