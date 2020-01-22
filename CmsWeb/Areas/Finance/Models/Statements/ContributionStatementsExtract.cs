using CmsData;
using CmsData.API;
using CmsWeb.Properties;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ContributionStatementsExtract
    {
        public DateTime fd { get; set; }
        public DateTime td { get; set; }
        public string OutputFile { get; set; }
        public string Host { get; set; }
        public int LastSet { get; set; }
        public string StartsWith { get; set; }
        public string Sort { get; set; }
        public int? TagId { get; set; }
        public bool ExcludeElectronic { get; set; }

        // For extended report
        public bool showCheckNo { get; set; }
        public bool showNotes { get; set; }

        public ContributionStatementsExtract(string host, DateTime fd, DateTime td, string outputFile, string startswith, string sort, int? tagid, bool excludeelectronic)
        {
            this.fd = fd;
            this.td = td;
            this.Host = host;
            this.OutputFile = outputFile;
            TagId = tagid;
            StartsWith = startswith;
            Sort = sort;
            ExcludeElectronic = excludeelectronic;
        }

        public CMSDataContext Db { get; set; }
        public void DoWork(StatementSpecification cs)
        {
            Db = CMSDataContext.Create(Host);
            Db.CommandTimeout = 1200;

            var noaddressok = !Db.Setting("RequireAddressOnStatement", true);
            showCheckNo = Db.Setting("RequireCheckNoOnStatement");
            showNotes = Db.Setting("RequireNotesOnStatement");
            const bool useMinAmt = true;

            var qc = APIContribution.Contributors(Db, fd, td, 0, 0, 0, cs.Funds, noaddressok, useMinAmt, StartsWith, Sort, tagid: TagId, excludeelectronic: ExcludeElectronic);
            var runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
            var contributorInfos = qc.ToList();
            runningtotals.Count = contributorInfos.Count();
            Db.SubmitChanges();
            if (showCheckNo || showNotes)
            {
                var c = new ContributionStatements
                {
                    FromDate = fd,
                    ToDate = td,
                    typ = 3,
                    NumberOfColumns = 1,
                    ShowCheckNo = showCheckNo,
                    ShowNotes = showNotes
                };
                using (var stream = new FileStream(OutputFile, FileMode.Create))
                {
                    c.Run(stream, Db, contributorInfos, cs);
                }

                LastSet = c.LastSet();
                var sets = c.Sets();
                foreach (var set in sets)
                {
                    using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                    {
                        c.Run(stream, Db, contributorInfos, cs, set);
                    }
                }

                runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
                runningtotals.LastSet = LastSet;
                runningtotals.Sets = string.Join(",", sets);
                runningtotals.Completed = DateTime.Now;
                Db.SubmitChanges();
            }
            else
            {
                var c = new ContributionStatements
                {
                    FromDate = fd,
                    ToDate = td,
                    typ = 3
                };
                using (var stream = new FileStream(OutputFile, FileMode.Create))
                {
                    c.Run(stream, Db, contributorInfos, cs);
                }

                LastSet = c.LastSet();
                var sets = c.Sets();
                foreach (var set in sets)
                {
                    using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                    {
                        c.Run(stream, Db, contributorInfos, cs, set);
                    }
                }

                runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
                runningtotals.LastSet = LastSet;
                runningtotals.Sets = string.Join(",", sets);
                runningtotals.Completed = DateTime.Now;
                Db.SubmitChanges();
            }
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

        public static string Output(string fn, int set)
        {
            var outf = fn.Replace(".pdf", $"-{set}.pdf");
            return outf;
        }
    }
}
