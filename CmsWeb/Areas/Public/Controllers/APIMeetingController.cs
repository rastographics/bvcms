using CmsData;
using CmsData.API;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIMeetingController : CmsController
    {
        public APIMeetingController(RequestManager requestManager) : base(requestManager)
        {
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
            return Content(new APIMeeting(DbUtil.Db)
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
            return Content(new APIMeeting(DbUtil.Db)
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
            return Content(new APIMeeting(DbUtil.Db)
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
            Attend.MarkRegistered(DbUtil.Db, peopleid, meetingid, CommitId);
            return Content("ok");
        }
    }
}
