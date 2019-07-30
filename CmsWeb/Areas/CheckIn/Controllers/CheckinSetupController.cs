using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using CmsWeb.Common;
using UtilityExtensions;
using CmsWeb.Code;

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
        public JsonResult GetCheckinProfiles()
        {
            var CheckinProfiles = CurrentDatabase.CheckinProfiles.ToList();

            return Json(CheckinProfiles, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/CreateCheckinSettings")]
        public JsonResult CreateCheckinSettings()
        {
            var CheckinProfileSettings = new CheckinProfileSettingsModel();

            return Json(CheckinProfileSettings, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/CreateCheckinProfile")]
        public JsonResult CreateCheckinProfile()
        {
            var CheckinProfile = new CheckinProfileModel(){
                CheckinProfileSettings = new CheckinProfileSettingsModel()
            };

            return Json(CheckinProfile, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/GetCampuses")]
        public JsonResult GetCampuses()
        {
            var Campuses = CurrentDatabase.Campus.ToList();

            return Json(Campuses, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/CheckinSetup/InsertCheckinProfile")]
        public JsonResult InsertCheckinProfile([System.Web.Http.FromBody]CheckinProfileModel json)
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

            return GetCheckinProfiles();
        }

        [HttpDelete]
        [Route("~/CheckinSetup/DeleteProfile")]
        public JsonResult DeleteProfile([System.Web.Http.FromBody]int ProfileId)
        {
            //var paymentProcess = CurrentDatabase.PaymentProcess.SingleOrDefault(x => x.ProcessId == ProcessId);

            //paymentProcess.GatewayAccountId = null;
            //CurrentDatabase.SubmitChanges();

            return GetCheckinProfiles();
        }
    }
}
