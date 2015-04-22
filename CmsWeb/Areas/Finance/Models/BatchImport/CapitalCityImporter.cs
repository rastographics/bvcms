using System;
using System.IO;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class CapitalCityImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
                return Import(csv, date, fundid);
        }

        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bundleHeader = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            while (csv.ReadNextRecord())
            {
                var batchDate = csv[0].ToDate();
                var amount = csv[10];
                var totalBatchAmount = csv[4];
                if (!amount.HasValue() || !batchDate.HasValue || amount == totalBatchAmount)
                    continue;

                var routingNumber = csv[7];
                var accountNumber = csv[9];
                var checkNumber = csv[11];

                if (bundleHeader == null)
                    bundleHeader = BatchImportContributions.GetBundleHeader(batchDate.Value, DateTime.Now);

                var bd = BatchImportContributions.AddContributionDetail(date, fid, amount, checkNumber, routingNumber, accountNumber);

                bundleHeader.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bundleHeader);

            return bundleHeader.BundleHeaderId;
        }
    }
}