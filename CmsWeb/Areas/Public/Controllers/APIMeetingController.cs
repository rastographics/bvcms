using CmsData;
using CmsData.API;
using CmsWeb.Lifecycle;
using CmsWeb.Services.MeetingCategory;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIMeetingController : CmsController
    {
        private readonly IMeetingCategoryService _meetingCategoryService;

        public APIMeetingController(IRequestManager requestManager, IMeetingCategoryService meetingCategoryService) : base(requestManager)
        {
            _meetingCategoryService = meetingCategoryService;
        }

        [HttpGet]
        public ActionResult ExtraValues(int id, string fields)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<ExtraValues error=\"{ret.Substring(1)}\" />");
            }

            DbUtil.LogActivity($"APIMeeting ExtraValues {id}, {fields}");
            return Content(new APIMeeting(CurrentDatabase)
                .ExtraValues(id, fields), "text/xml");
        }

        [HttpPost]
        public ActionResult AddEditExtraValue(int meetingid, string field, string value)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIMeeting AddEditExtraValue {meetingid}, {field}");
            return Content(new APIMeeting(CurrentDatabase)
                .AddEditExtraValue(meetingid, field, value));
        }

        [HttpPost]
        public ActionResult DeleteExtraValue(int meetingid, string field)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIMeeting DeleteExtraValue {meetingid}, {field}");
            return Content(new APIMeeting(CurrentDatabase)
                .DeleteExtraValue(meetingid, field));
        }

        [HttpPost]
        public ActionResult MarkRegistered(int meetingid, int peopleid, int? CommitId)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIMeeting MarkRegistered {meetingid}, {peopleid}");
            Attend.MarkRegistered(CurrentDatabase, peopleid, meetingid, CommitId);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult GetMeetingCategories()
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            var meetingCategories = _meetingCategoryService.GetMeetingCategories(false);
            return Json(meetingCategories.Select(x => new { x.Id, x.Description }));
        }

        [HttpPost]
        public ActionResult UpdateMeetingCategory(long meetingId, long meetingCategoryId)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIMeeting UpdateMeetingCategory FromId: {meetingId}");

            var meetingCatgory = _meetingCategoryService.GetById(meetingCategoryId);
            var updatedSuccessfully = _meetingCategoryService.TryUpdateMeetingCategory(meetingId, meetingCatgory.Description);

            return Content(updatedSuccessfully ? "ok" : "error");
        }

        [HttpPost]
        public ActionResult UpdateMeetingCategory(long meetingId, string meetingCategory)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIMeeting UpdateMeetingCategory FromString: {meetingId}");

            var updatedSuccessfully = _meetingCategoryService.TryUpdateMeetingCategory(meetingId, meetingCategory);

            return Content(updatedSuccessfully ? "ok" : "error");
        }
    }
}
