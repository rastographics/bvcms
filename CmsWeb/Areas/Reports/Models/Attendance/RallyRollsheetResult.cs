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
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class RallyRollsheetResult : ActionResult
    {
        public RallyRollsheetResult() { }
        public class PersonVisitorInfo
        {
            public int PeopleId { get; set; }
            public string Name2 { get; set; }
            public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
            public int? BirthYear { get; set; }
            public int? BirthMon { get; set; }
            public int? BirthDay { get; set; }
            public DateTime? LastAttended { get; set; }
            public string NameParent1 { get; set; }
            public string NameParent2 { get; set; }
            public string VisitorType { get; set; }
        }

        public OrgSearchModel OrgSearchModel;
        public NewMeetingInfo NewMeetingInfo;
        public int? meetingid, orgid;

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            CmsData.Meeting meeting = null;
            if (meetingid.HasValue)
            {
                meeting = DbUtil.Db.Meetings.Single(mt => mt.MeetingId == meetingid);
                if (meeting != null && meeting.MeetingDate.HasValue)
                {
                    NewMeetingInfo.MeetingDate = meeting.MeetingDate.Value;
                    orgid = meeting.OrganizationId;
                }
                else
                {
                    Response.Write("no meeting found");
                    return;
                }
            }

            if (OrgSearchModel == null)
            {
                OrgSearchModel = new OrgSearchModel();
            }

            var list1 = NewMeetingInfo.ByGroup == true ? ReportList2() : ReportList();

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
            box.Border = PdfPCell.NO_BORDER;
            box.CellEvent = new CellEvent();

            foreach (var o in list1)
            {
                t = StartPageSet(o);

                if (meeting != null)
                {
                    var q = from at in meeting.Attends
                            where at.AttendanceFlag == true || AttendCommitmentCode.committed.Contains(at.Commitment ?? 0)
                            select at.Person;
                    q = from p in q
                        from fm in DbUtil.Db.People.Where(ff => ff.FamilyId == p.FamilyId)
                        where (fm.PositionInFamilyId == 10 && p.PositionInFamilyId != 10)
                        || (fm.PeopleId == p.PeopleId && p.PositionInFamilyId == 10)
                        select fm;
                    q = q.Distinct();
                    var q2 = from p in q
                             orderby p.Name2
                             select new
                             {
                                 p.Name2,
                                 p.PeopleId,
                                 p.BibleFellowshipClassId
                             };
                    AddFirstRow(font);
                    foreach (var a in q2)
                    {
                        AddRow(a.Name2, a.PeopleId, a.BibleFellowshipClassId, font);
                    }
                }
                else
                {
                    var Groups = NewMeetingInfo.ByGroup == true ? o.Groups : "";
                    var q = from om in DbUtil.Db.OrganizationMembers
                            where om.OrganizationId == o.OrgId
                            join m in DbUtil.Db.OrgPeople(o.OrgId, Groups) on om.PeopleId equals m.PeopleId
                            where om.EnrollmentDate <= Util.Now
                            orderby om.Person.LastName, om.Person.FamilyId, om.Person.Name2
                            let p = om.Person
                            let ch = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select om.Person;

                    q = from p in q
                        from fm in DbUtil.Db.People.Where(ff => ff.FamilyId == p.FamilyId)
                        where (fm.PositionInFamilyId == 10 && p.PositionInFamilyId != 10)
                        || (fm.PeopleId == p.PeopleId && p.PositionInFamilyId == 10)
                        select fm;
                    q = q.Distinct();
                    var q2 = from p in q
                             orderby p.Name2
                             select new
                             {
                                 p.Name2,
                                 p.PeopleId,
                                 p.BibleFellowshipClassId
                             };
                    AddFirstRow(font);
                    foreach (var a in q2)
                    {
                        AddRow(a.Name2, a.PeopleId, a.BibleFellowshipClassId, font);
                    }
                }

                doc.Add(t);
            }
            pageEvents.EndPageSet();
            Response.Flush();
            doc.Close();
        }
        private static readonly int[] VisitAttendTypes = new int[]
        {
            AttendTypeCode.VisitingMember,
            AttendTypeCode.RecentVisitor,
            AttendTypeCode.NewVisitor
        };

        public class MemberInfo
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Organization { get; set; }
            public string Location { get; set; }
            public string MemberType { get; set; }
        }

        private PdfPCell box;
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        private readonly Font medfont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private PageEvent pageEvents = new PageEvent();
        private PdfPTable t;
        private Document doc;
        private PdfContentByte dc;

        private PdfPTable StartPageSet(OrgInfo o)
        {
            t = new PdfPTable(5);
            t.WidthPercentage = 100;
            t.SetWidths(new int[] { 40, 2, 5, 20, 30 });
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            pageEvents.StartPageSet(
                $"{o.Division}: {o.Name}, {o.Location} ({o.Teacher})",
                $"{NewMeetingInfo.MeetingDate:f} ({o.OrgId})",
                $"M.{o.OrgId}.{NewMeetingInfo.MeetingDate:MMddyyHHmm}");
            return t;
        }
        private void AddFirstRow(Font font)
        {
            t.AddCell("");
            t.AddCell("");
            t.AddCell("");

            {
                var p = new Phrase("Parent", boldfont);
                var c = new PdfPCell(p);
                c.Border = PdfPCell.NO_BORDER;
                c.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                c.PaddingBottom = 3f;
                t.AddCell(c);
            }
            {
                var p = new Phrase("(OrgId, PeopleId)", boldfont);
                var c = new PdfPCell(p);
                c.Border = PdfPCell.NO_BORDER;
                c.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                c.PaddingBottom = 3f;
                t.AddCell(c);
            }
        }
        private void AddRow(string name, int pid, int? oid, Font font)
        {
            var bco = new Barcode39();
            bco.X = 1.2f;
            bco.Font = null;
            bco.Code = $"M.{oid}.{pid}";
            var img = bco.CreateImageWithBarcode(dc, null, null);
            var c = new PdfPCell(img, false);
            c.PaddingTop = 3f;
            c.Border = PdfPCell.NO_BORDER;
            c.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            t.AddCell(c);

            t.AddCell("");
            t.AddCell(box);

            t.AddCell(name);
            t.AddCell($"({oid?.ToString() ?? " N/A "}, {pid})");
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
        private List<OrgInfo> ReportList()
        {
            var orgs = orgid.HasValue
                ? OrgSearchModel.FetchOrgs(orgid.Value)
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
            return q.ToList();
        }
        private List<OrgInfo> ReportList2()
        {
            var orgs = orgid.HasValue
                ? OrgSearchModel.FetchOrgs(orgid.Value)
                : OrgSearchModel.FetchOrgs();
            var q = from o in orgs
                    from sg in DbUtil.Db.MemberTags
                    where sg.OrgId == o.OrganizationId
                    where (NewMeetingInfo.GroupFilterPrefix ?? "") == "" || sg.Name.StartsWith(NewMeetingInfo.GroupFilterPrefix)
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Division = o.OrganizationName,
                        Name = sg.Name,
                        Teacher = "",
                        Location = o.Location,
                        Groups = sg.Name
                    };
            return q.ToList();
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
            private string Barcode;

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
            public void StartPageSet(string header1, string header2, string barcode)
            {
                EndPageSet();
                document.NewPage();
                document.ResetPageCount();
                this.HeadText = header1;
                this.HeadText2 = header2;
                this.Barcode = barcode;
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
                dc.BeginText();
                dc.SetFontAndSize(font, HeadFontSize);
                dc.SetTextMatrix(30, document.PageSize.Height - 30 - (HeadFontSize * 1.5f));
                dc.ShowText(HeadText2);
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


