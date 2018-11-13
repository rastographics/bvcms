/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Public.Models;
using CmsWeb.Areas.Search.Models;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UtilityExtensions;
using Xceed.Words.NET;
using Util = UtilityExtensions.Util;

namespace CmsWeb.Areas.Reports.Models
{
    public class DocXRollsheetResult : ActionResult
    {
        public int? Meetingid;
        public NewMeetingInfo NewMeetingInfo;
        public OrgSearchModel OrgSearchModel;
        public Guid? QueryId;
        private EmailReplacements replacements;
        private DocX docx;
        private DocX curr;
        private Meeting meeting;
        private OrgFilter Filter => QueryId.HasValue ? DbUtil.Db.OrgFilters.Single(vv => vv.QueryId == QueryId) : null;

        public class RollsheetPersonInfo
        {
            public Person Person { get; set; }
            public string MemberTypeCode { get; set; }
            public bool UseAltName { get; set; }
            public string Highlight { get; set; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            if (Meetingid.HasValue)
            {
                meeting = DbUtil.Db.Meetings.Single(mt => mt.MeetingId == Meetingid);
                Debug.Assert(meeting.MeetingDate != null, "meeting.MeetingDate != null");
                NewMeetingInfo = new NewMeetingInfo { MeetingDate = meeting.MeetingDate.Value };
            }
            var list1 = NewMeetingInfo.ByGroup ? ReportList2().ToList() : ReportList().ToList();

            if (!list1.Any())
            {
                response.Write("no data found");
                return;
            }

            var bytes = RollsheetTemplate() ?? Resource1.DocxRollsheet;
            var ms = new MemoryStream(bytes);
            docx = DocX.Load(ms);
            replacements = new EmailReplacements(DbUtil.Db, docx);
            var sources = new List<Source>();

            foreach (var o in list1)
            {
                curr = docx.Copy();

                foreach (var p in curr.Headers.Odd.Paragraphs)
                {
                    DoHeaderFooterParagraphReplacments(p, o);
                }

                foreach (var p in curr.Footers.Odd.Paragraphs)
                {
                    DoHeaderFooterParagraphReplacments(p, o);
                }

                var tbl = curr.Tables[0];
                var emptyrow = tbl.InsertRow();
                tbl.Rows.Add(emptyrow);
                tbl.RemoveRow(0);

                if (meeting != null)
                {
                    var q = from at in meeting.Attends
                            where at.AttendanceFlag || AttendCommitmentCode.committed.Contains(at.Commitment ?? 0)
                            orderby at.Person.LastName, at.Person.FamilyId, at.Person.Name2
                            select new RollsheetPersonInfo()
                            {
                                Person = at.Person,
                                MemberTypeCode = at.MemberType.Code,
                            };
                    foreach (var m in q)
                    {
                        AddRowWithReplacements(tbl, m, meeting.OrganizationId);
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
                            let useAltName = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select new RollsheetPersonInfo()
                            {
                                Person = p,
                                MemberTypeCode = om.MemberType.Code,
                                UseAltName = useAltName,
                                Highlight = om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == NewMeetingInfo.HighlightGroup)
                                        ? NewMeetingInfo.HighlightGroup
                                        : ""
                            };
                    foreach (var m in q)
                    {
                        AddRowWithReplacements(tbl, m, o.OrgId);
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
                            let useAltName = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select new RollsheetPersonInfo()
                            {
                                Person = p,
                                MemberTypeCode = om.MemberType.Code,
                                UseAltName = useAltName,
                                Highlight =
                                    om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == NewMeetingInfo.HighlightGroup)
                                        ? NewMeetingInfo.HighlightGroup
                                        : ""
                            };
                    foreach (var m in q)
                    {
                        AddRowWithReplacements(tbl, m, o.OrgId);
                    }
                }
                else
                {
                    var q = from m in DbUtil.Db.OrgFilterPeople(QueryId, null)
                            orderby m.Name2
                            let p = DbUtil.Db.People.Single(pp => pp.PeopleId == m.PeopleId)
                            let om = p.OrganizationMembers.SingleOrDefault(mm => mm.OrganizationId == Filter.Id)
                            let useAltName = NewMeetingInfo.UseAltNames && p.AltName != null && p.AltName.Length > 0
                            select new RollsheetPersonInfo
                            {
                                Person = p,
                                MemberTypeCode = om == null ? "Guest" : om.MemberType.Code,
                                UseAltName = useAltName,
                                Highlight = om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == NewMeetingInfo.HighlightGroup)
                                    ? NewMeetingInfo.HighlightGroup
                                    : ""
                            };

                    foreach (var m in q)
                    {
                        AddRowWithReplacements(tbl, m, o.OrgId);
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
                    var q = from vp in DbUtil.Db.OrgVisitorsAsOfDate(o.OrgId, NewMeetingInfo.MeetingDate, NoCurrentMembers: true)
                            let p = DbUtil.Db.People.Single(pp => pp.PeopleId == vp.PeopleId)
                            orderby p.LastName, p.FamilyId, p.PreferredName
                            select new RollsheetPersonInfo { Person = p, MemberTypeCode = vp.VisitorType };
                    foreach (var m in q)
                    {
                        AddRowWithReplacements(tbl, m, o.OrgId);
                    }
                }
                curr.Tables[0].RemoveRow(0);
                {
                    var memStream = new MemoryStream();
                    curr.SaveAs(memStream);
                    memStream.Position = 0;
                    var wmlDocument = new WmlDocument(null, memStream);
                    sources.Add(new Source(wmlDocument, keepSections: true));
                }
            }

            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            context.HttpContext.Response.AddHeader("Content-Disposition",
                $"attachment;filename=rollsheet.docx");
            var finaldoc = DocumentBuilder.BuildDocument(sources);
            context.HttpContext.Response.OutputStream.Write(finaldoc.DocumentByteArray, 0, finaldoc.DocumentByteArray.Length);
        }

