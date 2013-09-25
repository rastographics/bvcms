/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections;
using CmsData;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Diagnostics;

namespace CmsWeb.Areas.Main.Models.Report
{
    public class RollLabelsResult : ActionResult
    {
        public object qid { get; set; }
        public string format { get; set; }
        public bool titles { get; set; }
        public bool usephone { get; set; }

        public bool? sortzip; 
	    public string sort
	    {
			get { return (sortzip ?? false) ? "Zip" : "Name"; }
	    }

        protected float H = .925f;
        protected float W = 3f;

        protected PdfContentByte dc;
        private Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private Font smfont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            var document = new Document();
            document.SetPageSize(new Rectangle(72 * W, 72 * H));
            document.SetMargins(14f, 0f, 3.6f, 1f);
            var w = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            dc = w.DirectContent;

            var ctl = new MailingController();
            ctl.UseTitles = titles;

            IEnumerable<MailingController.MailingInfo> q = null;
            switch (format)
            {
                case "Individual":
                case "GroupAddress":
                    q = ctl.FetchIndividualList(sort, qid);
                    break;
                case "FamilyMembers":
                case "Family":
                    q = ctl.FetchFamilyList(sort, qid);
                    break;
                case "ParentsOf":
                    q = ctl.FetchParentsOfList(sort, qid);
                    break;
                case "CouplesEither":
                    q = ctl.FetchCouplesEitherList(sort, qid);
                    break;
                case "CouplesBoth":
                    q = ctl.FetchCouplesBothList(sort, qid);
                    break;
            }
            AddLabel(document, "=========", Util.UserName, "{0} labels printed".Fmt(q.Count()), "{0:g}".Fmt(DateTime.Now), String.Empty);
            foreach (var m in q)
                AddLabel(document, m.LabelName, m.Address, m.Address2, m.CityStateZip, Util.PickFirst(m.CellPhone.FmtFone("C "), m.HomePhone.FmtFone("H ")));

            document.Close();
        }
        public void AddLabel(Document d, string name, string addr, string addr2, string csz, string phone)
        {
            var t2 = new PdfPTable(1);
            t2.WidthPercentage = 100f;
            t2.DefaultCell.Border = PdfPCell.NO_BORDER;

            var c2 = new PdfPCell(new Phrase(name, font));
            c2.Border = PdfPCell.NO_BORDER;
            if (usephone)
            {
                var nt = new PdfPTable(new float[] { 20f, 10f });
                nt.WidthPercentage = 100f;
                nt.DefaultCell.Padding = 0;
                nt.DefaultCell.Border = PdfPCell.NO_BORDER;
                c2.Padding = 0.0F;
                nt.AddCell(c2);
                c2 = new PdfPCell(new Phrase(phone, smfont));
                c2.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                c2.Border = PdfPCell.NO_BORDER;
                c2.Padding = 0.0F;
                c2.PaddingRight = 3F;
                nt.AddCell(c2);
                t2.AddCell(nt);
            }
            else
                t2.AddCell(c2);

            var cc = new PdfPCell(new Phrase(addr, font));
            cc.Border = PdfPCell.NO_BORDER;
            t2.AddCell(cc);

            if (addr2.HasValue())
            {
                cc = new PdfPCell(new Phrase(addr2, font));
                cc.Border = PdfPCell.NO_BORDER;
                t2.AddCell(cc);
            }

            cc = new PdfPCell(new Phrase(csz, font));
            cc.Border = PdfPCell.NO_BORDER;
            t2.AddCell(cc);

            d.Add(t2);
            d.NewPage();
        }

    }
}

