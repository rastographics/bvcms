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
using CmsWeb.Code;
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
    public class FamilyResult : ActionResult
    {
        private Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
        private Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));
        private Font xsmallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7, new GrayColor(50));
        private PageEvent pageEvents = new PageEvent();
        private PdfPTable t;
        private Document doc;
        private DateTime dt;
        private PdfContentByte dc;

        private Guid qid;
        public FamilyResult(Guid id)
        {
            qid = id;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            dt = Util.Now;

            doc = new Document(PageSize.LETTER.Rotate(), 36, 36, 64, 64);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;

            t = new PdfPTable(1);
            t.WidthPercentage = 100;
            t.DefaultCell.Padding = 0;
            t.HeaderRows = 1;

            t.AddCell(StartPageSet());

            t.DefaultCell.Border = PdfPCell.TOP_BORDER;
            t.DefaultCell.BorderColor = BaseColor.BLACK;
            t.DefaultCell.BorderColorTop = BaseColor.BLACK;
            t.DefaultCell.BorderWidthTop = 2.0f;

            var q = DbUtil.Db.PeopleQuery(qid);
            var q2 = from p in q
                     let person = p
                     group p by p.FamilyId into g
                     let hhname = g.First().Family.HeadOfHousehold.Name2
                     orderby hhname
                     select new
                     {
                         members = from m in g.First().Family.People
                                   where !m.DeceasedDate.HasValue
                                   select new
                                   {
                                       order = m.PositionInFamilyId * 1000 + (m.PositionInFamilyId == 10 ? m.GenderId : 1000 - (m.Age ?? 0)),
                                       //                                           order = g.Any(p => p.PeopleId == m.PeopleId) ? 1 :
                                       //                                                 m.PositionInFamilyId,
                                       person = m
                                   }
                     };
            foreach (var f in q2)
            {
                var ft = new PdfPTable(HeaderWids);
                ft.DefaultCell.SetLeading(2.0f, 1f);
                ft.DefaultCell.Border = PdfPCell.NO_BORDER;
                ft.DefaultCell.Padding = 5;
                int fn = 1;
                var color = BaseColor.BLACK;
                foreach (var p in f.members.OrderBy(m => m.order))
                {
                    if (color == BaseColor.WHITE)
                        color = new GrayColor(240);
                    else
                        color = BaseColor.WHITE;
                    Debug.WriteLine("{0:##}: {1}".Fmt(p.order, p.person.Name));
                    AddRow(ft, p.person, fn, color);
                    fn++;
                }
                t.AddCell(ft);
            }
            if (t.Rows.Count > 1)
                doc.Add(t);
            else
                doc.Add(new Phrase("no data"));
            pageEvents.EndPageSet();
            doc.Close();
        }

        float[] HeaderWids = new float[] { 55, 40, 95 };

        private PdfPTable StartPageSet()
        {
            var t = new PdfPTable(HeaderWids);
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            t.WidthPercentage = 100;
            t.DefaultCell.Padding = 5;
            pageEvents.StartPageSet("Family Report: {0:d}".Fmt(dt));
            t.AddCell(new Phrase("Name (id)\nAddress/Contact info", boldfont));
            t.AddCell(new Phrase("Birthday (age, gender)\nMember (Other Church)", boldfont));
            t.AddCell(new Phrase("Position in Family\nPrimary Class", boldfont));
            return t;
        }

        private void AddRow(PdfPTable t, Person p, int fn, BaseColor color)
        {
            t.DefaultCell.BackgroundColor = color;

            var c1 = new Phrase();
            c1.Add(new Chunk(p.Name, boldfont));
            c1.Add(new Chunk("  ({0})\n".Fmt(p.PeopleId), smallfont));
            var contact = new StringBuilder();
            var cv = new CodeValueModel();
            if (fn == 1)
            {
                AddLine(contact, p.PrimaryAddress);
                AddLine(contact, p.PrimaryAddress2);
                AddLine(contact, "{0}, {1} {2}".Fmt(p.PrimaryCity, p.PrimaryState, p.PrimaryZip.FmtZip()));
            }
            AddPhone(contact, p.HomePhone, "h ");
            AddPhone(contact, p.CellPhone, "c ");
            AddPhone(contact, p.WorkPhone, "w ");
            AddLine(contact, p.EmailAddress);
            c1.Add(new Chunk(contact.ToString(), font));
            t.AddCell(c1);

            var c2 = new Phrase("{0} ({1}, {2})\n".Fmt(p.DOB, p.Age,
                p.GenderId == 1 ? "M" : p.GenderId == 2 ? "F" : "U"), font);
            c2.Add(new Chunk("{0} ({1})".Fmt(cv.MemberStatusCodes().ItemValue(p.MemberStatusId), "?"), font));
            t.AddCell(c2);


            var c3 = new Phrase((
                    p.PositionInFamilyId == 10 ? "Primary Adult" :
                    p.PositionInFamilyId == 20 ? "Secondary Adult" :
                                                 "Child") + "\n", font);
            if (p.BibleFellowshipClassId.HasValue)
            {
                c3.Add(new Chunk(p.BFClass.OrganizationName, font));
                if (p.BFClass.LeaderName.HasValue())
                    c3.Add(new Chunk(" ({0})".Fmt(p.BFClass.LeaderName), smallfont));
            }
            t.AddCell(c3);
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

        private Paragraph GetAttendance(Person p)
        {
            var q = from a in p.Attends
                    where a.AttendanceFlag == true
                    orderby a.MeetingDate.Date descending
                    group a by a.MeetingDate.Date into g
                    select g.Key;
            var list = q.ToList();

            var attstr = new StringBuilder("\n");
            var dt = Util.Now;
            var yearago = dt.AddYears(-1);
            while (dt > yearago)
            {
                var dt2 = dt.AddDays(-7);
                var indicator = ".";
                foreach (var d in list)
                {
                    if (d < dt2)
                        break;
                    if (d <= dt)
                    {
                        indicator = "P";
                        break;
                    }
                }
                attstr.Insert(0, indicator);
                dt = dt2;
            }
            var ph = new Paragraph(attstr.ToString(), monofont);
            ph.SetLeading(0, 1.2f);

            attstr = new StringBuilder();
            foreach (var d in list.Take(8))
                attstr.Insert(0, "{0:d}  ".Fmt(d));
            if (list.Count > 8)
            {
                attstr.Insert(0, "...  ");
                var q2 = q.OrderBy(d => d).Take(Math.Min(list.Count - 8, 3));
                foreach (var d in q2.OrderByDescending(d => d))
                    attstr.Insert(0, "{0:d}  ".Fmt(d));
            }
            ph.Add(new Chunk(attstr.ToString(), smallfont));
            return ph;
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

