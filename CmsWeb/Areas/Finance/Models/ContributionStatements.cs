/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.API;
using CmsData.View;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class MyHandler : IElementHandler
    {
        public List<IElement> elements = new List<IElement>();

        public void Add(IWritable w)
        {
            if (w is WritableElement)
            {
                elements.AddRange(((WritableElement)w).Elements());
            }
        }
    }
    public class ContributionStatements
    {
        public int FamilyId { get; set; }
        public int PeopleId { get; set; }
        public int? SpouseId { get; set; }
        public int typ { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        private PageEvent pageEvents = new PageEvent();
        private string _fundDisplaySetting;

        public int LastSet()
        {
            if (pageEvents.FamilySet.Count == 0)
            {
                return 0;
            }

            var m = pageEvents.FamilySet.Max(kp => kp.Value);
            return m;
        }
        public List<int> Sets()
        {
            if (pageEvents.FamilySet.Count == 0)
            {
                return new List<int>();
            }

            var m = pageEvents.FamilySet.Values.Distinct().ToList();
            return m;
        }

        public class StatementSpecification
        {
            public string Description { get; set; }
            public string Header { get; set; }
            public string Notice { get; set; }
            public List<int> Funds { get; set; }

        }

        public void Run(Stream stream, CMSDataContext db, IEnumerable<ContributorInfo> q, StatementSpecification cs, int set = 0)
        {
            pageEvents.set = set;
            pageEvents.PeopleId = 0;
            var contributors = q;
            var toDate = ToDate.Date.AddHours(24).AddSeconds(-1);

            PdfContentByte dc;
            var font = FontFactory.GetFont(FontFactory.HELVETICA, 11);
            var boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

            var doc = new Document(PageSize.LETTER);
            doc.SetMargins(36f, 30f, 24f, 36f);
            var w = PdfWriter.GetInstance(doc, stream);
            w.PageEvent = pageEvents;
            doc.Open();
            dc = w.DirectContent;

            var prevfid = 0;
            var runningtotals = db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
            runningtotals.Processed = 0;
            db.SubmitChanges();
            var count = 0;
            foreach (var ci in contributors)
            {
                if (set > 0 && pageEvents.FamilySet[ci.PeopleId] != set)
                {
                    continue;
                }

                var contributions = APIContribution.Contributions(db, ci, FromDate, toDate, cs.Funds).ToList();
                var pledges = APIContribution.Pledges(db, ci, toDate, cs.Funds).ToList();
                var giftsinkind = APIContribution.GiftsInKind(db, ci, FromDate, toDate, cs.Funds).ToList();
                var nontaxitems = db.Setting("DisplayNonTaxOnStatement", "false").ToBool()
                    ? APIContribution.NonTaxItems(db, ci, FromDate, toDate, cs.Funds).ToList()
                    : new List<NonTaxContribution>();

                if ((contributions.Count + pledges.Count + giftsinkind.Count + nontaxitems.Count) == 0)
                {
                    runningtotals.Processed += 1;
                    runningtotals.CurrSet = set;
                    db.SubmitChanges();
                    if (set == 0)
                    {
                        pageEvents.FamilySet[ci.PeopleId] = 0;
                    }

                    continue;
                }

                pageEvents.NextPeopleId = ci.PeopleId;
                doc.NewPage();
                if (prevfid != ci.FamilyId)
                {
                    prevfid = ci.FamilyId;
                    pageEvents.EndPageSet();
                    pageEvents.PeopleId = ci.PeopleId;
                }
                if (set == 0)
                {
                    pageEvents.FamilySet[ci.PeopleId] = 0;
                }

                count++;

                var css = @"
<style>
h1 { font-size: 24px; font-weight:normal; margin-bottom:0; }
h2 { font-size: 11px; font-weight:normal; margin-top: 0; }
p { font-size: 11px; }
</style>
";
                //----Church Name

                var t1 = new PdfPTable(1)
                {
                    TotalWidth = 72f * 5f
                };
                t1.DefaultCell.Border = Rectangle.NO_BORDER;
                var html1 = cs.Header ?? db.ContentHtml("StatementHeader", Resource1.ContributionStatementHeader);
                var html2 = cs.Notice ?? db.ContentHtml("StatementNotice", Resource1.ContributionStatementNotice);

                var mh = new MyHandler();
                using (var sr = new StringReader(css + html1))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(mh, sr);
                }

                var cell = new PdfPCell(t1.DefaultCell);
                foreach (var e in mh.elements)
                {
                    if (e.Chunks.Count > 0)
                    {
                        cell.AddElement(e);
                    }
                }
                //cell.FixedHeight = 72f * 1.25f;
                t1.AddCell(cell);
                t1.AddCell("\n");

                var t1a = new PdfPTable(1) { TotalWidth = 72f * 5f };
                t1a.DefaultCell.Border = Rectangle.NO_BORDER;

                var ae = new PdfPTable(1);
                ae.DefaultCell.Border = Rectangle.NO_BORDER;
                ae.WidthPercentage = 100;

                var a = new PdfPTable(1);
                a.DefaultCell.Indent = 25f;
                a.DefaultCell.Border = Rectangle.NO_BORDER;
                a.AddCell(new Phrase(ci.Name, font));
                foreach (var line in ci.MailingAddress.SplitLines())
                {
                    a.AddCell(new Phrase(line, font));
                }

                cell = new PdfPCell(a) { Border = Rectangle.NO_BORDER };
                //cell.FixedHeight = 72f * 1.0625f;
                ae.AddCell(cell);

                cell = new PdfPCell(t1a.DefaultCell);
                cell.AddElement(ae);
                t1a.AddCell(ae);

                //-----Notice

                var t2 = new PdfPTable(1)
                {
                    TotalWidth = 72f * 3f
                };
                t2.DefaultCell.Border = Rectangle.NO_BORDER;

                var envno = "";
                if (db.Setting("PrintEnvelopeNumberOnStatement"))
                {
                    var ev = Person.GetExtraValue(db, ci.PeopleId, "EnvelopeNumber");
                    var s = Util.PickFirst(ev.Data, ev.IntValue.ToString(), ev.StrValue);
                    if (s.HasValue())
                    {
                        envno = $"env: {s}";
                    }
                    t2.AddCell(new Phrase($"{envno}", font));
                }
                if (!db.Setting("NoPrintDateOnStatement"))
                {
                    t2.AddCell(new Phrase($"\nprinted: {DateTime.Now:M/d/yy}", font));
                }

                t2.AddCell("");
                var mh2 = new MyHandler();
                using (var sr = new StringReader(css + html2))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(mh2, sr);
                }

                cell = new PdfPCell(t1.DefaultCell);
                foreach (var e in mh2.elements)
                {
                    if (e.Chunks.Count > 0)
                    {
                        cell.AddElement(e);
                    }
                }

                t2.AddCell(cell);

                // POSITIONING OF ADDRESSES
                //----Header

                var yp = doc.BottomMargin + db.Setting("StatementRetAddrPos", "10.125").ToFloat() * 72f;
                t1.WriteSelectedRows(0, -1, doc.LeftMargin - 0.1875f * 72f, yp, dc);

                yp = doc.BottomMargin + db.Setting("StatementAddrPos", "8.3375").ToFloat() * 72f;
                t1a.WriteSelectedRows(0, -1, doc.LeftMargin, yp, dc);

                yp = doc.BottomMargin + 10.125f * 72f;
                t2.WriteSelectedRows(0, -1, doc.LeftMargin + 72f * 4.4f, yp, dc);

                //----Contributions

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(" ") { SpacingBefore = 72f * 2.125f });

                doc.Add(new Phrase($"\n  Period: {FromDate:d} - {toDate:d}", boldfont));

                var pos = w.GetVerticalPosition(true);

                var ct = new ColumnText(dc);
                var gutter = 20f;
                var colwidth = (doc.Right - doc.Left - gutter) / 2;

                var t = new PdfPTable(new[] { 10f, 24f, 10f })
                {
                    WidthPercentage = 100
                };
                t.DefaultCell.Border = Rectangle.NO_BORDER;
                t.HeaderRows = 2;

                cell = new PdfPCell(t.DefaultCell)
                {
                    Colspan = 3,
                    Phrase = new Phrase("Contributions\n", boldfont)
                };
                t.AddCell(cell);

                t.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                t.AddCell(new Phrase("Date", boldfont));
                t.AddCell(new Phrase("Description", boldfont));
                cell = new PdfPCell(t.DefaultCell)
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Phrase = new Phrase("Amount", boldfont)
                };
                t.AddCell(cell);

                t.DefaultCell.Border = Rectangle.NO_BORDER;

                var total = 0m;
                foreach (var c in contributions)
                {
                    t.AddCell(new Phrase(c.ContributionDate.ToString2("d"), font));
                    t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription), font));
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase(c.ContributionAmount.ToString2("N2"), font)
                    };
                    t.AddCell(cell);
                    total += (c.ContributionAmount ?? 0);
                }
                t.DefaultCell.Border = Rectangle.TOP_BORDER;
                cell = new PdfPCell(t.DefaultCell)
                {
                    Colspan = 2,
                    Phrase = new Phrase("Total Contributions for period", boldfont)
                };
                t.AddCell(cell);
                cell = new PdfPCell(t.DefaultCell)
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Phrase = new Phrase(total.ToString("N2"), font)
                };
                t.AddCell(cell);

                ct.AddElement(t);

                //------Pledges

                if (pledges.Count > 0)
                {
                    t = new PdfPTable(new[] { 16f, 12f, 12f })
                    {
                        WidthPercentage = 100
                    };
                    t.DefaultCell.Border = Rectangle.NO_BORDER;
                    t.HeaderRows = 2;

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 3,
                        Phrase = new Phrase("\n\nPledges\n", boldfont)
                    };
                    t.AddCell(cell);

                    t.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                    t.AddCell(new Phrase("Fund", boldfont));
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase("Pledge", boldfont)
                    };
                    t.AddCell(cell);
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase("Given", boldfont)
                    };
                    t.AddCell(cell);

                    t.DefaultCell.Border = Rectangle.NO_BORDER;

                    foreach (var c in pledges)
                    {
                        t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription), font));
                        cell = new PdfPCell(t.DefaultCell)
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Phrase = new Phrase(c.Pledged.ToString2("N2"), font)
                        };
                        t.AddCell(cell);
                        cell = new PdfPCell(t.DefaultCell)
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Phrase = new Phrase(c.Given.ToString2("N2"), font)
                        };
                        t.AddCell(cell);
                    }
                    ct.AddElement(t);
                }

                //------Gifts In Kind

                if (giftsinkind.Count > 0)
                {
                    t = new PdfPTable(new[] { 12f, 18f, 20f })
                    {
                        WidthPercentage = 100
                    };
                    t.DefaultCell.Border = Rectangle.NO_BORDER;
                    t.HeaderRows = 2;

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 3,
                        Phrase = new Phrase("\n\nGifts in Kind\n", boldfont)
                    };
                    t.AddCell(cell);

                    t.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                    t.AddCell(new Phrase("Date", boldfont));
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Phrase = new Phrase("Fund", boldfont)
                    };
                    t.AddCell(cell);
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Phrase = new Phrase("Description", boldfont)
                    };
                    t.AddCell(cell);

                    t.DefaultCell.Border = Rectangle.NO_BORDER;

                    foreach (var c in giftsinkind)
                    {
                        t.AddCell(new Phrase(c.ContributionDate.ToString2("d"), font));
                        cell = new PdfPCell(t.DefaultCell)
                        {
                            Phrase = new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription), font)
                        };
                        t.AddCell(cell);
                        cell = new PdfPCell(t.DefaultCell)
                        {
                            Phrase = new Phrase(c.Description, font)
                        };
                        t.AddCell(cell);
                    }
                    ct.AddElement(t);
                }

                //-----Summary

                t = new PdfPTable(new[] { 29f, 9f })
                {
                    WidthPercentage = 100
                };
                t.DefaultCell.Border = Rectangle.NO_BORDER;
                t.HeaderRows = 2;

                cell = new PdfPCell(t.DefaultCell)
                {
                    Colspan = 2,
                    Phrase = new Phrase("\n\nPeriod Summary\n", boldfont)
                };
                t.AddCell(cell);

                t.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                t.AddCell(new Phrase("Fund", boldfont));
                cell = new PdfPCell(t.DefaultCell)
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Phrase = new Phrase("Amount", boldfont)
                };
                t.AddCell(cell);

                t.DefaultCell.Border = Rectangle.NO_BORDER;
                foreach (var c in APIContribution.GiftSummary(db, ci, FromDate, toDate, cs.Funds))
                {
                    t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription), font));
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase(c.Total.ToString2("N2"), font)
                    };
                    t.AddCell(cell);
                }
                t.DefaultCell.Border = Rectangle.TOP_BORDER;
                t.AddCell(new Phrase("Total contributions for period", boldfont));
                cell = new PdfPCell(t.DefaultCell)
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Phrase = new Phrase(total.ToString("N2"), font)
                };
                t.AddCell(cell);
                ct.AddElement(t);

                //------NonTax

                if (nontaxitems.Count > 0)
                {
                    t = new PdfPTable(new[] { 10f, 24f, 10f })
                    {
                        WidthPercentage = 100
                    };
                    t.DefaultCell.Border = Rectangle.NO_BORDER;
                    t.HeaderRows = 2;

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 3,
                        Phrase = new Phrase("\n\nNon Tax-Deductible Items\n", boldfont)
                    };
                    t.AddCell(cell);

                    t.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                    t.AddCell(new Phrase("Date", boldfont));
                    t.AddCell(new Phrase("Description", boldfont));
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase("Amount", boldfont)
                    };
                    t.AddCell(cell);

                    t.DefaultCell.Border = Rectangle.NO_BORDER;

                    var ntotal = 0m;
                    foreach (var c in nontaxitems)
                    {
                        t.AddCell(new Phrase(c.ContributionDate.ToString2("d"), font));
                        t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription), font));
                        cell = new PdfPCell(t.DefaultCell)
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Phrase = new Phrase(c.ContributionAmount.ToString2("N2"), font)
                        };
                        t.AddCell(cell);
                        ntotal += (c.ContributionAmount ?? 0);
                    }
                    t.DefaultCell.Border = Rectangle.TOP_BORDER;
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 2,
                        Phrase = new Phrase("Total Non Tax-Deductible Items for period", boldfont)
                    };
                    t.AddCell(cell);
                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase(ntotal.ToString("N2"), font)
                    };
                    t.AddCell(cell);

                    ct.AddElement(t);
                }

                var col = 0;
                var status = 0;
                while (ColumnText.HasMoreText(status))
                {
                    switch (col)
                    {
                        case 0:
                            ct.SetSimpleColumn(doc.Left, doc.Bottom, doc.Left + colwidth, pos);
                            break;
                        case 1:
                            ct.SetSimpleColumn(doc.Right - colwidth, doc.Bottom, doc.Right, pos);
                            break;
                    }

                    status = ct.Go();
                    ++col;
                    if (col <= 1)
                    {
                        continue;
                    }

                    col = 0;
                    pos = doc.Top;
                    doc.NewPage();
                }

                runningtotals.Processed += 1;
                runningtotals.CurrSet = set;
                db.SubmitChanges();
            }

            if (count == 0)
            {
                doc.NewPage();
                doc.Add(new Paragraph("no data"));
                var a = new Anchor("see this help document docs.touchpointsoftware.com/Finance/ContributionStatements.html")
                { Reference = "https://docs.touchpointsoftware.com/Finance/ContributionStatements.html#troubleshooting" };
                doc.Add(a);
            }
            doc.Close();

            if (set == LastSet())
            {
                runningtotals.Completed = DateTime.Now;
            }

            db.SubmitChanges();
        }

        private string GetFundDisplayText(CMSDataContext db, Func<string> defaultSelector, Func<string> overridenSelector)
        {
            if (string.IsNullOrEmpty(_fundDisplaySetting))
            {
                _fundDisplaySetting = db.GetSetting("ContributionStatementFundDisplayFieldName", "FundName");
            }

            return _fundDisplaySetting.Equals("FundName", StringComparison.OrdinalIgnoreCase) ? defaultSelector() : overridenSelector();
        }

        public static StatementSpecification GetStatementSpecification(CMSDataContext db, string name)
        {
            var standardheader = db.ContentHtml("StatementHeader", Resource1.ContributionStatementHeader);
            var standardnotice = db.ContentHtml("StatementNotice", Resource1.ContributionStatementNotice);

            if (name == null || name == "all")
            {
                return new StatementSpecification()
                {
                    Description = "All Statements",
                    Notice = standardnotice,
                    Header = standardheader,
                    Funds = null
                };
            }
            var standardsetlabel = db.Setting("StandardFundSetName", "Standard Statements");
            if (name == standardsetlabel)
            {
                var funds = APIContributionSearchModel.GetCustomStatementsList(db, name);
                return new StatementSpecification()
                {
                    Description = standardsetlabel,
                    Notice = standardnotice,
                    Header = standardheader,
                    Funds = funds
                };
            }
            var xd = XDocument.Parse(Util.PickFirst(db.ContentOfTypeText("CustomStatements"), "<CustomStatement/>"));
            var ele = xd.XPathSelectElement($"//Statement[@description='{name}']");
            if (ele == null)
            {
                return null;
            }

            var desc = ele.Attribute("description")?.Value;
            var cs = new StatementSpecification
            {
                Description = desc
            };
            var headerele = ele.Element("Header");
            cs.Header = headerele != null
                ? string.Concat(headerele.Nodes().Select(x => x.ToString()).ToArray())
                : standardheader;
            var noticeele = ele.Element("Notice");
            cs.Notice = noticeele != null
                ? string.Concat(noticeele.Nodes().Select(x => x.ToString()).ToArray())
                : standardnotice;
            cs.Funds = APIContributionSearchModel.GetCustomStatementsList(db, desc);
            return cs;
        }
        public static List<string> CustomStatementsList(CMSDataContext db)
        {
            var xd = XDocument.Parse(Util.PickFirst(db.ContentOfTypeText("CustomStatements"), "<CustomStatement/>"));
            var list = new List<string>();
            foreach (var ele in xd.Descendants().Elements("Statement"))
            {
                list.Add(ele.Attribute("description")?.Value);
            }

            if (list.Count == 0)
            {
                return null;
            }

            var standardsetlabel = db.Setting("StandardFundSetName", "Standard Statements");
            list.Insert(0, standardsetlabel);
            return list;
        }
        public static List<string> CustomFundSetList(CMSDataContext db)
        {
            var xd = XDocument.Parse(Util.PickFirst(db.ContentOfTypeText("CustomFundSets"), "<CustomFundSets/>"));
            var list = xd.Descendants().Elements("FundSet").Select(ele => ele.Attribute("description")?.Value).ToList();

            var xd2 = XDocument.Parse(Util.PickFirst(db.ContentOfTypeText("CustomStatements"), "<CustomStatements/>"));
            if (!xd2.Descendants().Elements("Statement").Any())
            {
                return list.Count == 0 ? null : list;
            }

            var standardsetlabel = db.Setting("StandardFundSetName", "Standard Statements");
            list.Add(standardsetlabel);
            list.AddRange(xd2.Descendants().Elements("Statement").Select(ele => ele.Attribute("description")?.Value));

            return list.Count == 0 ? null : list;
        }
        public static SelectList CustomStatementsSelectList()
        {
            var cslist = CustomStatementsList(DbUtil.Db);
            return cslist == null ? null : new SelectList(cslist);
        }
        public static SelectList CustomFundSetSelectList(CMSDataContext db)
        {
            var cslist = CustomFundSetList(db);
            if (cslist == null)
            {
                return null;
            }

            cslist.Insert(0, "(not specified)");
            return new SelectList(cslist);
        }

        private class PageEvent : PdfPageEventHelper
        {
            private class NPages
            {
                public NPages(PdfContentByte dc)
                {
                    template = dc.CreateTemplate(50, 50);
                }
                public bool juststartednewset;
                public PdfTemplate template;
                public int n;
            }
            private NPages npages;
            private int pg;
            private PdfWriter writer;
            private Document document;
            private PdfContentByte dc;
            private BaseFont font;

            public int set { get; set; }
            public int PeopleId { get; set; }
            public int NextPeopleId { get; set; }

            public Dictionary<int, int> FamilySet { get; set; }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                this.writer = writer;
                this.document = document;
                base.OnOpenDocument(writer, document);
                font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                dc = writer.DirectContent;
                if (set == 0)
                {
                    FamilySet = new Dictionary<int, int>();
                }

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
                if (set == 0)
                {
                    var list = FamilySet.Where(kp => kp.Value == 0).ToList();
                    foreach (var kp in list)
                    {
                        if (kp.Value == 0)
                        {
                            FamilySet[kp.Key] = npages.n;
                        }
                    }
                }
                pg = 1;
                npages.template.EndText();
                npages = new NPages(dc);
            }
            public void StartPageSet()
            {
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

                text = $"id: {PeopleId}   Page {pg} of ";
                PeopleId = NextPeopleId;
                len = font.GetWidthPoint(text, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(30, 30);
                dc.ShowText(text);
                dc.EndText();
                dc.AddTemplate(npages.template, 30 + len, 30);
                npages.n = pg++;
            }
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                EndPageSet();
            }
        }
    }
}

