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
            var groupid = Util.GetFromSessionTemp("groupid");
            if (groupid != null)
            {
                m.GroupId = groupid.ToInt2() ?? -1;
                m.PopulateActions();
                ViewBag.Message = "Saved";
            }
            return View(m);
        }

        [HttpPost]
        [Route("~/SmsReplyWords/GroupChanged")]
        public PartialViewResult GroupChanged(SmsReplyWordsModel model)
        {
            model.PopulateActions();
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
            Util.SetValueInSession("groupid", model.GroupId);
            return RedirectToAction("Index");
        }

        public SmsReplyWordsController(IRequestManager requestManager) : base(requestManager)
        {
        }
    }
}
