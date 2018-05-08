using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class CspcImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return BatchProcess(csv, date, fundid);
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
                var rec = new Record
                {
                    ItemType = csv["ItemTypeName"],
                    Amount = csv["Amount"],
                    Dt = csv["CapturedDate"].ToDate(),
                    Account = csv["Account"],
                    Routing = csv["RoutingNumber"],
                    CheckNo = csv["TranCode"],
                };
                if(rec.ItemType == "CK")
                    list.Add(rec);
            }
            bh = BatchImportContributions.GetBundleHeader(date, now);
            foreach(var rec in list)
            {
                if (!rec.HasAmount || !rec.Dt.HasValue)
                    continue;

                var bd = BatchImportContributions.AddContributionDetail(rec.Dt.Value, fid, rec.Amount, rec.CheckNo, rec.Routing, rec.Account);
                bd.Contribution.PostingDate = now;

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId ?? 0;
        }

        class Record
        {
            public DateTime? Dt { get; set; }
            public string Routing { get; set; }
            public string Account { get; set; }
            public string ItemType { get; set; }
            public string Amount { get; set; }
            public string CheckNo { get; set; }
            public bool HasAmount => Amount.GetAmount() > 0;
        }
    }
}
