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
        public GatewayController(IRequestManager requestManager) : base(requestManager)
        {

        }

        [Route("~/Gateway")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("~/Gateway/GetProcesses")]
        public JsonResult GetProcesses()
        {
            var Processes = (from e in CurrentDatabase.PaymentProcess
                            join d in CurrentDatabase.GatewayAccount on e.GatewayAccountId equals d.GatewayAccountId into gj
                            from sub in gj.DefaultIfEmpty()
                            select new
                            {
                                e.ProcessId,
                                e.ProcessName,
                                sub.GatewayId,
                                GatewayAccountId = e.GatewayAccountId ?? null,
                                GatewayAccountName = sub.GatewayAccountName ?? null
                            }).ToList();

            return Json(Processes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Gateway/GetGateways")]
        public JsonResult GetGateways()
        {
            var Gateways = CurrentDatabase.Gateways.ToList();
            return Json(Gateways, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Gateway/GetGatewayDetails")]
        public JsonResult GetGatewayDetails()
        {
            var GatewayDetails = CurrentDatabase.GatewayDetails.ToList();
            return Json(GatewayDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Gateway/GetGatewayTemplate/{GatewayId}")]
        public JsonResult GetGatewayTemplate(int GatewayId)
        {
            var GatewayConfigurationTemplate = CurrentDatabase.GatewayConfigurationTemplate.Where(x => x.GatewayId == GatewayId).ToList();
            return Json(GatewayConfigurationTemplate, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Gateway/GetGatewayAccounts")]
        public JsonResult GetGatewayAccounts()
        {
            var GatewayAccount = CurrentDatabase.GatewayAccount.ToList();
            return Json(GatewayAccount, JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]
        [Route("~/Gateway/GetGatewayAccounts")]
        public JsonResult InsertAccount(CmsData.GatewayAccount json)
        {
            var gtAccount = new CmsData.GatewayAccount();
            gtAccount.GatewayAccountName = json.GatewayAccountName;
            gtAccount.GatewayId = json.GatewayId;
            CurrentDatabase.GatewayAccount.Select(x => x.)

            return GetGatewayAccounts();
        }*/
    }
}
