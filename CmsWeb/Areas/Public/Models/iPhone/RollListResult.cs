using CmsData;
using CmsWeb.Areas.Reports.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;

namespace CmsWeb.Models.iPhone
{
    public class RollListResult : ActionResult
    {
        private int? NewPeopleId;
        private int MeetingId;
        private readonly IEnumerable<RollsheetModel.AttendInfo> people;

        public RollListResult(Meeting meeting, int? PeopleId = null)
        {
            MeetingId = meeting.MeetingId;
            NewPeopleId = PeopleId;
            if (meeting.MeetingDate != null)
            {
                people = RollsheetModel.RollList(MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value);
            }
        }
        public RollListResult(int orgid, DateTime dt)
        {
            MeetingId = DbUtil.Db.CreateMeeting(orgid, dt);
            people = RollsheetModel.RollList(MeetingId, orgid, dt);
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            settings.Indent = true;

            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                w.WriteStartElement("RollList");
                w.WriteAttributeString("MeetingId", MeetingId.ToString());
                if (NewPeopleId.HasValue)
                {
                    w.WriteAttributeString("NewPeopleId", NewPeopleId.ToString());
                }

                foreach (var p in people)
                {
                    w.WriteStartElement("Person");
                    w.WriteAttributeString("Id", p.PeopleId.ToString());
                    w.WriteAttributeString("Name", p.Name);
                    w.WriteAttributeString("Attended", p.Attended.ToString());
                    w.WriteAttributeString("Member", p.Member.ToString());
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
    }
}
