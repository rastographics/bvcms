using CmsData;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class ChristLutheranVailImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
            {
                return BatchProcess(csv, date, fundid);
            }
        }

        private static int? BatchProcess(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();
            var now = DateTime.Now;
            var list = new List<Record>();

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                if (csv[0] != "Deposit") // Type
                {
                    continue;
                }

                var rec = new Record
                {
                    CheckNo = csv[2], //Num
                    Identifer = csv[3], //Name
                    Dt = csv[4].ToDate(), //Memo
                    Fund = csv[6], //Split
                    Amount = csv[7], //Amount
                };
                list.Add(rec);
            }
            foreach (var rec in list)
            {
                if (!rec.Dt.HasValue)
                {
                    continue;
                }

                if (bh == null)
                {
                    bh = BatchImportContributions.GetBundleHeader(date, now);
                }

                var f = DbUtil.Db.FetchOrCreateFund(rec.Fund);
                var bd = BatchImportContributions.AddContributionDetail(rec.Dt.Value, f.FundId, rec.Amount, rec.CheckNo, "", rec.Identifer);
                bd.Contribution.PostingDate = now;
                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId ?? 0;
        }

        private class Record
        {
            public DateTime? Dt { get; set; }
            public string Identifer { get; set; }
            public string Amount { get; set; }
            public string CheckNo { get; set; }
            public string Fund { get; set; }
        }
    }
}
