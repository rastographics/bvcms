/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CmsData;
using UtilityExtensions;
using System.Web.Mvc;
using System.Collections.Generic;
using CmsWeb.Models;

namespace CmsWeb.Areas.Reports.Models
{
    public class AttendeeResult : ActionResult
    {
        public class AttendInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string Email { get; set; }
            public string Birthday { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string CSZ { get; set; }
            public string Status { get; set; }
            public DateTime? LastAttend { get; set; }
            public decimal? AttendPct { get; set; }
            public string AttendStr { get; set; }
        }
        private Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
        private Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));
        private Document doc;
        private DateTime dt;

        private int? mtgid;
        public AttendeeResult(int? meetingid)
        {
            mtgid = meetingid;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            dt = Util.Now;

            doc = new Document(PageSize.LETTER.Rotate(), 36, 36, 64, 64);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);

            var i = (from m in CurrentDatabase.Meetings
                     where m.MeetingId == mtgid
                     select new
                     {
                         m.Organization.OrganizationName,
                         m.Organization.LeaderName,
                         m.MeetingDate
                     }).SingleOrDefault();

            w.PageEvent = new HeadFoot
            {
                HeaderText = $"Attendee Report: {i.OrganizationName} - {i.LeaderName} {i.MeetingDate:g}",
                FooterText = "Attendee Report"
            };
            doc.Open();

            var q = Attendees(mtgid.Value);

            if (!mtgid.HasValue || i == null || !q.Any())
                doc.Add(new Phrase("no data"));
            else
            {
                var mt = new PdfPTable(1);
                mt.SetNoPadding();
                mt.HeaderRows = 1;

                float[] widths = { 4f, 6f, 7f, 2.6f, 2f, 3f };
                var t = new PdfPTable(widths);
                t.DefaultCell.Border = PdfPCell.NO_BORDER;
                t.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                t.DefaultCell.SetLeading(2.0f, 1f);
                t.WidthPercentage = 100;

                t.AddHeader("Name", boldfont);
                t.AddHeader("Address", boldfont);
                t.AddHeader("Phone/Email", boldfont);
                t.AddHeader("Last Att.", boldfont);
                t.AddHeader("Birthday", boldfont);
                t.AddHeader("Status", boldfont);
                mt.AddCell(t);

                var color = BaseColor.BLACK;

                foreach (var p in q)
                {
                    if (color == BaseColor.WHITE)
                        color = new GrayColor(240);
                    else
                        color = BaseColor.WHITE;

                    t = new PdfPTable(widths);
                    t.DefaultCell.Border = PdfPCell.NO_BORDER;
                    t.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    t.DefaultCell.SetLeading(2.0f, 1f);
                    t.DefaultCell.BackgroundColor = color;
                    t.WidthPercentage = 100;

                    t.Add(p.Name, font);

                    var ph = new Paragraph();
                    ph.AddLine(p.Address, font);
                    ph.AddLine(p.Address2, font);
                    ph.AddLine(p.CSZ, font);
                    t.AddCell(ph);

                    ph = new Paragraph();
                    ph.AddLine(p.HomePhone.FmtFone("H"), font);
                    ph.AddLine(p.CellPhone.FmtFone("C"), font);
                    ph.AddLine(p.Email, font);
                    t.AddCell(ph);

                    t.Add(p.LastAttend.FormatDate(), font);
                    t.Add(p.Birthday, font);
                    t.Add(p.Status, font);
                    t.CompleteRow();

                    t.Add("", font);
                    t.Add(p.AttendStr, 4, monofont);
                    t.AddRight($"{p.AttendPct:n1}{(p.AttendPct.HasValue ? "%" : "")}", font);

                    mt.AddCell(t);
                }
                doc.Add(mt);
            }
            doc.Close();
        }
        public static IEnumerable<AttendInfo> Attendees(int mtgid)
        {
            var q = from a in CurrentDatabase.Attends
                    where a.MeetingId == mtgid
                    where a.EffAttendFlag == true
                    let p = a.Person
                    let lastattend = p.Attends
                                    .Where(aa => aa.Meeting.OrganizationId == a.OrganizationId && aa.AttendanceFlag == true)
                                    .Where(aa => aa.MeetingDate < a.MeetingDate)
                                    .Max(aa => aa.MeetingDate)
                    let status = a.AttendType.Description
                    let attendpct = a.Meeting.Organization.OrganizationMembers
                                    .Where(aa => aa.PeopleId == a.PeopleId)
                                    .Select(aa => aa.AttendPct)
                                    .SingleOrDefault()
                    let attendstr = a.Meeting.Organization.OrganizationMembers
                                    .Where(aa => aa.PeopleId == a.PeopleId)
                                    .Select(aa => aa.AttendStr)
                                    .SingleOrDefault()
                    orderby status == "Visitor" ? 0 : 1, a.Person.Name2

                    select new AttendInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        Address = p.PrimaryAddress,
                        Birthday = p.DOB.ToDate().ToString2("m"),
                        Email = p.EmailAddress,
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        CSZ = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                        Status = status,
                        LastAttend = lastattend,
                        AttendPct = attendpct,
                        AttendStr = attendstr
                    };
            return q;
        }
   }
}

