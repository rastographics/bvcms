using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Org", AreaPrefix = "RegSettings"), Route("{action=index}/{id?}")]
    public class RegSettingController : CmsStaffController
    {
        [HttpGet, Route("~/RegSettings/{id:int}")]
        public ActionResult Index(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            ViewData["OrganizationId"] = id;
            ViewData["orgname"] = org.OrganizationName;
            var regsetting = org.RegSettingXml;
            var os = DbUtil.Db.CreateRegistrationSettings(regsetting, id);
            regsetting = os.ToString();
                ViewData["text"] = regsetting;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult Update(int id, string text)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            try
            {
                var os = DbUtil.Db.CreateRegistrationSettings(text, id);
                org.UpdateRegSetting(os);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                TempData["regsetting"] = text;
                return Redirect("/RegSettings/" + id);
            }
            DbUtil.Db.SubmitChanges();
            return Redirect("/RegSettings/" + id);
        }
    }
}