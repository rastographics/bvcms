/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Volunteers"), Route("{action}/{id?}")]
    public class VolunteersController : CmsStaffController
    {
        public VolunteersController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Codes(string id)
        {
            var q = from p in CurrentDatabase.VolInterestInterestCodes
                    where p.VolInterestCode.Org == id
                    select new
                    {
                        Key = p.VolInterestCode.Org + p.VolInterestCode.Code,
                        PeopleId = "p" + p.PeopleId,
                        p.Person.Name
                    };
            return Json(q);
        }

        public ActionResult Calendar(int id, string sg1, string sg2, bool? SortByWeek)
        {
            var m = new VolunteerCommitmentsModel(id);
            m.SmallGroup1 = sg1;
            m.SmallGroup2 = sg2;
            m.SortByWeek = SortByWeek ?? false;
            return View(m);
        }

        [Route("AltCalendar/{id:int}/{curMonth:int?}/{curYear:int?}")]
        [Route("AltCalendar/{id:int}/{SortByWeek:bool?}/{weekNum:int?}/{pageNumber:int?}")]
        public ActionResult AltCalendar(int id, bool? sortByWeek, int? curMonth, int? curYear, int? weekNum, int? pageNumber)
        {
            var m = new VolunteerCommitmentsModel(id);
            m.SortByWeek = sortByWeek ?? false;
            m.WeekNumber = weekNum ?? 0;
            m.PageNumber = pageNumber ?? 1;

            if (curMonth.IsNotNull())
            {
                if (curYear.IsNotNull())
                {
                    m.CurYear = curYear;
                }

                if (curMonth == 13)
                {
                    m.CurYear = m.CurYear + 1;
                    m.CurMonth = 1;
                }
                else if (curMonth == 0)
                {
                    m.CurYear = m.CurYear - 1;
                    m.CurMonth = 12;
                }
                else
                {
                    m.CurMonth = curMonth.ToInt();
                }
            }
            if (m.CurMonth.IsNull() && curMonth.IsNull())
            {
                m.CurMonth = m.Sunday.Month;
                m.CurYear = m.Sunday.Year;
            }

            if (m.CurYear.IsNull() && curYear.IsNull())
            {
                m.CurYear = m.Sunday.Year;
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult ManageArea(PostTargetInfo i)
        {
            var m = new VolunteerCommitmentsModel(i.id);
            m.SmallGroup1 = i.sg1;
            m.SmallGroup2 = i.sg2;
            m.SortByWeek = i.SortByWeek;
            foreach (var s in i.list)
            {
                m.ApplyDragDrop(i.target, i.week, i.time, s);
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult AltManageArea(PostTargetInfo i)
        {
            var m = new VolunteerCommitmentsModel(i.id);
            m.SmallGroup1 = i.sg1;
            m.SmallGroup2 = i.sg2;
            m.SortByWeek = i.SortByWeek;
            foreach (var s in i.list)
            {
                m.ApplyDragDrop(i.target, i.week, i.time, s);
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult ManageArea2(int id, string sg1, string sg2, bool? SortByWeek)
        {
            var m = new VolunteerCommitmentsModel(id);
            m.SmallGroup1 = sg1;
            m.SmallGroup2 = sg2;
            m.SortByWeek = SortByWeek ?? false;
            return View("ManageArea", m);
        }

        public ActionResult AddVolunteers(int id, int? week, DateTime? time, string sg1, string sg2, bool? isWeek = false)
        {
            TempData["selectedWeek"] = week?.ToInt();
            TempData["selectedTime"] = time?.ToString("g");
            TempData["isWeek"] = isWeek;
            var m = new VolunteerCommitmentsModel(id);
            m.SmallGroup1 = sg1;
            m.SmallGroup2 = sg2;
            return View("AddVolunteers", m);
        }

        public ActionResult EmailReminders(int id)
        {
            var qb = CurrentDatabase.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause(QueryType.RegisteredForMeetingId, CompareType.Equal, id.ToString());
            qb.Save(CurrentDatabase);

            var meeting = CurrentDatabase.Meetings.Single(m => m.MeetingId == id);

            CurrentDatabase.SetCurrentOrgId(meeting.OrganizationId);
            var subject = $"{meeting.Organization.OrganizationName} Reminder";
            var body =
                $@"<blockquote><table>
<tr><td>Time:</td><td>{meeting.MeetingDate:f}</td></tr>
<tr><td>Location:</td><td>{meeting.Organization.Location}</td></tr>
</table></blockquote><p>{meeting.Organization.LeaderName}</p>";

            TempData["body"] = body;
            return Redirect($"/Email/{qb.Id}?subj={Server.UrlEncode(subject)}&ishtml=true");
        }

        [HttpGet, Route("Request/{mid:int}/{limit:int}")]
        public new ActionResult Request(int mid, int limit)
        {
            var vs = new VolunteerRequestModel(mid, Util.UserPeopleId.Value) { limit = limit };
            vs.ComposeMessage();
            return View(vs);
        }

        [HttpGet, Route("Request0/{ticks:long}/{oid:int}/{limit:int}")]
        public ActionResult Request0(long ticks, int oid, int limit)
        {
            var time = new DateTime(ticks); // ticks here is meeting time
            var mid = CurrentDatabase.CreateMeeting(oid, time);
            var vs = new VolunteerRequestModel(mid, Util.UserPeopleId.Value) { limit = limit };
            vs.ComposeMessage();
            return View("Request", vs);
        }

        [HttpPost]
        [ValidateInput(false)]
        public new ActionResult Request(long ticks, int mid, int limit, int[] pids, string subject, string message, int? additional)
        {
            var m = new VolunteerRequestModel(mid, Util.UserPeopleId.Value, ticks)
            { subject = subject, message = message, pids = pids, limit = limit };

            if (pids == null || pids.Length == 0)
            {
                ViewBag.Error = "Please select some recipients";
                return View("Request", m);
            }

            m.SendEmails(additional ?? 0);
            return Content("Emails are being sent, thank you.");
        }

        public ActionResult EmailSlot(int id)
        {
            var m = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == id);
            var qb = CurrentDatabase.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause(QueryType.RegisteredForMeetingId, CompareType.Equal, m.MeetingId);
            qb.Save(CurrentDatabase);
            return Redirect($"/Email/{qb.Id}?TemplateId=0&body={m.Organization.OrganizationName} {m.MeetingDate.FormatDateTm()}&subj={m.Organization.OrganizationName} {m.MeetingDate.FormatDateTm()}");
        }

        [Route("EmailPersonInSlot/{meetingId:int}/{pId:int}")]
        public ActionResult EmailPersonInSlot(int meetingId, int pId)
        {
            var m = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == meetingId);
            var qb = CurrentDatabase.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause(QueryType.PeopleId, CompareType.Equal, pId);
            qb.Save(CurrentDatabase);
            return Redirect($"/Email/{qb.Id}?TemplateId=0&body={m.Organization.OrganizationName} {m.MeetingDate.FormatDateTm()}&subj={m.Organization.OrganizationName} {m.MeetingDate.FormatDateTm()}");
        }

        public class PostTargetInfo
        {
            public int id { get; set; }
            public DragDropInfo[] list { get; set; }
            public string target { get; set; }
            public int? week { get; set; }
            public DateTime? time { get; set; }
            public bool SortByWeek { get; set; }
            public string sg1 { get; set; }
            public string sg2 { get; set; }
        }
    }
}
