using CmsData;
using CmsData.Classes;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
using Dapper;
using Elmah;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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
            var ed = CurrentDatabase.RegistrationDatas.SingleOrDefault(e => e.Id == pf.DatumId);
            if (ed != null)
            {
                m = Util.DeSerialize<OnlineRegModel>(ed.Data);
            }

#if DEBUG
#else
            if (m != null && m.History.Any(h => h.Contains("ProcessPayment")))
                return Content("Already submitted");
#endif

            int? datumid = null;
            if (m != null)
            {
                datumid = m.DatumId;
                var msg = m.CheckDuplicateGift(pf.AmtToPay);
                if (Util.HasValue(msg))
                {
                    return Message(msg);
                }
            }
            if (IsCardTester(pf, "Payment Page"))
            {
                return Message("Found Card Tester");
            }

            if (CurrentDatabase.Setting("UseRecaptcha"))
            {
                if (!GoogleRecaptcha.IsValidResponse(HttpContext, CurrentDatabase))
                {
                    CurrentDatabase.LogActivity("OnlineReg Error ReCaptcha validation failed.", pf.OrgId, did: datumid);
                    ModelState.AddModelError("form", "ReCaptcha validation failed.");
                    return View("Payment/Process", pf);
                }
            }

            SetHeaders(pf.OrgId ?? 0);
            var ret = pf.ProcessPayment(ModelState, m);
            switch (ret.Route)
            {
                case RouteType.ModelAction:
                    return View(ret.View, ret.Model);
                case RouteType.AmtDue:
                    ViewBag.amtdue = ret.AmtDue;
                    return View(ret.View, ret.Transaction);
                case RouteType.Error:
                    CurrentDatabase.LogActivity("OnlineReg Error " + ret.Message, pf.OrgId, did: datumid);
                    return Message(ret.Message);
                case RouteType.ValidationError:
                    return View(ret.View, pf);
                default: // unexptected Route
                    if (ModelState.IsValid)
                    {
                        ErrorSignal.FromCurrentContext().Raise(new Exception("OnlineReg Unexpected route datum= " + datumid));
                        CurrentDatabase.LogActivity("OnlineReg Unexpected Route " + ret.Message, oid: pf.OrgId, did: datumid);
                        ModelState.AddModelError("form", "unexpected error in payment processing");
                    }
                    return View(ret.View ?? "Payment/Process", pf);
            }
        }

        private bool IsCardTester(PaymentForm pf, string from)
        {
            if (!Util.IsHosted || !pf.CreditCard.HasValue())
            {
                return false;
            }

            var hash = Pbkdf2Hasher.HashString(pf.CreditCard);
            CurrentDatabase.InsertIpLog(Request.UserHostAddress, hash);

            if (pf.IsProblemUser())
            {
                return LogRogueUser("Problem User", from);
            }

            var iscardtester = ConfigurationManager.AppSettings["IsCardTester"];
            if (!iscardtester.HasValue())
            {
                return false;
            }
            var result = CurrentDatabase.Connection.ExecuteScalar<string>(iscardtester, new { ip = Request.UserHostAddress });
            if (result.Equal("OK"))
            {
                return false;
            }

            return LogRogueUser(result, from);
        }

        public static bool LogRogueUser(string why, string from)
        {
            //todo: static?
            var request = HttpContextFactory.Current.Request;
            var insertRogueIp = ConfigurationManager.AppSettings["InsertRogueIp"];
            if (insertRogueIp.HasValue())
            {
                DbUtil.Db.Connection.Execute(insertRogueIp, new { ip = request.UserHostAddress, db = Util.Host });
            }

            var form = Encoding.Default.GetString(request.BinaryRead(request.TotalBytes));
            var sendto = Util.PickFirst(ConfigurationManager.AppSettings["CardTesterEmail"], Util.AdminMail);
            DbUtil.Db.SendEmail(Util.FirstAddress(sendto),
                $"CardTester on {Util.Host}", $"why={why} from={from} ip={request.UserHostAddress}<br>{form.HtmlEncode()}",
                Util.EmailAddressListFromString(sendto));
            return true;
        }

        [ActionName("Confirm")]
        [HttpPost]
        public ActionResult Confirm_Post(int? id, string transactionId, decimal? amount)
        {
            if (!id.HasValue)
            {
                return View("Other/Unknown");
            }

            var m = OnlineRegModel.GetRegistrationFromDatum(id ?? 0);
            if (m == null || m.Completed)
            {
                if (m == null)
                {
                    DbUtil.LogActivity("OnlineReg NoPendingConfirmation");
                }
                else
                {
                    m.Log("NoPendingConfirmation");
                }

                return Content("no pending confirmation found");
            }
            if (!Util.HasValue(transactionId))
            {
                m.Log("NoTransactionId");
                return Content("error no transaction");
            }
            if (m.List.Count == 0)
            {
                m.Log("NoRegistrants");
                return Content("no registrants found");
            }
            try
            {
                OnlineRegModel.LogOutOfOnlineReg();
                var view = m.ConfirmTransaction(transactionId);
                m.UpdateDatum(completed: true);
                SetHeaders(m);
                if (view == ConfirmEnum.ConfirmAccount)
                {
                    m.Log("ConfirmAccount");
                    return View("Continue/ConfirmAccount", m);
                }
                m.Log("Confirm");
                return View("Confirm", m);
            }
            catch (Exception ex)
            {
                m.Log("Error " + ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["error"] = ex.Message;
                return Redirect("/Error");
            }
        }
        [HttpGet]
        public ActionResult Confirm(int? id, string transactionId, decimal? amount)
        {
            if (!id.HasValue)
            {
                return View("Other/Unknown");
            }

            var m = OnlineRegModel.GetRegistrationFromDatum(id ?? 0);
            if (m == null || m.Completed)
            {
                if (m == null)
                {
                    DbUtil.LogActivity("OnlineReg NoPendingConfirmation");
                }
                else
                {
                    m.Log("NoPendingConfirmation");
                }

                return Content("no pending confirmation found");
            }
            if (!Util.HasValue(transactionId))
            {
                m.Log("NoTransactionId");
                return Content("error no transaction");
            }
            if (m.List.Count == 0)
            {
                m.Log("NoRegistrants");
                return Content("no registrants found");
            }
            try
            {
                OnlineRegModel.LogOutOfOnlineReg();
                var view = m.ConfirmTransaction(transactionId);
                m.UpdateDatum(completed: true);
                SetHeaders(m);
                if (view == ConfirmEnum.ConfirmAccount)
                {
                    m.Log("ConfirmAccount");
                    return View("Continue/ConfirmAccount", m);
                }
                m.Log("Confirm");
                return View("Confirm", m);
            }
            catch (Exception ex)
            {
                m.Log("Error " + ex.Message);
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

            if (!Util.HasValue(q))
            {
                return Message("unknown");
            }

            var id = Util.Decrypt(q).ToInt2();
            var qq = from t in CurrentDatabase.Transactions
                     where t.OriginalId == id || t.Id == id
                     orderby t.Id descending
                     select new { t, email = t.TransactionPeople.FirstOrDefault().Person.EmailAddress };
            var i = qq.FirstOrDefault();
            if (i == null)
            {
                return Message("no outstanding transaction");
            }

            var ti = i.t;
            var email = i.email;
            var amtdue = PaymentForm.AmountDueTrans(CurrentDatabase, ti);
            if (amtdue == 0)
            {
                return Message("no outstanding transaction");
            }

#if DEBUG
            ti.Testing = true;
            if (!Util.HasValue(ti.Address))
            {
                ti.Address = "235 Riveredge";
                ti.City = "Cordova";
                ti.Zip = "38018";
                ti.State = "TN";
            }
#endif
            var pf = PaymentForm.CreatePaymentFormForBalanceDue(ti, amtdue, email);

            SetHeaders(pf.OrgId ?? 0);

            DbUtil.LogActivity("OnlineReg PayDueStart", ti.OrgId, ti.LoginPeopleId ?? ti.FirstTransactionPeopleId());
            return View("Payment/Process", pf);
        }

        [ActionName("ConfirmDuePaid")]
        [HttpPost]
        public ActionResult ConfirmDuePaid_Post(int? id, string transactionId, decimal amount)
        {
            Response.NoCache();
            if (!id.HasValue)
            {
                return View("Other/Unknown");
            }

            if (!Util.HasValue(transactionId))
            {
                DbUtil.LogActivity("OnlineReg PayDueNoTransactionId");
                return Message("error no transactionid");
            }
            var ti = CurrentDatabase.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (ti == null)
            {
                DbUtil.LogActivity("OnlineReg PayDueNoPendingTrans");
                return Message("no pending transaction");
            }
#if DEBUG
            ti.Testing = true;
#endif
            OnlineRegModel.ConfirmDuePaidTransaction(ti, transactionId, sendmail: true);
            ViewBag.amtdue = PaymentForm.AmountDueTrans(CurrentDatabase, ti).ToString("C");
            SetHeaders(ti.OrgId ?? 0);
            DbUtil.LogActivity("OnlineReg PayDueConfirm", ti.OrgId, ti.LoginPeopleId ?? ti.FirstTransactionPeopleId());
            return View("PayAmtDue/Confirm", ti);
        }

        [HttpGet]
        public ActionResult ConfirmDuePaid(int? id, string transactionId, decimal amount)
        {
            Response.NoCache();
            if (!id.HasValue)
            {
                return View("Other/Unknown");
            }

            if (!Util.HasValue(transactionId))
            {
                DbUtil.LogActivity("OnlineReg PayDueNoTransactionId");
                return Message("error no transactionid");
            }
            var ti = CurrentDatabase.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (ti == null)
            {
                DbUtil.LogActivity("OnlineReg PayDueNoPendingTrans");
                return Message("no pending transaction");
            }
#if DEBUG
            ti.Testing = true;
#endif
            OnlineRegModel.ConfirmDuePaidTransaction(ti, transactionId, sendmail: true);
            ViewBag.amtdue = PaymentForm.AmountDueTrans(CurrentDatabase, ti).ToString("C");
            SetHeaders(ti.OrgId ?? 0);
            DbUtil.LogActivity("OnlineReg PayDueConfirm", ti.OrgId, ti.LoginPeopleId ?? ti.FirstTransactionPeopleId());
            return View("PayAmtDue/Confirm", ti);
        }

        [HttpGet]
        public ActionResult PayDueTest(string q)
        {
            if (!Util.HasValue(q))
            {
                return Message("unknown");
            }

            var id = Util.Decrypt(q);
            var ed = CurrentDatabase.ExtraDatas.SingleOrDefault(e => e.Id == id.ToInt());
            if (ed == null)
            {
                return Message("no outstanding transaction");
            }

            return Content(ed.Data);
        }
    }
}
