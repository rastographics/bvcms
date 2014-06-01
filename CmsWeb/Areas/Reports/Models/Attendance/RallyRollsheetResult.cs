/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using CmsWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CmsData;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData.Codes;

namespace CmsWeb.Areas.Reports.Models
{
	public class RallyRollsheetResult : ActionResult
	{
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

        public OrgSearchModel Model;
		public int? meetingid, orgid;
		public int[] groups;
		public bool? bygroup;
		public bool? altnames;
		public string sgprefix, highlightsg;
		public DateTime? dt;

		public override void ExecuteResult(ControllerContext context)
		{
			var Response = context.HttpContext.Response;

			CmsData.Meeting meeting = null;
			if (meetingid.HasValue)
			{
				meeting = DbUtil.Db.Meetings.Single(mt => mt.MeetingId == meetingid);
				dt = meeting.MeetingDate;
				orgid = meeting.OrganizationId;
			}

		    var list1 = bygroup == true ? ReportList2() : ReportList();

			if (!list1.Any())
			{
				Response.Write("no data found");
				return;
			}
			if (!dt.HasValue)
			{
				Response.Write("bad date");
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
							where at.AttendanceFlag == true || at.Commitment == AttendCommitmentCode.Attending
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
						AddRow(a.Name2, a.PeopleId, a.BibleFellowshipClassId, font);
				}
				else
				{
					var Groups = o.Groups;
					if (Groups == null)
						Groups = new int[] { 0 };
					var q = from om in DbUtil.Db.OrganizationMembers
							where om.OrganizationId == o.OrgId
							let gc = om.OrgMemMemTags.Count(mt => Groups.Contains(mt.MemberTagId))
							where gc == Groups.Length || Groups[0] <= 0
							where !Groups.Contains(-1) || (Groups.Contains(-1) && om.OrgMemMemTags.Count() == 0)
							where (om.Pending ?? false) == false
							where om.MemberTypeId != MemberTypeCode.InActive
							where om.EnrollmentDate <= Util.Now
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
						AddRow(a.Name2, a.PeopleId, a.BibleFellowshipClassId, font);
				}

				doc.Add(t);
			}
			pageEvents.EndPageSet();
			Response.Flush();
			doc.Close();
		}
		private static int[] VisitAttendTypes = new int[] 
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
		private Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD);
		private Font font = FontFactory.GetFont(FontFactory.HELVETICA);
		private Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
		private Font medfont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
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
									"{0}: {1}, {2} ({3})".Fmt(o.Division, o.Name, o.Location, o.Teacher),
									"{0:f} ({1})".Fmt(dt, o.OrgId),
									"M.{0}.{1:MMddyyHHmm}".Fmt(o.OrgId, dt));
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
			bco.Code = "M.{0}.{1}".Fmt(oid,pid);
			var img = bco.CreateImageWithBarcode(dc, null, null);
			var c = new PdfPCell(img, false);
			c.PaddingTop = 3f;
			c.Border = PdfPCell.NO_BORDER;
			c.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
			t.AddCell(c);

			t.AddCell("");
			t.AddCell(box);

			t.AddCell(name);
			t.AddCell("({0}, {1})".Fmt(
				oid.HasValue ? oid.ToString() : " N/A ", pid));
		}
		private class OrgInfo
		{
			public int OrgId { get; set; }
			public string Division { get; set; }
			public string Name { get; set; }
			public string Teacher { get; set; }
			public string Location { get; set; }
			public int[] Groups { get; set; }
		}
		private IEnumerable<OrgInfo> ReportList()
		{
            var roles = DbUtil.Db.CurrentRoles();
            var q = from o in Model.FetchOrgs()
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where o.OrganizationId == orgid || (orgid ?? 0) == 0
                    orderby o.Division.Name, o.OrganizationName
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Division = o.Division.Name,
                        Name = o.OrganizationName,
                        Teacher = o.LeaderName,
                        Location = o.Location,
                        Groups = groups
                    };
            return q;
		}
		private IEnumerable<OrgInfo> ReportList2()
		{
            var roles = DbUtil.Db.CurrentRoles();
            var q = from o in Model.FetchOrgs()
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    from sg in o.MemberTags
                    where (sgprefix ?? "") == "" || sg.Name.StartsWith(sgprefix)
                    where o.OrganizationId == orgid || (orgid ?? 0) == 0
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Division = o.OrganizationName,
                        Name = sg.Name,
                        Teacher = "",
                        Location = o.Location,
                        Groups = new int[] { sg.Id }
                    };
            return q;
		}
		class CellEvent : IPdfPCellEvent
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
		class PageEvent : PdfPageEventHelper
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
					return;
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


