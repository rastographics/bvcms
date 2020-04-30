using CmsWeb.Areas.Setup.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "SmsReplyWords")]
    public class SmsReplyWordsController : CmsStaffController
    {
        [Route("~/SmsReplyWords")]
        public ActionResult Index()
        {
            var m = new SmsReplyWordsModel(CurrentDatabase);
            var number = Util.GetFromSessionTemp("number");
            if (number != null)
            {
                m.Number = number.ToString();
                m.PopulateNumber();
                ViewBag.Message = "Saved";
            }
            return View(m);
        }

        [HttpPost]
        [Route("~/SmsReplyWords/NumberChanged")]
        public PartialViewResult NumberChanged(SmsReplyWordsModel model)
        {
            model.PopulateNumber();
            return PartialView("ReplyWords", model);
        }
        [HttpPost]
        [Route("~/SmsReplyWords/ActionChanged")]
        public PartialViewResult ActionChanged(SmsReplyWordsModel model)
        {
            model.PopulateMetaData();
            return PartialView("ReplyWords", model);
        }
        [HttpPost]
        [Route("~/SmsReplyWords/AddAction")]
        public PartialViewResult AddAction(SmsReplyWordsModel model)
        {
            model.PopulateMetaData();
            model.Actions.Add(new SmsActionModel());
            return PartialView("ReplyWords", model);
        }

        [HttpPost, Route("~/SmsReplyWords/Save")]
        public ActionResult Save(SmsReplyWordsModel model)
        {
            model.Save();
            model.PopulateMetaData();
            Util.SetValueInSession("number", model.Number);
            return RedirectToAction("Index");
        }

        public SmsReplyWordsController(IRequestManager requestManager) : base(requestManager)
        {
        }
    }
}
