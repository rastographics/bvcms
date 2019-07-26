using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using CmsWeb.Common;
using UtilityExtensions;

namespace CmsWeb.Areas.CheckIn.Controllers
{
    [Authorize(Roles = "Checkin")]
    [RouteArea("CheckIn", AreaPrefix = "CheckinSetup"), Route("{action}")]
    public class CheckinSetupController : CmsStaffController
    {
        public CheckinSetupController(IRequestManager requestManager) : base(requestManager)
        {

        }

        [Route("~/CheckinSetup")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("~/CheckinSetup/GetCheckinProfiles")]
        public JsonResult GetProfiles()
        {
            var CheckinProfiles = CurrentDatabase.CheckinProfiles.Where(c => c.CheckinProfileId > 0);
            //var Processes = (from e in CurrentDatabase.PaymentProcess
            //                 join d in CurrentDatabase.GatewayAccount on e.GatewayAccountId equals d.GatewayAccountId into gj
            //                 from sub in gj.DefaultIfEmpty()
            //                 select new
            //                 {
            //                     e.ProcessId,
            //                     e.ProcessName,
            //                     sub.GatewayId,
            //                     GatewayAccountId = e.GatewayAccountId ?? null,
            //                     GatewayAccountName = sub.GatewayAccountName ?? null
            //                 }).ToList();

            //return Json(Processes, JsonRequestBehavior.AllowGet);

            return Json(CheckinProfiles, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/GetProfileSettings")]
        public JsonResult GetProfileSettings()
        {
            //var Gateways = CurrentDatabase.Gateways.ToList();
            //return Json(Gateways, JsonRequestBehavior.AllowGet);

            return Json("Ok");
        }

        [HttpPost]
        [Route("~/CheckinSetup/InsertProfile/{IsInsert}")]
        public JsonResult InsertProfile([System.Web.Http.FromBody]CheckinProfiles json, bool IsInsert)
        {
            //var paymentProcess = CurrentDatabase.PaymentProcess.SingleOrDefault(x => x.ProcessId == json.ProcessId);

            //if (IsInsert)
            //{
            //    var gtAccount = new CmsData.GatewayAccount();
            //    var gtDetailAccount = new CmsData.GatewayDetails();

            //    gtAccount.GatewayAccountName = json.GatewayAccountName;
            //    gtAccount.GatewayId = json.GatewayId;
            //    CurrentDatabase.GatewayAccount.InsertOnSubmit(gtAccount);
            //    CurrentDatabase.SubmitChanges();

            //    for (int i = 0; i < json.GatewayAccountInputs.Count(); i++)
            //    {
            //        gtDetailAccount.GatewayAccountId = gtAccount.GatewayAccountId;
            //        gtDetailAccount.GatewayDetailName = json.GatewayAccountInputs[i];
            //        gtDetailAccount.GatewayDetailValue = json.GatewayAccountValues[i];
            //        gtDetailAccount.IsBoolean = json.GatewayAccountValues[i] == "true" || json.GatewayAccountValues[i] == "false" ? true : false;
            //        CurrentDatabase.GatewayDetails.InsertOnSubmit(gtDetailAccount);
            //        CurrentDatabase.SubmitChanges();
            //        gtDetailAccount = new CmsData.GatewayDetails();
            //    }

            //    if (json.UseForAll)
            //    {
            //        for (int i = 0; i < CurrentDatabase.PaymentProcess.Select(x => x.ProcessId).Count(); i++)
            //        {
            //            paymentProcess = CurrentDatabase.PaymentProcess.SingleOrDefault(x => x.ProcessId == i + 1);
            //            paymentProcess.GatewayAccountId = gtAccount.GatewayAccountId;
            //            CurrentDatabase.SubmitChanges();
            //        }
            //    }
            //    else
            //    {
            //        paymentProcess.GatewayAccountId = gtAccount.GatewayAccountId;
            //        CurrentDatabase.SubmitChanges();
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < json.GatewayAccountInputs.Count(); i++)
            //    {
            //        var gtDetailAccount = CurrentDatabase.GatewayDetails.SingleOrDefault(
            //            x => x.GatewayDetailName == json.GatewayAccountInputs[i]
            //            && x.GatewayAccountId == json.GatewayAccountId);

            //        gtDetailAccount.GatewayDetailValue = json.GatewayAccountValues[i];
            //        CurrentDatabase.SubmitChanges();
            //    }

            //    if (json.UseForAll)
            //    {
            //        for (int i = 0; i < CurrentDatabase.PaymentProcess.Select(x => x.ProcessId).Count(); i++)
            //        {
            //            paymentProcess = CurrentDatabase.PaymentProcess.SingleOrDefault(x => x.ProcessId == i + 1);
            //            paymentProcess.GatewayAccountId = json.GatewayAccountId;
            //            CurrentDatabase.SubmitChanges();
            //        }
            //    }
            //    else
            //    {
            //        paymentProcess.GatewayAccountId = json.GatewayAccountId;
            //        CurrentDatabase.SubmitChanges();
            //    }
            //}

            return GetProfiles();
        }

        [HttpDelete]
        [Route("~/CheckinSetup/DeleteProfile")]
        public JsonResult DeleteProfile([System.Web.Http.FromBody]int ProfileId)
        {
            //var paymentProcess = CurrentDatabase.PaymentProcess.SingleOrDefault(x => x.ProcessId == ProcessId);

            //paymentProcess.GatewayAccountId = null;
            //CurrentDatabase.SubmitChanges();

            return GetProfiles();
        }
    }
}
