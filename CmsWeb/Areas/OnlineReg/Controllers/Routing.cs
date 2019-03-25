using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        private ActionResult RouteRegistration(OnlineRegModel m, int pid, bool? showfamily)
        {
            if(pid == 0)
                return View(m);
#if DEBUG
            m.DebugCleanUp();
#endif

            var link = RouteExistingRegistration(m, pid);
            if (link.HasValue())
                return Redirect(link);

            OnlineRegPersonModel p;
            PrepareFirstRegistrant(ref m, pid, showfamily, out p);        

            if (!ModelState.IsValid)
            {
                m.Log("CannotProceed");
                return View(m);
            }

            link = RouteManageGivingSubscriptionsPledgeVolunteer(m);
            if(link.HasValue())
                if (m.ManageGiving()) // use Direct ActionResult instead of redirect
                    return ManageGiving(m.Orgid.ToString(), m.testing);
                else if (m.RegisterLinkMaster())
                    return Redirect(link);
                else
                    return Redirect(link);

            // check for forcing show family, master org, or not found
            if (showfamily == true || p.org == null || p.Found != true)
                return View(m);

            // ready to answer questions, make sure registration is ok to go
            m.Log("Authorized");
            if (!m.SupportMissionTrip)
                p.IsFilled = p.org.RegLimitCount(CurrentDatabase) >= p.org.Limit;
            if (p.IsFilled)
            {
                m.Log("Closed");
                ModelState.AddModelError(m.GetNameFor(mm => mm.List[0].Found), "Sorry, but registration is closed.");
            }

            p.FillPriorInfo();
            p.SetSpecialFee();

            m.HistoryAdd($"index, pid={pid}, !showfamily, p.org, found=true");
            return View(m);
        }

        private string RouteManageGivingSubscriptionsPledgeVolunteer(OnlineRegModel m)
        {
            if (m.RegisterLinkMaster())
            {
                if(m.registerLinkType.HasValue())
                    if (m.registerLinkType.StartsWith("registerlink") || m.registerLinkType == "masterlink" || User.Identity.IsAuthenticated)
                    {
                        TempData["token"] = m.registertag;
                        TempData["PeopleId"] = m.UserPeopleId ?? Util.UserPeopleId;
                    }
                return $"/OnlineReg/RegisterLinkMaster/{m.Orgid}";
            }
            TempData["PeopleId"] = m.UserPeopleId ?? Util.UserPeopleId;
            if (m.ManagingSubscriptions())
                return $"/OnlineReg/ManageSubscriptions/{m.masterorgid}";
            if (m.ManageGiving())
                return $"/OnlineReg/ManageGiving/{m.Orgid}";
            if (m.OnlinePledge())
                return $"/OnlineReg/ManagePledge/{m.Orgid}";
            if (m.ChoosingSlots())
                return $"/OnlineReg/ManageVolunteer/{m.Orgid}";
            TempData.Remove("PeopleId");
            return null;
        }

        private string RouteExistingRegistration(OnlineRegModel m, int? pid = null)
        {
            if (m.SupportMissionTrip)
                return null;
            var existingRegistration = m.GetExistingRegistration(pid ?? Util.UserPeopleId ?? 0);
            if (existingRegistration == null)
                return null;
            m.Log("Existing");
            TempData["PeopleId"] = existingRegistration.UserPeopleId;
            return "/OnlineReg/Existing/" + existingRegistration.DatumId;
        }

        private ActionResult RouteSpecialLogin(OnlineRegModel m)
        {
            if (Util.UserPeopleId == null)
                throw new Exception("Util.UserPeopleId is null on login");

            var link = RouteExistingRegistration(m);
            if(link.HasValue())
                return Redirect(link);

            m.CreateAnonymousList();
            m.UserPeopleId = Util.UserPeopleId;

            if (m.OnlineGiving())
            {
                m.Log("Login OnlineGiving");
                return RegisterFamilyMember(Util.UserPeopleId.Value, m);
            }

            link = RouteManageGivingSubscriptionsPledgeVolunteer(m);
            if (link.HasValue())
                return Content(link); // this will be used for a redirect in javascript

            return null;
        }
    }
}
