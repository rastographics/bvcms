using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        private ActionResult RouteRegistration(OnlineRegModel m, int pid, bool? showfamily)
        {
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
                p.ValidateModelForFind(ModelState, 0);
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
                if (!m.SupportMissionTrip)
                    p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                if (p.IsFilled)
                    ModelState.AddModelError(m.GetNameFor(mm => mm.List[0].Found), "Sorry, but registration is closed.");
                if (p.Found == true)
                    p.FillPriorInfo();
                p.CheckSetFee();
                m.HistoryAdd("index, pid={0}, !showfamily, p.org, found=true".Fmt(pid));
                return View(m);
            }
            return View(m);
        }

        private ActionResult RouteSpecialLogin(OnlineRegModel m)
        {
            if (Util.UserPeopleId == null)
                throw new Exception("Util.UserPeopleId is null on login");

            var existingRegistration = m.GetExistingRegistration(Util.UserPeopleId.Value);
            if (existingRegistration != null)
            {
                TempData["er"] = m.UserPeopleId = Util.UserPeopleId;
                return Content("/OnlineReg/Existing/" + existingRegistration.DatumId);
            }

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

            return null;
        }
    }
}
