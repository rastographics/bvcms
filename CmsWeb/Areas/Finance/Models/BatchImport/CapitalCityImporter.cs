using System;
using System.Collections.Generic;
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

            var details = new List<BundleDetail>();

            while (csv.ReadNextRecord())
            {
                var batchDate = csv[0].ToDate();
                var amount = csv[14];
                var type = csv[13];
                if (!amount.HasValue() || !batchDate.HasValue || type == "Credit")
                    continue;

                var routingNumber = csv[17];
                var accountNumber = csv[16];
                var checkNumber = csv[15];

                if (bundleHeader == null)
                    bundleHeader = BatchImportContributions.GetBundleHeader(batchDate.Value, DateTime.Now);

                details.Add(BatchImportContributions.AddContributionDetail(date, fid, amount, checkNumber, routingNumber, accountNumber));
            }

            details.Reverse();
            foreach (var bd in details)
            {
                bundleHeader.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bundleHeader);

            return bundleHeader.BundleHeaderId;
        }
    }
}
