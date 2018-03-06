using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;
using CsvHelper;
using Dapper;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class CrossroadsBaptistImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return Import(csv, fundid, date);
        }

        private static int? Import(CsvReader csv, int? fundid, DateTime date)
        {
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = "\t";
            var bundleHeader = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var list = new List<DepositRecord>();
            var n = 0;

            // this is a peculiar file, no headers and all rows are included twice.
            // C is my marker to stop before the duplicated rows.
            // C is the cumulative deposit record.
            // Here I am skiping the first row
            csv.Read(); // skip C record

            while (csv.Read())
            {
                if (csv[3] == "C") // start of repeated rows, we are done
                    break;

                var r = new DepositRecord() { Row = ++n, Valid = true};

                r.Account = csv[5];
                r.Amount = (csv[7].ToDecimal() / 100).ToString();
                r.CheckNo = csv[8];
                r.Routing = csv[9];

                list.Add(r);
            }
            foreach(var r in list)
            {
                var bd = BatchImportContributions.AddContributionDetail(date, fid, r.Amount, r.CheckNo, r.Routing, r.Account);
                bundleHeader.BundleDetails.Add(bd);
            }
            BatchImportContributions.FinishBundle(bundleHeader);
            return bundleHeader.BundleHeaderId;
        }
    }
}
