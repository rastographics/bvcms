using CmsData;
using CmsData.API;
using System;
using System.IO;
using System.Linq;

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

        public ContributionStatementsExtract(string Host, DateTime fd, DateTime td, string OutputFile, string startswith, string sort, int? tagid, bool excludeelectronic)
        {
            this.fd = fd;
            this.td = td;
            this.Host = Host;
            this.OutputFile = OutputFile;
            TagId = tagid;
            StartsWith = startswith;
            Sort = sort;
            ExcludeElectronic = excludeelectronic;
        }

        public CMSDataContext Db { get; set; }
        public void DoWork(ContributionStatements.StatementSpecification cs)
        {
            Db = DbUtil.Create(Host);
            DbUtil.Db.CommandTimeout = 1200;

            var noaddressok = !DbUtil.Db.Setting("RequireAddressOnStatement", true);
            showCheckNo = DbUtil.Db.Setting("RequireCheckNoOnStatement");
            showNotes = DbUtil.Db.Setting("RequireNotesOnStatement");
            const bool UseMinAmt = true;

            var qc = APIContribution.Contributors(DbUtil.Db, fd, td, 0, 0, 0, cs.Funds, noaddressok, UseMinAmt, StartsWith, Sort, tagid: TagId, excludeelectronic: ExcludeElectronic);
            var runningtotals = DbUtil.Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
            runningtotals.Count = qc.Count();
            DbUtil.Db.SubmitChanges();
            if (showCheckNo || showNotes)
            {
                var c = new ContributionStatementsExtra
                {
                    FromDate = fd,
                    ToDate = td,
                    typ = 3,
                    ShowCheckNo = showCheckNo,
                    ShowNotes = showNotes
                };
                using (var stream = new FileStream(OutputFile, FileMode.Create))
                {
                    c.Run(stream, Db, qc, cs);
                }

                LastSet = c.LastSet();
                var sets = c.Sets();
                foreach (var set in sets)
                {
                    using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                    {
                        c.Run(stream, Db, qc, cs, set);
                    }
                }

                runningtotals = DbUtil.Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
                runningtotals.LastSet = LastSet;
                runningtotals.Sets = string.Join(",", sets);
                runningtotals.Completed = DateTime.Now;
                DbUtil.Db.SubmitChanges();
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
                    c.Run(stream, Db, qc, cs);
                }

                LastSet = c.LastSet();
                var sets = c.Sets();
                foreach (var set in sets)
                {
                    using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                    {
                        c.Run(stream, Db, qc, cs, set);
                    }
                }

                runningtotals = DbUtil.Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
                runningtotals.LastSet = LastSet;
                runningtotals.Sets = string.Join(",", sets);
                runningtotals.Completed = DateTime.Now;
                DbUtil.Db.SubmitChanges();
            }
        }
        public static string Output(string fn, int set)
        {
            var outf = fn.Replace(".pdf", $"-{set}.pdf");
            return outf;
        }
    }
}
