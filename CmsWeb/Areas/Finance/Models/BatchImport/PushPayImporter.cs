using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using UtilityExtensions;
using CsvHelper;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class PushPayImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return Import(csv, date, fundid);
        }

        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bundleHeader = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var details = new List<BundleDetail>();

            while (csv.Read())
            {
                var batchDate = csv["Date"].ToDate();
                var amount = csv["Amount"];

                var paymentMethod = csv["Payment Method"];
                var payerName = csv["Payer Name"];
                var fundText = csv["Giving Type Label"];

                ContributionFund f = null;
                if(fundText.HasValue())
                    f = DbUtil.Db.FetchOrCreateFund(fundText);

                if (bundleHeader == null)
                    bundleHeader = BatchImportContributions.GetBundleHeader(batchDate ?? DateTime.Today, DateTime.Now);

                var bd = BatchImportContributions.NewBundleDetail(date, f?.FundId ?? fid, amount);

                var eac = Util.Encrypt(paymentMethod);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;

                var pid = q.SingleOrDefault();
                if (pid != null)
                    bd.Contribution.PeopleId = pid;

                bd.Contribution.BankAccount = paymentMethod;
                bd.Contribution.ContributionDesc = payerName;

                details.Add(bd);
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
