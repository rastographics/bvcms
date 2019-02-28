/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Search.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class RollsheetResult : ActionResult
    {
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA);
        private readonly Font medfont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly PageEvent pageEvents = new PageEvent();
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        private PdfPCell box;
        private Font china;
        private PdfContentByte dc;
        private Document doc;
        private bool hasRows;
        public int? MeetingId;
        public NewMeetingInfo NewMeetingInfo;
        public OrgSearchModel OrgSearchModel;
        public Guid? QueryId;
        private bool pageSetStarted;
        private Meeting meeting;
        private OrgFilter Filter => QueryId.HasValue ? DbUtil.Db.OrgFilters.Single(vv => vv.QueryId == QueryId) : null;

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            if (MeetingId.HasValue)
            {
                meeting = DbUtil.Db.Meetings.Single(mt => mt.MeetingId == MeetingId);
                Debug.Assert(meeting.MeetingDate != null, "meeting.MeetingDate != null");
                NewMeetingInfo = new NewMeetingInfo { MeetingDate = meeting.MeetingDate.Value };
            }

            var list1 = NewMeetingInfo.ByGroup ? ReportList2().ToList() : ReportList().ToList();

            if (!list1.Any())
            {
                Response.Write("no data found");
                return;
            }
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            doc = new Document(PageSize.LETTER.Rotate(), 36, 36, 64, 64);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;

            box = new PdfPCell();
            box.Border = Rectangle.NO_BORDER;
            box.CellEvent = new CellEvent();

            OrgInfo lasto = null;
            foreach (var o in list1)
            {
                lasto = o;
                var table = new PdfPTable(1);
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.DefaultCell.Padding = 0;
                table.WidthPercentage = 100;

                if (meeting != null)
                {
                    var q = from at in meeting.Attends
                            where at.AttendanceFlag || AttendCommitmentCode.committed.Contains(at.Commitment ?? 0)
                            orderby at.Person.LastName, at.Person.FamilyId, at.Person.Name2
                            select new
                            {
                                at.MemberType.Code,
                                Name2 = NewMeetingInfo.UseAltNames && at.Person.AltName.HasValue() ? at.Person.AltName : at.Person.Name2,
                                at.PeopleId,
                                at.Person.DOB
                            };
                    if (q.Any())
                    {
                        StartPageSet(o);
                    }

                    foreach (var a in q)
                    {
                        table.AddCell(AddRow(a.Code, a.Name2, a.PeopleId, a.DOB, "", font));
                    }
                }
                else if (OrgSearchModel != null)
                {
                    var q = from om in DbUtil.Db.OrganizationMembers
                            where om.OrganizationId == o.OrgId
                            join m in DbUtil.Db.OrgPeople(o.OrgId, o.Groups) on om.PeopleId equals m.PeopleId
                            where om.EnrollmentDate <= Util.Now
                            orderby om.Person.LastName, om.Person.FamilyId, om.Person.Name2
                            let p = om.Person
                            let ch = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select new
                            {
                                p.PeopleId,
                                Name2 = ch ? p.AltName : p.Name2,
                                BirthDate = Person.FormatBirthday(
                                    p.BirthYr,
                                    p.BirthMonth,
                                    p.BirthDay, p.PeopleId),
                                MemberTypeCode = om.MemberType.Code,
                                ch,
                                highlight =
                                    om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == NewMeetingInfo.HighlightGroup)
                                        ? NewMeetingInfo.HighlightGroup
                                        : ""
                            };
                    if (q.Any())
                    {
                        StartPageSet(o);
                    }

                    foreach (var m in q)
                    {
                        table.AddCell(AddRow(m.MemberTypeCode, m.Name2, m.PeopleId, m.BirthDate, m.highlight, m.ch ? china ?? font : font));
                    }
                }
                else if (Filter?.GroupSelect == GroupSelectCode.Member)
                {
                    var q = from om in DbUtil.Db.OrganizationMembers
                            where om.OrganizationId == Filter.Id
                            join m in DbUtil.Db.OrgFilterPeople(QueryId, null)
                                on om.PeopleId equals m.PeopleId
                            where om.EnrollmentDate <= Util.Now
                            where NewMeetingInfo.ByGroup == false || m.Groups.Contains((char)10 + o.Groups + (char)10)
                            orderby om.Person.LastName, om.Person.FamilyId, om.Person.Name2
                            let p = om.Person
                            let ch = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select new
                            {
                                p.PeopleId,
                                Name2 = ch ? p.AltName : p.Name2,
                                BirthDate = Person.FormatBirthday(
                                    p.BirthYr,
                                    p.BirthMonth,
                                    p.BirthDay,
                                    p.PeopleId),
                                MemberTypeCode = om.MemberType.Code,
                                ch,
                                highlight =
                                    om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == NewMeetingInfo.HighlightGroup)
                                        ? NewMeetingInfo.HighlightGroup
                                        : ""
                            };
                    if (q.Any())
                    {
                        StartPageSet(o);
                    }

                    foreach (var m in q)
                    {
                        table.AddCell(AddRow(m.MemberTypeCode, m.Name2, m.PeopleId, m.BirthDate, m.highlight, m.ch ? china ?? font : font));
                    }
                }
                else
                {
                    var q = from m in DbUtil.Db.OrgFilterPeople(QueryId, null)
                            orderby m.Name2
                            let p = DbUtil.Db.People.Single(pp => pp.PeopleId == m.PeopleId)
                            let om = p.OrganizationMembers.SingleOrDefault(mm => mm.OrganizationId == Filter.Id)
                            let ch = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select new
                            {
                                p.PeopleId,
                                Name2 = ch ? p.AltName : p.Name2,
                                BirthDate = Person.FormatBirthday(
                                    p.BirthYr,
                                    p.BirthMonth,
                                    p.BirthDay,
                                    p.PeopleId),
                                MemberTypeCode = om == null ? "Guest" : om.MemberType.Code,
                                ch,
                                highlight = om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == NewMeetingInfo.HighlightGroup) ? NewMeetingInfo.HighlightGroup : ""
                            };
                    if (q.Any())
                    {
                        StartPageSet(o);
                    }

                    foreach (var m in q)
                    {
                        table.AddCell(AddRow(m.MemberTypeCode, m.Name2, m.PeopleId, m.BirthDate, m.highlight, m.ch ? china ?? font : font));
                    }
                }
                if ((OrgSearchModel != null && NewMeetingInfo.ByGroup == false)
                    || (Filter != null
                        && Filter.GroupSelect == GroupSelectCode.Member
                        && meeting == null
                        && !Filter.SgFilter.HasValue()
                        && !Filter.NameFilter.HasValue()
                        && !Filter.FilterIndividuals == true
                        && !Filter.FilterTag == true
                        && NewMeetingInfo.ByGroup == false))
                {
                    foreach (var m in RollsheetModel.FetchVisitors(o.OrgId, NewMeetingInfo.MeetingDate, true, NewMeetingInfo.UseAltNames))
                    {
                        if (table.Rows.Count == 0)
                        {
                            StartPageSet(o);
                        }

                        table.AddCell(AddRow(m.VisitorType, m.Name2, m.PeopleId, m.BirthDate, "", boldfont));
                    }
                }
                if (!pageSetStarted)
                {
                    continue;
                }

                var col = 0;
                var gutter = 20f;
                var colwidth = (doc.Right - doc.Left - gutter) / 2;
                var ct = new ColumnText(w.DirectContent);
                ct.AddElement(table);

                var status = 0;

                while (ColumnText.HasMoreText(status))
                {
                    if (col == 0)
                    {
                        ct.SetSimpleColumn(doc.Left, doc.Bottom, doc.Left + colwidth, doc.Top);
                    }
                    else
                    {
                        ct.SetSimpleColumn(doc.Right - colwidth, doc.Bottom, doc.Right, doc.Top);
                    }

                    status = ct.Go();
                    ++col;
                    if (col > 1)
                    {
                        col = 0;
                        doc.NewPage();
                    }
                }
            }
            if (!hasRows)
            {
                if (!pageSetStarted)
                {
                    StartPageSet(lasto);
                }

                doc.Add(new Paragraph("no members as of this meeting date and time to show on rollsheet"));
            }
            doc.Close();
        }

        public Font CreateChineseFont()
        {
            var simsunfont = Util.SimSunFont;
            if (simsunfont == null)
            {
                return null;
            }

            if (simsunfont.StartsWith("~"))
            {
                simsunfont = HttpContextFactory.Current.Server.MapPath(Util.SimSunFont);
            }

            if (!File.Exists(simsunfont))
            {
                return null;
            }

            var baseFont = BaseFont.CreateFont(
                simsunfont,
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED);
            return new Font(baseFont);
        }

        private void StartPageSet(OrgInfo o)
        {
            doc.NewPage();
            pageSetStarted = true;
            if (NewMeetingInfo.UseAltNames)
            {
                china = CreateChineseFont();
            }

            pageEvents.StartPageSet(
                $"{o.Division}: {o.Name}, {o.Location} ({o.Teacher})",
                $"{NewMeetingInfo.MeetingDate:f} ({o.OrgId})",
                $"M.{o.OrgId}.{NewMeetingInfo.MeetingDate:MMddyyHHmm}");
        }

        private PdfPTable AddRow(string Code, string name, int pid, string dob, string highlight, Font font)
        {
            var t = new PdfPTable(4);
            //t.SplitRows = false;
            t.WidthPercentage = 100;
            t.SetWidths(new[] { 30, 4, 6, 30 });
            t.DefaultCell.Border = Rectangle.NO_BORDER;

            var bc = new Barcode39();
            bc.X = 1.2f;
            bc.Font = null;
            bc.Code = pid.ToString();
            var img = bc.CreateImageWithBarcode(dc, null, null);
            var c = new PdfPCell(img, false);
            c.PaddingTop = 3f;
            c.Border = Rectangle.NO_BORDER;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            t.AddCell(c);

            t.AddCell("");
            t.AddCell(box);

            DateTime bd;
            DateTime.TryParse(dob, out bd);

            var p = new Phrase(name, font);
            p.Add("\n");
            p.Add(new Chunk(" ", medfont));
            p.Add(new Chunk($"({Code}) {bd:MMM d}", smallfont));
            if (highlight.HasValue())
            {
                p.Add("\n" + highlight);
            }

            t.AddCell(p);
            hasRows = true;
            return t;
        }

        private IEnumerable<OrgInfo> ReportList()
        {
            var orgs = OrgSearchModel == null
                ? OrgSearchModel.FetchOrgs(meeting?.OrganizationId ?? Filter?.Id ?? 0)
                : OrgSearchModel.FetchOrgs();
            var q = from o in orgs
                    orderby o.Division, o.OrganizationName
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Division = o.Division,
                        Name = o.OrganizationName,
                        Teacher = o.LeaderName,
                        Location = o.Location,
                        Groups = NewMeetingInfo.GroupFilterPrefix
                    };
            return q;
        }

        private IEnumerable<OrgInfo> ReportList2()
        {
            var orgs = OrgSearchModel == null
                ? OrgSearchModel.FetchOrgs(Filter?.Id ?? 0)
                : OrgSearchModel.FetchOrgs();
            var q = from o in orgs
                    from sg in DbUtil.Db.MemberTags
                    where sg.OrgId == o.OrganizationId
                    where (NewMeetingInfo.GroupFilterPrefix ?? "") == "" || sg.Name.StartsWith(NewMeetingInfo.GroupFilterPrefix)
                    orderby sg.Name
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Division = o.OrganizationName,
                        Name = sg.Name,
                        Teacher = "",
                        Location = o.Location,
                        Groups = sg.Name
                    };
            return q;
        }

        public class PersonVisitorInfo
        {
            public int PeopleId { get; set; }
            public string Name2 { get; set; }
            public string BirthDate { get; set; }
            public DateTime? LastAttended { get; set; }
            public string NameParent1 { get; set; }
            public string NameParent2 { get; set; }
            public string VisitorType { get; set; }
        }

        public class MemberInfo
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Organization { get; set; }
            public string Location { get; set; }
            public string MemberType { get; set; }
        }

        private class OrgInfo
        {
            public int OrgId { get; set; }
            public string Division { get; set; }
            public string Name { get; set; }
            public string Teacher { get; set; }
            public string Location { get; set; }
            public string Groups { get; set; }
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
            private string Barcode;
            private PdfContentByte dc;
            private Document document;
            private BaseFont font;
            private string HeadText;
            private string HeadText2;
            private NPages npages;
            private int pg;
            private PdfWriter writer;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                this.writer = writer;
                this.document = document;
                base.OnOpenDocument(writer, document);
                font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                dc = writer.DirectContent;
                npages = new NPages(dc);
            }

            public void EndPageSet()
            {
                if (npages == null)
                {
                    return;
                }

                npages.template.BeginText();
                npages.template.SetFontAndSize(font, 8);
                npages.template.ShowText(npages.n.ToString());
                pg = 1;
                npages.template.EndText();
                npages = new NPages(dc);
            }

            public void StartPageSet(string header1, string header2, string barcode)
            {
                HeadText = header1;
                HeadText2 = header2;
                Barcode = barcode;
                npages.juststartednewset = true;
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                if (npages.juststartednewset)
                {
                    EndPageSet();
                }

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
                dc.BeginText();
                dc.SetFontAndSize(font, HeadFontSize);
                dc.SetTextMatrix(30, document.PageSize.Height - 30 - (HeadFontSize * 1.5f));
                dc.ShowText(HeadText2);
                dc.EndText();

                //---Barcode right
                var bc = new Barcode39();
                bc.Font = null;
                bc.Code = Barcode;
                bc.X = 1.2f;
                var img = bc.CreateImageWithBarcode(dc, null, null);
                var h = font.GetAscentPoint(text, HeadFontSize);
                img.SetAbsolutePosition(document.PageSize.Width - img.Width - 30, document.PageSize.Height - 30 - img.Height + h);
                dc.AddImage(img);

                //---Column 1
                text = "Page " + (pg) + " of ";
                len = font.GetWidthPoint(text, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(30, 30);
                dc.ShowText(text);
                dc.EndText();
                dc.AddTemplate(npages.template, 30 + len, 30);
                npages.n = pg++;

                //---Column 2
                text = "Attendance Rollsheet";
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

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                EndPageSet();
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

            private class NPages
            {
                public readonly PdfTemplate template;
                public bool juststartednewset;
                public int n;

                public NPages(PdfContentByte dc)
                {
                    template = dc.CreateTemplate(50, 50);
                }
            }
        }
    }
}
