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
    public class AveryResult : ActionResult
    {
        public Guid id;
        protected float W = 197f;
        public bool namesonly;

        protected PdfContentByte dc;
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 20);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            var q = DbUtil.Db.PeopleQuery(id);
            var q2 = from p in q
                     orderby p.Name2
                     select new
                     {
                         First = p.PreferredName,
                         Last = p.LastName,
                         p.PeopleId,
                         dob = p.DOB,
                         Phone = p.CellPhone.Length > 0 ? p.CellPhone : p.HomePhone,
                         p.DoNotPublishPhones
                     };
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            var document = new Document(PageSize.LETTER);
            document.SetMargins(40f, 36f, 32f, 36f);
            var w = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            dc = w.DirectContent;

            var cols = new float[] { W, W, W - 10f };
            var t = new PdfPTable(cols);
            t.SetTotalWidth(cols);

            t.HorizontalAlignment = Element.ALIGN_CENTER;
            t.LockedWidth = true;
            t.DefaultCell.Border = PdfPCell.NO_BORDER;

            if (!q2.Any())
            {
                document.Add(new Phrase("no data"));
            }
            else
            {
                foreach (var m in q2)
                {
                    AddCell(t, m.First, m.Last, m.Phone, m.PeopleId, m.DoNotPublishPhones);
                }
            }

            t.CompleteRow();
            document.Add(t);

            document.Close();
        }
        public void AddCell(PdfPTable t, string fname, string lname, string phone, int pid, bool? DoNotPublishPhones)
        {
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

            if (!namesonly)
            {
                var pcell = new PdfPCell(new Phrase(pid.ToString(), smallfont));
                pcell.Border = PdfPCell.NO_BORDER;
                pcell.HorizontalAlignment = Element.ALIGN_LEFT;
                t2.AddCell(pcell);

                pcell = new PdfPCell(new Phrase(DoNotPublishPhones != true ? phone.FmtFone() : "", smallfont));
                pcell.Border = PdfPCell.NO_BORDER;
                pcell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                t2.AddCell(pcell);
            }

            var cell = new PdfPCell(t2);
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell.PaddingLeft = 8f;
            cell.PaddingRight = 8f;
            cell.Border = PdfPCell.NO_BORDER;
            cell.FixedHeight = 72f;

            t.AddCell(cell);
        }
    }
}

