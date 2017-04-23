/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CmsData;
using UtilityExtensions;
using System.Text;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;

namespace CmsWeb.Areas.Reports.Models
{
    public class ClassListResult : ActionResult
    {
        private readonly OrgSearchModel model;

        public ClassListResult(OrgSearchModel m)
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

            var haddata = false;
            foreach (var o in list1)
            {
                var t = StartPageSet(o);

                var color = BaseColor.BLACK;
                foreach (var m in RollsheetModel.FetchOrgMembers(o.OrgId, null))
                {
                    if (color == BaseColor.WHITE)
                        color = new GrayColor(240);
                    else
                        color = BaseColor.WHITE;
                    AddRow(t, m,  color);
                    haddata = true;
                }
                doc.Add(t);
            }
            if (!haddata)
                doc.Add(new Phrase("\nno data"));
            pageEvents.EndPageSet();
            doc.Close();
        }

        private Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
        private Font font = FontFactory.GetFont(FontFactory.HELVETICA);
        private Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        private Font medfont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private PageEvent pageEvents = new PageEvent();
        private Document doc;
        private PdfContentByte dc;

        float[] HeaderWids = { 45, 35, 15 };

        private PdfPTable StartPageSet(OrgInfo o)
        {
            var t = new PdfPTable(HeaderWids);
            t.HeaderRows = 1;
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            t.WidthPercentage = 100;
            t.DefaultCell.Padding = 5;
            pageEvents.StartPageSet($"Class List: {o.Division} - {o.Name} ({o.Teacher})");

            t.AddCell(new Phrase("Name\nAddress", boldfont));
            t.AddCell(new Phrase("Phones\nEmail", boldfont));
            t.AddCell(new Phrase("\nMember Type", boldfont));
            return t;
        }

        private void AddRow(PdfPTable t, RollsheetModel.PersonMemberInfo p, BaseColor color)
        {
            t.DefaultCell.BackgroundColor = color;

            var c = new Phrase();
            c.Add(new Chunk(p.Name, boldfont));
            c.Add(new Chunk($"  ({p.PeopleId})\n", smallfont));
            var sb = new StringBuilder();
            AddLine(sb, p.Address);
            AddLine(sb, p.Address2);
            AddLine(sb, p.CityStateZip);
            c.Add(new Chunk(sb.ToString(), font));
            t.AddCell(c);

            sb = new StringBuilder();
            AddPhone(sb, p.HomePhone, "h ");
            AddPhone(sb, p.CellPhone, "c ");
            AddPhone(sb, p.WorkPhone, "w ");
            AddLine(sb, p.Email);
            t.AddCell(new Phrase(sb.ToString(), font));

            t.AddCell(new Phrase(p.MemberType));
        }
        private void AddLine(StringBuilder sb, string value)
        {
            AddLine(sb, value, String.Empty);
        }
        private void AddLine(StringBuilder sb, string value, string postfix)
        {
            if (value.HasValue())
            {
                if (sb.Length > 0)
                    sb.Append("\n");
                sb.Append(value);
                if (postfix.HasValue())
                    sb.Append(postfix);
            }
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
        private class OrgInfo
        {
            public int OrgId { get; set; }
            public string Division { get; set; }
            public string Name { get; set; }
            public string Teacher { get; set; }
            public string Location { get; set; }
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
                        Location = o.Location,
                    };
            return q;
        }
        class PageEvent : PdfPageEventHelper
        {
            private PdfTemplate npages;
            private PdfWriter writer;
            private Document document;
            private PdfContentByte dc;
            private BaseFont font;
            private string HeadText;

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
                this.HeadText = header1;
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
                dc.SetTextMatrix(document.PageSize.Width / 2 - len / 2, 30);
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
        class MemberInfo
        {
            public string Name { get; set; }
            public string Addr1 { get; set; }
            public string Addr2 { get; set; }
            public string CSZ { get; set; }
            public string HomePhone { get; set; }
            public string Cell { get; set; }
            public string MemberStatus { get; set; }
            public string ThisChurch { get; set; }
            public string ActiveOther { get; set; }
            public string MemberType { get; set; }
            public string PropertyName { get; set; }
        }
    }
}


