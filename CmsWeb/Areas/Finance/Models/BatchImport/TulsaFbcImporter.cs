using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class TulsaFbcImporter : IContributionBatchImporter
    {
        private const string Patterns = @"
matchnothinghere
|\A\s*(?!/)(?<ac>\d+)/\s*(?<ck>\d+)
|\A\s*/(?<ac>(\d|\s)+)/\s*(?<ck>\d+)
|\A\s*(?<ck>\d+)\s*(?<ac>\d+)/
|\A\s*(?<ac>\d+)/(?<ck>\d+)
|\A\s*(?<ck>\d+)\s*/(?<ac>\d+)/
|\A\s*(?<ac>(\d|\s|-)+)/\s*(?<ck>\d+)
|\A\s*(?<ac>(\d|\s|-)+)/
|\A(?<ac>\d+)
";

        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return BatchProcess(csv, date, fundid);
        }

        private static int? BatchProcess(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();
            var now = DateTime.Now;
            var list = new List<Record>();

            // for parsing account and checkno
            var regex = new Regex(Patterns, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            csv.Read();
            csv.ReadHeader();
            csv.Read(); // read header;
            var newBundle = true;
            while (csv.Read())
            {
                if (csv[4] == "Bank On Us")
                {
                    csv.Read(); // read the next row past the header row
                    newBundle = true;
                }

                var rec = new Record
                {
                    Dt = csv[1].PadLeft(8, '0').ToDate(),
                    Amount = csv[5],
                    Routing = csv[3],
                    AccCk = csv[4],
                    NewBundle = newBundle,
                };
                // Parse out account and checkno
                var m = regex.Match(rec.AccCk);
                if (!m.Success)
                    throw new Exception($@"account **""{rec.AccCk}""**, does not match known patterns, contact support with this information.

No Contributions have been imported from this batch.");
                rec.Account = m.Groups["ac"].Value;
                rec.CheckNo = m.Groups["ck"].Value;
                list.Add(rec);
                newBundle = false;
            }
            foreach(var rec in list)
            {
                if (!rec.HasAmount || !rec.Dt.HasValue)
                    continue;

                if(rec.NewBundle)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(date, now);
                }
                if (bh == null)
                    throw new Exception($@"Unexpected error: header row not found, aborting");

                var bd = BatchImportContributions.AddContributionDetail(rec.Dt.Value, fid, rec.Amount, rec.CheckNo, rec.Routing, rec.Account);
                bd.Contribution.PostingDate = now;

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId ?? 0;
        }

        class Record
        {
            public DateTime? Dt { get; set; }
            public string Routing { get; set; }
            public string AccCk { get; set; }
            public string Amount { get; set; }
            public bool HasAmount => Amount.GetAmount() > 0;
            public string Account { get; set; }
            public string CheckNo { get; set; }
            public bool NewBundle { get; set; }
        }
    }
}
