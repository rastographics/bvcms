using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Models;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    [ValidateInput(false)]
    [RouteArea("OnlineReg", AreaPrefix = "OnlineReg"), Route("{action}/{id?}")]
    public partial class OnlineRegController : CmsController
    {
        private string fromMethod;

        [HttpGet]
        [Route("~/OnlineReg/Index/{id:int}")]
        [Route("~/OnlineReg/{id:int}")]
        public ActionResult Index(int? id, bool? testing, string email, bool? login, string registertag, bool? showfamily, int? goerid, int? gsid, string source)
        {
            Response.NoCache();
            try
            {
                var m = new OnlineRegModel(Request, CurrentDatabase, id, testing, email, login, source);

                if (m.ManageGiving())
                {
                    Session["Campus"] = Request.QueryString["campus"];
                    Session["DefaultFunds"] = Request.QueryString["funds"];
                    m.Campus = Session["Campus"]?.ToString();
                    m.DefaultFunds = Session["DefaultFunds"]?.ToString();
                }
                
                if (m.org != null && m.org.IsMissionTrip == true)
                {
                    if (gsid != null || goerid != null)
                    {
                        m.PrepareMissionTrip(gsid, goerid);
                    }
                }

                SetHeaders(m);
                var pid = m.CheckRegisterLink(registertag);
                if (m.NotActive())
                {
                    return View("OnePageGiving/NotActive", m);
                }
                if (m.MissionTripSelfSupportPaylink.HasValue() && m.GoerId > 0)
                {
                    return Redirect(m.MissionTripSelfSupportPaylink);
                }

                return RouteRegistration(m, pid, showfamily);
            }
            catch (Exception ex)
            {
                if (ex is BadRegistrationException)
                {
                    return Message(ex.Message);
                }

                throw;
            }
        }
        [HttpPost]
        public ActionResult Login(OnlineRegModel m)
        {
            fromMethod = "Login";
            var ret = AccountModel.AuthenticateLogon(m.username, m.password, Session, Request);

            if (ret is string)
            {
                ModelState.AddModelError("authentication", ret.ToString());
                return FlowList(m);
            }
            Session["OnlineRegLogin"] = true;

            if (m.Orgid == Util.CreateAccountCode)
            {
                DbUtil.LogActivity("OnlineReg CreateAccount Existing", peopleid: Util.UserPeopleId, datumId: m.DatumId);
                return Content("/Person2/" + Util.UserPeopleId); // they already have an account, so take them to their page
            }
            m.UserPeopleId = Util.UserPeopleId;
            var route = RouteSpecialLogin(m);
            if (route != null)
            {
                return route;
            }

            m.HistoryAdd("login");
            if (m.org != null && m.org.IsMissionTrip == true && m.SupportMissionTrip)
            {
                OnlineRegPersonModel p;
                PrepareFirstRegistrant(ref m, m.UserPeopleId.Value, false, out p);    
            }
            return FlowList(m);
        }

        private void PrepareFirstRegistrant(ref OnlineRegModel m, int pid, bool? showfamily, out OnlineRegPersonModel p)
        {
            p = null;
            if (showfamily != true)
            {
                // No need to pick family, so prepare first registrant ready to answer questions
                p = m.LoadExistingPerson(pid, 0);
                if (p == null)
                    throw new Exception($"No person found with PeopleId = {pid}");

                p.ValidateModelForFind(ModelState, 0);
                if (m.masterorg == null)
                {
                    if (m.List.Count == 0)
                        m.List.Add(p);
                    else
                        m.List[0] = p;
                }
            }
        }

        [HttpPost]
        public ActionResult NoLogin(OnlineRegModel m)
        {
            fromMethod = "NoLogin";
            // Clicked the register without logging in link
            m.nologin = true;
            m.CreateAnonymousList();
            m.Log("NoLogin");
            return FlowList(m);
        }

        [HttpPost]
        public ActionResult YesLogin(OnlineRegModel m)
        {
            fromMethod = "YesLogin";
            // clicked the Login Here button
            m.HistoryAdd("yeslogin");
            m.nologin = false;
            m.List = new List<OnlineRegPersonModel>();
#if DEBUG
            m.username = "David";
#endif
            return FlowList(m);
        }

        [HttpPost]
        public ActionResult RegisterFamilyMember(int id, OnlineRegModel m)
        {
            // got here by clicking on a link in the Family list
            var msg = m.CheckExpiredOrCompleted();
            if (msg.HasValue())
            {
                return PageMessage(msg);
            }

            fromMethod = "Register";

            m.StartRegistrationForFamilyMember(id, ModelState);

            // show errors or take them to the Questions page
            return FlowList(m);
        }

        [HttpPost]
        public ActionResult Cancel(int id, OnlineRegModel m)
        {
            // After clicking Cancel, remove a person from the completed registrants list
            fromMethod = "Cancel";
            m.CancelRegistrant(id);
            return FlowList(m);
        }

        [HttpPost]
        public ActionResult FindRecord(int id, OnlineRegModel m)
        {
            // Anonymous person clicks submit to find their record
            var msg = m.CheckExpiredOrCompleted();
            if (msg.HasValue())
            {
                return PageMessage(msg);
            }

            fromMethod = "FindRecord";
            m.HistoryAdd("FindRecord id=" + id);
            if (id >= m.List.Count)
            {
                return FlowList(m);
            }

            var p = m.GetFreshFindInfo(id);

            if (p.NeedsToChooseClass())
            {
                p.RegistrantProblem = "Please Make Selection Above";
                return FlowList(m);
            }
            p.ValidateModelForFind(ModelState, id);
            if (!ModelState.IsValid)
            {
                return FlowList(m);
            }

            if (p.AnonymousReRegistrant())
            {
                return View("Continue/ConfirmReregister", m); // send email with link to reg-register
            }

            if (p.IsSpecialReg())
            {
                p.QuestionsOK = true;
            }
            else if (p.RegistrationFull())
            {
                m.Log("Closed");
                ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].DateOfBirth), "Sorry, but registration is closed.");
            }

            p.FillPriorInfo();
            p.SetSpecialFee();

            if (!ModelState.IsValid || p.count == 1)
            {
                return FlowList(m);
            }

            // form is ok but not found, so show AddressGenderMarital Form
            p.PrepareToAddNewPerson(ModelState, id);
            p.Found = false;
            return FlowList(m);
        }


        [HttpPost]
        public ActionResult SubmitNew(int id, OnlineRegModel m)
        {
            // Submit from AddressMaritalGenderForm
            var msg = m.CheckExpiredOrCompleted();
            if (msg.HasValue())
            {
                return PageMessage(msg);
            }

            fromMethod = "SubmitNew";
            ModelState.Clear();
            m.HistoryAdd("SubmitNew id=" + id);
            var p = m.List[id];
            if (p.ComputesOrganizationByAge())
            {
                p.orgid = null; // forget any previous information about selected org, may have new information like gender
            }

            p.ValidateModelForNew(ModelState, id);

            SetHeaders(m);
            var ret = p.AddNew(ModelState, id);
            return ret.HasValue()
                ? View(ret, m)
                : FlowList(m);
        }

        [HttpPost]
        public ActionResult SubmitQuestions(int id, OnlineRegModel m)
        {
            var ret = m.CheckExpiredOrCompleted();
            if (ret.HasValue())
            {
                return PageMessage(ret);
            }

            fromMethod = "SubmitQuestions";
            m.HistoryAdd("SubmitQuestions id=" + id);
            if (m.List.Count <= id)
            {
                return Content("<p style='color:red'>error: cannot find person on submit other info</p>");
            }

            m.List[id].ValidateModelQuestions(ModelState, id);
            return FlowList(m);
        }

        [HttpPost]
        public ActionResult AddAnotherPerson(OnlineRegModel m)
        {
            var ret = m.CheckExpiredOrCompleted();
            if (ret.HasValue())
            {
                return PageMessage(ret);
            }

            fromMethod = "AddAnotherPerson";
            m.HistoryAdd("AddAnotherPerson");
            m.ParseSettings();
            if (!ModelState.IsValid)
            {
                return FlowList(m);
            }

            m.List.Add(new OnlineRegPersonModel
            {
                orgid = m.Orgid,
                masterorgid = m.masterorgid,
            });
            return FlowList(m);
        }

        [HttpPost]
        public ActionResult AskDonation(OnlineRegModel m)
        {
            m.HistoryAdd("AskDonation");
            if (m.List.Count == 0)
            {
                m.Log("AskDonationError NoRegistrants");
                return Content("Can't find any registrants");
            }
            m.RemoveLastRegistrantIfEmpty();
            SetHeaders(m);
            return View("Other/AskDonation", m);
        }
        [HttpPost]
        public ActionResult PostDonation(OnlineRegModel m)
        {
            if (m.donor == null && m.donation > 0)
            {
                ModelState.AddModelError("donation", "Please indicate who is the donor");
                SetHeaders(m);
                return View("Other/AskDonation", m);
            }
            TempData["onlineregmodel"] = Util.Serialize(m);
            return Redirect("/OnlineReg/CompleteRegistration");
        }

