using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text.RegularExpressions;
using CmsData.Codes;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Models
{
    public class OrganizationModel : OrgPeopleModel
    {
        public List<ScheduleInfo> Schedules
        {
            get
            {
                if (schedules == null && Id != null)
                    populate(Id.Value);
                if (schedules == null)
                    throw new Exception("missing schedules");
                return schedules;
            }
            set { schedules = value; }
        }
        private List<ScheduleInfo> schedules;

        public string Schedule { get; set; }
        public bool IsVolunteerLeader { get; set; }

        public OrganizationModel()
        {
        }
        public OrganizationModel(int id)
        {
            populate(id);
        }

        private void populate(int id)
        {
            Id = id;
            DbUtil.Db.CurrentOrg.Id = id;
            GroupSelect = GroupSelectCode.Member;
            var q = from sc in DbUtil.Db.OrgSchedules
                    where sc.OrganizationId == Id
                    select sc;
            var u = from s in q
                    orderby s.Id
                    select new ScheduleInfo(s);
            schedules = u.ToList();
            Schedule = schedules.Count > 0 ? schedules[0].Display : "None";

            IsVolunteerLeader = VolunteerLeaderInOrg(Id);

        }
        public static bool VolunteerLeaderInOrg(int? orgid)
        {
            if (orgid == null)
                return false;
            var o = DbUtil.Db.LoadOrganizationById(orgid);
            if (o == null || o.RegistrationTypeId != RegistrationTypeCode.ChooseVolunteerTimes)
                return false;
            if (HttpContext.Current.User.IsInRole("Admin") ||
                HttpContext.Current.User.IsInRole("ManageVolunteers"))
                return true;
            var leaderorgs = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            if (leaderorgs == null)
                return false;
            return leaderorgs.Contains(orgid.Value);
        }

        private CodeValueModel cv = new CodeValueModel();

        public IEnumerable<SelectListItem> Groups()
        {
            var q = from g in DbUtil.Db.MemberTags
                    where g.OrgId == Id
                    orderby g.Name
                    select new SelectListItem
                    {
                        Text = g.Name,
                        Value = g.Id.ToString()
                    };
            return q;
        }
        public static IEnumerable<SelectListItem> Tags()
        {
            var cv = new CodeValueModel();
            var tg = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Id").ToList();
            if (HttpContext.Current.User.IsInRole("Edit"))
                tg.Insert(0, new SelectListItem { Value = "-1", Text = "(last query)" });
            tg.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return tg;
        }
        public void UpdateSchedules()
        {
            DbUtil.Db.OrgSchedules.DeleteAllOnSubmit(Org.OrgSchedules);
            Org.OrgSchedules.Clear();
            DbUtil.Db.SubmitChanges();
            foreach (var s in Schedules.OrderBy(ss => ss.Id))
                Org.OrgSchedules.Add(new OrgSchedule
                {
                    OrganizationId = Id ?? 0,
                    Id = s.Id,
                    SchedDay = s.SchedDay,
                    SchedTime = s.Time.ToDate(),
                    AttendCreditId = s.AttendCreditId
                });
            DbUtil.Db.SubmitChanges();
        }
        public SelectList SchedulesPrev()
        {
            var q = new SelectList(Schedules.OrderBy(cc => cc.Id), "ValuePrev", "Display");
            return q;
        }
        public SelectList SchedulesNext()
        {
            var q = new SelectList(Schedules.OrderBy(cc => cc.Id), "ValueNext", "Display");
            return q;
        }
        public static IEnumerable<SearchDivision> Divisions(int? id)
        {
            var q = from d in DbUtil.Db.SearchDivisions(id, null)
                where d.IsChecked == true                
                orderby d.IsMain descending, d.IsChecked descending, d.Program, d.Division
                select d;
            return q;
        }

        public IEnumerable<SelectListItem> CampusList()
        {
            return CodeValueModel.ConvertToSelect(cv.AllCampuses0(), "Id");
        }
        public IEnumerable<SelectListItem> OrgStatusList()
        {
            return CodeValueModel.ConvertToSelect(cv.OrganizationStatusCodes(), "Id");
        }
        public IEnumerable<SelectListItem> LeaderTypeList()
        {
            var items = CodeValueModel.MemberTypeCodes0().Select(c => new CodeValueItem { Code = c.Code, Id = c.Id, Value = c.Value });
            return CodeValueModel.ConvertToSelect(items, "Id");
        }
        public IEnumerable<SelectListItem> EntryPointList()
        {
            return CodeValueModel.ConvertToSelect(cv.EntryPoints(), "Id");
        }
        public IEnumerable<SelectListItem> OrganizationTypes()
        {
            return CodeValueModel.ConvertToSelect(cv.OrganizationTypes0(), "Id");
        }
        public IEnumerable<SelectListItem> GenderList()
        {
            return CodeValueModel.ConvertToSelect(cv.GenderCodes(), "Id");
        }
        public IEnumerable<SelectListItem> AttendCreditList()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.AttendCredits(), "Id");
        }
        public IEnumerable<SelectListItem> SecurityTypeList()
        {
            return CodeValueModel.ConvertToSelect(cv.SecurityTypeCodes(), "Id");
        }
        public static string SpaceCamelCase(string s)
        {
            return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
        public static IEnumerable<SelectListItem> RegistrationTypes()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.RegistrationTypes(), "Id");
        }
        public string NewMeetingTime
        {
            get
            {
                var sc = Org.OrgSchedules.FirstOrDefault(); // SCHED
                if (sc != null && sc.SchedTime != null)
                    return sc.SchedTime.ToString2("t");
                return "08:00 AM";
            }
        }
        public DateTime PrevMeetingDate
        {
            get
            {
                var sc = Org.OrgSchedules.FirstOrDefault(); // SCHED
                if (sc != null && sc.SchedTime != null && sc.SchedDay < 9)
                    return Util.Now.Date.Sunday().AddDays(sc.SchedDay ?? 0)
                        .Add(sc.SchedTime.Value.TimeOfDay);
                return Util.Now.Date;
            }
        }
        public DateTime NextMeetingDate
        {
            get { return PrevMeetingDate.AddDays(7); }
        }
        private Settings _RegSettings;

        public Settings RegSettings
        {
            get
            {
                if (_RegSettings == null)
                {
                    _RegSettings = new Settings(Org.RegSetting, DbUtil.Db, Org.OrganizationId);
                    _RegSettings.org = Org;
                }
                return _RegSettings;
            }
        }
    }
}
