using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class Home2Controller : CmsController
    {
        public Home2Controller(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/Home/MyDataSupport")]
        public ActionResult MyDataSupport()
        {
            return View("../Home/MyDataSupport");
        }

        [HttpPost, Route("~/HideTip")]
        public ActionResult HideTip(string tip)
        {
            CurrentDatabase.SetUserPreference("hide-tip-" + tip, "true");
            return new EmptyResult();
        }

        [HttpGet, Route("~/ResetTips")]
        public ActionResult ResetTips()
        {
            CurrentDatabase.ExecuteCommand("DELETE dbo.Preferences WHERE Preference LIKE 'hide-tip-%' AND UserId = {0}",
                Util.UserId);
            var d = Session["preferences"] as Dictionary<string, string>;
            var keys = d.Keys.Where(kk => kk.StartsWith("hide-tip-")).ToList();
            foreach (var k in keys)
            {
                d.Remove(k);
            }

            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return Redirect("/");
        }

        [HttpGet]
        [Route("~/Person/TinyImage/{id}")]
        [Route("~/Person2/TinyImage/{id}")]
        [Route("~/TinyImage/{id}")]
        public ActionResult TinyImage(int id)
        {
            return new PictureResult(id, portrait: true, tiny: true);
        }

        [HttpGet]
        [Route("~/Person/Image/{id:int}/{w:int?}/{h:int?}")]
        [Route("~/Person2/Image/{id:int}/{w:int?}/{h:int?}")]
        [Route("~/Image/{id:int}/{w:int?}/{h:int?}")]
        public ActionResult Image(int id, int? w, int? h, string mode)
        {
            return new PictureResult(id);
        }

        [Route("~/BackgroundImage/{id:int}")]
        public ActionResult BackgroundImage(int id)
        {
            return new PictureResult(id, shouldBePublic: true);
        }

        [HttpGet, Route("~/ImageSized/{id:int}/{w:int}/{h:int}/{mode}")]
        public ActionResult ImageSized(int id, int w, int h, string mode)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            return new PictureResult(p.Picture.LargeId ?? 0, w, h, portrait: true, mode: mode);
        }

        [Authorize(Roles = "Finance")]
        public ActionResult TurnFinanceOn()
        {
            Session.Remove("testnofinance");
            return Redirect("/Person2/Current");
        }

        [Authorize(Roles = "Finance")]
        public ActionResult TurnFinanceOff()
        {
            Session["testnofinance"] = "true";
            return Redirect("/Person2/Current");
        }
    }
}
