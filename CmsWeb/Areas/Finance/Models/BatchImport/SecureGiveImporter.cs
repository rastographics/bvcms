using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using UtilityExtensions;
using CsvHelper;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class SecureGiveImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return Import(csv, date, fundid);
        }

        // INCOMPLETE, started not finished
        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bundleHeader = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            while (csv.Read())
            {
                var batchDate = csv["Date"].ToDate();
                var amount = csv["Amount"];

                //var paymentMethod = csv["Payment Method"];
                var method = csv["Method"];
                //var payerName = csv["Payer Name"];
                var email = csv["Email address"];
                var phone = csv["Mobile Number"];
                var fundText = "";
                if(csv.FieldHeaders.Contains("Giving Type Label"))
                    fundText = csv["Giving Type Label"];

                ContributionFund f = null;
                if(fundText.HasValue())
                    f = DbUtil.Db.FetchOrCreateFund(fundText);

                if (bundleHeader == null)
                    bundleHeader = BatchImportContributions.GetBundleHeader(batchDate ?? DateTime.Today, DateTime.Now);

                var bd = BatchImportContributions.AddContributionDetail(date, f?.FundId ?? fid, amount, method, null, $"{email}|{phone}");
                bundleHeader.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bundleHeader);
            return bundleHeader.BundleHeaderId;
        }
    }
}
