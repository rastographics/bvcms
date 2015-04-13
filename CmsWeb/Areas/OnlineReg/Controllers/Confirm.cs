using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using CmsData;
using CmsData.Finance;
using CmsWeb.Models;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;
using CmsData.Codes;
using CmsWeb.Code;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        [HttpPost]
        public ActionResult SaveProgressPayment(int id)
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed != null)
            {
                var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                m.HistoryAdd("saveprogress");
                if (m.UserPeopleId == null)
                    m.UserPeopleId = Util.UserPeopleId;
                m.UpdateDatum();
                return Json(new { confirm = "/OnlineReg/FinishLater/" + id });
            }
            return Json(new { confirm = "/OnlineReg/Unknown" });
        }

        [HttpGet]
        public ActionResult FinishLater(int id)
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed == null) 
                return View("Unknown");
            var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
            m.FinishLaterNotice();
            return View(m);
        }


        [HttpPost]
        public ActionResult ProcessPayment(PaymentForm pf)
        {
            Response.NoCache();

#if DEBUG
#else
			if (Session["FormId"] != null)
				if ((Guid)Session["FormId"] == pf.FormId)
					return Message("Already submitted");
#endif

            OnlineRegModel m = null;
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == pf.DatumId);
            if (ed != null)
                m = Util.DeSerialize<OnlineRegModel>(ed.Data);
            var peopleId = 0;
            if (m != null)
                peopleId = m.UserPeopleId ?? 0;

#if DEBUG
#else
            if (m != null && m.History.Any(h => h.Contains("ProcessPayment")))
				return Content("Already submitted");
#endif

            if (m != null)
            {
                var msg = m.CheckDuplicateGift(pf.AmtToPay);
                if (msg.HasValue())
                    return Message(msg);
            }

            SetHeaders(pf.OrgId ?? 0);
            pf.ProcessPayment(ModelState, m);

        }

        public ActionResult Confirm(int? id, string transactionId, decimal? amount)
        {
            if (!id.HasValue)
                return View("Unknown");
            if (!transactionId.HasValue())
                return Content("error no transaction");

            var m = OnlineRegModel.GetRegistrationFromDatum(id ?? 0);
            if (m == null || m.Completed)
                return Content("no pending confirmation found");

            if (m.List.Count == 0)
                return Content("no registrants found");
            try
            {
                var view = m.ConfirmTransaction(transactionId);
                m.UpdateDatum(completed: true);
                SetHeaders(m);
                if (view == ConfirmEnum.ConfirmAccount)
                    return View("ConfirmAccount", m);
                return View("Confirm", m);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["error"] = ex.Message;
                return Redirect("/Error");
            }
        }
    }
}
