using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using CmsData;
using CmsData.Registration;
using CmsWeb.Models;
using Elmah;
using UtilityExtensions;
using System.Collections.Generic;
using CmsData.Codes;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    [ValidateInput(false)]
    [RouteArea("OnlineReg", AreaPrefix = "OnlineReg"), Route("{action}/{id?}")]
    public partial class OnlineRegController : CmsController
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;
            filterContext.Result = Message(filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
        }

        [HttpGet]
        [Route("~/OnlineReg/{id:int}")]
        public ActionResult Index(int? id, bool? testing, string email, bool? login, string registertag, bool? showfamily, int? goerid, int? gsid, string source)
        {
            Response.NoCache();
            var m = new OnlineRegModel(Request, id, testing, email, login, source);
            m.PrepareMissionTrip(gsid, goerid);
            SetHeaders(m);

            var pid = m.CheckRegisterTag(registertag);

            return pid > 0
                ? RouteRegistration(m, pid, showfamily)
                : View(m);
                }

        // authenticate user
        [HttpPost]
        public ActionResult Login(OnlineRegModel m)
        {
            var ret = AccountModel.AuthenticateLogon(m.username, m.password, Session, Request);
            if (ret is string)
            {
                ModelState.AddModelError("authentication", ret.ToString());
                return FlowList(m, "Login");
            }
            Session["OnlineRegLogin"] = true;

            if (m.Orgid == Util.CreateAccountCode)
                return Content("/Person2/" + Util.UserPeopleId);

            var route = RouteSpecialLogin(m);
            if (route != null)
                return route;

            if (m.UserSelectsOrganization())
                m.List[0].ValidateModelForFind(ModelState, 0);

            m.List[0].LoggedIn = true;
            m.HistoryAdd("login");
            return FlowList(m, "Login");
        }

        [HttpPost]
        public ActionResult NoLogin(OnlineRegModel m)
        {
            // User clicked the register without logging in
            m.nologin = true;
            m.CreateList();
            m.HistoryAdd("nologin");
            return FlowList(m, "NoLogin");
        }

        [HttpPost]
        public ActionResult YesLogin(OnlineRegModel m)
        {
            // Takes them to the login screen after clicking the Login Here button
            m.HistoryAdd("yeslogin");
            m.nologin = false;
            m.List = new List<OnlineRegPersonModel>();
#if DEBUG
            m.username = "trecord";
#endif
            return FlowList(m, "NoLogin");
        }

        [HttpPost]
        public ActionResult Register(int id, OnlineRegModel m)
        {
            // Click a person from the family list * Take them to the Questions page
            m.StartRegistrationForFamilyMember(id, ModelState);
                return FlowList(m, "Register");
        }

        [HttpPost]
        public ActionResult Cancel(int id, OnlineRegModel m)
        {
            // After clicking Cancel, remove a person from the completed registrants list
            m.CancelRegistrant(id);
            return FlowList(m, "Cancel");
        }

        [HttpPost]
        public ActionResult ShowMoreInfo(int id, OnlineRegModel m)
        {
            // Show the Address, Gender, Marital form because record is not found
            var p = m.List[id];
            p.ValidateModelForFind(ModelState, id);
            p.PrepareToAddNewPerson(ModelState, id);
                return FlowList(m, "ShowMoreInfo");
            }

        [HttpPost]
        public ActionResult PersonFind(int id, OnlineRegModel m)
        {
            // Anonymous person tries to find their record
            m.HistoryAdd("PersonFind id=" + id);
            if (id >= m.List.Count)
                return FlowList(m, "PersonFind");
            var p = m.List[id];
            if (p.IsValidForNew)
                return ErrorResult(m, new Exception("Unexpected onlinereg state: IsValidForNew is true and in PersonFind"), "PersonFind, unexpected state");
            p.ValidateModelForFind(ModelState, id);
            if(p.AnonymousReRegistrant()) 
                return View("ConfirmReregister", m); // send email with link to reg-register
            p.FillPriorInfo();
            if(p.IsSpecialReg())
                p.OtherOK = true;
            else if (p.RegistrationFull())
                    ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].DateOfBirth), "Sorry, but registration is closed.");
            p.SetClassId();
            p.CheckSetFee();
            return FlowList(m, "PersonFind");
        }


        private ActionResult ErrorResult(OnlineRegModel m, Exception ex, string errorDisplay)
        {
            try
            {
            m.UpdateDatum();
            }
            catch (Exception)
            {
            }
            var ex2 = new Exception("{0}, {2}".Fmt(errorDisplay, m.DatumId, DbUtil.Db.ServerLink("/OnlineReg/RegPeople/") + m.DatumId), ex);
            ErrorSignal.FromCurrentContext().Raise(ex2);
            TempData["error"] = errorDisplay;
            TempData["stack"] = ex.StackTrace;
            return Content("/Error/");
        }

        [HttpPost]
        public ActionResult SubmitNew(int id, OnlineRegModel m)
        {
            ModelState.Clear();
            m.HistoryAdd("SubmitNew id=" + id);
            var p = m.List[id];
            p.ValidateModelForNew(ModelState, id);

                    SetHeaders(m);
            var ret = p.AddNew(ModelState, id);
            return ret.HasValue() 
                ? View(ret, m) 
                : FlowList(m, "SubmitNew");
                }

        [HttpPost]
        public ActionResult SubmitOtherInfo(int id, OnlineRegModel m)
        {
            m.HistoryAdd("SubmitOtherInfo id=" + id);
            if (m.List.Count <= id)
                return Content("<p style='color:red'>error: cannot find person on submit other info</p>");
            m.List[id].ValidateModelQuestions(ModelState, id);
            return FlowList(m, "SubmitOtherInfo");
        }

        [HttpPost]
        public ActionResult AddAnotherPerson(OnlineRegModel m)
        {
            m.HistoryAdd("AddAnotherPerson");
            m.ParseSettings();
            if (!ModelState.IsValid)
                return FlowList(m, "AddAnotherPerson");
#if DEBUG2
            m.List.Add(new OnlineRegPersonModel
            {
                guid = Guid.NewGuid(),
                divid = m.divid,
                orgid = m.orgid,
                masterorgid = m.masterorgid,
                first = "Bethany",
                last = "Carroll",
                //bmon = 1,
                //bday = 29,
                //byear = 1987,
                dob = "1/29/87",
                email = "davcar@pobox.com",
                phone = "9017581862".FmtFone(),
                LoggedIn = m.UserPeopleId.HasValue,
            });
#else
            m.List.Add(new OnlineRegPersonModel
            {
                orgid = m.Orgid,
                masterorgid = m.masterorgid,
                LoggedIn = m.UserPeopleId.HasValue,
            });
#endif
            return FlowList(m, "AddAnotherPerson");
        }

        [HttpPost]
        public ActionResult AskDonation(OnlineRegModel m)
        {
            m.HistoryAdd("AskDonation");
            if (m.List.Count == 0)
                return Content("Can't find any registrants");
            m.RemoveLastRegistrantIfEmpty();
            SetHeaders(m);
            return View(m);
        }


        [HttpPost]
        public ActionResult CompleteRegistration(OnlineRegModel m)
        {
            if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript)
                m.List[0].SpecialTest = SpecialRegModel.ParseResults(Request.Form);
            TempData["onlineregmodel"] = Util.Serialize<OnlineRegModel>(m);
            return Redirect("/OnlineReg/CompleteRegistration");
        }

        [HttpGet]
        public ActionResult CompleteRegistration()
        {
            Response.NoCache();
            var s = (string)TempData["onlineregmodel"];
            if (s == null)
                return Message("Registration cannot be completed after a page refresh.");
            var m = Util.DeSerialize<OnlineRegModel>(s);

            var ret = m.CompleteRegistration(this);
            switch (ret.RouteType)
            {
                case OnlineRegModel.RouteType.Error:
                    return Message(ret.Message);
                case OnlineRegModel.RouteType.Action:
                    return View(ret.View);
                case OnlineRegModel.RouteType.Redirect:
                    return RedirectToAction(ret.View, ret.RouteData);
                case OnlineRegModel.RouteType.Terms:
                    return View(ret.View, m);
                case OnlineRegModel.RouteType.Payment:
                    return View(ret.View, ret.PaymentForm);
            }
            throw new Exception("unexpected value on CompleteRegistration");

        }

        [HttpPost]
        public JsonResult CityState(string id)
        {
            var z = DbUtil.Db.ZipCodes.SingleOrDefault(zc => zc.Zip == id);
            if (z == null)
                return Json(null);
            return Json(new { city = z.City.Trim(), state = z.State });
        }

        public ActionResult Timeout(string ret)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            ViewBag.Url = ret;
            return View();
        }

        private ActionResult FlowList(OnlineRegModel m, string function)
        {
            try
            {
                m.UpdateDatum();
                var content = ViewExtensions2.RenderPartialViewToString2(this, "Flow2/List", m);
                return Content(content);
            }
            catch (Exception ex)
            {
                return ErrorResult(m, ex, "In " + function + "<br>" + ex.Message);
        }
        }

    }
}
