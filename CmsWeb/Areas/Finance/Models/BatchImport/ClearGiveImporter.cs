using CmsData;
using CsvHelper;
using System;
using System.IO;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class ClearGiveImporter : IContributionBatchImporter
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
            var fid = fundid ?? Contribution.FirstFundId(DbUtil.Db);

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var userId = csv["userID"];
                var transDate = csv["transDate"].ToDate();
                var settleDate = csv["settleDate"].ToDate() ?? DateTime.Today;
                var amountpaid = csv["amountpaid"];
                var nameonCard = csv["nameonCard"];
                var paymentType = csv["paymentType"];
                var transType = csv["transType"];

                if (bundleHeader == null)
                {
                    bundleHeader = Contribution.GetBundleHeader(DbUtil.Db, date, DateTime.Now);
                }

                var bd = Contribution.AddContributionDetail(DbUtil.Db, transDate ?? settleDate, fid, amountpaid, paymentType, null, userId);
                bd.Contribution.ContributionDesc = nameonCard;
                if (transType != "SALE")
                {
                    bd.Contribution.ContributionDesc += $" ({transType})";
                }

                bundleHeader.BundleDetails.Add(bd);
            }

            Contribution.FinishBundle(DbUtil.Db, bundleHeader);
            return bundleHeader?.BundleHeaderId;
        }
    }
}
