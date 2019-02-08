using System;
using System.Collections.Generic;
using System.Linq;
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
                var m = new OnlineRegModel(Request, CurrentDatabase, id, testing, null, null, source);

                var pid = Util.UserPeopleId;
                if (pid.HasValue)
                    PrePopulate(m, pid.Value);

                SetHeaders(m);
                m.CheckRegisterLink(null);

                if (m.NotActive())
                {
                    return View("OnePageGiving/NotActive", m);
                }

                var pf = PaymentForm.CreatePaymentForm(m);
                pf.AmtToPay = null;

                if (string.IsNullOrWhiteSpace(pf.Type))
                    pf.Type = pf.NoCreditCardsAllowed ? "B" : "C";

#if DEBUG
                if (!pid.HasValue)
                {
                    pf.First = "Otis";
                    pf.Last = "Sukamotis";
                    pf.Email = "davcar@pobox.com";
                    pf.Address = "135 Riveredge Cv";
                    pf.Zip = "";
                    pf.CreditCard = "3111111111111111";
                    pf.Expires = "1018";
                    pf.CVV = "123";
                    pf.AmtToPay = 23M;
                }
#endif

                var p = m.List[0];
                if (pf.ShowCampusOnePageGiving)
                    pf.Campuses = p.Campuses().ToList();

                var designatedFund = p.DesignatedDonationFund().FirstOrDefault();
                pf.Description = designatedFund != null ? designatedFund.Text : m.DescriptionForPayment;

                SetInstructions(m);

                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }
            catch (Exception ex)
            {
                if (ex is BadRegistrationException)
                    return Message(ex.Message);
                throw;
            }
        }

        private void PrePopulate(OnlineRegModel m, int pid)
        {
            m.UserPeopleId = pid;
            var p = m.LoadExistingPerson(pid, 0);
            m.List[0] = p;
        }

        private void SetInstructions(OnlineRegModel m)
        {
            var s = m.SubmitInstructions();
            ViewBag.Instructions = s.HasValue() ? s : $"<h4>{m.Header}</h4>";
        }


        [HttpPost, Route("~/OnePageGiving")]
        public ActionResult OnePageGiving(PaymentForm pf, Dictionary<int, decimal?> fundItem)
        {
            // need save off the original amt to pay if there is an error later on.
            var amtToPay = pf.AmtToPay;

            var id = pf.OrgId;
            if (id == null)
                return Message("Missing OrgId");

            if (!Util.ValidEmail(pf.Email))
                ModelState.AddModelError("Email", "Need a valid email address");
            if (pf.IsUs && !pf.Zip.HasValue())
                ModelState.AddModelError("Zip", "Zip is Required for US");
            if (pf.ShowCampusOnePageGiving)
                if ((pf.CampusId ?? 0) == 0)
                    ModelState.AddModelError("CampusId", "Campus is Required");

            var m = new OnlineRegModel(Request, CurrentDatabase, pf.OrgId, pf.testing, null, null, pf.source)
            {
                URL = $"/OnePageGiving/{pf.OrgId}"
            };

            var pid = Util.UserPeopleId;
            if (pid.HasValue)
                PrePopulate(m, pid.Value);

            // we need to always retrieve the entire list of funds for one page giving calculations.
            m.List[0].RetrieveEntireFundList = true;

            // first re-build list of fund items with only ones that contain a value (amt).
            var fundItems = fundItem.Where(f => f.Value.GetValueOrDefault() > 0).ToDictionary(f => f.Key, f => f.Value);

            var designatedFund = m.settings[id.Value].DonationFundId ?? 0;
            if (designatedFund != 0)
                fundItems.Add(designatedFund, pf.AmtToPay);

            // set the fund items on online reg person if there are any.
            if (fundItems.Any())
            {
                m.List[0].FundItem = fundItems;
                pf.AmtToPay = m.List[0].FundItemsChosen().Sum(f => f.Amt);
            }

            if (pf.AmtToPay.GetValueOrDefault() == 0)
                ModelState.AddModelError("AmtToPay", "Invalid Amount");

            SetHeaders(m);
            SetInstructions(m);

            var p = m.List[0];
            if (pf.ShowCampusOnePageGiving)
                pf.Campuses = p.Campuses().ToList();

            if (!ModelState.IsValid)
            {
                m.List[0].FundItem = fundItem;
                pf.AmtToPay = amtToPay;
                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }

            if (CheckAddress(pf) == false)
            {
                m.List[0].FundItem = fundItem;
                pf.AmtToPay = amtToPay;
                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }


            if (!ModelState.IsValid)
            {
                m.List[0].FundItem = fundItem;
                pf.AmtToPay = amtToPay;
                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }

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
            if (pf.ShowCampusOnePageGiving)
                p.Campus = pf.CampusId.ToString();

            p.IsNew = p.person == null;

            if (pf.testing)
                pf.CheckTesting();

            if (pf.Country.HasValue() && !pf.Zip.HasValue())
                pf.Zip = "NA";

            pf.ValidatePaymentForm(ModelState, false);
            if (!ModelState.IsValid)
            {
                m.List[0].FundItem = fundItem;
                pf.AmtToPay = amtToPay;
                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }


            if (m?.UserPeopleId != null && m.UserPeopleId > 0)
                pf.CheckStoreInVault(ModelState, m.UserPeopleId.Value);
            if (!ModelState.IsValid)
            {
                m.List[0].FundItem = fundItem;
                pf.AmtToPay = amtToPay;
                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }
            
            if (CurrentDatabase.Setting("UseRecaptcha"))
            {
                if (!GoogleRecaptcha.IsValidResponse(HttpContext, CurrentDatabase))
                {
                    m.List[0].FundItem = fundItem;
                    pf.AmtToPay = amtToPay;
                    ModelState.AddModelError("TranId", "ReCaptcha validation failed.");
                    return View("OnePageGiving/Index", new OnePageGivingModel {
                        OnlineRegPersonModel = m.List[0],
                        PaymentForm = pf
                    });
                }
            }

            var ti = pf.ProcessPaymentTransaction(m);
            if ((ti.Approved ?? false) == false)
            {
                ModelState.AddModelError("TranId", ti.Message);

                m.List[0].FundItem = fundItem;
                pf.AmtToPay = amtToPay;
                return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }
            if (pf.Zip == "NA")
                pf.Zip = null;

            var ret = m.ConfirmTransaction(ti);
            switch (ret.Route)
            {
                case RouteType.ModelAction:
                    if (ti.Approved == true)
                    {
                        var url = $"/OnePageGiving/ThankYou/{id}{(pf.testing ? $"?testing=true&source={pf.source}" : $"?source={pf.source}")}";
                        return Redirect(url);
                    }
                    ErrorSignal.FromCurrentContext().Raise(new Exception(ti.Message));
                    ModelState.AddModelError("TranId", ti.Message);

                    m.List[0].FundItem = fundItem;
                    pf.AmtToPay = amtToPay;
                    return View("OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
                case RouteType.Error:
                    CurrentDatabase.LogActivity("OnePageGiving Error " + ret.Message, pf.OrgId);
                    return Message(ret.Message);
                default: // unexptected Route
                    ErrorSignal.FromCurrentContext().Raise(new Exception("OnePageGiving Unexpected route"));
                    CurrentDatabase.LogActivity("OnlineReg Unexpected Route " + ret.Message, pf.OrgId);
                    ModelState.AddModelError("TranId", "unexpected error in payment processing");

                    m.List[0].FundItem = fundItem;
                    pf.AmtToPay = amtToPay;
                    return View(ret.View ?? "OnePageGiving/Index", new OnePageGivingModel() { OnlineRegPersonModel = m.List[0], PaymentForm = pf });
            }
        }

        [HttpGet, Route("~/OnePageGiving/ThankYou/{id:int}")]
        public ActionResult OnePageGivingThankYou(int id, bool? testing, string source)
        {
            Response.NoCache();
            var m = new OnlineRegModel(Request, CurrentDatabase, id, testing, null, null, source)
            { URL = "/OnePageGiving/" + id };
            return View("OnePageGiving/ThankYou", m);
        }

        [HttpGet, Route("~/OnePageGiving/Login/{id:int}")]
        public ActionResult OnePageGivingLogin(int id, bool? testing, string source)
        {
            var m = new OnlineRegModel(Request, CurrentDatabase, id, testing, null, null, source);
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
                var m = new OnlineRegModel(Request, CurrentDatabase, id, testing, null, null, source);
                SetHeaders(m);
                return View("OnePageGiving/Login", m);
            }
            Session["OnlineRegLogin"] = true;


            var ev = CurrentDatabase.OrganizationExtras.SingleOrDefault(vv => vv.OrganizationId == id && vv.Field == "LoggedInOrgId");
            id = ev?.IntValue ?? id;
            var url = $"/OnePageGiving/{id}{(testing == true ? "?testing=true" : "")}";
            return Redirect(url);
        }

        private bool CheckAddress(PaymentForm pf)
        {
            if (!pf.IsUs)
            {
                pf.NeedsCityState = true;
                return pf.City.HasValue() && pf.State.HasValue();
            }
            var r = AddressVerify.LookupAddress(pf.Address, null, pf.City, pf.State, pf.Zip);
            if (r.Line1 == "error" || r.found == false)
            {
                if (pf.City.HasValue()
                    && pf.State.HasValue()
                    && pf.Zip.HasValue()
                    && pf.Address.HasValue())
                    return true; // not found but complete
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
