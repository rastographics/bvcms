/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsWeb.Code;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class ContactsResult : ActionResult
    {
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private readonly int border = Rectangle.NO_BORDER; //PdfPCell.BOX;
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
        private readonly PageEvent pageEvents = new PageEvent();
        private readonly Guid qid;
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));
        private readonly float[] w = { 40 + 70 + 80, 40 + 130 };
        private readonly float[] w2 = { 40, 70, 80 };
        private readonly float[] w3 = { 40, 130 };
        private PdfContentByte dc;
        private Document doc;
        private DateTime dt;
        private PdfPTable t;
        private readonly Font xsmallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7, new GrayColor(50));

        public ContactsResult(Guid id, bool? sortAddress, string orgname)
        {
            qid = id;
            this.sortAddress = sortAddress ?? false;
            OrganizationName = orgname;
        }

        private bool sortAddress { get; }
        private string OrganizationName { get; }

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

            StartPageSet();
            var q = DbUtil.Db.PeopleQuery(qid);

            if (sortAddress)
            {
                q = from p in q
                    orderby p.PrimaryState, p.PrimaryCity, p.PrimaryZip, p.PrimaryCity, p.PrimaryAddress, p.Name2
                    select p;
            }
            else
            {
                q = from p in q
                    orderby p.Name2
                    select p;
            }

            foreach (var p in q)
            {
                t.AddCell(NewCell(p));
            }

            t.SplitLate = false;
            t.SplitRows = true;
            if (t.Rows.Count > 1)
            {
                doc.Add(t);
            }
            else
            {
                doc.Add(new Phrase("no data"));
            }

            pageEvents.EndPageSet();
            doc.Close();
        }

        private void StartPageSet()
        {
            t = new PdfPTable(w);
            t.WidthPercentage = 100;
            t.DefaultCell.Border = border;
            t.DefaultCell.Padding = 5;
            t.HeaderRows = 1;
            if (OrganizationName.HasValue())
            {
                pageEvents.StartPageSet($"Contacts - {OrganizationName}: {dt:d}");
            }
            else
            {
                pageEvents.StartPageSet($"Contact Report: {dt:d}");
            }

            var t2 = new PdfPTable(w2);
            t2.WidthPercentage = 100;
            t2.DefaultCell.Border = border;
            t2.DefaultCell.Padding = 5;
            t2.AddCell(new Phrase("\nPerson", boldfont));
            t2.AddCell(new Phrase("\nAddress", boldfont));
            t2.AddCell(new Phrase("Phone\nEmail", boldfont));
            var c = new PdfPCell(t.DefaultCell);
            c.AddElement(t2);
            c.Padding = 0;
            t.AddCell(c);

            var t3 = new PdfPTable(w3);
            t3.WidthPercentage = 100;
            t3.DefaultCell.Border = border;
            t3.DefaultCell.Padding = 5;
            t3.DefaultCell.PaddingBottom = 0;
            t3.AddCell(new Phrase("\nBirthday", boldfont));
            t3.AddCell(new Phrase("\nMember Status", boldfont));
            c = new PdfPCell(t.DefaultCell);
            c.Padding = 0;
            c.AddElement(t3);
            t.AddCell(c);
        }

        private PdfPCell NewCell(Person p)
        {
            if (t.Rows.Count % 2 == 0)
            {
                t.DefaultCell.BackgroundColor = new GrayColor(240);
            }
            else
            {
                t.DefaultCell.BackgroundColor = BaseColor.WHITE;
            }

            var t2 = new PdfPTable(w2);
            t2.WidthPercentage = 100;
            t2.DefaultCell.Border = border;
            t2.DefaultCell.Padding = 5;
            var name = new Phrase(p.Name + "\n", font);
            name.Add(new Chunk($"  ({p.PeopleId})", smallfont));
            t2.AddCell(name);
            var addr = new StringBuilder(p.PrimaryAddress);
            AddLine(addr, p.PrimaryAddress2);
            AddLine(addr, $"{p.PrimaryCity}, {p.PrimaryState} {p.PrimaryZip.FmtZip()}");
            t2.AddCell(new Phrase(addr.ToString(), font));
            var phones = new StringBuilder();
            AddPhone(phones, p.HomePhone, "h ");
            AddPhone(phones, p.CellPhone, "c ");
            AddPhone(phones, p.WorkPhone, "w ");
            AddLine(phones, p.EmailAddress);
            t2.AddCell(new Phrase(phones.ToString(), font));
            var c = new PdfPCell(t.DefaultCell);
            c.AddElement(GetAttendance(p));
            c.Colspan = 3;
            t2.AddCell(c);
            c = new PdfPCell(t.DefaultCell);
            c.Padding = 0;
            c.AddElement(t2);
            t.AddCell(c);

            var t3 = new PdfPTable(w3);
            t3.WidthPercentage = 100;
            t3.DefaultCell.Border = border;
            t3.DefaultCell.Padding = 5;
            t3.DefaultCell.PaddingBottom = 0;
            t3.AddCell(new Phrase(p.DOB, font));
            t3.AddCell(new Phrase(p.MemberStatus.Description));
            var contacts = GetContacts(p);
            if (contacts.Items.Count > 0)
            {
                c = new PdfPCell(t.DefaultCell);
                c.Colspan = 2;
                c.AddElement(new Chunk("Contacts", boldfont));
                t3.AddCell(c);
                c = new PdfPCell(t.DefaultCell);
                c.Colspan = 2;
                c.AddElement(contacts);
                t3.AddCell(c);
            }
            c = new PdfPCell(t.DefaultCell);
            c.Padding = 0;
            t3.SplitLate = false;
            t3.SplitRows = true;
            c.AddElement(t3);
            return c;
        }

        private void AddLine(StringBuilder sb, string value)
        {
            AddLine(sb, value, string.Empty);
        }

        private void AddLine(StringBuilder sb, string value, string postfix)
        {
            if (value.HasValue())
            {
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                sb.Append(value);
                if (postfix.HasValue())
                {
                    sb.Append(postfix);
                }
            }
        }

        private void AddPhone(StringBuilder sb, string value, string prefix)
        {
            if (value.HasValue())
            {
                value = value.FmtFone(prefix);
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                sb.Append(value);
            }
        }

        private Paragraph GetAttendance(Person p)
        {
            var q = from a in p.Attends
                    where a.AttendanceFlag
                    orderby a.MeetingDate.Date descending
                    group a by a.MeetingDate.Date
                    into g
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
                    {
                        break;
                    }

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
            {
                attstr.Insert(0, $"{d:d}  ");
            }

            if (list.Count > 8)
            {
                attstr.Insert(0, "...  ");
                var q2 = q.OrderBy(d => d).Take(Math.Min(list.Count - 8, 3));
                foreach (var d in q2.OrderByDescending(d => d))
                {
                    attstr.Insert(0, $"{d:d}  ");
                }
            }
            ph.Add(new Chunk(attstr.ToString(), smallfont));
            return ph;
        }

        private List GetContacts(Person p)
        {
            var ctl = new CodeValueModel();
            var cts = ctl.ContactTypeCodes();

            var managecontacts = DbUtil.Db.CurrentUser.InRole("ManagePrivateContacts");
            var cq = from ce in DbUtil.Db.Contactees
                     where ce.PeopleId == p.PeopleId
                     where (ce.contact.LimitToRole ?? "") == "" || managecontacts
                     orderby ce.contact.ContactDate descending
                     select new
                     {
                         ce.contact,
                         madeby = ce.contact.contactsMakers.FirstOrDefault().person
                     };
            var list = new List(false, 10);
            list.ListSymbol = new Chunk("\u2022", font);
            var ep = p.EntryPoint != null ? p.EntryPoint.Description : "";
            var ip = p.InterestPoint != null ? p.InterestPoint.Description : "";
            if (ep.HasValue() || ip.HasValue())
            {
                list.Add(new ListItem(1.2f * font.Size, $"Entry, Interest: {ep}, {ip}", font));
            }

            foreach (var pc in cq.Take(10))
            {
                var cname = "unknown";
                if (pc.madeby != null)
                {
                    cname = pc.madeby.Name;
                }

                var ctype = cts.ItemValue(pc.contact.ContactTypeId);
                string comments = null;
                if (pc.contact.Comments.HasValue())
                {
                    comments = pc.contact.Comments.Replace("\r\n\r\n", "\r\n");
                    var lines = Regex.Split(comments, "\r\n");
                    comments = string.Join("\r\n", lines.Take(6));
                    if (lines.Length > 6)
                    {
                        comments += "... (see rest of comment online)";
                    }
                }
                string s = $"{pc.contact.ContactDate:d}: {ctype} by {cname}\n{comments}";
                list.Add(new ListItem(1.2f * font.Size, s, font));
            }
            if (cq.Count() > 10)
            {
                list.Add(new ListItem(1.2f * font.Size, $"(showing most recent 10 of {cq.Count()})", font));
            }

            return list;
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
