/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class VisitsAbsentsResult : ActionResult
    {
        private readonly Font bigboldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
        private Document doc;
        private DateTime dt;
        private int? mtgid;
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));

        public VisitsAbsentsResult(int? meetingid)
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

            var i = (from m in DbUtil.Db.Meetings
                     where m.MeetingId == mtgid
                     select new
                     {
                         m.Organization.OrganizationName,
                         m.Organization.LeaderName,
                         m.MeetingDate
                     }).SingleOrDefault();

            w.PageEvent = new HeadFoot
            {
                HeaderText = $"Guests/Absents Report: {i.OrganizationName} - {i.LeaderName} {i.MeetingDate:g}",
                FooterText = "Guests/Absents Report"
            };
            doc.Open();

            var q = VisitsAbsents(mtgid.Value);

            if (!mtgid.HasValue || i == null || !q.Any())
            {
                doc.Add(new Paragraph("no data"));
            }
            else
            {
                var mt = new PdfPTable(1);
                mt.SetNoPadding();
                mt.HeaderRows = 1;

                float[] widths = { 4f, 6f, 7f, 2.6f, 2f, 3f };
                var t = new PdfPTable(widths);
                t.DefaultCell.Border = Rectangle.NO_BORDER;
                t.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
                t.DefaultCell.SetLeading(2.0f, 1f);
                t.WidthPercentage = 100;

                t.AddHeader("Name", boldfont);
                t.AddHeader("Address", boldfont);
                t.AddHeader("Phone/Email", boldfont);
                t.AddHeader("Last Att.", boldfont);
                t.AddHeader("Birthday", boldfont);
                t.AddHeader("Guest/Member", boldfont);
                mt.AddCell(t);

                var color = BaseColor.BLACK;
                bool? v = null;
                foreach (var p in q)
                {
                    if (color == BaseColor.WHITE)
                    {
                        color = new GrayColor(240);
                    }
                    else
                    {
                        color = BaseColor.WHITE;
                    }

                    t = new PdfPTable(widths);
                    t.SetNoBorder();
                    t.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
                    t.DefaultCell.BackgroundColor = color;

                    if (v != p.visitor)
                    {
                        t.Add($"             ------ {(p.visitor ? "Guests" : "Absentees")} ------", 6, bigboldfont);
                    }

                    v = p.visitor;

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

                    if (p.Status == null || !p.Status.StartsWith("Visit"))
                    {
                        t.Add("", font);
                        t.Add($"{p.AttendStr}           {p.AttendPct:n1}{(p.AttendPct.HasValue ? "%" : "")}", 5, monofont);
                    }

                    mt.AddCell(t);
                }
                doc.Add(mt);
            }
            doc.Close();
        }

        public IEnumerable<AttendInfo> VisitsAbsents(int mtgid)
        {
            var q = from va in DbUtil.Db.VisitsAbsents(mtgid)
                    orderby va.AttendanceFlag descending, va.Name
                    select new AttendInfo
                    {
                        PeopleId = va.PeopleId,
                        Name = va.Name,
                        Address = va.PrimaryAddress,
                        Birthday = va.Birthday.ToString2("m"),
                        Email = va.EmailAddress,
                        HomePhone = va.HomePhone,
                        CellPhone = va.CellPhone,
                        CSZ = Util.FormatCSZ4(va.PrimaryCity, va.PrimaryState, va.PrimaryZip),
                        Status = va.Status,
                        LastAttend = va.LastAttended,
                        AttendPct = va.AttendPct,
                        AttendStr = va.AttendStr,
                        visitor = va.AttendanceFlag
                    };
            return q;
        }

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
            public bool visitor { get; set; }
        }
    }
}
