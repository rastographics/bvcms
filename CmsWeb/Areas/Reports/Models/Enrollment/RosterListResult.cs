/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Search.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    internal class MemberInfo
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string CityStateZip { get; set; }
        public string Parents { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string MemberStatus { get; set; }
        public bool ThisChurch { get; set; }
        public bool ActiveOther { get; set; }
        public bool FamMemberThis { get; set; }
        public string MemberType { get; set; }
        public string Medical { get; set; }
    }
    public class RosterListResult : ActionResult
    {
        public Guid? qid;
        public int? orgid;
        private OrgSearchModel model;

        public RosterListResult()
        {

        }
        public RosterListResult(OrgSearchModel m)
        {
            model = m;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            doc = new Document(PageSize.LETTER, 36, 36, 36, 36);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;

            if (qid != null)
            {
                var color = BaseColor.BLACK;
                var o = ReportList().First();
                var t = StartPageSet(o);
                var q = DbUtil.Db.PeopleQuery(qid.Value);
                var q2 = from p in q
                         let rr = p.RecRegs.FirstOrDefault()
                         join m in RollsheetModel.FetchOrgMembers(o.OrgId, null) on p.PeopleId equals m.PeopleId into j
                         from m in j.DefaultIfEmpty()
                         orderby p.Name2
                         select new MemberInfo
                         {
                             ActiveOther = rr.ActiveInAnotherChurch ?? false,
                             ThisChurch = rr.Member ?? false,
                             Address = p.PrimaryAddress,
                             Address2 = p.PrimaryAddress2,
                             CityStateZip = p.CityStateZip5,
                             CellPhone = p.CellPhone,
                             HomePhone = p.HomePhone,
                             MemberStatus = p.MemberStatus.Description,
                             MemberType = m.MemberType,
                             FamMemberThis = p.Family.People.Any(f => f.PositionInFamilyId == 10 && f.MemberStatusId == MemberStatusCode.Member),
                             Name = p.Name,
                             Medical = rr.MedicalDescription,
                             PeopleId = p.PeopleId,
                             Parents = DbUtil.Db.ParentNamesAndCells(p.PeopleId),
                             Age = p.Age
                         };
                foreach (var m in q2)
                {
                    if (color.Equals(BaseColor.WHITE))
                    {
                        color = new GrayColor(240);
                    }
                    else
                    {
                        color = BaseColor.WHITE;
                    }

                    AddRow(t, m, color);
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
                    var t = StartPageSet(o);

                    var color = BaseColor.BLACK;

                    var q = from om in DbUtil.Db.OrganizationMembers
                            where om.OrganizationId == o.OrgId
                            where (om.Pending ?? false) == false
                            where om.MemberTypeId != MemberTypeCode.InActive
                            let rr = om.Person.RecRegs.FirstOrDefault()
                            orderby om.Person.Name2
                            select new MemberInfo
                            {
                                ActiveOther = rr.ActiveInAnotherChurch ?? false,
                                ThisChurch = rr.Member ?? false,
                                Address = om.Person.PrimaryAddress,
                                Address2 = om.Person.PrimaryAddress2,
                                CityStateZip = om.Person.CityStateZip5,
                                CellPhone = om.Person.CellPhone,
                                HomePhone = om.Person.HomePhone,
                                MemberStatus = om.Person.MemberStatus.Description,
                                MemberType = om.MemberType.Description,
                                FamMemberThis = om.Person.Family.People.Any(f => f.PositionInFamilyId == 10 && f.MemberStatusId == MemberStatusCode.Member),
                                Name = om.Person.Name,
                                Medical = rr.MedicalDescription,
                                PeopleId = om.PeopleId,
                                Parents = DbUtil.Db.ParentNamesAndCells(om.PeopleId),
                                Age = om.Person.Age
                            };

                    foreach (var m in q)
                    {
                        if (color == BaseColor.WHITE)
                        {
                            color = new GrayColor(240);
                        }
                        else
                        {
                            color = BaseColor.WHITE;
                        }

                        AddRow(t, m, color);
                    }
                    doc.Add(t);
                }
            }

            pageEvents.EndPageSet();
            doc.Close();
        }

        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        private PageEvent pageEvents = new PageEvent();
        private Document doc;
        private PdfContentByte dc;
        private readonly float[] HeaderWids = { 35, 43, 53, 22, 30 };

        private PdfPTable StartPageSet(OrgInfo o)
        {
            var t = new PdfPTable(HeaderWids);
            t.HeaderRows = 1;
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            t.WidthPercentage = 100;
            t.DefaultCell.Padding = 5;
            pageEvents.StartPageSet($"{o.Division}: {o.Name}, {o.Location} ({o.Teacher})");

            t.AddCell(new Phrase("\nName", boldfont));
            t.AddCell(new Phrase("\nContact Info", boldfont));
            t.AddCell(new Phrase("\nChurch", boldfont));
            t.AddCell(new Phrase("Member\nType", boldfont));
            t.AddCell(new Phrase("\nAllergies", boldfont));
            return t;
        }

        private void AddRow(PdfPTable t, MemberInfo p, BaseColor color)
        {
            t.DefaultCell.BackgroundColor = color;

            var ph = new Phrase();
            ph.Add(new Chunk(p.Name, font));
            ph.Add(new Chunk($"\n  ({p.PeopleId})", smallfont));
            t.AddCell(ph);

            var sb = new StringBuilder();
            AddLine(sb, p.Address);
            AddLine(sb, p.Address2);
            AddLine(sb, p.CityStateZip);
            AddPhone(sb, p.HomePhone, "h ");
            if ((p.Age ?? 0) <= 18 && p.Parents != null)
            {
                var a = p.Parents.Replace(", c ", "|c ").Split('|');
                foreach (var li in a)
                {
                    AddLine(sb, li);
                }
            }
            else
            {
                AddPhone(sb, p.CellPhone, "c ");
            }

            t.AddCell(new Phrase(sb.ToString(), font));

            sb = new StringBuilder();
            AddLine(sb, p.MemberStatus);
            AddLine(sb, p.FamMemberThis ? "Family member is member here" : "");
            AddLine(sb, p.ActiveOther ? "Active in another church" : "Not Active in another church");
            t.AddCell(new Phrase(sb.ToString(), font));
            t.AddCell(new Phrase(p.MemberType, font));
            t.AddCell(new Phrase(p.Medical, font));
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
            var orgs = orgid == null
                ? model.FetchOrgs()
                : OrgSearchModel.FetchOrgs(orgid.Value);
            var q = from o in orgs
                    orderby o.OrganizationName
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


