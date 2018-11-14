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
    public class PastAttendeeResult : ActionResult
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
            public bool visitor { get; set; }
            public DateTime? LastAttend { get; set; }
            public decimal? AttendPct { get; set; }
            public string AttendStr { get; set; }
            public string AttendType { get; set; }
        }
        private readonly Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private readonly Font bigboldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));
        private Document doc;
        private DateTime dt;

        private int? orgid;
        public PastAttendeeResult(int? orgid)
        {
            this.orgid = orgid;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            dt = Util.Now;

            doc = new Document(PageSize.LETTER.Rotate(), 36, 36, 64, 64);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);

            var roles = DbUtil.Db.CurrentRoles();
            var i = (from o in DbUtil.Db.Organizations
                     where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                     where o.OrganizationId == orgid
                     select new
                     {
                         o.OrganizationName,
                         o.LeaderName,
                         o.FirstMeetingDate
                     }).SingleOrDefault();

            w.PageEvent = new HeadFoot
            {
                HeaderText = $"Recent Attendee Report: {i.OrganizationName} - {i.LeaderName} ({(i.FirstMeetingDate.HasValue ? "since " + i.FirstMeetingDate.FormatDate() : "no First Meeting Date set")})",
                FooterText = "Recent Attendee Report"
            };
            doc.Open();

            var q = Attendees(orgid.Value);

            if (!orgid.HasValue || i == null || !q.Any())
            {
                doc.Add(new Phrase("no data"));
            }
            else
            {
                var mt = new PdfPTable(1);
                mt.SetNoPadding();
                mt.HeaderRows = 1;

                float[] widths = new float[] { 4f, 6f, 7f, 2.6f, 2f, 3f };
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
                        t.Add($"             ------ {(p.visitor ? "Guests and Previous Members" : "Members")} ------", 6, bigboldfont);
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
                    t.Add(p.AttendType, font);
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
        public static IEnumerable<AttendInfo> Attendees(int orgid)
        {
            var q = from ra in DbUtil.Db.RecentAttendance(orgid)
                    let p = DbUtil.Db.People.Single(pp => pp.PeopleId == ra.PeopleId)
                    orderby ra.Visitor descending, ra.Lastattend descending, p.Name2
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
                        visitor = ra.Visitor == 1,
                        LastAttend = ra.Lastattend,
                        AttendPct = ra.AttendPct,
                        AttendStr = ra.AttendStr,
                        AttendType = ra.Attendtype
                    };
            return q;
        }
    }
}

