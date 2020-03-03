using System;
using System.IO;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class GraceCcImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
                return BatchProcessGraceCc(csv, date, fundid);
        }

        private static int? BatchProcessGraceCc(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            while (csv.ReadNextRecord())
            {
                var dt = csv[3].ToDate();
                var amount = csv[13];
                if (!amount.HasValue() || !dt.HasValue)
                    continue;

                var routing = csv[10];
                var account = csv[9];
                var checkno = csv[12];

                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(dt.Value, DateTime.Now);

                var bd = BatchImportContributions.AddContributionDetail(date, fid, amount, checkno, routing, account);

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}
