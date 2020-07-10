using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Manage.Models.SmsMessages;
using CmsWeb.Lifecycle;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "SmsMessages"), Route("{action=index}/{id?}")]
    public class SmsController : CmsStaffController
    {
        public SmsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var m = new SmsMessagesModel(CurrentDatabase);
            return View(m);
        }

        #region SentMessages

        [HttpPost]
        public ActionResult Sent(SmsSentMessagesModel m)
        {
            return View(m);
        }

        public ActionResult SentDetails(int id)
        {
            var m = new SmsSentDetailsModel(CurrentDatabase, id);
            return View(m);
        }
        [HttpPost]
        public ActionResult SentResults(SmsSentMessagesModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ContentResult EmailSent(SmsSentMessagesModel m)
        {
            var guid = m.ToolBarSend();
            return Content($"/Email/{guid}");
        }
        [HttpPost]
        public ContentResult TextSent(SmsSentMessagesModel m)
        {
            var guid = m.ToolBarSend();
            return Content($"/SMS/Options/{guid}");
        }
        [HttpPost]
        public ContentResult TagSentDialog(SmsSentMessagesModel m)
        {
            var q = m.Recipients();
            var i = q.Count();
            var people = i == 1 ? "person" : "people";
            return Content($"{i} {people}");
        }
        [HttpPost]
        public ContentResult TagSent(string tagname, bool? cleartagfirst, SmsSentMessagesModel m)
        {
            m.TagAll(tagname, cleartagfirst);
            return Content("Added to Tag");
        }
        [HttpPost]
        public EpplusResult ExportSent(SmsSentMessagesModel m)
        {
            return m.ExportSent();
        }

        [HttpPost]
        public ActionResult SentFilterGroupIdChanged(SmsSentMessagesModel m)
        {
            return View("SentFilterMembersDropdown", m);
        }

        #endregion

        #region ReceivedMessages

        [HttpPost]
        public ActionResult Received(SmsReceivedMessagesModel m)
        {
            return View(m);
        }

        public ActionResult ReceivedDetails(int id)
        {
            var detail = SmsReceivedMessagesModel.Detail(CurrentDatabase, id);
            return View(detail);
        }
        [HttpPost]
        public ActionResult ReceivedResults(SmsReceivedMessagesModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ContentResult TagReceivedDialog(SmsReceivedMessagesModel m)
        {
            var q = m.DefineModelList().Select(p => p.Person.PeopleId);
            var i = q.Distinct().Count();
            var people = i == 1 ? "person" : "people";
            return Content($"{i} {people}");
        }
        [HttpPost]
        public ContentResult TagReceived(string tagname, bool? cleartagfirst, SmsReceivedMessagesModel m)
        {
            m.TagAll(tagname, cleartagfirst);
            return Content("Added to Tag");
        }
        [HttpPost]
        public EpplusResult ExportReceived(SmsReceivedMessagesModel m)
        {
            return m.ExportReceived();
        }
        [HttpPost]
        public ContentResult EmailReceived(SmsReceivedMessagesModel m)
        {
            var guid = m.ToolBarSend();
            return Content($"/Email/{guid}");
        }
        [HttpPost]
        public ContentResult TextReceived(SmsReceivedMessagesModel m)
        {
            var guid = m.ToolBarSend();
            return Content($"/SMS/Options/{guid}");
        }

        [HttpPost]
        public ContentResult ReplyingTo(int id)
        {
            var d = SmsReceivedMessagesModel.FetchReplyToData(CurrentDatabase, id);
            return Content(JsonConvert.SerializeObject(d));
        }
        [HttpPost]
        public ContentResult SendReply(ReplyModel m)
        {
            var ret = m.Send(CurrentDatabase);
            return Content(ret);
        }

        #endregion

        #region ReplyWords
        [HttpPost]
        public PartialViewResult ReplyWordsGroupChanged(SmsReplyWordsModel model)
        {
            model.PopulateActions();
            return PartialView("ReplyWords", model);
        }
        [HttpPost]
        public PartialViewResult ReplyWordActionChanged(SmsReplyWordsModel model)
        {
            model.PopulateMetaData();
            return PartialView("ReplyWords", model);
        }
        [HttpPost]
        public PartialViewResult AddReplyWord(SmsReplyWordsModel model)
        {
            model.PopulateMetaData();
            model.Actions.Add(new SmsReplyWordsActionModel());
            return PartialView("ReplyWords", model);
        }

        [HttpPost]
        public PartialViewResult SaveReplyWords(SmsReplyWordsModel model)
        {
            model.Save();
            model.PopulateMetaData();
            return PartialView("ReplyWordsTab", model);
        }
        #endregion
    }
}
