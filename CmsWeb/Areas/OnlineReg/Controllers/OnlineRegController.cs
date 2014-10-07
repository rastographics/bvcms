using System;
using System.Diagnostics;
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
        [HttpGet]
        [Route("~/OnlineReg/{id:int}")]
        [Route("~/OnlineReg/Index/{id:int}")]
        public ActionResult Index(int? id, bool? testing, string email, bool? nologin, bool? login, string registertag, bool? showfamily, int? goerid, int? gsid)
        {
            if (Util.IsDebug())
            {
                var q = from om in DbUtil.Db.OrganizationMembers
                        where om.OrganizationId == 89539 && om.PeopleId == 828612
                        select om;
                foreach (var om in q)
                    om.Drop(DbUtil.Db, addToHistory: false);
                //        DbUtil.Db.PurgePerson(om.PeopleId);
//                var dr = DbUtil.Db.People.SingleOrDefault(mm => mm.Name == "David Roll");
//                if (dr != null)
//                    foreach (var mm in dr.Family.People)
//                        if (mm.PeopleId != dr.PeopleId)
//                            DbUtil.Db.PurgePerson(mm.PeopleId);
                DbUtil.Db.SubmitChanges();
            }
            if (DbUtil.Db.Roles.Any(rr => rr.RoleName == "disabled"))
                return Content("Site is disabled for maintenance, check back later");
            Response.NoCache();
            if (!id.HasValue)
                return Message("no organization");
            var m = new OnlineRegModel { Orgid = id };
            if (m.org == null && m.masterorg == null)
                return Message("invalid registration");

            if (m.masterorg != null)
            {
                if (!OnlineRegModel.UserSelectClasses(m.masterorg).Any())
                    return Message("no classes available on this org");
            }
            else if (m.org != null)
            {
                if ((m.org.RegistrationTypeId ?? 0) == RegistrationTypeCode.None)
                    return Message("no registration allowed on this org");
                if (m.org.IsMissionTrip == true)
                {
                    if (gsid.HasValue)
                    {
                        var gs = DbUtil.Db.GoerSupporters.Single(gg => gg.Id == gsid);
                        m.GoerId = gs.GoerId;
                        m.GoerSupporterId = gsid;
                    }
                    else if (goerid.HasValue)
                    {
                        m.GoerId = goerid;
                    }
                }
            }
            if (Request.Url != null) m.URL = Request.Url.OriginalString;

            SetHeaders(m);

            m.testing = testing == true || DbUtil.Db.Setting("OnlineRegTesting", Util.IsDebug() ? "true" : "false").ToBool();

            if (Util.ValidEmail(email) || login != true)
                m.nologin = true;

            if (m.nologin)
                m.CreateList();
            else
                m.List = new List<OnlineRegPersonModel>();

            if (Util.ValidEmail(email))
                m.List[0].EmailAddress = email;


            var pid = 0;
            if (registertag.HasValue())
            {
                var guid = registertag.ToGuid();
                if (guid == null)
                    return Message("invalid link");
                var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                if (ot == null)
                    return Message("invalid link");
#if DEBUG
#else
                if (ot.Used)
                    return Message("link used");
#endif
                if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                    return Message("link expired");
                var a = ot.Querystring.Split(',');
                pid = a[1].ToInt();
                m.registertag = registertag;
            }
            else if (User.Identity.IsAuthenticated)
            {
                pid = Util.UserPeopleId ?? 0;
            }

            if (pid > 0)
            {
                //m.List = new List<OnlineRegPersonModel>();
                m.UserPeopleId = pid;
                var existingRegistration = m.GetExistingRegistration(pid);
                if (existingRegistration != null)
                {
                    TempData["er"] = m.UserPeopleId;
                    return Redirect("/OnlineReg/Existing/" + existingRegistration.DatumId);
                }
                OnlineRegPersonModel p = null;
                if (showfamily != true)
                {
                    p = m.LoadExistingPerson(pid, 0);
                    p.ValidateModelForFind(ModelState, m, 0);
                    p.LoggedIn = true;
                    if (m.masterorg == null)
                    {
                        if (m.List.Count == 0)
                            m.List.Add(p);
                        else
                            m.List[0] = p;
                    }
                }
                if (!ModelState.IsValid)
                    return View(m);

                if (m.masterorg != null && m.masterorg.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions2)
                {
                    TempData["ms"] = m.UserPeopleId;
                    return Redirect("/OnlineReg/ManageSubscriptions/{0}".Fmt(m.masterorgid));
                }
                if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.ManageGiving)
                {
                    TempData["mg"] = m.UserPeopleId;
                    return ManageGiving(m.Orgid.ToString(), m.testing);
                    //return Redirect("/OnlineReg/ManageGiving/{0}".Fmt(m.Orgid));
                }
                if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.OnlinePledge)
                {
                    TempData["mp"] = m.UserPeopleId;
                    return Redirect("/OnlineReg/ManagePledge/{0}".Fmt(m.Orgid));
                }
                if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                {
                    TempData["ps"] = m.UserPeopleId;
                    return Redirect("/OnlineReg/ManageVolunteer/{0}".Fmt(m.Orgid));
                }
                if (showfamily != true && p.org != null && p.Found == true)
                {
                    p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                    if (p.IsFilled)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[0].Found), "Sorry, but registration is closed.");
                    if (p.Found == true)
                        p.FillPriorInfo();
                    p.CheckSetFee();
                    m.HistoryAdd("index, pid={0}, !showfamily, p.org, found=true".Fmt(pid));
                    return View(m);
                }
                m.HistoryAdd("index, pid=" + pid);
                return View(m);
            }
            m.HistoryAdd("index");
            return View(m);
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
            var existingRegistration = m.GetExistingRegistration(Util.UserPeopleId ?? 0);
            if (existingRegistration != null)
            {
                TempData["er"] = m.UserPeopleId = Util.UserPeopleId;
                return Content("/OnlineReg/Existing/" + existingRegistration.DatumId);
            }
            Debug.Assert(Util.UserPeopleId != null, "Util.UserPeopleId != null");

            m.CreateList();
            m.UserPeopleId = Util.UserPeopleId;

            if (m.ManagingSubscriptions())
            {
                TempData["ms"] = Util.UserPeopleId;
                return Content("/OnlineReg/ManageSubscriptions/{0}".Fmt(m.masterorgid));
            }
            if (m.ChoosingSlots())
            {
                TempData["ps"] = Util.UserPeopleId;
                return Content("/OnlineReg/ManageVolunteer/{0}".Fmt(m.Orgid));
            }
            if (m.OnlinePledge())
            {
                TempData["mp"] = Util.UserPeopleId;
                return Content("/OnlineReg/ManagePledge/{0}".Fmt(m.Orgid));
            }
            if (m.ManageGiving())
            {
                TempData["mg"] = Util.UserPeopleId;
                return Content("/OnlineReg/ManageGiving/{0}".Fmt(m.Orgid));
            }
            if (m.OnlineGiving())
                return Register(Util.UserPeopleId.Value, m);

            if (m.UserSelectsOrganization())
                m.List[0].ValidateModelForFind(ModelState, m, 0);
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
        [HttpPost]
        public ActionResult Register(int id, OnlineRegModel m)
        {
            ModelState.Clear();
            m.HistoryAdd("Register");
            int index = m.List.Count - 1;
            if (m.List[index].classid.HasValue)
                m.classid = m.List[index].classid;
            var p = m.LoadExistingPerson(id, index);
            p.ValidateModelForFind(ModelState, m, id, selectfromfamily: true);
            if (!ModelState.IsValid)
                return FlowList(m, "Register");
            m.List[index] = p;
            if (p.ManageSubscriptions() && p.Found == true)
                //p.OtherOK = true;
                return FlowList(m, "Register");

            if (p.org != null && p.Found == true)
            {
                p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                if (p.IsFilled)
                    ModelState.AddModelError(m.GetNameFor(mm => mm.List[m.List.IndexOf(p)].Found), "Sorry, but registration is filled.");
                if (p.Found == true)
                    p.FillPriorInfo();
                //if (!p.AnyOtherInfo())
                //p.OtherOK = true;
                return FlowList(m, "Register");
            }
            if (p.org == null && p.ComputesOrganizationByAge())
                ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].Found), p.NoAppropriateOrgError);
            if (p.ShowDisplay() && p.org != null && p.ComputesOrganizationByAge())
                p.classid = p.org.OrganizationId;
            return FlowList(m, "Register");
        }
        [HttpPost]
        public ActionResult Cancel(int id, OnlineRegModel m)
        {
            m.HistoryAdd("Cancel id=" + id);
            m.List.RemoveAt(id);
            if (m.List.Count == 0)
                m.List.Add(new OnlineRegPersonModel
                {
                    orgid = m.Orgid,
                    masterorgid = m.masterorgid,
                    LoggedIn = m.UserPeopleId.HasValue,
#if DEBUG
                    FirstName = "Another",
                    LastName = "Child",
                    DateOfBirth = "12/1/02",
                    EmailAddress = "karen@bvcms.com",
#endif
                });
            return FlowList(m, "Cancel");
        }
        [HttpPost]
        public ActionResult ShowMoreInfo(int id, OnlineRegModel m)
        {
            m.HistoryAdd("ShowMoreInfo id=" + id);
            DbUtil.Db.SetNoLock();
            var p = m.List[id];
            p.ValidateModelForFind(ModelState, m, id);
            if (p.org != null && p.Found == true)
            {
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
            p.ValidateModelForFind(ModelState, m, id);
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
            m.UpdateDatum();
            var ex2 = new Exception("{0}, {2}".Fmt(errorDisplay, m.DatumId, DbUtil.Db.ServerLink("/OnlineReg/RegPeople/") + m.DatumId), ex);
            ErrorSignal.FromCurrentContext().Raise(ex2);
            TempData["error"] = errorDisplay;
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
                    p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                    if (p.IsFilled)
                        ModelState.AddModelError(m.GetNameFor(mm => mm.List[id].DateOfBirth), "Sorry, that age group is filled");
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
            m.List[id].ValidateModelForOther(ModelState, id);
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
            TempData["onlineregmodel"] = Util.Serialize<OnlineRegModel>(m);
            return Redirect("/OnlineReg/CompleteRegistration");
        }
        [HttpGet]
        public ActionResult CompleteRegistration()
        {
            var s = (string) TempData["onlineregmodel"];
            if (s == null)
                return Message("Registration cannot be completed after a page refresh.");
            var m = Util.DeSerialize<OnlineRegModel>(s);

            m.HistoryAdd("CompleteRegistration");

            if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript)
            {
                SpecialRegModel.ParseResults(m.Orgid ?? 0, m.List[0].PeopleId ?? 0, Request.Form);
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
        private Dictionary<int, Settings> _settings;
        public Dictionary<int, Settings> settings
        {
            get
            {
                if (_settings == null)
                    _settings = HttpContext.Items["RegSettings"] as Dictionary<int, Settings>;
                return _settings;
            }
        }
        public class CurrentRegistration
        {
            public string OrgName { get; set; }
            public int OrgId { get; set; }
            public string RegType { get; set; }
        }
        public ActionResult Current()
        {
            var q = from o in DbUtil.Db.Organizations
                    where o.LastMeetingDate == null || o.LastMeetingDate < DateTime.Today
                    where o.RegistrationTypeId > 0
                    where o.OrganizationStatusId == 30
                    where !(o.RegistrationClosed ?? false)
                    select new { o.FullName2, o.OrganizationId, o.RegistrationTypeId, o.LastMeetingDate, o.OrgPickList };
            var list = q.ToList();
            var q2 = from i in list
                     where !list.Any(ii => (ii.OrgPickList ?? "0").Split(',').Contains(i.OrganizationId.ToString()))
                     orderby i.OrganizationId
                     select new CurrentRegistration
                              {
                                  OrgName = i.FullName2,
                                  OrgId = i.OrganizationId,
                                  RegType = RegistrationTypeCode.Lookup(i.RegistrationTypeId)
                              };
            return View(q2);
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
                var view = m.UseBootstrap ? "Flow2/List" : "Flow/List";
                var content = ViewExtensions2.RenderPartialViewToString2(this, view, m);
                return Content(content);
            }
            catch (Exception ex)
            {
                return ErrorResult(m, ex, "In " + function + ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Continue(int id)
        {
            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            if (m == null)
                return Message("no existing registration available");
            var n = m.List.Count - 1;
            m.List[n].ValidateModelForOther(ModelState, n);
            m.HistoryAdd("continue");
            m.UpdateDatum();
            return View("Index", m);
        }
        [HttpGet]
        public ActionResult StartOver(int id)
        {
            var pid = (int)TempData["er"];
            if (pid == 0)
                return Message("not logged in");
            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            if (m == null)
                return Message("no existing registration available");
            m.HistoryAdd("startover");
            m.UpdateDatum(abandoned: true);
            return Redirect(m.URL);
        }
        [HttpPost]
        public ActionResult SaveProgress(OnlineRegModel m)
        {
            m.HistoryAdd("saveprogress");
            if(m.UserPeopleId == null)
                m.UserPeopleId = Util.UserPeopleId;
            m.UpdateDatum();
            return Message("We have saved your progress, an email with a link to finish this registration will come to you shortly.");
        }
        [HttpGet]
        public ActionResult Existing(int id)
        {
            var pid = (int)TempData["er"];
            if (pid == 0)
                return Message("not logged in");
            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            if (m == null)
                return Message("no existing registration available");
            if (m.UserPeopleId != m.Datum.UserPeopleId)
                return Message("incorrect user");
            TempData["er"] = pid;
            return View(m);
        }
    }
}
