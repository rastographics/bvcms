using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models.iPhone
{
    public class OrgResult : ActionResult
    {
        private int? pid;
        public OrgResult(int? pid)
        {
            this.pid = pid;
        }
        private IEnumerable<OrgInfo> OrgList()
        {
            var oids = DbUtil.Db.GetLeaderOrgIds(pid);
            var dt = DateTime.Parse("8:00 AM");

            var roles = DbUtil.Db.CurrentRoles();
            IQueryable<Organization> q = null;
            if (Util2.OrgLeadersOnly)
            {
                q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where oids.Contains(o.OrganizationId)
                    where o.SecurityTypeId != 3
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    select o;
            }
            else
            {
                q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where (o.OrganizationMembers.Any(om => om.PeopleId == pid // either a leader, who is not pending / inactive
                              && (om.Pending ?? false) == false
                              && (om.MemberTypeId != MemberTypeCode.InActive)
                              && (om.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                              )
                          || oids.Contains(o.OrganizationId)) // or a leader of a parent org
                    where o.SecurityTypeId != 3
                    select o;
            }

            return from o in q
                   let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                   select new OrgInfo
                   {
                       OrgId = o.OrganizationId,
                       OrgName = o.OrganizationName,
                       MeetingTime = sc.SchedTime ?? dt,
                       MeetingDay = sc.SchedDay ?? 0
                   };
        }
        public class OrgInfo
        {
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTime? MeetingTime { get; set; }
            public int? MeetingDay { get; set; }
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            settings.Indent = true;

            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                w.WriteStartElement("Organizations");

                foreach (var o in OrgList())
                {
                    w.WriteStartElement("Organization");
                    w.WriteAttributeString("Id", o.OrgId.ToString());
                    w.WriteAttributeString("Name", o.OrgName);
                    w.WriteAttributeString("MeetingDate", NewMeetingDate(o.MeetingDay).ToShortDateString());
                    w.WriteAttributeString("MeetingTime", o.MeetingTime.Value.ToShortTimeString());
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
        public DateTime NewMeetingDate(int? day)
        {
            var d = Util.Now.Date;
            d = d.AddDays(-(int)d.DayOfWeek); // prev sunday
            d = d.AddDays(day ?? 0);
            if (d > Util.Now.Date)
            {
                d = d.AddDays(-7);
            }

            return d;
        }
    }
}
