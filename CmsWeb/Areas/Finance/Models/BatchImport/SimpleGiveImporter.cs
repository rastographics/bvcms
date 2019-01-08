using CmsData;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class SimpleGiveImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text))
            { Configuration = { Delimiter = "\t" } })
            {
                return Import(csv, date, fundid);
            }
        }

        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bundleHeader = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var details = new List<BundleDetail>();

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                if (csv[0].EndsWith("ENDTRNS"))
                {
                    continue;
                }

                var amount = csv[6];
                var dt = csv[3].ToDate();
                if (!amount.HasValue())
                {
                    continue;
                }

                var fund = csv[4];
                int ffid = !fund.HasValue() ? fid : DbUtil.Db.FetchOrCreateFund(fund).FundId;

                var name = csv[5];
                var address = csv[12];
                var city = csv[13];
                var st = csv[14];
                var zip = csv[15];

                if (bundleHeader == null)
                {
                    bundleHeader = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                }

                var bd = BatchImportContributions.AddContributionDetail(dt ?? date, ffid, amount, null, "", $"{name};{address}");
                details.Add(bd);
                bd.Contribution.ContributionDesc = $"{name};{address},{city},{st},{zip}";
            }

            details.Reverse();
            if (bundleHeader != null)
            {
                foreach (var bd in details)
                {
                    bundleHeader.BundleDetails.Add(bd);
                }

                BatchImportContributions.FinishBundle(bundleHeader);
                return bundleHeader.BundleHeaderId;
            }
            return null;
        }
    }
}
