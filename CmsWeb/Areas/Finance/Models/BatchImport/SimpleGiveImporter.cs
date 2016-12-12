using System;
using System.Collections.Generic;
using System.IO;
using CmsData;
using CsvHelper;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class SimpleGiveImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            var lines = text.SplitLines().SkipUntil(vv => vv.StartsWith("Amount,Type"));
            var txt = string.Join("\n", lines);
            using (var csv = new CsvReader(new StringReader(txt)))
                return Import(csv, date, fundid);
        }

        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bundleHeader = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var details = new List<BundleDetail>();

            while (csv.Read())
            {
                var amount = csv[0];
                var dt = csv[2].ToDate();
                if (!amount.HasValue())
                    continue;

                var fund = csv[4];
                int ffid = !fund.HasValue() ? fid : DbUtil.Db.FetchOrCreateFund(fund).FundId;
                var name = csv[5];
                var email = csv[6];
                var address = csv[7];
                var phone = csv[8];

                if (bundleHeader == null)
                    bundleHeader = BatchImportContributions.GetBundleHeader(date, DateTime.Now);

                var bd = BatchImportContributions.AddContributionDetail(dt ?? date, ffid, amount, null, "", email);
                details.Add(bd);
                bd.Contribution.ContributionDesc = $"{name};{address};{phone}";
            }

            details.Reverse();
            if(bundleHeader != null)
            {
                foreach (var bd in details)
                    bundleHeader.BundleDetails.Add(bd);
                BatchImportContributions.FinishBundle(bundleHeader);
                return bundleHeader.BundleHeaderId;
            }
            return null;
        }
    }
}
