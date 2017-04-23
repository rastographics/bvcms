/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData.Codes;
using CmsWeb.Areas.Search.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class OrgLeadersResult : ActionResult
    {
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA);
        private readonly float[] HeaderWids = {12, 40, 25, 20};
        private readonly OrgSearchModel model;
        private readonly PageEvent pageEvents = new PageEvent();
        private PdfContentByte dc;
        private Document doc;
        private Font medfont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        public int? meetingid;
        public string name;
        private Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);

        public OrgLeadersResult(OrgSearchModel m)
        {
            model = m;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            var list1 = ReportList().ToList();

            if (!list1.Any())
            {
                Response.Write("no data found");
                return;
            }
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            doc = new Document(PageSize.LETTER, 36, 36, 36, 36);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;

            foreach (var o in list1)
            {
                var t = StartPageSet(o);

                var color = BaseColor.BLACK;
                foreach (var m in RollsheetModel.FetchOrgMembers(o.OrgId, null)
                    .Where(om => om.MemberTypeId != MemberTypeCode.Member))
                {
                    if (color == BaseColor.WHITE)
                        color = new GrayColor(240);
                    else
                        color = BaseColor.WHITE;
                    AddRow(t,
                        m.PeopleId,
                        m.Name,
                        m.Email,
                        m.HomePhone,
                        m.CellPhone,
                        m.WorkPhone,
                        m.MemberType,
                        color);
                }
                doc.Add(t);
            }
            pageEvents.EndPageSet();
            doc.Close();
        }

        private PdfPTable StartPageSet(OrgInfo o)
        {
            var t = new PdfPTable(HeaderWids);
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = Rectangle.NO_BORDER;
            t.WidthPercentage = 100;
            t.DefaultCell.Padding = 5;
            pageEvents.StartPageSet($"Leader Report: {o.Division} - {o.Name} ({o.Teacher})");

            t.AddCell(new Phrase("\nPeopleId", boldfont));
            t.AddCell(new Phrase("Name\nEmail", boldfont));
            t.AddCell(new Phrase("\nPhones", boldfont));
            t.AddCell(new Phrase("\nMember Type", boldfont));
            return t;
        }

        private void AddRow(PdfPTable t, int PeopleId, string Name, string Email, string HomePhone, string CellPhone, string WorkPhone, string MemberType, BaseColor color)
        {
            t.DefaultCell.BackgroundColor = color;

            t.AddCell(new Phrase(PeopleId.ToString(), font));

            t.AddCell(new Phrase(Name + "\n" + Email, font));

            var sb = new StringBuilder();
            AddPhone(sb, HomePhone, "h ");
            AddPhone(sb, CellPhone, "c ");
            AddPhone(sb, WorkPhone, "w ");
            t.AddCell(new Phrase(sb.ToString(), font));

            t.AddCell(new Phrase(MemberType));
        }

        private void AddPhone(StringBuilder sb, string value, string prefix)
        {
            if (value.HasValue())
            {
                value = value.FmtFone(prefix);
                if (sb.Length > 0)
                    sb.Append("\n");
                sb.Append(value);
            }
        }

        private IEnumerable<OrgInfo> ReportList()
        {
            var orgs = model.FetchOrgs();
            var q = from o in orgs
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Division = o.Division,
                        Name = o.OrganizationName,
                        Teacher = o.LeaderName,
                        Location = o.Location
                    };
            return q;
        }

        private class OrgInfo
        {
            public int OrgId { get; set; }
            public string Division { get; set; }
            public string Name { get; set; }
            public string Teacher { get; set; }
            public string Location { get; set; }
        }

        private class CellEvent : IPdfPCellEvent
        {
            public void CellLayout(PdfPCell cell, Rectangle pos, PdfContentByte[] canvases)
            {
                var cb = canvases[PdfPTable.BACKGROUNDCANVAS];
                cb.SetGrayStroke(0f);
                cb.SetLineWidth(.2f);
                cb.RoundRectangle(pos.Left + 4, pos.Bottom, pos.Width - 8, pos.Height - 4, 4);
                cb.Stroke();
                cb.ResetRGBColorStroke();
            }
        }

        private class PageEvent : PdfPageEventHelper
        {
            private PdfContentByte dc;
            private Document document;
            private BaseFont font;
            private string HeadText;
            private PdfTemplate npages;
            private PdfWriter writer;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                this.writer = writer;
                this.document = document;
                base.OnOpenDocument(writer, document);
                font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                dc = writer.DirectContent;
            }

            public void EndPageSet()
            {
                if (npages == null)
                    return;
                npages.BeginText();
                npages.SetFontAndSize(font, 8);
                npages.ShowText((writer.PageNumber + 1).ToString());
                npages.EndText();
            }

            public void StartPageSet(string header1)
            {
                EndPageSet();
                document.NewPage();
                document.ResetPageCount();
                HeadText = header1;
                npages = dc.CreateTemplate(50, 50);
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                string text;
                float len;

                //---Header left
                text = HeadText;
                const float HeadFontSize = 11f;
                len = font.GetWidthPoint(text, HeadFontSize);
                dc.BeginText();
                dc.SetFontAndSize(font, HeadFontSize);
                dc.SetTextMatrix(30, document.PageSize.Height - 30);
                dc.ShowText(text);
                dc.EndText();
                //dc.BeginText();
                //dc.SetFontAndSize(font, HeadFontSize);
                //dc.SetTextMatrix(30, document.PageSize.Height - 30 - (HeadFontSize * 1.5f));
                //dc.ShowText(HeadText2);
                //dc.EndText();

                //---Column 1
                text = "Page " + (writer.PageNumber + 1) + " of ";
                len = font.GetWidthPoint(text, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(30, 30);
                dc.ShowText(text);
                dc.EndText();
                dc.AddTemplate(npages, 30 + len, 30);

                //---Column 2
                text = HeadText;
                len = font.GetWidthPoint(text, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(document.PageSize.Width/2 - len/2, 30);
                dc.ShowText(text);
                dc.EndText();

                //---Column 3
                text = Util.Now.ToShortDateString();
                len = font.GetWidthPoint(text, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(document.PageSize.Width - 30 - len, 30);
                dc.ShowText(text);
                dc.EndText();
            }

            private float PutText(string text, BaseFont font, float size, float x, float y)
            {
                dc.BeginText();
                dc.SetFontAndSize(font, size);
                dc.SetTextMatrix(x, y);
                dc.ShowText(text);
                dc.EndText();
                return font.GetWidthPoint(text, size);
            }
        }
    }
}
