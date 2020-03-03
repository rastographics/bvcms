using System;
using System.IO;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class EnonImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return BatchProcessEnon(csv, date, fundid);
        }

        private static int? BatchProcessEnon(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var dt = csv[1].ToDate();
                var amount = csv[6];
                if (!amount.HasValue() || !dt.HasValue)
                    continue;

                var account = csv[4];
                var checkno = csv[5];

                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(dt.Value, DateTime.Now);

                var bd = BatchImportContributions.AddContributionDetail(date, fid, amount, checkno, "", account);

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}
