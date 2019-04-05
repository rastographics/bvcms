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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ContributionStatementsExtra
    {
        private readonly PageEvent pageEvents = new PageEvent();

        private string _fundDisplaySetting;

        public int FamilyId { get; set; }
        public int PeopleId { get; set; }
        public int? SpouseId { get; set; }
        public int typ { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool ShowCheckNo { get; set; }
        public bool ShowNotes { get; set; }


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

        public void Run(Stream stream, CMSDataContext db, IEnumerable<ContributorInfo> q, ContributionStatements.StatementSpecification cs, int set = 0)
        {
            pageEvents.set = set;
            pageEvents.PeopleId = 0;
            var contributors = q;

            var font = FontFactory.GetFont(FontFactory.HELVETICA, 11);
            var boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

            var doc = new Document(PageSize.LETTER);
            doc.SetMargins(36f, 30f, 24f, 36f);
            var w = PdfWriter.GetInstance(doc, stream);
            w.PageEvent = pageEvents;
            doc.Open();
            var dc = w.DirectContent;

            var prevfid = 0;
            var runningtotals = db.ContributionsRuns.OrderByDescending(mm => mm.Id).FirstOrDefault();
            if (runningtotals != null)
            {
                runningtotals.Processed = 0;
                db.SubmitChanges();
                var count = 0;
                foreach (var ci in contributors)
                {
                    if (set > 0 && pageEvents.FamilySet[ci.PeopleId] != set)
                    {
                        continue;
                    }

                    var contributions = APIContribution.Contributions(db, ci, FromDate, ToDate, cs.Funds).ToList();
                    var pledges = APIContribution.Pledges(db, ci, ToDate, cs.Funds).ToList();
                    var giftsinkind = APIContribution.GiftsInKind(db, ci, FromDate, ToDate, cs.Funds).ToList();
                    var nontaxitems = db.Setting("DisplayNonTaxOnStatement", "false").ToBool()
                        ? APIContribution.NonTaxItems(db, ci, FromDate, ToDate, cs.Funds).ToList()
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

                    var mh = new MyHandler();
                    using (var sr = new StringReader(css + cs.Header))
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

                    var t1a = new PdfPTable(1)
                    {
                        TotalWidth = 72f * 5f
                    };
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
                            envno = $" env: {Util.PickFirst(ev.Data, ev.IntValue.ToString(), ev.StrValue)}";
                        }
                    }

                    t2.AddCell(db.Setting("NoPrintDateOnStatement")
                        ? new Phrase($"\nid:{ci.PeopleId}{envno} {ci.CampusId}", font)
                        : new Phrase($"\nprinted: {DateTime.Now:d} id:{ci.PeopleId}{envno} {ci.CampusId}", font));

                    t2.AddCell("");
                    var mh2 = new MyHandler();
                    using (var sr = new StringReader(css + cs.Notice))
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

                    var yp = doc.BottomMargin +
                             db.Setting("StatementRetAddrPos", "10.125").ToFloat() * 72f;
                    t1.WriteSelectedRows(0, -1,
                        doc.LeftMargin - 0.1875f * 72f, yp, dc);

                    yp = doc.BottomMargin +
                         db.Setting("StatementAddrPos", "8.3375").ToFloat() * 72f;
                    t1a.WriteSelectedRows(0, -1, doc.LeftMargin, yp, dc);

                    yp = doc.BottomMargin + 10.125f * 72f;
                    t2.WriteSelectedRows(0, -1, doc.LeftMargin + 72f * 4.4f, yp, dc);

                    //----Contributions

                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph(" ") { SpacingBefore = 72f * 2.125f });

                    doc.Add(new Phrase($"\n  Period: {FromDate:d} - {ToDate:d}", boldfont));

                    var pos = w.GetVerticalPosition(true);

                    var ct = new ColumnText(dc);
                    var colwidth = (doc.Right - doc.Left);

                    var t = new PdfPTable(new[] { 15f, 25f, 15f, 15f, 30f })
                    {
                        WidthPercentage = 100
                    };
                    t.DefaultCell.Border = Rectangle.NO_BORDER;
                    t.HeaderRows = 2;

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 5,
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

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Phrase = ShowCheckNo ? new Phrase("Check No", boldfont) : new Phrase("", boldfont)
                    };


                    t.AddCell(cell);

                    t.AddCell(ShowNotes ? new Phrase("Notes", boldfont) : new Phrase("", boldfont));

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

                        cell = new PdfPCell(t.DefaultCell)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Phrase = ShowCheckNo ? new Phrase(c.CheckNo, font) : new Phrase("", font)
                        };


                        t.AddCell(cell);

                        t.AddCell(ShowNotes ? new Phrase(c.Description?.Trim() ?? "", font) : new Phrase("", font));

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

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 2,
                        Phrase = new Phrase("")
                    };
                    t.AddCell(cell);

                    ct.AddElement(t);

                    //------Pledges

                    if (pledges.Count > 0)
                    {
                        t = new PdfPTable(new[] { 25f, 15f, 15f, 15f, 30f })
                        {
                            WidthPercentage = 100
                        };
                        t.DefaultCell.Border = Rectangle.NO_BORDER;
                        t.HeaderRows = 2;

                        cell = new PdfPCell(t.DefaultCell)
                        {
                            Colspan = 5,
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
                        t.AddCell(new Phrase("", boldfont));
                        t.AddCell(new Phrase("", boldfont));

                        foreach (var c in pledges)
                        {
                            t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription),
                                font));

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

                            t.AddCell(new Phrase("", boldfont));
                            t.AddCell(new Phrase("", boldfont));
                        }

                        ct.AddElement(t);
                    }

                    //------Gifts In Kind

                    if (giftsinkind.Count > 0)
                    {
                        t = new PdfPTable(new[] { 15f, 25f, 15f, 15f, 30f })
                        {
                            WidthPercentage = 100
                        };
                        t.DefaultCell.Border = Rectangle.NO_BORDER;
                        t.HeaderRows = 2;

                        // Headers
                        cell = new PdfPCell(t.DefaultCell)
                        {
                            Colspan = 5,
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

                        t.AddCell(new Phrase("", boldfont));
                        t.AddCell(new Phrase("", boldfont));

                        t.DefaultCell.Border = Rectangle.NO_BORDER;

                        foreach (var c in giftsinkind)
                        {
                            t.AddCell(new Phrase(c.ContributionDate.ToString2("d"), font));
                            cell = new PdfPCell(t.DefaultCell)
                            {
                                Phrase = new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription),
                                    font)
                            };
                            t.AddCell(cell);

                            cell = new PdfPCell(t.DefaultCell)
                            {
                                Colspan = 3,
                                Phrase = new Phrase(c.Description, font)
                            };
                            t.AddCell(cell);
                        }

                        ct.AddElement(t);
                    }

                    //-----Summary

                    t = new PdfPTable(new[] { 40f, 15f, 45f })
                    {
                        WidthPercentage = 100
                    };
                    t.DefaultCell.Border = Rectangle.NO_BORDER;
                    t.HeaderRows = 2;

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Colspan = 3,
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
                    t.AddCell(new Phrase("", boldfont));

                    foreach (var c in APIContribution.GiftSummary(db, ci, FromDate, ToDate, cs.Funds))
                    {
                        t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription), font));

                        cell = new PdfPCell(t.DefaultCell)
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Phrase = new Phrase(c.Total.ToString2("N2"), font)
                        };
                        t.AddCell(cell);

                        t.AddCell(new Phrase("", boldfont));
                    }

                    t.DefaultCell.Border = Rectangle.NO_BORDER;

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Border = Rectangle.TOP_BORDER,
                        Colspan = 1,
                        Phrase = new Phrase("Total Contributions for period", boldfont)
                    };
                    t.AddCell(cell);

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Border = Rectangle.TOP_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Phrase = new Phrase(total.ToString("N2"), font)
                    };
                    t.AddCell(cell);

                    cell = new PdfPCell(t.DefaultCell)
                    {
                        Phrase = new Phrase("")
                    };
                    t.AddCell(cell);

                    ct.AddElement(t);

                    //------NonTax

                    if (nontaxitems.Count > 0)
                    {
                        t = new PdfPTable(new[] { 15f, 25f, 15f, 15f, 30f })
                        {
                            WidthPercentage = 100
                        };
                        t.DefaultCell.Border = Rectangle.NO_BORDER;
                        t.HeaderRows = 2;

                        cell = new PdfPCell(t.DefaultCell)
                        {
                            Colspan = 5,
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
                        t.AddCell(new Phrase("", boldfont));
                        t.AddCell(new Phrase("", boldfont));

                        t.DefaultCell.Border = Rectangle.NO_BORDER;

                        var ntotal = 0m;
                        foreach (var c in nontaxitems)
                        {
                            t.AddCell(new Phrase(c.ContributionDate.ToString2("d"), font));
                            t.AddCell(new Phrase(GetFundDisplayText(db, () => c.FundName, () => c.FundDescription),
                                font));
                            cell = new PdfPCell(t.DefaultCell)
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Phrase = new Phrase(c.ContributionAmount.ToString2("N2"), font)
                            };
                            t.AddCell(cell);
                            t.AddCell(new Phrase("", boldfont));
                            t.AddCell(ShowNotes ? new Phrase(c.Description, font) : new Phrase("", font));

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
                        t.AddCell(new Phrase("", boldfont));
                        t.AddCell(new Phrase("", boldfont));


                        ct.AddElement(t);
                    }

                    var status = 0;
                    while (ColumnText.HasMoreText(status))
                    {
                        ct.SetSimpleColumn(doc.Left, doc.Bottom, doc.Left + colwidth, pos);

                        status = ct.Go();
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
                    var a = new Anchor(
                        "see this help document docs.touchpointsoftware.com/Finance/ContributionStatements.html")
                    {
                        Reference =
                            "https://docs.touchpointsoftware.com/Finance/ContributionStatements.html#troubleshooting"
                    };
                    doc.Add(a);
                }

                doc.Close();

                if (set == LastSet())
                {
                    runningtotals.Completed = DateTime.Now;
                }
            }

            db.SubmitChanges();
        }


        private string GetFundDisplayText(CMSDataContext Db, Func<string> defaultSelector, Func<string> overridenSelector)
        {
            if (string.IsNullOrEmpty(_fundDisplaySetting))
            {
                _fundDisplaySetting = Db.GetSetting("ContributionStatementFundDisplayFieldName", "FundName");
            }

            return _fundDisplaySetting.Equals("FundName", StringComparison.OrdinalIgnoreCase) ? defaultSelector() : overridenSelector();
        }

        private class PageEvent : PdfPageEventHelper
        {
            private PdfContentByte dc;
            private BaseFont font;
            private NPages npages;
            private int pg;
            private PdfWriter writer;
            private Document document;

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
