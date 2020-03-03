using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Reports.Models
{
    public class CheckinControlModel : OrgSearchModel
    {
        public DateTime CheckinDate { get; set; }
        public bool CheckinExport { get; set; }

        public class AttendInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Organization { get; set; }
            public string Location { get; set; }
            public DateTime MeetingTime { get; set; }
            public string AttendType { get; set; }
        }

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public CheckinControlModel() { }

        public CheckinControlModel(CMSDataContext db) : base(db) { }

        public IEnumerable<AttendInfo> list()
        {
            var orgs = FetchOrgs();
            var q = from a in CurrentDatabase.Attends
                    join o in orgs on a.Meeting.OrganizationId equals o.OrganizationId
                    where a.MeetingDate.Date == CheckinDate
                    where a.AttendanceFlag
                    where ScheduleId == null || a.MeetingDate.TimeOfDay == o.SchedTime.Value.TimeOfDay
                    select a;
            var q2 = from a in q
                     orderby a.Person.Name2
                     select new AttendInfo
                     {
                         Name = a.Person.Name2,
                         Id = a.PeopleId,
                         Organization = a.Organization.OrganizationName,
                         MeetingTime = a.MeetingDate,
                         Location = a.Organization.Location,
                         AttendType = a.AttendType.Description,
                     };
            return q2;
        }
    }
}

