/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class BarCodeLabelsResult : ActionResult
    {
        private Guid qid;
        public BarCodeLabelsResult(Guid id)
        {
            qid = id;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            var document = new Document(PageSize.LETTER);
            document.SetMargins(36f, 36f, 33f, 36f);
            var w = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            dc = w.DirectContent;

            var cols = new float[3 * 2 - 1];
            var twid = 0f;
            var t = new PdfPTable(cols.Length);
            for (var i = 0; i < cols.Length; i++)
            {
                if (i % 2 == 1)
                {
                    cols[i] = GAP * 72f;
                }
                else
                {
                    cols[i] = W * 72f;
                }
            }

            foreach (var wid in cols)
            {
                twid += wid;
            }

            t.TotalWidth = twid;
            t.SetWidths(cols);
            t.HorizontalAlignment = Element.ALIGN_CENTER;
            t.LockedWidth = true;
            t.DefaultCell.Border = PdfPCell.NO_BORDER;

            var q = DbUtil.Db.PeopleQuery(qid);
            var q2 = from p in q
                     orderby p.Name2
                     select new
                     {
                         First = p.PreferredName,
                         Last = p.LastName,
                         PeopleId = p.PeopleId,
                         Phone = p.CellPhone ?? p.HomePhone
                     };
            if (!q2.Any())
            {
                document.Add(new Phrase("no data"));
            }
            else
            {
                foreach (var m in q2)
                {
                    AddRow(t, m.First, m.Last, m.Phone, m.PeopleId);
                }
            }

            document.Add(t);

            document.Close();
        }
        protected float H = 1.0f;
        protected float W = 2.625f;
        protected float GAP = .125f;

        protected PdfContentByte dc;
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 20);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

        public void AddRow(PdfPTable t, string fname, string lname, string phone, int pid)
        {
            var bc = new Barcode39();
            bc.X = 1.2f;
            bc.Font = null;
            bc.Code = pid.ToString();
            var img = bc.CreateImageWithBarcode(dc, null, null);
            var p1 = new Phrase();
            p1.Add(new Chunk(img, 0, 0));
            p1.Add(new Phrase("\n\n" + fname + " " + lname + " (" + pid + ")", smallfont));
            var c = new PdfPCell(p1);
            c.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            c.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            c.Border = PdfPCell.NO_BORDER;
            c.FixedHeight = H * 72f;

            var t2 = new PdfPTable(2);
            t2.WidthPercentage = 100f;
            t2.DefaultCell.Border = PdfPCell.NO_BORDER;

            var cc = new PdfPCell(new Phrase(fname, font));
            cc.Border = PdfPCell.NO_BORDER;
            cc.Colspan = 2;
            t2.AddCell(cc);

            cc = new PdfPCell(new Phrase(lname, font));
            cc.Border = PdfPCell.NO_BORDER;
            cc.Colspan = 2;
            t2.AddCell(cc);

            var pcell = new PdfPCell(new Phrase(pid.ToString(), smallfont));
            pcell.Border = PdfPCell.NO_BORDER;
            pcell.HorizontalAlignment = Element.ALIGN_LEFT;
            t2.AddCell(pcell);

            pcell = new PdfPCell(new Phrase(phone.FmtFone(), smallfont));
            pcell.Border = PdfPCell.NO_BORDER;
            pcell.HorizontalAlignment = Element.ALIGN_RIGHT;
            t2.AddCell(pcell);

            var cell = new PdfPCell(t2);
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell.PaddingLeft = 8f;
            cell.PaddingRight = 8f;
            cell.Border = PdfPCell.NO_BORDER;
            cell.FixedHeight = H * 72f;

            t.AddCell(c);
            t.AddCell("");
            t.AddCell(cell);
            t.AddCell("");
            t.AddCell(cell);
        }

        private class PageEvent : PdfPageEventHelper
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
                {
                    return;
                }

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

