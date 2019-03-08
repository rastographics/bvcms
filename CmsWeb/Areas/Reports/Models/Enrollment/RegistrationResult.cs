/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class RegistrationResult : ActionResult
    {
        private const float FLOAT_t1SpacingAfter = 20f;
        private readonly PageEvent pageEvents = new PageEvent();
        private readonly Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private PdfContentByte dc;
        private Document doc;
        private DateTime dt;
        private readonly Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
        private int? oid;
        private Guid? qid;
        private readonly Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));
        private readonly Font xsmallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7, new GrayColor(50));

        public RegistrationResult(Guid? id, int? oid)
        {
            qid = id;
            this.oid = oid;
        }

        private static void SetDefaults(PdfPTable t)
        {
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = Rectangle.NO_BORDER;
            t.LockedWidth = false;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            dt = Util.Now;

            doc = new Document(PageSize.LETTER, 72, 72, 72, 72);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;

            if (qid != null) // print using a query
            {
                pageEvents.StartPageSet($"Registration Report: {dt:d}");
                var q2 = DbUtil.Db.PeopleQuery(qid.Value);
                if (!oid.HasValue)
                {
                    oid = DbUtil.Db.CurrentSessionOrgId;
                }

                var q = from p in q2
                        orderby p.Name2
                        select new
                        {
                            p,
                            h = p.Family.HeadOfHousehold,
                            s = p.Family.HeadOfHouseholdSpouse,
                            m = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == oid),
                            o = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == oid).Organization
                        };
                if (!q.Any())
                {
                    doc.Add(new Phrase("no data"));
                }
                else
                {
                    foreach (var i in q)
                    {
                        Settings setting = null;
                        if (i.o != null)
                        {
                            setting = DbUtil.Db.CreateRegistrationSettings(i.o.OrganizationId);
                        }

                        var t1 = new PdfPTable(1);
                        SetDefaults(t1);
                        t1.AddCell(i.p.Name);
                        t1.AddCell(i.p.PrimaryAddress);
                        if (i.p.PrimaryAddress2.HasValue())
                        {
                            t1.AddCell(i.p.PrimaryAddress2);
                        }

                        t1.AddCell(i.p.CityStateZip);
                        t1.AddCell(i.p.EmailAddress);
                        if (i.p.HomePhone.HasValue())
                        {
                            t1.AddCell(i.p.HomePhone.FmtFone("H"));
                        }

                        if (i.p.CellPhone.HasValue())
                        {
                            t1.AddCell(i.p.CellPhone.FmtFone("C"));
                        }

                        t1.SpacingAfter = FLOAT_t1SpacingAfter;
                        doc.Add(t1);

                        var t2 = new PdfPTable(new float[] { 35, 65 });
                        SetDefaults(t2);
                        if (i.h != null
                            && i.h.PeopleId != i.p.PeopleId
                            && i.h.PositionInFamilyId == PositionInFamily.PrimaryAdult)
                        {
                            t2.AddCell(i.h.Name);
                            if (i.h.CellPhone.HasValue())
                            {
                                t2.AddCell(i.h.CellPhone.FmtFone("C"));
                            }
                            else if (i.h.HomePhone.HasValue())
                            {
                                t2.AddCell(i.h.HomePhone.FmtFone("H"));
                            }
                            else
                            {
                                t2.AddCell(" ");
                            }
                        }
                        if (i.s != null)
                        {
                            t2.AddCell(i.s.Name);
                            if (i.s.CellPhone.HasValue())
                            {
                                t2.AddCell(i.s.CellPhone.FmtFone("C"));
                            }
                            else if (i.s.HomePhone.HasValue())
                            {
                                t2.AddCell(i.s.HomePhone.FmtFone("H"));
                            }
                            else
                            {
                                t2.AddCell(" ");
                            }
                        }
                        t2.AddCell(" ");
                        t2.AddCell(" ");

                        var rr = GetRecRegOrTemp(i.p);

                        t2.AddCell("Date of Birth");
                        t2.AddCell(i.p.DOB);
                        if (i.o == null || SettingVisible(setting, "AskSize"))
                        {
                            t2.AddCell("Shirt Size:");
                            t2.AddCell(rr.ShirtSize);
                        }
                        t2.SpacingAfter = FLOAT_t1SpacingAfter;
                        doc.Add(t2);

                        if (rr.MedicalDescription.HasValue())
                        {
                            doc.Add(new Phrase("Allergies: " + rr.MedicalDescription));
                        }

                        if (i.o == null || SettingVisible(setting, "AskTylenolEtc"))
                        {
                            var t4 = new PdfPTable(new float[] { 20, 80 });
                            SetDefaults(t4);
                            t4.AddCell("Tylenol:");
                            t4.AddCell(rr.Tylenol == true ? "Yes" : "No");
                            t4.AddCell("Advil:");
                            t4.AddCell(rr.Advil == true ? "Yes" : "No");
                            t4.AddCell("Robitussin:");
                            t4.AddCell(rr.Robitussin == true ? "Yes" : "No");
                            t4.AddCell("Maalox:");
                            t4.AddCell(rr.Maalox == true ? "Yes" : "No");
                            t4.SpacingAfter = FLOAT_t1SpacingAfter;
                            doc.Add(t4);
                        }
                        var t5 = new PdfPTable(new float[] { 45, 55 });
                        SetDefaults(t5);

                        if (i.o == null || SettingVisible(setting, "AskRequest"))
                        {
                            var label = ((AskRequest)setting?.AskItem("AskRequest"))?.Label ?? "Request";
                            t5.AddCell(label);
                            t5.AddCell(i.m?.Request);
                        }

                        if (i.o == null || SettingVisible(setting, "AskEmContact"))
                        {
                            t5.AddCell("Emergency Contact:");
                            t5.AddCell(rr.Emcontact);
                            t5.AddCell("Emergency Phone:");
                            t5.AddCell(rr.Emphone.FmtFone());
                        }
                        if (i.o == null || SettingVisible(setting, "AskInsurance"))
                        {
                            t5.AddCell("Health Insurance Carrier:");
                            t5.AddCell(rr.Insurance);
                            t5.AddCell("Policy #:");
                            t5.AddCell(rr.Policy);
                        }
                        if (i.o == null || SettingVisible(setting, "AskDoctor"))
                        {
                            t5.AddCell("Family Physician Name:");
                            t5.AddCell(rr.Doctor);
                            t5.AddCell("Family Physician Phone:");
                            t5.AddCell(rr.Docphone.FmtFone());
                        }
                        if (i.o == null || SettingVisible(setting, "AskParents"))
                        {
                            t5.AddCell("Mother's Name:");
                            t5.AddCell(rr.Mname);
                            t5.AddCell("Father's Name:");
                            t5.AddCell(rr.Fname);
                        }
                        if (i.m?.OnlineRegData != null)
                        {
                            var qlist = from qu in DbUtil.Db.ViewOnlineRegQAs
                                        where qu.OrganizationId == i.m.OrganizationId
                                        where qu.Type == "question" || qu.Type == "text"
                                        where qu.PeopleId == i.m.PeopleId
                                        select qu;
                            foreach (var qu in qlist)
                            {
                                t5.AddCell(qu.Question);
                                t5.AddCell(qu.Answer);
                            }
                        }
                        if (i.m?.UserData != null)
                        {
                            var a = Regex.Split(i.m.UserData, @"\s*--Add comments above this line--\s*", RegexOptions.Multiline);
                            if (a.Length > 0)
                            {
                                t5.AddCell("Comments");
                                t5.AddCell(a[0]);
                            }
                        }
                        doc.Add(t5);
                        if (i.m != null)
                        {
                            var groups = string.Join(", ", i.m.OrgMemMemTags.Select(om => om.MemberTag.Name).ToArray());
                            doc.Add(new Paragraph("Groups: " + groups));
                        }
                        doc.Add(Chunk.NEXTPAGE);
                    }
                }
            }
            else
            {
                doc.Add(new Phrase("no data"));
            }

            pageEvents.EndPageSet();
            doc.Close();
        }

        private static bool SettingVisible(Settings setting, string name)
        {
            if (setting != null)
            {
                return setting.AskVisible(name);
            }

            return false;
        }

        private static RecReg GetRecRegOrTemp(Person p)
        {
            var rr = p.RecRegs.SingleOrDefault();
            if (rr == null)
            {
                rr = new RecReg();
            }

            return rr;
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
        }

        public static DataTable ExcelData(Guid? queryid, int? orgId)
        {
            if (queryid == null)
            {
                return null;
            }

            var peopleQuery = DbUtil.Db.PeopleQuery(queryid.Value);
            var results = (from p in peopleQuery
                           let rr = p.RecRegs.SingleOrDefault() ?? new RecReg()
                           let headOfHousehold = p.Family.HeadOfHousehold
                           let headOfHouseholdSpouse = p.Family.HeadOfHouseholdSpouse
                           orderby p.Name2
                           select new
                           {
                               Person = p,
                               RecReg = rr,
                               HeadOfHousehold = headOfHousehold,
                               HeadOfHouseholdSpouse = headOfHouseholdSpouse,
                               OrgMembers = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == orgId),
                               p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == orgId).Organization,
                           }).ToList();

            if (!results.Any())
            {
                return null;
            }

            var table = new DataTable();

            foreach (var x in results)
            {
                Settings setting = null;
                if (x.Organization != null)
                {
                    setting = DbUtil.Db.CreateRegistrationSettings(x.Organization.OrganizationId);
                }

                var row = table.NewRow();

                AddValue(table, row, "Name", x.Person.Name);
                AddValue(table, row, "PrimaryAddress", x.Person.PrimaryAddress);
                AddValue(table, row, "PrimaryAddress2", x.Person.PrimaryAddress2);
                AddValue(table, row, "CityStateZip", x.Person.CityStateZip);
                AddValue(table, row, "EmailAddress", x.Person.EmailAddress);

                if (x.Person.HomePhone.HasValue())
                {
                    AddValue(table, row, "HomePhone", x.Person.HomePhone.FmtFone("H"));
                }

                if (x.Person.CellPhone.HasValue())
                {
                    AddValue(table, row, "CellPhone", x.Person.CellPhone.FmtFone("C"));
                }

                AddValue(table, row, "DOB", x.Person.DOB);
                AddValue(table, row, "Grade", x.Person.Grade);
                AddValue(table, row, "RegGrade", x?.OrgMembers?.Grade);

                AddValue(table, row, "HeadOfHouseholdName", x.HeadOfHousehold?.Name);
                if (!string.IsNullOrEmpty(x.HeadOfHousehold?.CellPhone))
                {
                    AddValue(table, row, "HeadOfHouseholdCellPhone", x.HeadOfHousehold?.CellPhone.FmtFone("C"));
                }

                if (!string.IsNullOrEmpty(x.HeadOfHousehold?.HomePhone))
                {
                    AddValue(table, row, "HeadOfHouseholdHomePhone", x.HeadOfHousehold?.HomePhone.FmtFone("H"));
                }

                AddValue(table, row, "HeadOfHouseholdSpouseName", x.HeadOfHouseholdSpouse?.Name);
                if (!string.IsNullOrEmpty(x.HeadOfHouseholdSpouse?.CellPhone))
                {
                    AddValue(table, row, "HeadOfHouseholdSpouseCellPhone", x.HeadOfHouseholdSpouse?.CellPhone.FmtFone("C"));
                }

                if (!string.IsNullOrEmpty(x.HeadOfHouseholdSpouse?.HomePhone))
                {
                    AddValue(table, row, "HeadOfHouseholdSpouseHomePhone", x.HeadOfHouseholdSpouse?.HomePhone.FmtFone("H"));
                }

                if (x.Organization == null || SettingVisible(setting, "AskSize"))
                {
                    AddValue(table, row, "ShirtSize", x.RecReg.ShirtSize);
                }

                if (x.Organization == null || SettingVisible(setting, "AskRequest"))
                {
                    var label = ((AskRequest)setting?.AskItem("AskRequest"))?.Label ?? "Request";
                    AddValue(table, row, label, x.OrgMembers?.Request);
                }

                AddValue(table, row, "Allergies", x.RecReg.MedicalDescription);

                if (x.Organization == null || SettingVisible(setting, "AskTylenolEtc"))
                {
                    AddValue(table, row, "Tylenol", x.RecReg.Tylenol);
                    AddValue(table, row, "Advil", x.RecReg.Advil);
                    AddValue(table, row, "Robitussin", x.RecReg.Robitussin);
                    AddValue(table, row, "Maalox", x.RecReg.Maalox);
                }

                if (x.Organization == null || SettingVisible(setting, "AskEmContact"))
                {
                    AddValue(table, row, "Emcontact", x.RecReg.Emcontact);
                    AddValue(table, row, "Emphone", x.RecReg.Emphone.FmtFone());
                }

                if (x.Organization == null || SettingVisible(setting, "AskInsurance"))
                {
                    AddValue(table, row, "Insurance", x.RecReg.Insurance);
                    AddValue(table, row, "Policy", x.RecReg.Policy);
                }

                if (x.Organization == null || SettingVisible(setting, "AskDoctor"))
                {
                    AddValue(table, row, "Doctor", x.RecReg.Doctor);
                    AddValue(table, row, "Docphone", x.RecReg.Docphone.FmtFone());
                }

                if (x.Organization == null || SettingVisible(setting, "AskParents"))
                {
                    AddValue(table, row, "Mname", x.RecReg.Mname);
                    AddValue(table, row, "Fname", x.RecReg.Fname);
                }

                if (x.OrgMembers?.OnlineRegData != null)
                {
                    var qlist = from qu in DbUtil.Db.ViewOnlineRegQAs
                                where qu.OrganizationId == x.OrgMembers.OrganizationId
                                where qu.Type == "question" || qu.Type == "text"
                                where qu.PeopleId == x.OrgMembers.PeopleId
                                select qu;
                    var counter = 0;
                    foreach (var qu in qlist)
                    {
                        AddValue(table, row, $"Question{counter}", qu.Question);
                        AddValue(table, row, $"Answer{counter}", qu.Answer);
                        counter++;
                    }

                    if (x.OrgMembers?.UserData != null)
                    {
                        var a = Regex.Split(x.OrgMembers.UserData, @"\s*--Add comments above this line--\s*",
                            RegexOptions.Multiline);
                        if (a.Length > 0)
                        {
                            AddValue(table, row, "Comments", a[0]);
                        }
                    }

                    if (x.OrgMembers != null)
                    {
                        var groups = string.Join(", ", x.OrgMembers.OrgMemMemTags.Select(om => om.MemberTag.Name).ToArray());
                        AddValue(table, row, "Groups", groups);
                    }
                }

                table.Rows.Add(row);
            }
            return table;

        }
        private static void AddValue(DataTable table, DataRow row, string columnName, object value)
        {
            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(columnName);
            }

            row[columnName] = value;
        }
    }
}
