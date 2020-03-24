using CmsData;
using CmsData.API;
using CmsWeb.Properties;
using System;
using System.Collections.Generic;
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
        public string id { get; set; }
        public Guid UUId => Guid.Parse(id);
        public string OutputFile { get; }
        public string Host { get; }
        public int LastSet { get; set; }
        public string StartsWith { get; set; }
        public string Sort { get; set; }
        public int? TagId { get; set; }
        public bool ExcludeElectronic { get; set; }

        public ContributionStatementsExtract(string host, DateTime fd, DateTime td, string outputFile, string startswith, string sort, int? tagid, bool excludeelectronic)
        {
            this.fd = fd;
            this.td = td;
            Host = host;
            OutputFile = outputFile;
            TagId = tagid;
            StartsWith = startswith;
            Sort = sort;
            ExcludeElectronic = excludeelectronic;
        }

        public CMSDataContext Db { get; set; }
        public void DoWork(ContributionStatements statements, StatementSpecification cs, List<ContributorInfo> contributorInfos)
        {
            Db = CMSDataContext.Create(Host);

            var runningtotals = Db.ContributionsRuns.Where(mm => mm.UUId == UUId).Single();
            runningtotals.Count = contributorInfos.Count();
            Db.SubmitChanges();

            using (var stream = new FileStream(OutputFile, FileMode.Create))
            {
                statements.Run(stream, Db, contributorInfos, cs);
            }

            LastSet = statements.LastSet();
            var sets = statements.Sets();
            foreach (var set in sets)
            {
                using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                {
                    statements.Run(stream, Db, contributorInfos, cs, set);
                }
            }

            runningtotals.LastSet = LastSet;
            runningtotals.Sets = string.Join(",", sets);
            runningtotals.Completed = DateTime.Now;
            Db.SubmitChanges();
        }

        public static StatementSpecification GetStatementSpecification(CMSDataContext db, string name)
        {
            var standardheader = db.ContentHtml("StatementHeader", Resource1.ContributionStatementHeader);
            var standardnotice = db.ContentHtml("StatementNotice", Resource1.ContributionStatementNotice);

            if (name == null || name == "all")
            {
                return new StatementSpecification()
                {
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
