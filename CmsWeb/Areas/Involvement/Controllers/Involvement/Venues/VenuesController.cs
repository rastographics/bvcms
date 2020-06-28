using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.View;
using CmsWeb.Areas.Involvement.Models;
using CmsWeb.Areas.Involvement.Models.Involvement.Venues;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using UtilityExtensions;
using RestSharp;
using SeatsioDotNet.Subaccounts;
using SeatsioDotNet.Util;
using SeatsioDotNet.Events;
using SeatsioDotNet;

namespace CmsWeb.Areas.Involvement.Controllers.Involvement
{
    [RouteArea("Involvement", AreaPrefix = "Involvement"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [SessionExpire]
    public class VenuesController : CmsStaffController
    {
        protected SeatsioClient Client;
        protected static readonly string BaseUrl = "https://api.seatsio.net";
        protected string SecretKey = "cf8a6c45-cc68-44f8-938e-14d8d27004c1";
        protected string WorkspaceKey = "80e61a21-6a93-4af8-9b75-64f41e37eb51";

        public VenuesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/Venues")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("~/ListVenues")]
        public ActionResult ListVenues()
        {
            Client = new SeatsioClient(SecretKey, WorkspaceKey, BaseUrl);
            List<ChartModel> charts = new List<ChartModel>();

            var chartsFromAPI = Client.Charts.ListAll(expandEvents: true).ToList();

            foreach (var ch in chartsFromAPI)
            {
                var chart = new ChartModel
                {
                    Id = ch.Id,
                    Key = ch.Key,
                    Name = ch.Name,
                    Status = ch.Status,
                    VenueType = "MIXED" // can't find this property in their Chart model
                };

                chart.Events = new List<Event>();

                foreach (var ev in ch.Events)
                {
                    chart.Events.Add(ev);
                }

                charts.Add(chart);
            }

            return View(charts);
        }
    }
}
