/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using CmsData;
using CmsWeb.Areas.Search.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class RosterResult : ActionResult
    {
        public Guid? qid;
        public int? org;
        private OrgSearchModel model;

        public RosterResult()
        {

        }

        public RosterResult(OrgSearchModel m)
        {
            model = m;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=foo.pdf");

            doc = new Document(PageSize.LETTER, 36, 36, 64, 64);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();

            dc = w.DirectContent;

            if (qid != null)
            {
                var o = ReportList().First();
                StartPageSet(o);
                var q = DbUtil.Db.PeopleQuery(qid.Value);
                var q2 = from p in q
                         let rr = p.GetRecReg()
                         join m in RollsheetModel.FetchOrgMembers(o.OrgId, null) on p.PeopleId equals m.PeopleId into j
                         from m in j.DefaultIfEmpty()
                         orderby p.Name2
                         select new
                         {
                             p.Name,
                             MembertypeCode = (m == null ? "V" : m.MemberTypeCode),
                             p.PeopleId,
                             rr.MedicalDescription
                         };
                foreach (var i in q2)
                {
                    AddRow(i.MembertypeCode, i.Name, i.MedicalDescription, i.PeopleId, font);
                }

                if (t.Rows.Count > 1)
                {
                    doc.Add(t);
                }
                else
                {
                    doc.Add(new Phrase("no data"));
                }
            }
            else
            {
                foreach (var o in ReportList())
                {
                    var q = from m in RollsheetModel.FetchOrgMembers(o.OrgId, null)
                            orderby m.Name2
                            select new
                            {
                                m.Name,
                                m.MemberType,
                                m.PeopleId,
                                m.MedicalDescription
                            };
                    if (!q.Any())
                    {
                        continue;
                    }

                    StartPageSet(o);
                    foreach (var i in q)
                    {
                        AddRow(i.MemberType, i.Name, i.MedicalDescription, i.PeopleId, font);
                    }

                    if (t.Rows.Count > 1)
                    {
                        doc.Add(t);
                    }
                    else
                    {
                        doc.Add(new Phrase("no data"));
                    }
                }
            }

            if (!pagesetstarted)
            {
                w.PageEvent = null;
                doc.Add(new Phrase("no data"));
            }
            pageEvents.EndPageSet();
            doc.Close();
        }
        public class MemberInfo
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Organization { get; set; }
            public string Location { get; set; }
            public string MemberType { get; set; }
        }

        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        private PageEvent pageEvents = new PageEvent();
        private PdfPTable t;
        private Document doc;
        private PdfContentByte dc;
        private bool pagesetstarted = false;

        private void StartPageSet(OrgInfo o)
        {
            t = new PdfPTable(4);
            t.WidthPercentage = 100;
            t.SetWidths(new[] { 15, 30, 15, 40 });
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            pageEvents.StartPageSet(
                $"{o.Division}: {o.Name}, {o.Location} ({o.Teacher})",
                $"({o.OrgId})");

            var boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
            t.AddCell(new Phrase("PeopleId", boldfont));
            t.AddCell(new Phrase("Name", boldfont));
            t.AddCell(new Phrase("Member Type", boldfont));
            t.AddCell(new Phrase("Allergies", boldfont));
            pagesetstarted = true;
        }
        private void AddRow(string Code, string name, string med, int pid, Font font)
        {
            t.AddCell(pid.ToString());
            t.AddCell(name);
            t.AddCell(Code);
            t.AddCell(med);
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
            var orgs = org == null
                ? model.FetchOrgs()
                : OrgSearchModel.FetchOrgs(org.Value);
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
            private PdfTemplate npages;
            private PdfWriter writer;
            private Document document;
            private PdfContentByte dc;
            private BaseFont font;
            private string HeadText;
            private string HeadText2;

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
                {
                    return;
                }

                npages.BeginText();
                npages.SetFontAndSize(font, 8);
                npages.ShowText((writer.PageNumber + 1).ToString());
                npages.EndText();
            }
            public void StartPageSet(string header1, string header2)
            {
                EndPageSet();
                document.NewPage();
                document.ResetPageCount();
                this.HeadText = header1;
                this.HeadText2 = header2;
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
                if (text != null)
                {
                    len = font.GetWidthPoint(text, HeadFontSize);
                    dc.BeginText();
                    dc.SetFontAndSize(font, HeadFontSize);
                    dc.SetTextMatrix(30, document.PageSize.Height - 30);
                    dc.ShowText(text);
                    dc.EndText();
                    dc.BeginText();
                    dc.SetFontAndSize(font, HeadFontSize);
                    dc.SetTextMatrix(30, document.PageSize.Height - 30 - (HeadFontSize * 1.5f));
                    dc.ShowText(HeadText2);
                    dc.EndText();
                }

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
                text = "Roster Report";
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
    }
}