        private void AddRowWithReplacements(Xceed.Words.NET.Table tbl, RollsheetPersonInfo m, int orgId)
        {
            var row = tbl.InsertRow(docx.Tables[0].Rows[0]);
            tbl.Rows.Add(row);
            var dict = replacements.DocXReplacementsDictionary(m.Person, orgId);
            foreach (var p in row.Paragraphs.Where(vv => vv.Text.HasValue()))
            {
                if (dict.Keys.Any(vv => p.Text.Contains(vv)))
                {
                    foreach (var d in dict)
                    {
                        if (p.Text.Contains(d.Key))
                        {
                            if (d.Key == "{barcodepeopleid}")
                            {
                                var s = BarCodeStream(m.Person.PeopleId.ToString(), 40, showtext: false);
                                var img = curr.AddImage(s);
                                p.AppendPicture(img.CreatePicture());
                                p.ReplaceText(d.Key, "");
                            }
                            else if (d.Key.Equal("{MLG}"))
                            {
                                p.ReplaceText(d.Key, m.MemberTypeCode);
                            }
                            else if (d.Key.Equal("{highlight}"))
                            {
                                if (m.Highlight.HasValue())
                                {
                                    p.ReplaceText(d.Key, m.Highlight);
                                }
                                else
                                {
                                    p.Remove(false);
                                }
                            }
                            else if (d.Key.Equal("{altname}"))
                            {
                                p.ReplaceText(d.Key, m.UseAltName ? m.Person.AltName : "");
                            }
                            else if (d.Key.Equal("{name}"))
                            {
                                p.ReplaceText(d.Key, m.Person.Name2);
                                if (m.MemberTypeCode == "VS")
                                {
                                    row.Cells.Last().Shading = Color.FromArgb(226, 239, 217); // light green
                                }
                            }
                            else
                            {
                                p.ReplaceText(d.Key, d.Value);
                            }
                        }
                    }
                }
            }
        }

        private void DoHeaderFooterParagraphReplacments(Paragraph p, OrgInfo o)
        {
            var list = EmailReplacements.TextReplacementsList(p.Text);
            foreach (var code in list)
            {
                if (code.StartsWith("{datemeeting"))
                {
                    p.ReplaceText(code,
                        Util.PickFirst(EmailReplacements
                                .DateFormattedReplacement(NewMeetingInfo.MeetingDate, code)
                            , "____"));
                }
                else if (code == "{orgname}")
                {
                    p.ReplaceText(code, o.Name);
                }
                else if (code == "{today}")
                {
                    p.ReplaceText(code, DateTime.Today.ToShortDateString());
                }
                else if (code == "{orgid}")
                {
                    p.ReplaceText(code, o.OrgId.ToString());
                }
                else if (code == "{barcodemeeting}")
                {
                    var text = $"M.{o.OrgId}.{NewMeetingInfo.MeetingDate:MMddyyHHmm}";
                    var s = BarCodeStream(text, 50, showtext: false);
                    var img = curr.AddImage(s);
                    p.AppendPicture(img.CreatePicture());
                    p.ReplaceText(code, "");
                    p.Alignment = Alignment.right;
                }
            }
        }

        private static MemoryStream BarCodeStream(string text, int height, bool showtext = true)
        {
            var bc = new Code39BarCode()
            {
                ShowBarCodeText = showtext,
                BarCodePadding = 5,
                BarCodeText = text,
                Height = height,
                ImageFormat = ImageFormat.Png,
                Weight = Code39BarCode.BarCodeWeight.Small
            };
            var bytes = bc.Generate();
            var x = (Bitmap)new ImageConverter().ConvertFrom(bytes);
            var ms = new MemoryStream();
            x.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return ms;
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
            public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
            public int? BirthYear { get; set; }
            public int? BirthMon { get; set; }
            public int? BirthDay { get; set; }
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


        private static byte[] RollsheetTemplate()
        {
            var loc = DbUtil.Db.Setting("RollsheetTemplate", "");
            if (loc.HasValue())
            {
                var wc = new WebClient();
                return wc.DownloadData(loc);
            }
            return null;
        }
    }
}
