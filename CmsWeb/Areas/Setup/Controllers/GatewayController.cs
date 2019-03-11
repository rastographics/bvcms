using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Lifecycle;
using CmsWeb.Common;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Gateway"), Route("{action}")]
    public class GatewayController : CmsStaffController
    {
        public GatewayController(IRequestManager requestManager): base(requestManager)
        {
            if(CurrentDatabase.GatewayDetails.AsQueryable().Count() == 0)
            {
                AddGatewayDetailModel model = new AddGatewayDetailModel();
                foreach (KeyValuePair<string, object> obj in new Configuration().AsDictionary())
                {
                    model.GatewayDetailId = null;
                    model.GatewayId = 1;
                    model.GatewayDetailName = obj.Key;
                    model.GatewayDetailValue = obj.Value.ToString();
                    model.IsDefault = true;
                    model.Operation = 0;
                    AddGatewayDetail(model);
                }

                foreach(string key in new List<string>{
                    "M_ID"
                    ,"M_KEY"
                    ,"SageOriginatorId"
                    ,"GatewayTesting" })
                {
                    model.GatewayDetailId = null;
                    model.GatewayId = 2;
                    model.GatewayDetailName = key;
                    model.GatewayDetailValue = CurrentDatabase.Settings.Where(x => x.SettingX.Equals(key)).Select(x => x.SettingX).FirstOrDefault();
                    model.IsDefault = true;
                    model.Operation = 0;
                    AddGatewayDetail(model);
                }

                foreach (string key in new List<string>{
                    "TNBUsername"
                    ,"TNBPassword"
                    ,"GatewayTesting" })
                {
                    model.GatewayDetailId = null;
                    model.GatewayId = 3;
                    model.GatewayDetailName = key;
                    model.GatewayDetailValue = CurrentDatabase.Settings.Where(x => x.Id.Equals(key)).Select(x => x.SettingX).FirstOrDefault();
                    model.IsDefault = true;
                    model.Operation = 0;
                    AddGatewayDetail(model);
                }
            }
        }

        [Route("~/Gateway")]
        public ActionResult Index()
        {
            var m = CurrentDatabase.ViewMyGatewaySettings.AsQueryable();
            ViewBag.AvailableProcess = new SelectList(CurrentDatabase.ViewAvailableProcess.AsQueryable(), "ProcessId", "ProcessName");
            ViewBag.Gateways = new SelectList(CurrentDatabase.Gateways.Where(x => x.GatewayId != 5 || !x.GatewayName.Equals("DEFAULT")).AsQueryable(), "GatewayId", "GatewayName");

            ViewBag.AvailableGateways = new SelectList(CurrentDatabase.Gateways.Where(
                x => CurrentDatabase.GatewayDetails.Select(y => y.GatewayId)
                .Contains(x.GatewayId)).AsQueryable(), "GatewayId", "GatewayName");

            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("~/Gateway/AddGatewaySettings")]
        public ActionResult AddGatewaySettings(int? GatewaySettingId = null, int ProcessId = 0, int GatewayId = 0, int Operation = 0, bool applyall = false)
        {
            System.Data.Linq.ISingleResult<CmsData.CMSDataContext.Result> result = null;

            if (applyall)
            {
                List<int> Processes = CurrentDatabase.PaymmentProcess.Select(x => x.ProcessId).ToList();
                foreach(int Process in Processes)
                    result = CurrentDatabase.AddGatewaySettings(GatewaySettingId, GatewayId, Process, Operation);
            }
            else
                result = CurrentDatabase.AddGatewaySettings(GatewaySettingId, GatewayId, ProcessId, Operation);

            if (Operation == 2)
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
            var result = CurrentDatabase.AddGatewayDetail(model.GatewayDetailId, model.GatewayId, model.GatewayDetailName, model.GatewayDetailValue, model.IsDefault, model.Operation);
            return Json(result);
        }
    }

    public class AddGatewayDetailModel
    {
        public int? GatewayDetailId { get; set; }
        public int GatewayId { get; set; }
        public string GatewayDetailName { get; set; }
        public string GatewayDetailValue { get; set; }
        public bool IsDefault { get; set; }
        public int Operation { get; set; }
    }
}