#if DEBUG
        // For testing only
        [HttpGet]
        [Route("~/OnlineReg/CompleteRegistration/{id:int}")]
        public ActionResult CompleteRegistration(int id)
        {
            var ed = CurrentDatabase.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            var m = Util.DeSerialize<OnlineRegModel>(ed?.Data);
            TempData["onlineregmodel"] = Util.Serialize(m);
            return Redirect("/OnlineReg/CompleteRegistration");
        }
#endif
        [HttpPost]
        public ActionResult CompleteRegistration(OnlineRegModel m)
        {
            if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript)
            {
                m.List[0].SpecialTest = SpecialRegModel.ParseResults(Request.Form);
            }

            TempData["onlineregmodel"] = Util.Serialize(m);
            return Redirect("/OnlineReg/CompleteRegistration");
        }

        [HttpGet]
        public ActionResult CompleteRegistration()
        {
            Response.NoCache();
            var s = (string)TempData["onlineregmodel"];
            if (s == null)
            {
                DbUtil.LogActivity("OnlineReg Error PageRefreshNotAllowed");
                return Message("Registration cannot be completed after a page refresh.");
            }
            var m = Util.DeSerialize<OnlineRegModel>(s);
            var msg = m.CheckExpiredOrCompleted();
            if (msg.HasValue())
            {
                return Message(msg);
            }

            var ret = m.CompleteRegistration(this);
            switch (ret.Route)
            {
                case RouteType.Error:
                    m.Log(ret.Message);
                    return Message(ret.Message);
                case RouteType.Action:
                    return View(ret.View);
                case RouteType.Redirect:
                    return RedirectToAction(ret.View, ret.RouteData);
                case RouteType.Terms:
                    return View(ret.View, m);
                case RouteType.Payment:
                    return View(ret.View, ret.PaymentForm);
            }
            m.Log("BadRouteOnCompleteRegistration");
            return Message("unexpected value on CompleteRegistration");
        }

        [HttpPost]
        public JsonResult CityState(string id)
        {
            var z = CurrentDatabase.ZipCodes.SingleOrDefault(zc => zc.Zip == id);
            if (z == null)
            {
                return Json(null);
            }

            return Json(new { city = z.City.Trim(), state = z.State });
        }

        public ActionResult Timeout(string ret)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            ViewBag.Url = ret;
            return View("Other/Timeout");
        }

        private ActionResult FlowList(OnlineRegModel m)
        {
            try
            {
                m.UpdateDatum();
                m.Log(fromMethod);
                var content = ViewExtensions2.RenderPartialViewToString2(this, "Flow2/List", m);
                return Content(content);
            }
            catch (Exception ex)
            {
                return ErrorResult(m, ex, "In " + fromMethod + "<br>" + ex.Message);
            }
        }

        private ActionResult ErrorResult(OnlineRegModel m, Exception ex, string errorDisplay)
        {
            // ReSharper disable once EmptyGeneralCatchClause
            try
            {
                m.UpdateDatum();
            }
            catch
            {
            }

            var ex2 = new Exception($"{errorDisplay}, {CurrentDatabase.ServerLink("/OnlineReg/RegPeople/") + m.DatumId}", ex);
            ErrorSignal.FromCurrentContext().Raise(ex2);
            m.Log(ex2.Message);
            TempData["error"] = errorDisplay;
            TempData["stack"] = ex.StackTrace;
            return Content("/Error/");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
            DbUtil.LogActivity("OnlineReg Error:" + filterContext.Exception.Message);
            filterContext.Result = Message(filterContext.Exception.Message, filterContext.Exception.StackTrace);
            filterContext.ExceptionHandled = true;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            requestContext.HttpContext.Items["controller"] = this;
        }

        [HttpGet]
        [Route("~/OnlineReg/{id:int}/Giving/")]
        [Route("~/OnlineReg/{id:int}/Giving/{goerid:int}")]
        public ActionResult Giving(int id, int? goerid, int? gsid)
        {
            var m = new OnlineRegModel(Request, CurrentDatabase, id, false, null, null, null);
            if (m.org != null && m.org.IsMissionTrip == true && m.org.TripFundingPagesEnable == true)
            {
                m.PrepareMissionTrip(gsid, goerid);
            }
            else
            {
                return new HttpNotFoundResult();
            }

            SetHeaders(m);
            if (m.MissionTripCost == null)
            {
                // goer specified isn't part of this trip
                return new HttpNotFoundResult();
            }
            if (Util.UserPeopleId == goerid)
            {
                return View("Giving/Goer", m);
            }
            else if (m.org.TripFundingPagesPublic)
            {
                return View("Giving/Guest", m);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
    }
}
