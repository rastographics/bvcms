using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        [HttpGet]
        public ActionResult Continue(int id)
        {
            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            if (m == null)
                return Message("no Existing registration available");
            var n = m.List.Count - 1;
            m.HistoryAdd("continue");
            m.UpdateDatum();
            SetHeaders(m);
            if (m.RegistrantComplete)
            {
                return Redirect("/OnlineReg/CompleteRegistration/"+id);                
            }
            return View("Index", m);
        }

        [HttpGet]
        public ActionResult StartOver(int id)
        {
            var pid = (int?)TempData["PeopleId"];
            if (!pid.HasValue || pid == 0)
                return Message("not logged in");
            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            if (m == null)
                return Message("no Existing registration available");
            m.StartOver();
            return Redirect(m.URL);
        }


        [HttpPost]
        public ActionResult AutoSaveProgress(OnlineRegModel m)
        {
            try { m.UpdateDatum(); }
            catch { }
            return Content(m.DatumId.ToString());
        }

        [HttpPost]
        public ActionResult SaveProgress(OnlineRegModel m)
        {
            m.HistoryAdd("saveprogress");
            if (m.UserPeopleId == null)
                m.UserPeopleId = Util.UserPeopleId;
            m.UpdateDatum();
            var p = m.UserPeopleId.HasValue ? CurrentDatabase.LoadPersonById(m.UserPeopleId.Value) : m.List[0].person;

            if (p == null)
                return Content("We have not found your record yet, cannot save progress, sorry");
            if (m.masterorgid == null && m.Orgid == null)
                return Content("Registration is not far enough along to save, sorry.");

            var msg = CurrentDatabase.ContentHtml("ContinueRegistrationLink", @"
<p>Hi {first},</p>
<p>Here is the link to continue your registration:</p>
Resume [registration for {orgname}]
").Replace("{orgname}", m.Header);
            var linktext = Regex.Match(msg, @"(\[(.*)\])", RegexOptions.Singleline).Groups[2].Value;
            var registerlink = EmailReplacements.CreateRegisterLink(m.masterorgid ?? m.Orgid, linktext);
            msg = Regex.Replace(msg, @"(\[.*\])", registerlink, RegexOptions.Singleline);

            var notifyids = CurrentDatabase.NotifyIds((m.masterorg ?? m.org).NotifyIds);
            CurrentDatabase.Email(notifyids[0].FromEmail, p, $"Continue your registration for {m.Header}", msg);

            /* We use Content as an ActionResult instead of Message because we want plain text sent back
             * This is an HttpPost ajax call and will have a SiteLayout wrapping this.
             */
            return Content(@"
We have saved your progress. An email with a link to finish this registration will come to you shortly.
<input type='hidden' id='SavedProgress' value='true'/>
");
        }

        [HttpGet]
        public ActionResult Existing(int id)
        {
            if(!TempData.ContainsKey("PeopleId"))
                return Message("not logged in");
            var pid = (int?)TempData["PeopleId"];
            if (!pid.HasValue || pid == 0)
                return Message("not logged in");
            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            if (m == null)
                return Message("no Existing registration available");
            if (m.UserPeopleId != m.Datum.UserPeopleId)
                return Message("incorrect user");
            TempData["PeopleId"] = pid;
            return View("Continue/Existing", m);
        }

        [HttpPost]
        public ActionResult SaveProgressPayment(int id)
        {
            var ed = CurrentDatabase.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed != null)
            {
                var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                m.HistoryAdd("saveprogress");
                if (m.UserPeopleId == null)
                    m.UserPeopleId = Util.UserPeopleId;
                m.UpdateDatum();
                return Json(new { confirm = "/OnlineReg/FinishLater/" + id,
                    formmethod = "GET"
                });
            }
            return Json(new { confirm = "/OnlineReg/Unknown" });
        }

        [HttpGet]
        public ActionResult FinishLater(int id)
        {
            var ed = CurrentDatabase.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed == null)
                return View("Other/Unknown");
            var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
            m.FinishLaterNotice();
            return View("Continue/FinishLater", m);
        }
    }
}
