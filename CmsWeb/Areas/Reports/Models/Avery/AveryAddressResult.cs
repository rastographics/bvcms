/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using Paragraph = iTextSharp.text.Paragraph;

namespace CmsWeb.Areas.Reports.Models
{
    public class AveryAddressResult : ActionResult
    {
        public Guid id;
        public string format;
        public bool? titles;
        public bool? useMailFlags;
        public bool usephone { get; set; }
        public int skip = 0;

        public bool? sortzip;
        public string sort
        {
            get { return (sortzip ?? false) ? "Zip" : "Name"; }
        }

        const float H = 72f;
        const float W = 197f;
        private Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private Font smfont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

        protected PdfContentByte dc;

        public override void ExecuteResult(ControllerContext context)
        {
            var ctl = new MailingController { UseTitles = titles ?? false, UseMailFlags = useMailFlags ?? false };
            var Response = context.HttpContext.Response;

            IEnumerable<MailingController.MailingInfo> q = null;
            switch (format)
            {
                case "Individual":
                    q = ctl.FetchIndividualList(sort, id);
                    break;
                case "GroupAddress":
                    q = ctl.GroupByAddress(id);
                    break;
                case "Family":
                case "FamilyMembers":
                    q = ctl.FetchFamilyList(sort, id);
                    break;
                case "ParentsOf":
                    q = ctl.FetchParentsOfList(sort, id);
                    break;
                case "CouplesEither":
                    q = ctl.FetchCouplesEitherList(sort, id);
                    break;
                case "CouplesBoth":
                    q = ctl.FetchCouplesBothList(sort, id);
                    break;
                default:
                    Response.Write("unknown format");
                    return;
            }
            if (!q.Any())
            {
                Response.Write("no data found");
                return;
            }
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            var document = new Document(PageSize.LETTER);
            document.SetMargins(50f, 36f, 32f, 36f);
            var w = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            dc = w.DirectContent;

            var cols = new float[] { W, W, W - 25f };
            var t = new PdfPTable(cols);
            t.SetTotalWidth(cols);
            t.HorizontalAlignment = Element.ALIGN_CENTER;
            t.LockedWidth = true;
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            t.DefaultCell.FixedHeight = H;
            t.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            t.DefaultCell.PaddingLeft = 8f;
            t.DefaultCell.PaddingRight = 8f;
            t.DefaultCell.SetLeading(2.0f, 1f);

            if (skip > 0)
            {
                var blankCell = new PdfPCell(t.DefaultCell);

                for (int iX = 0; iX < skip; iX++)
                {
                    t.AddCell(blankCell);
                }
            }

            foreach (var m in q)
            {
                var c = new PdfPCell(t.DefaultCell);
                var ph = new Paragraph();
                if (format == "GroupAddress")
                    ph.AddLine(m.LabelName + " " + m.LastName, font);
                else
                    ph.AddLine(m.LabelName, font);
                ph.AddLine(m.Address, font);
                ph.AddLine(m.Address2, font);
                ph.AddLine(m.CityStateZip, font);
                c.AddElement(ph);
                if (usephone)
                {
                    var phone = Util.PickFirst(m.CellPhone.FmtFone("C "), m.HomePhone.FmtFone("H "));
                    var p = new Paragraph();
                    c.PaddingRight = 7f;
                    p.Alignment = Element.ALIGN_RIGHT;
                    p.Add(new Chunk(phone, smfont));
                    p.ExtraParagraphSpace = 0f;
                    c.AddElement(p);
                }
                t.AddCell(c);
            }
            t.CompleteRow();
            document.Add(t);

            document.Close();
        }
    }
}