using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
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
                pf.Type = "C";
                pf.NoCreditCardsAllowed = false;

                pf.First = null;
                pf.Last = null;
                pf.Email = null;

//                pf.First = "David";
//                pf.Last = "Carroll" + DateTime.Now.Millisecond;
//                pf.Email = "david@bvcms.com";
//                pf.Phone = "9017581862";
//                pf.Address = "235 Riveredge Cv";
//                pf.Zip = "38018";
//                pf.CreditCard = "4111111111111111";
//                pf.Expires = "1218";
//                pf.CVV = "123";
//                pf.testing = false;
//                pf.AmtToPay = 33.44M;

                return View("OnePageGiving/Index", pf);
            }
            catch (Exception ex)
            {
                if (ex is BadRegistrationException)
                    return Message(ex.Message);
                throw;
            }
        }

        [HttpPost, Route("~/OnePageGiving/{id:int}")]
        public ActionResult OnePageGiving(int id, PaymentForm pf)
        {
            if (!Util.ValidEmail(pf.Email))
                ModelState.AddModelError("Email", "Need a valid email address");
            if(pf.AmtToPay == 0)
                ModelState.AddModelError("AmtToPay", "Invalid Amount");

            var m = new OnlineRegModel(Request, pf.OrgId, pf.testing, null, null, pf.source);
            SetHeaders(m);
            if(!ModelState.IsValid)
                return View("OnePageGiving/Index", pf);

            var p = m.List[0];
            p.orgid = m.Orgid;
            p.FirstName = pf.First;
            p.LastName = pf.Last;
            p.EmailAddress = pf.Email;
            p.Phone = pf.Phone;
            p.AddressLineOne = pf.Address;
            p.ZipCode = pf.Zip;

            p.ValidateModelForFind(ModelState, id);

            pf.ShouldValidateBilling = false;
            pf.CheckTesting();
            var ret = pf.ProcessPayment(ModelState, m);
            switch (ret.Route)
            {
                case RouteType.ModelAction:
                    TempData["PaymentForm"] = pf;
                    return Redirect("/OnePageGiving");
                case RouteType.Error:
                    DbUtil.Db.LogActivity("OnlineReg Error " + ret.Message, pf.OrgId);
                    return Message(ret.Message);
                case RouteType.ValidationError:
                    return View("OnePageGiving/Index", pf);
            }
            ErrorSignal.FromCurrentContext().Raise(new Exception("Unexpected state in OnePageGiving"));
            return Message("Something did not go well");
        }

        [HttpGet, Route("~/OnePageGiving")]
        public ActionResult OnePageGiving()
        {
            Response.NoCache();
            var pf = (PaymentForm) TempData["PaymentForm"];
            if (pf == null)
            {
                DbUtil.LogActivity("OnePageGiving Missing Id");
                return Message("OnePageGiving cannot be started without an Id");
            }
            return Message("Thank You");
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
    }
}
