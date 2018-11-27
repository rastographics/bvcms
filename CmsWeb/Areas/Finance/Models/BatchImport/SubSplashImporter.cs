using CmsData;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class SubSplashImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
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
                var amount = csv["give_amount"];
                var dt = csv["date"].ToDate();
                if (!amount.HasValue())
                {
                    continue;
                }

                var fund = csv["fund"];
                var ffid = DbUtil.Db.ContributionFunds.FirstOrDefault(f => f.FundName == fund && f.FundStatusId == 1)?.FundId ?? fid;
                var desc = $"{csv["first_name"]} {csv["last_name"]};{csv["email"]}";

                if (bundleHeader == null)
                {
                    bundleHeader = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                }

                var bd = BatchImportContributions.AddContributionDetail(dt ?? date, ffid, amount, null, "", desc);
                details.Add(bd);
                bd.Contribution.ContributionDesc = desc;
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
