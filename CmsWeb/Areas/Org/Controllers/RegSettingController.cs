using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Web.Mvc;

namespace CmsWeb.Areas.Org.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Org", AreaPrefix = "RegSettings"), Route("{action=index}/{id?}")]
    public class RegSettingController : CmsStaffController
    {
        public RegSettingController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/RegSettings/{id:int}")]
        public ActionResult Index(int id)
        {
            var org = CurrentDatabase.LoadOrganizationById(id);
            ViewData["OrganizationId"] = id;
            ViewData["orgname"] = org.OrganizationName;
            var regsetting = org.RegSettingXml;
            var os = CurrentDatabase.CreateRegistrationSettings(regsetting, id);
            regsetting = os.ToString();
            ViewData["text"] = regsetting;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult Update(int id, string text)
        {
            var org = CurrentDatabase.LoadOrganizationById(id);
            try
            {
                var os = CurrentDatabase.CreateRegistrationSettings(text, id);
                org.UpdateRegSetting(os);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                TempData["regsetting"] = text;
                return Redirect("/RegSettings/" + id);
            }
            CurrentDatabase.SubmitChanges();
            return Redirect("/RegSettings/" + id);
        }
    }
}
