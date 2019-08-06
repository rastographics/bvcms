using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [ValidateInput(false)]
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
            if (!CurrentUser.IsInRole("Developer"))
            {
                GatewayDetails.RemoveAll(d => d.GatewayDetailName == "GatewayTesting");
            }
            return Json(GatewayDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Gateway/GetGatewayTemplate/{GatewayId}")]
        public JsonResult GetGatewayTemplate(int GatewayId)
        {
            var GatewayConfigurationTemplate = CurrentDatabase.GatewayConfigurationTemplate.Where(x => x.GatewayId == GatewayId).ToList();
            if (!CurrentUser.IsInRole("Developer"))
            {
                GatewayConfigurationTemplate.RemoveAll(d => d.GatewayDetailName == "GatewayTesting");
            }
            return Json(GatewayConfigurationTemplate, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Gateway/GetGatewayAccounts")]
        public JsonResult GetGatewayAccounts()
        {
            var GatewayAccount = CurrentDatabase.GatewayAccount.ToList();
            return Json(GatewayAccount, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Gateway/InsertAccount/{IsInsert}")]
        public JsonResult InsertAccount([System.Web.Http.FromBody]Models.GatewayAccountJsonModel json, bool IsInsert)
        {
            var paymentProcesses = CurrentDatabase.PaymentProcess.ToList();
            var paymentProcess = paymentProcesses.Single(x => x.ProcessId == json.ProcessId);

            if (IsInsert)
            {
                var gtAccount = new GatewayAccount();
                var gtDetailAccount = new GatewayDetails();

                gtAccount.GatewayAccountName = json.GatewayAccountName;
                gtAccount.GatewayId = json.GatewayId;
                CurrentDatabase.GatewayAccount.InsertOnSubmit(gtAccount);
                CurrentDatabase.SubmitChanges();

                for (int i = 0; i < json.GatewayAccountInputs.Count(); i++)
                {
                    gtDetailAccount.GatewayAccountId = gtAccount.GatewayAccountId;
                    gtDetailAccount.GatewayDetailName = json.GatewayAccountInputs[i];
                    gtDetailAccount.GatewayDetailValue = json.GatewayAccountValues[i];
                    gtDetailAccount.IsBoolean = json.GatewayAccountValues[i] == "true" || json.GatewayAccountValues[i] == "false" ? true : false;
                    CurrentDatabase.GatewayDetails.InsertOnSubmit(gtDetailAccount);
                    gtDetailAccount = new GatewayDetails();
                }

                if (json.UseForAll)
                {
                    foreach (var process in paymentProcesses.Where(p => p.ProcessId != (int)PaymentProcessTypes.TemporaryRecurringGiving))
                    {
                        process.GatewayAccountId = gtAccount.GatewayAccountId;
                    }
                }
                else
                {
                    paymentProcess.GatewayAccountId = gtAccount.GatewayAccountId;
                }
            }
            else
            {
                for (int i = 0; i < json.GatewayAccountInputs.Count(); i++)
                {
                    var gtDetailAccount = CurrentDatabase.GatewayDetails.Single(
                        x => x.GatewayDetailName == json.GatewayAccountInputs[i]
                        && x.GatewayAccountId == json.GatewayAccountId);

                    gtDetailAccount.GatewayDetailValue = json.GatewayAccountValues[i];
                }

                if (json.UseForAll)
                {
                    foreach (var process in paymentProcesses.Where(p => p.ProcessId != (int)PaymentProcessTypes.TemporaryRecurringGiving))
                    {
                        process.GatewayAccountId = json.GatewayAccountId;
                    }
                }
                else
                {
                    paymentProcess.GatewayAccountId = json.GatewayAccountId;
                }
            }
            CurrentDatabase.SubmitChanges();

            return GetGatewayAccounts();
        }

        [HttpPost]
        [Route("~/Gateway/DeleteProcessAccount")]
        public JsonResult DeleteProcessAccount([System.Web.Http.FromBody]int ProcessId)
        {
            var paymentProcess = CurrentDatabase.PaymentProcess.SingleOrDefault(x => x.ProcessId == ProcessId);

            paymentProcess.GatewayAccountId = null;
            CurrentDatabase.SubmitChanges();

            return GetGatewayAccounts();
        }

        [HttpGet]
        [Route("~/Gateway/GetGatewayConfig/{ProcessType}/{Key}")]
        public JsonResult GetGatewayConfig(int ProcessType, string Key)
        {
            string PushpayMerchant = MultipleGatewayUtils.Setting(CurrentDatabase, Key, "", ProcessType);
            return Json(PushpayMerchant, JsonRequestBehavior.AllowGet);
        }
    }
}
