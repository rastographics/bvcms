using System;
using System.Web.Mvc;
using CmsWeb.Areas.Org.Models.Ticketing;
using CmsWeb.Lifecycle;
using RestSharp.Extensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "Ticketing"), Route("{action}/{id?}")]
    public class TicketingController : CmsController
    {
        [Route("~/Ticketing/{id:int}")]
        public ActionResult Index(int id, string errorMessage = null)
        {
            if (!CurrentDatabase.Setting("TicketingEnabled"))
                return Message("Ticketing not enabled");
            var m = new TicketingModel(CurrentDatabase, id);
            if (errorMessage.HasValue())
                ViewBag.ErrorMessage = errorMessage;
            return View(m);
        }
        [Authorize(Roles = "Ticketing")]
        public ActionResult EventManager(int id)
        {
            if (!CurrentDatabase.Setting("TicketingEnabled"))
                return Message("Ticketing not enabled");
            var m = new TicketingModel(CurrentDatabase, id);
            return View(m);
        }
        public ActionResult BookSeats(TicketingModel m)
        {
            try
            {
                m.BookSeats();
                return View("Payment", m);
            }
            catch (Exception e)
            {
                return Redirect($"/Ticketing/{m.MeetingId}?errorMessage={e.Message}");
            }
        }

        public ActionResult PaymentSucceeded(TicketingModel m)
        {
            m.PaymentSucceeded();
            return View(m);
        }

        public ActionResult AbandonSeats(TicketingModel m)
        {
            m.AbandonSeats();
            return View(m);
        }
        public TicketingController(IRequestManager requestManager) : base(requestManager)
        {
        }
    }
}
