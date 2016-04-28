using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Models;
using Elmah;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        [HttpGet]
        [Route("~/OnePageGiving/{id:int}")]
        public ActionResult OnePageGiving(int id, bool? testing, string source)
        {
            Response.NoCache();
            try
            {
                var m = new OnlineRegModel(Request, id, testing, null, null, source);
                if (!m.ShouldPullSpecificFund())
                    throw new Exception("Must be a single fund OnlineGiving organization");
                SetHeaders(m);
                var pf = PaymentForm.CreatePaymentForm(m);
                pf.AmtToPay = null;
                pf.Type = pf.NoCreditCardsAllowed ? "B" : "C";
                pf.First = null;
                pf.Last = null;
                pf.Email = null;

                return View("OnePageGiving/Index", pf);
            }
            catch (Exception ex)
            {
                if (ex is BadRegistrationException)
                    return Message(ex.Message);
                throw;
            }
        }

        [HttpPost, Route("~/OnePageGiving")]
        public ActionResult OnePageGiving(PaymentForm pf)
        {
            if (!Util.ValidEmail(pf.Email))
                ModelState.AddModelError("Email", "Need a valid email address");
            if (pf.AmtToPay == 0)
                ModelState.AddModelError("AmtToPay", "Invalid Amount");
            if (!pf.Country.HasValue() && !pf.Zip.HasValue())
                ModelState.AddModelError("Zip", "Zip is Required for US");

            var m = new OnlineRegModel(Request, pf.OrgId, pf.testing, null, null, pf.source);
            SetHeaders(m);
            if (!ModelState.IsValid)
                return View("OnePageGiving/Index", pf);

            if (CheckAddress(pf) == false)
                return View("OnePageGiving/Index", pf);

            pf.ValidatePaymentForm(ModelState, shouldValidateBilling: false);
            if (!ModelState.IsValid)
                return View("OnePageGiving/Index", pf);

            var p = m.List[0];
            p.orgid = m.Orgid;
            p.FirstName = pf.First;
            p.LastName = pf.Last;
            p.EmailAddress = pf.Email;
            p.Phone = pf.Phone;
            p.AddressLineOne = pf.Address;
            p.City = pf.City;
            p.State = pf.State;
            p.ZipCode = pf.Zip;
            p.Country = pf.Country;
            pf.State = pf.State;

            p.IsNew = p.person == null;

            if (pf.testing)
                pf.CheckTesting();
            var id = pf.OrgId;
            if(id == null)
                return Message("Missing OrgId");
            var ti = pf.ProcessPaymentTransaction(m);
            var fundid = m.settings[id.Value].DonationFundId ?? 0;
            p.FundItem.Add(fundid, pf.AmtToPay);
            var ret = m.ConfirmTransaction(ti);
            switch (ret.Route)
            {
                case RouteType.ModelAction:
                    if (ti.Approved == true)
                    {
                        TempData["PaymentForm"] = pf;
                        return Redirect("/OnePageGiving/ThankYou");
                    }
                    ErrorSignal.FromCurrentContext().Raise(new Exception(ti.Message));
                    ModelState.AddModelError("TranId", ti.Message);
                    return View("OnePageGiving/Index", pf);
                case RouteType.Error:
                    DbUtil.Db.LogActivity("OnePageGiving Error " + ret.Message, pf.OrgId);
                    return Message(ret.Message);
                default: // unexptected Route
                    if (ModelState.IsValid)
                    {
                        ErrorSignal.FromCurrentContext().Raise(new Exception("OnePageGiving Unexpected route"));
                        DbUtil.Db.LogActivity("OnlineReg Unexpected Route " + ret.Message, pf.OrgId);
                        ModelState.AddModelError("TranId", "unexpected error in payment processing");
                    }
                    return View(ret.View ?? "OnlineReg/Index", pf);
            }
        }

        [HttpGet, Route("~/OnePageGiving/ThankYou")]
        public ActionResult OnePageGivingThankYou()
        {
            Response.NoCache();
            var pf = (PaymentForm) TempData["PaymentForm"];
            if (pf == null)
            {
                DbUtil.LogActivity("OnePageGiving Missing Id");
                return Message("OnePageGiving cannot be started without an Id");
            }
            var m = new OnlineRegModel(Request, pf.OrgId, pf.testing, null, null, pf.source)
            {
                URL = "/OnePageGiving/" + pf.OrgId
            };
            return View("OnePageGiving/ThankYou", m);
        }

        [HttpGet, Route("~/OnePageGiving/Login/{id:int}")]
        public ActionResult OnePageGivingLogin(int id, bool? testing, string source)
        {
            var m = new OnlineRegModel(Request, id, testing, null, null, source);
            SetHeaders(m);
            return View("OnePageGiving/Login", m);
        }

        [HttpPost, Route("~/OnePageGiving/Login/{id:int}")]
        public ActionResult OnePageGivingLogin(int id, string username, string password, bool? testing, string source)
        {
            var ret = AccountModel.AuthenticateLogon(username, password, Session, Request);

            if (ret is string)
            {
                ModelState.AddModelError("loginerror", ret.ToString());
                return View("OnePageGiving/Login");
            }
            Session["OnlineRegLogin"] = true;
            return Redirect($"/OnlineReg/{id}{(testing == true ? "?testing=true" : "")}");
        }

        private bool CheckAddress(PaymentForm pf)
        {
            if (pf.Country.HasValue() && pf.Country != "United States")
            {
                pf.NeedsCityState = true;
                return false;
            }
            var r = AddressVerify.LookupAddress(pf.Address, null, pf.City, pf.State, pf.Zip);
            if (r.Line1 == "error" || r.found == false)
            {
                pf.NeedsCityState = true;
                return false;
            }

            // populate Address corrections
            if (r.Line1 != pf.Address)
                pf.Address = r.Line1;
            if (r.City != (pf.City ?? ""))
                pf.City = r.City;
            if (r.State != (pf.State ?? ""))
                pf.State = r.State;
            if (r.Zip != (pf.Zip ?? ""))
                pf.Zip = r.Zip;
            return true;
        }
    }
}