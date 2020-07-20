using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Org.Models.Venues;
using CmsWeb.Lifecycle;
using SeatsioDotNet.Events;
using SeatsioDotNet;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "Venues"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [SessionExpire]
    public class VenuesController : CmsStaffController
    {
        protected SeatsioClient Client;
        const string BaseUrl = "https://api.seatsio.net";
        protected string SecretKey => CurrentDatabase.Setting("TicketingWorkspaceSecretKey", "");
        protected string WorkspaceKey => CurrentDatabase.Setting("TicketingWorkspaceKey", "");

        [HttpGet, Route("~/Venues")]
        public ActionResult Index()
        {
            Client = new SeatsioClient(SecretKey, WorkspaceKey, BaseUrl);
            var charts = new List<ChartModel>();
            var chartsFromApi = Client.Charts.ListAll(expandEvents: true).ToList();

            foreach (var ch in chartsFromApi)
            {
                var chart = new ChartModel
                {
                    Id = ch.Id,
                    Key = ch.Key,
                    Name = ch.Name,
                    Status = ch.Status,
                    PublishedVersionThumbnailUrl = ch.PublishedVersionThumbnailUrl,
                    Events = new List<Event>()
                };

                foreach (var ev in ch.Events)
                {
                    chart.Events.Add(ev);
                }
                charts.Add(chart);
            }

            return View(charts);
        }

        [HttpGet, Route("~/ListVenues")]
        public ActionResult ListVenues()
        {
            Client = new SeatsioClient(SecretKey, WorkspaceKey, BaseUrl);
            List<ChartModel> charts = new List<ChartModel>();

            var chartsFromApi = Client.Charts.ListAll(expandEvents: true).ToList();

            foreach (var ch in chartsFromApi)
            {
                var chart = new ChartModel
                {
                    Id = ch.Id,
                    Key = ch.Key,
                    Name = ch.Name,
                    Status = ch.Status,
                    VenueType = "MIXED",
                    Events = new List<Event>()
                };

                foreach (var ev in ch.Events)
                {
                    chart.Events.Add(ev);
                }
                charts.Add(chart);
            }
            return View(charts);
        }

        [HttpGet, Route("~/CreateVenue")]
        public ActionResult CreateVenue()
        {
            ChartModel model = new ChartModel
            {
                Key = "",
                SecretKey = SecretKey
            };
            return View(model);
        }

        [HttpGet, Route("~/ChartDesigner")]
        public ActionResult ChartDesigner(string key)
        {
            ChartModel model = new ChartModel
            {
                Key = key,
                SecretKey = SecretKey
            };
            return View("CreateVenue", model);
        }

        [HttpGet, Route("~/DeleteChart")]
        public ActionResult DeleteChart(string key)
        {
            // TODO: add logic to delete here

            return View("Index");
        }

        public VenuesController(IRequestManager requestManager) : base(requestManager)
        {
        }
    }
}
