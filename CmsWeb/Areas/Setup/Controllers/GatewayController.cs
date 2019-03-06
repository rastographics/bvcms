using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Lifecycle;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Gateway"), Route("{action}")]
    public class GatewayController : CmsStaffController
    {
        public GatewayController(IRequestManager requestManager): base(requestManager)
        {

        }

        [Route("~/Gateway")]
        public ActionResult Index()
        {
            var m = CurrentDatabase.ViewMyGatewaySettings.AsQueryable();
            ViewBag.AvailableProcess = new SelectList(CurrentDatabase.ViewAvailableProcess.AsQueryable(), "ProcessId", "ProcessName");
            ViewBag.Gateways = new SelectList(CurrentDatabase.Gateways.AsQueryable(), "GatewayId", "GatewayName");

            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("~/Gateway/AddGatewaySettings")]
        public ActionResult AddGatewaySettings(int? GatewaySettingId = null, int ProcessId = 0, int GatewayId = 0, int Operation = 0)
        {
            var result = CurrentDatabase.AddGatewaySettings(GatewaySettingId, GatewayId, ProcessId, Operation);

            if(Operation == 2)
                return Json(result, JsonRequestBehavior.AllowGet);
            else
                return RedirectToAction("Index", "Gateway");
        }

        [HttpGet]
        [Route("~/Gateway/Get_Gateway_Config/{Id:int}")]
        public JsonResult Get_Gateway_Config(int Id)
        {
            var Config = CurrentDatabase.GatewayDetails.Where(x => x.GatewayId == Id);
            return Json(Config, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Gateway/AddGatewayDetail")]
        public ActionResult AddGatewayDetail(AddGatewayDetailModel model)
        {
            if(model.GatewayDetailName == null && model.Operation != 2)
                return Json(null);
            else if (model.GatewayDetailValue == null)
                model.GatewayDetailValue = String.Empty;
            var result = CurrentDatabase.AddGatewayDetail(model.GatewayDetailId, model.GatewayId, model.GatewayDetailName, model.GatewayDetailValue, model.Operation);
            return Json(result);
        }
    }

    public class AddGatewayDetailModel
    {
        public int? GatewayDetailId { get; set; }
        public int GatewayId { get; set; }
        public string GatewayDetailName { get; set; }
        public string GatewayDetailValue { get; set; }
        public int Operation { get; set; }
    }
}
