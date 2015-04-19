using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using Elmah;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
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
            var ret = pf.ProcessPayment(ModelState, m);
            if (ret.Route == RouteType.AmtDue)
            {
                ViewBag.amtdue = ret.AmtDue;
                return View(ret.View, ret.Transaction);
            }
            return View(ret.View);
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
                OnlineRegModel.LogOutOfOnlineReg();
                var view = m.ConfirmTransaction(transactionId);
                m.UpdateDatum(completed: true);
                SetHeaders(m);
                if (view == ConfirmEnum.ConfirmAccount)
                    return View("ConfirmAccount", m);
                return View("Confirm", m);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["error"] = ex.Message;
                return Redirect("/Error");
            }
        }

        [HttpGet]
        public ActionResult PayAmtDue(string q)
        {
            // reached by the paylink in the confirmation email
            // which is produced in EnrollAndConfirm
            Response.NoCache();

            if (!q.HasValue())
                return Message("unknown");
            var id = Util.Decrypt(q).ToInt2();
            var qq = from t in DbUtil.Db.Transactions
                     where t.OriginalId == id || t.Id == id
                     orderby t.Id descending
                     select new {t, email = t.TransactionPeople.FirstOrDefault().Person.EmailAddress };
            var i = qq.FirstOrDefault();
            if(i == null)
                return Message("no outstanding transaction");

            var ti = i.t;
            var email = i.email;
            var amtdue = PaymentForm.AmountDueTrans(DbUtil.Db, ti);
            if (amtdue == 0)
                return Message("no outstanding transaction");

#if DEBUG
            ti.Testing = true;
            if (!ti.Address.HasValue())
            {
                ti.Address = "235 Riveredge";
                ti.City = "Cordova";
                ti.Zip = "38018";
                ti.State = "TN";
            }
#endif
            var pf = PaymentForm.CreatePaymentFormForBalanceDue(ti, amtdue, email);

            SetHeaders(pf.OrgId ?? 0);

            return View("Payment/Process", pf);
        }

        public ActionResult ConfirmDuePaid(int? id, string transactionId, decimal amount)
        {
            if (!id.HasValue)
                return View("Unknown");
            if (!transactionId.HasValue())
                return Message("error no transaction");

            var ti = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (ti == null)
                return Message("no pending transaction");
#if DEBUG
            ti.Testing = true;
#endif
            OnlineRegModel.ConfirmDuePaidTransaction(ti, transactionId, sendmail: true);
            ViewBag.amtdue = PaymentForm.AmountDueTrans(DbUtil.Db, ti).ToString("C");
            SetHeaders(ti.OrgId ?? 0);
            return View("PayAmtDue/Confirm", ti);
        }

        [HttpGet]
        public ActionResult PayDueTest(string q)
        {
            if (!q.HasValue())
                return Message("unknown");
            var id = Util.Decrypt(q);
            var ed = DbUtil.Db.ExtraDatas.SingleOrDefault(e => e.Id == id.ToInt());
            if (ed == null)
                return Message("no outstanding transaction");
            return Content(ed.Data);
        }
    }
}
