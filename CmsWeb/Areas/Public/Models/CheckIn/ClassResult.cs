using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ClassResult : ActionResult
    {
        private List<string> items;
        private Meeting meeting;
        public ClassResult(int OrgId, int thisday)
        {
            var mid = DbUtil.Db.GetTodaysMeetingId(OrgId, thisday);
            if (mid != null)
            {
                meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingId == mid);
                if (meeting != null)
                {
                    var q = from a in meeting.Attends
                            where a.AttendanceFlag == true
                            orderby a.Person.Name2
                            select a.Person.Name;
                    items = q.ToList();
                }
            }
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (meeting == null)
            {
                return;
            }

            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                w.WriteStartElement("Class");
                if (items != null && items.Count > 0)
                {
                    w.WriteAttributeString("Name", meeting.Organization.OrganizationName);
                    w.WriteAttributeString("Teacher", meeting.Organization.LeaderName);
                    w.WriteAttributeString("Date", meeting.MeetingDate.FormatDate());
                    w.WriteAttributeString("Time", meeting.MeetingDate.Value.ToString("t"));
                    w.WriteAttributeString("Count", meeting.MaxCount.ToString());
                    foreach (var f in items)
                    {
                        w.WriteElementString("Name", f);
                    }
                }
                w.WriteEndElement();
            }
        }
    }
}
