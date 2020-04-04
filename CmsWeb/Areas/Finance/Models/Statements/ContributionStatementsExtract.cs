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
            string nameOfChurch = db.Setting("NameOfChurch", "Name of Church");
            string startAddress = db.Setting("StartAddress", "Start Address");
            string churchPhone = db.Setting("ChurchPhone", "(000) 000-0000");
            var standardheader = db.ContentHtml("StatementHeader", string.Format(Resource1.ContributionStatementHeader, nameOfChurch, startAddress, churchPhone));
            var standardnotice = db.ContentHtml("StatementNotice", string.Format(Resource1.ContributionStatementNotice, nameOfChurch));

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
            var statementSpec = xd.XPathSelectElement($"//Statement[@description='{name}']");
            if (statementSpec == null)
            {
                return null;
            }

            var desc = statementSpec.Attribute("description")?.Value;
            var cs = new StatementSpecification
            {
                Description = desc
            };

            cs.Funds = APIContributionSearchModel.GetCustomStatementsList(db, desc);
            cs.Header = GetSectionHTML("Header", statementSpec, standardheader);
            cs.Notice = GetSectionHTML("Notice", statementSpec, standardnotice);
            cs.Template = GetSectionHTML("Template", statementSpec);
            cs.TemplateBody = GetSectionHTML("TemplateBody", statementSpec);
            cs.Footer = GetSectionHTML("Footer", statementSpec);

            return cs;
        }

        private static string GetSectionHTML(string elementName, XElement statementSpec, string defaultValue = null)
        {
            var element = statementSpec.Element(elementName);
            return element != null
                ? string.Concat(element.Nodes().Select(x => x.ToString()).ToArray())
                : defaultValue;
        }

        public static string Output(string fn, int set)
        {
            var outf = fn.Replace(".pdf", $"-{set}.pdf");
            return outf;
        }
    }
}
