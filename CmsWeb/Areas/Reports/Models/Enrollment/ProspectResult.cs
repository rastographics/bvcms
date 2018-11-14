/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class ProspectResult : ActionResult
    {
        private PageEvent pageEvents = new PageEvent();
        private Document doc;
        private DateTime dt;
        private PdfContentByte dc;
        private ColumnText ct;
        private readonly Font bfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        private readonly bool ShowForm;
        private readonly bool AlphaSort;

        private Guid qid;
        public ProspectResult(Guid id, bool ShowForm, bool Alpha)
        {
            qid = id;
            this.ShowForm = ShowForm;
            this.AlphaSort = Alpha;
        }

        public class ProspectInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string CityStateZip { get; set; }
            public string Age { get; set; }
            public string CellPhone { get; set; }
            public string HomePhone { get; set; }
            public string WorkPhone { get; set; }
            public string EMail { get; set; }
            public DateTime? Joined { get; set; }

            public string MemberStatus { get; set; }
            public int MemberStatusID { get; set; }
            public string Gender { get; set; }
            public string MaritalStatus { get; set; }
            public string PositionInFamily { get; set; }
            public string Origin { get; set; }
            public string Comment { get; set; }
            public string ChristAsSavior { get; set; }
            public string InterestedInJoining { get; set; }
            public string InfoBecomeAChristian { get; set; }
            public string PleaseVisit { get; set; }
            public IEnumerable<FamilyMember> Family { get; set; }
            public IEnumerable<OrganizationView> Memberships { get; set; }
            public IEnumerable<ContactInfo> Contacts { get; set; }
            public IEnumerable<CommentInfo> Comments { get; set; }
            public IEnumerable<CommentInfo> FamilyComments { get; set; }
            public IEnumerable<AttendInfo> Attends { get; set; }
        }
        public class FamilyMember
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? Age { get; set; }
            public bool Deceased { get; set; }
            public string PositionInFamily { get; set; }
            public int PositionInFamilyId { get; set; }
            public string MemberStatus { get; set; }
            public int PeopleId { get; set; }
            public string CellPhone { get; set; }
        }
        public class OrganizationView
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Location { get; set; }
            public string LeaderName { get; set; }
            public DateTime? MeetingTime { get; set; }
            public string Schedule => $"{MeetingTime:dddd h:mm tt}";
            public int? LeaderId { get; set; }
            public string MemberType { get; set; }
            public DateTime? EnrollDate { get; set; }
            public DateTime? DropDate { get; set; }
            public string DivisionName { get; set; }
            public decimal? AttendPct { get; set; }
        }
        public class ContactInfo
        {
            public int ContactId { get; set; }
            public string Comments { get; set; }
            public DateTime ContactDate { get; set; }
            public string TypeOfContact { get; set; }
            public string ContactReason { get; set; }
            public string Team { get; set; }
        }
        public class CommentInfo
        {
            public string Comments { get; set; }
            public string CommentField { get; set; }
        }
        public class AttendInfo
        {
            public int MeetingId { get; set; }
            public DateTime? MeetingDate { get; set; }
            public string OrganizationName { get; set; }
            public string MeetingName { get; set; }
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string MemberType { get; set; }
            public string AttendType { get; set; }
            public bool AttendFlag { get; set; }
            public bool RegisteredFlag { get; set; }
            public int RollSheetSectionId { get; set; }
            public string Teacher { get; set; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            dt = Util.Now;

            doc = new Document(PageSize.LETTER, 36, 36, 36, 36);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;
            ct = new ColumnText(dc);

            pageEvents.StartPageSet($"Outreach/Inreach Report: {dt:d}");
            if (qid == null)
            {
                doc.Add(new Phrase("no data"));
            }
            else
            {
                IQueryable<ProspectInfo> q = GetProspectInfo(AlphaSort);
                if (!q.Any())
                {
                    doc.Add(new Phrase("no data"));
                }
                else
                {
                    foreach (var p in q)
                    {
                        doc.NewPage();
                        var t = new PdfPTable(new[] { 62f, 61f, 67f });
                        t.SetNoPadding();
                        var t1 = new PdfPTable(1);
                        t1.SetNoBorder();
                        var t2 = new PdfPTable(new[] { 30f, 31f });
                        t2.SetNoBorder();
                        var t3 = new PdfPTable(new[] { 27f, 40f });
                        t3.SetNoBorder();


                        var ph = new Paragraph();
                        ph.Add(new Chunk(p.Name, bfont));
                        ph.Add(new Chunk($" ({p.PeopleId})", smallfont));
                        t1.AddCell(ph);
                        ph = new Paragraph();
                        ph.AddLine(p.Address, font);
                        ph.AddLine(p.Address2, font);
                        ph.AddLine(p.CityStateZip, font);
                        ph.Add("\n");
                        ph.AddLine(p.HomePhone.FmtFone("H"), font);
                        ph.AddLine(p.CellPhone.FmtFone("C"), font);
                        ph.AddLine(p.EMail, font);
                        t1.AddCell(ph);
                        t.AddCell(t1);

                        t2.Add("Position in Family:", font);
                        t2.Add(p.PositionInFamily, font);
                        t2.Add("Gender:", font);
                        t2.Add(p.Gender, font);
                        t2.Add("Marital Status:", font);
                        t2.Add(p.MaritalStatus, font);
                        t2.Add("", font);
                        t2.CompleteRow();

                        if (p.ChristAsSavior.HasValue())
                        {
                            t2.Add(p.ChristAsSavior, 2, font);
                        }

                        if (p.InfoBecomeAChristian.HasValue())
                        {
                            t2.Add(p.InfoBecomeAChristian, 2, font);
                        }

                        if (p.InterestedInJoining.HasValue())
                        {
                            t2.Add(p.InterestedInJoining, 2, font);
                        }

                        if (p.PleaseVisit.HasValue())
                        {
                            t2.Add(p.PleaseVisit, 2, font);
                        }

                        t.AddCell(t2);

                        t3.Add("Member Status:", font);

                        if (p.MemberStatusID == MemberStatusCode.Member)
                        {
                            t3.Add(p.MemberStatus + " (" + (p.Joined?.ToString("d") ?? "Unknown") + ")", font);
                        }
                        else
                        {
                            t3.Add(p.MemberStatus, font);
                        }

                        t3.Add("Origin:", font);
                        t3.Add(p.Origin, font);
                        t3.Add("Age:", font);
                        t3.Add(Person.AgeDisplay(p.Age, p.PeopleId), font);
                        t3.Add("Comments:", 2, font);
                        t3.Add(p.Comment, 2, font);

                        t.AddCell(t3);
                        doc.Add(t);

                        if (p.Family.Any())
                        {
                            t = new PdfPTable(5);
                            t.SetNoBorder();
                            t.AddRow("Family Summary", bfont);
                            t.AddHeader("Name", bfont);
                            t.AddHeader("Age", bfont);
                            t.AddHeader("Cell Phone", bfont);
                            t.AddHeader("Position in Family", bfont);
                            t.AddHeader("Member Status", bfont);
                            foreach (var fm in p.Family)
                            {
                                t.Add(fm.Name, font);
                                t.Add(Person.AgeDisplay(fm.Age, fm.PeopleId).ToString(), font);
                                t.Add(fm.CellPhone.FmtFone(), font);
                                t.Add(fm.PositionInFamily, font);
                                t.Add(fm.MemberStatus, font);
                            }
                            doc.Add(t);
                        }
                        if (p.Comments.Any())
                        {
                            t = new PdfPTable(new[] { 31f, 134f });
                            t.SetNoBorder();
                            t.AddRow("Comments", bfont);
                            t.AddHeader("Field", bfont);
                            t.AddHeader("Comments", bfont);
                            foreach (var c in p.Comments)
                            {
                                t.Add(c.CommentField, bfont);
                                t.Add(c.Comments, font);
                            }
                            doc.Add(t);
                        }

                        if (p.Attends.Any())
                        {
                            t = new PdfPTable(new[] { 24f, 73f, 56f, 34f });
                            t.SetNoBorder();
                            t.AddRow("Attendance Summary", bfont);
                            t.AddHeader("Date", bfont);
                            t.AddHeader("Event", bfont);
                            t.AddHeader("Teacher", bfont);
                            t.AddHeader("Schedule", bfont);
                            foreach (var a in p.Attends)
                            {
                                t.Add(a.MeetingDate.FormatDate(), font);
                                t.Add(a.MeetingName, font);
                                t.Add(a.Teacher, font);
                                t.Add(a.MeetingDate.ToString2("t"), font);
                            }
                            doc.Add(t);
                        }

                        if (p.Contacts.Any())
                        {
                            t = new PdfPTable(new[] { 31f, 134f });
                            t.SetNoBorder();
                            t.AddRow("Contacts", font);
                            t.AddHeader("Date/Type/Team", font);
                            t.AddHeader("Comments", font);
                            foreach (var a in p.Contacts)
                            {
                                t.AddHeader($"{a.ContactDate:d}\n{a.TypeOfContact}\n{a.Team}", font);
                                t.AddHeader(a.Comments, font);
                            }
                            doc.Add(t);
                        }

                        if (p.Memberships.Any())
                        {
                            t = new PdfPTable(4);
                            t.SetNoBorder();
                            t.AddRow("Current Enrollment", bfont);
                            t.AddHeader("Division", bfont);
                            t.AddHeader("Organization", bfont);
                            t.AddHeader("Member Type", bfont);
                            t.AddHeader("Enroll Date", bfont);
                            foreach (var m in p.Memberships)
                            {
                                t.Add(m.DivisionName, font);
                                t.Add(m.Name, font);
                                t.Add(m.MemberType, font);
                                t.Add(m.EnrollDate.FormatDate(), font);
                            }
                            doc.Add(t);
                        }
                        if (p.FamilyComments.Any())
                        {
                            t = new PdfPTable(new[] { 31f, 134f });
                            t.SetNoBorder();
                            t.AddRow("Family Comments", bfont);
                            t.AddHeader("Field", bfont);
                            t.AddHeader("Comments", bfont);
                            foreach (var c in p.FamilyComments)
                            {
                                t.Add(c.CommentField, bfont);
                                t.Add(c.Comments, font);
                            }
                            doc.Add(t);
                        }
                        if (ShowForm)
                        {
                            ContactForm(p);
                        }
                    }
                }
            }
            pageEvents.EndPageSet();
            doc.Close();
        }

        private IQueryable<ProspectInfo> GetProspectInfo(bool Alpha = false)
        {
            //var Db = Db;
            var q = DbUtil.Db.PeopleQuery(qid);
            if (Alpha)
            {
                q = q.OrderBy(pp => pp.Name2);
            }
            else
            {
                q = q.OrderBy(pp => pp.PrimaryZip).ThenBy(pp => pp.Name2);
            }

            var EvCommentFields = DbUtil.Db.Setting("EvCommentFields", "").Split(',');
            var q2 = from p in q
                     select new ProspectInfo
                     {
                         PeopleId = p.PeopleId,
                         Name = p.Name,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                         MemberStatusID = p.MemberStatusId,
                         MemberStatus = p.MemberStatus.Description,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         Gender = p.GenderId == 1 ? "Male" : p.GenderId == 2 ? "Female" : "",
                         MaritalStatus = p.MaritalStatus.Description,
                         PositionInFamily = p.FamilyPosition.Description,
                         Origin = p.Origin.Description,
                         Comment = p.Comments,
                         ChristAsSavior = p.ChristAsSavior ? "Prayed to receive Christ as Savior" : "",
                         InterestedInJoining = p.InterestedInJoining ? "Interested in joining Church" : "",
                         PleaseVisit = p.PleaseVisit ? "Requests a visit" : "",
                         InfoBecomeAChristian = p.InfoBecomeAChristian ? "Interested in becoming a Christian" : "",
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         WorkPhone = p.WorkPhone,
                         EMail = p.EmailAddress,
                         Joined = p.JoinDate,

                         Family = from m in p.Family.People
                                  where m.DeceasedDate == null
                                  where m.PeopleId != p.PeopleId
                                  orderby m.PositionInFamilyId, m.Age descending
                                  select new FamilyMember
                                  {
                                      Id = m.PeopleId,
                                      Name = m.Name,
                                      Age = Person.AgeDisplay(m.Age, m.PeopleId),
                                      Deceased = m.DeceasedDate != null,
                                      PositionInFamily = m.FamilyPosition.Description,
                                      MemberStatus = m.MemberStatus.Description,
                                      CellPhone = m.CellPhone
                                  },
                         Memberships = from om in p.OrganizationMembers
                                       where dt > om.EnrollmentDate
                                       let o = om.Organization
                                       let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                                       let l = DbUtil.Db.People.SingleOrDefault(l => l.PeopleId == o.LeaderId)
                                       orderby om.Organization.OrganizationName
                                       select new OrganizationView
                                       {
                                           Id = o.OrganizationId,
                                           Name = o.OrganizationName,
                                           Location = o.Location,
                                           LeaderName = l.Name,
                                           MeetingTime = sc.MeetingTime,
                                           MemberType = om.MemberType.Description,
                                           EnrollDate = om.EnrollmentDate,
                                           DivisionName = o.Division.Name
                                       },
                         Contacts = from ch in p.contactsHad
                                    let c = ch.contact
                                    where (c.LimitToRole ?? "") == ""
                                    orderby c.ContactDate descending
                                    select new ContactInfo
                                    {
                                        ContactId = c.ContactId,
                                        Comments = c.Comments,
                                        ContactDate = c.ContactDate,
                                        ContactReason = c.ContactReason.Description,
                                        TypeOfContact = c.ContactType.Description,
                                        Team = string.Join(",", c.contactsMakers.Select(cm => cm.person.Name).ToArray())
                                    },
                         Comments = from ex in p.PeopleExtras
                                    where EvCommentFields.Contains(ex.Field)
                                    select new CommentInfo
                                    {
                                        CommentField = ex.Field,
                                        Comments = ex.Data,
                                    },
                         FamilyComments = from ex in p.Family.FamilyExtras
                                          where EvCommentFields.Contains(ex.Field)
                                          select new CommentInfo
                                          {
                                              CommentField = ex.Field,
                                              Comments = ex.Data,
                                          },
                         Attends = (from a in p.Attends
                                    where a.AttendanceFlag == true
                                    let o = a.Meeting.Organization
                                    orderby a.MeetingDate descending, o.OrganizationName
                                    select new AttendInfo
                                    {
                                        MeetingId = a.MeetingId,
                                        OrganizationName = o.OrganizationName,
                                        Teacher = o.LeaderName,
                                        AttendType = a.AttendType.Description,
                                        MeetingName = o.Division.Name + ": " + o.OrganizationName,
                                        MeetingDate = a.MeetingDate,
                                        MemberType = a.MemberType.Description,
                                    }).Take(10)
                     };
            return q2;
        }

        private const float cm2pts = 28.34f;
        private readonly Font h1font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
        private readonly Font h2font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


        public void ContactForm(ProspectInfo p)
        {
            /* There is no look up table for Contact Results because they are hardcoded into the form for actually adding a contact to a record
             * Another Setting is used to allow for customization of the blank Contact Form.  It is called "ContactFormResults"
             * The code takes a semi-colon seperated string stored in ContactFormResults and converts it to a list.
             * If ContactFormResults does not exist in Settings then the default list is used.
             *
             * However, because the results are hardcoded, there will be no translation between the results on this form and the results on the Contact page itself.
             * We will implement extra values for this table to accomodate this at some point.
             * */
            //var Db = Db;
            List<string> ContactResult =
                DbUtil.Db.Setting(
                "ContactFormResults",
                "Not at Home;Left Door Hanger;Left Message;Contact Made;Gospel Shared;Profession of Faith;Prayer Request Rec'd;Prayed for Person;Already Saved"
                ).Split(';').ToList();

            doc.NewPage();

            var t = new PdfPTable(2);

            t.SetNoBorder();
            t.AddCentered($"InReach/Outreach Card for {p.Name} ({p.PeopleId})", 2, h1font);
            t.AddCentered("Contact Summary", 2, h2font);
            /* Added line for noting the Ministry recording the contact*/
            t.AddLeft("Ministry: ______________________", 1, bfont);
            t.AddRight("Contact Date: _______________", 1, bfont);

            doc.Add(t);

            var ContactReason = DbUtil.Db.ContactReasons.Select(rr => rr.Description).Take(10).ToList();
            var ContactType = DbUtil.Db.ContactTypes.Select(rr => rr.Description).Take(10).ToList();

            DisplayTable("Contact Reason", 5.7f, 1.2f, 24.2f, ContactReason);
            DisplayTable("Type of Contact", 5.7f, 8f, 24.2f, ContactType);
            DisplayTable("Results", 5.7f, 14.5f, 24.2f, ContactResult);

            DisplayNotes("Team Members", 3, 6f, 13.7f, 6.5f);
            DisplayNotes("Specific Comments on Contact", 5, 18.5f, 1.2f, 13f);
            DisplayTable("Actions to be taken", 12f, 1f, 6.5f, new List<string>
            {
                "Recycle to me on ____/____/____",
                "Random Recycle",
                "Follow-up Completed",
                "Other Action:___________________________________",
            });

            var t2 = new PdfPTable(1);
            t2.TotalWidth = 7.5f * 72f;
            t2.SetNoBorder();
            t2.LockedWidth = true;
            t2.AddCentered("Internal Use Only", 1, smallfont);
            t2.WriteSelectedRows(0, -1, 36f, 56f, dc);
        }
        private void DisplayTable(string title, float width, float x, float y, List<string> reasons)
        {
            var t = new PdfPTable(new[] { 1.3f, width - 1.3f });
            t.TotalWidth = width * cm2pts;
            t.SetNoBorder();
            t.LockedWidth = true;
            t.DefaultCell.MinimumHeight = 1f * cm2pts;
            t.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            t.AddPlainRow(title, bfont);
            foreach (var r in reasons)
            {
                t.AddRight("_____", font);
                t.Add(r, font);
            }
            t.WriteSelectedRows(0, -1, x * cm2pts, y * cm2pts, dc);
        }
        private void DisplayNotes(string title, int nrows, float width, float x, float y)
        {
            var t = new PdfPTable(1);
            t.TotalWidth = width * cm2pts;
            t.LockedWidth = true;
            t.DefaultCell.MinimumHeight = 1f * cm2pts;
            t.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            t.AddPlainRow(title, bfont);
            for (int r = 0; r < nrows; r++)
            {
                t.AddCell("");
            }

            t.WriteSelectedRows(0, -1, x * cm2pts, y * cm2pts, dc);
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
                text = $"Page {writer.PageNumber + 1} of ";
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
        }
    }
}
