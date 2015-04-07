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
        [Route("~/OnlineReg/Index/{id:int}")]
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

        // Register without logging in
        [HttpPost]
        public ActionResult NoLogin(OnlineRegModel m)
        {
            m.nologin = true;
            m.CreateList();
            m.HistoryAdd("nologin");
            return FlowList(m, "NoLogin");
        }

        [HttpPost]
        public ActionResult YesLogin(OnlineRegModel m)
        {
            m.HistoryAdd("yeslogin");
            m.nologin = false;
            m.List = new List<OnlineRegPersonModel>();
#if DEBUG
            m.username = "trecord";
#endif
            return FlowList(m, "NoLogin");
        }

        /* Register a person from the Family List 
         * there is no need to Find their record 
         * this will take them to the registration Questions page
         */
        [HttpPost]
        public ActionResult Register(int id, OnlineRegModel m)
        {
            m.StartRegistrationForFamilyMember(id, ModelState);
            return FlowList(m, "Register");
        }

        // Cancel will remove a person from the completed registrants list
        [HttpPost]
        public ActionResult Cancel(int id, OnlineRegModel m)
        {
            m.CancelRegistrant(id);
            return FlowList(m, "Cancel");
        }

        [HttpPost]
        public ActionResult ShowMoreInfo(int id, OnlineRegModel m)
        {
            m.HistoryAdd("ShowMoreInfo id=" + id);
            DbUtil.Db.SetNoLock();
            var p = m.List[id];
            p.ValidateModelForFind(ModelState, id);


            if (p.org != null && p.Found == true)
            {
                if (!m.SupportMissionTrip)
                    p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                if (p.IsFilled)
                    ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].DateOfBirth), "Sorry, but registration is closed.");
                if (p.Found == true)
                    p.FillPriorInfo();
                return FlowList(m, "ShowMoreInfo");
            }
            if (!p.whatfamily.HasValue && (id > 0 || p.LoggedIn == true))
            {
                ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].whatfamily), "Choose a family option");
                return FlowList(m, "ShowMoreInfo");
            }
            switch (p.whatfamily)
            {
                case 1:
                    var u = DbUtil.Db.LoadPersonById(m.UserPeopleId.Value);
                    p.AddressLineOne = u.PrimaryAddress;
                    p.City = u.PrimaryCity;
                    p.State = u.PrimaryState;
                    p.ZipCode = u.PrimaryZip.FmtZip();
                    break;
                case 2:
                    var pb = m.List[id - 1];
                    p.AddressLineOne = pb.AddressLineOne;
                    p.City = pb.City;
                    p.State = pb.State;
                    p.ZipCode = pb.ZipCode;
                    break;
                default:
#if DEBUG
                    p.AddressLineOne = "235 Riveredge Cv.";
                    p.City = "Cordova";
                    p.State = "TN";
                    p.ZipCode = "38018";
                    p.gender = 1;
                    p.married = 10;
                    p.HomePhone = "9017581862";
#endif
                    break;
            }
            p.ShowAddress = true;
            return FlowList(m, "ShowMoreInfo");
        }

        [HttpPost]
        public ActionResult PersonFind(int id, OnlineRegModel m)
        {
            m.HistoryAdd("PersonFind id=" + id);

            if (id >= m.List.Count)
                return FlowList(m, "PersonFind");

            DbUtil.Db.SetNoLock();

            var p = m.List[id];
            if (p.IsValidForNew)
                return ErrorResult(m, new Exception("Unexpected onlinereg state: IsValidForNew is true and in PersonFind"), "PersonFind, unexpected state");

            if (p.classid.HasValue)
            {
                m.Orgid = p.classid;
                m.classid = p.classid;
                p.orgid = p.classid;
            }
            p.PeopleId = null;
            p.ValidateModelForFind(ModelState, id);
            if (p.Found == true && m.org != null)
            {
                var setting = settings[m.org.OrganizationId];
                if (setting.AllowReRegister)
                {
                    var om = m.org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == p.PeopleId);
                    if (om != null)
                    {
                        m.ConfirmReregister();
                        DbUtil.Db.SubmitChanges();
                        return View("ConfirmReregister", m);
                    }
                }
            }
            if (p.ManageSubscriptions()
                 || p.OnlinePledge()
                 || p.ManageGiving()
                 || m.ChoosingSlots())
            {
                p.OtherOK = true;
            }
            else if (p.org != null)
            {
                if (!m.SupportMissionTrip)
                    p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                if (p.IsFilled)
                    ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].DateOfBirth), "Sorry, but registration is closed.");
                if (p.Found == true)
                    p.FillPriorInfo();
            }
            if (p.org != null && p.ShowDisplay() && p.ComputesOrganizationByAge())
                p.classid = p.org.OrganizationId;

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

            if (ModelState.IsValid)
            {
                if (m.ManagingSubscriptions())
                {
                    p.IsNew = true;
                    m.ConfirmManageSubscriptions();
                    DbUtil.Db.SubmitChanges();
                    return View("ManageSubscriptions/OneTimeLink", m);
                }
                if (m.OnlinePledge())
                {
                    p.IsNew = true;
                    m.SendLinkForPledge();
                    DbUtil.Db.SubmitChanges();
                    SetHeaders(m);
                    return View("ManagePledge/OneTimeLink", m);
                }
                if (m.ManageGiving())
                {
                    p.IsNew = true;
                    m.SendLinkToManageGiving();
                    DbUtil.Db.SubmitChanges();
                    SetHeaders(m);
                    return View("ManageGiving/OneTimeLink", m);
                }
                if (p.ComputesOrganizationByAge())
                {
                    if (p.org == null)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].Found), "Sorry, cannot find an appropriate age group");
                    else if (p.org.RegEnd.HasValue && DateTime.Now > p.org.RegEnd)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].Found), "Sorry, registration has ended for that group");
                    else if (p.org.OrganizationStatusId == OrgStatusCode.Inactive)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].Found), "Sorry, that group is inactive");
                    else if (p.org.OrganizationStatusId == OrgStatusCode.Inactive)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].Found), "Sorry, that group is inactive");
                }
                else if (!p.ManageSubscriptions())
                {
                    if (!m.SupportMissionTrip)
                        p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                    if (p.IsFilled)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].Found), "Sorry, registration is filled");
                }
                p.IsNew = true;
            }
            p.IsValidForExisting = ModelState.IsValid == false;
            if (p.IsNew)
                p.FillPriorInfo();
            if (p.org != null && p.ShowDisplay() && p.ComputesOrganizationByAge())
                p.classid = p.org.OrganizationId;
            //if (!p.AnyOtherInfo())
            //    p.OtherOK = ModelState.IsValid;
            return FlowList(m, "SubmitNew");
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
            RemoveLastRegistrantIfEmpty(m);
            SetHeaders(m);
            return View(m);
        }

        private static void RemoveLastRegistrantIfEmpty(OnlineRegModel m)
        {
            if (!m.last.IsNew && !m.last.Found == true)
                m.List.Remove(m.last);
            if (!(m.last.IsValidForNew || m.last.IsValidForExisting))
                m.List.Remove(m.last);
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

            m.HistoryAdd("CompleteRegistration");

            if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript)
            {
                var p = m.List[0];
                if (p.IsNew)
                    p.AddPerson(null, p.org.EntryPointId ?? 0);
                SpecialRegModel.SaveResults(m.Orgid ?? 0, m.List[0].PeopleId ?? 0, m.List[0].SpecialTest);
                return View("SpecialRegistrationResults");
            }

            if (m.AskDonation() && !m.donor.HasValue && m.donation > 0)
            {
                SetHeaders(m);
                ModelState.AddModelError("donation",
                     "Please indicate a donor or clear the donation amount");
                return View("AskDonation", m);
            }

            if (m.List.Count == 0)
                return Message("Can't find any registrants");

            RemoveLastRegistrantIfEmpty(m);

            m.UpdateDatum();
            DbUtil.LogActivity("Online Registration: {0} ({1})".Fmt(m.Header, m.DatumId));

            if (m.PayAmount() == 0 && (m.donation ?? 0) == 0 && !m.Terms.HasValue())
                return RedirectToAction("Confirm",
                     new
                     {
                         id = m.DatumId,
                         TransactionID = "zero due",
                     });

            var terms = Util.PickFirst(m.Terms, "");
            if (terms.HasValue())
                ViewData["Terms"] = terms;

            SetHeaders(m);
            if (m.PayAmount() == 0 && m.Terms.HasValue())
            {
                return View("Terms", new PaymentModel
                     {
                         Terms = m.Terms,
                         _URL = m.URL,
                         PostbackURL = DbUtil.Db.ServerLink("/OnlineReg/Confirm/" + m.DatumId),
                         _timeout = m.TimeOut
                     });
            }

            var om =
                 DbUtil.Db.OrganizationMembers.SingleOrDefault(
                      mm => mm.OrganizationId == m.Orgid && mm.PeopleId == m.List[0].PeopleId);
            m.ParseSettings();

            if (om != null && m.settings[om.OrganizationId].AllowReRegister == false && !m.SupportMissionTrip)
                return Message("You are already registered it appears");

            var pf = PaymentForm.CreatePaymentForm(m);
            if (OnlineRegModel.GetTransactionGateway() == "serviceu")
                return View("Payment/ServiceU", pf);
            ModelState.Clear();
            return View("Payment/Process", pf);
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
