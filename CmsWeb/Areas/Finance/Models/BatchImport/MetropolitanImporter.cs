using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class MetropolitanImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
                return BatchProcessMetropolitan(csv, date, fundid);
        }

        private static int? BatchProcessMetropolitan(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<DepositRecord>();
            while (csv.ReadNextRecord())
                list.Add(new DepositRecord()
                {
                    Type = csv[13],
                    Batch = csv[0],
                    Routing = csv[17],
                    Account = csv[16],
                    CheckNo = csv[15],
                    Amount = csv[14],
                });
            var q = from r in list
                    where r.Type.Equal("debit")
                    orderby r.Batch
                    select r;
            var prevbatch = "";
            foreach (var r in q)
            {
                if (r.Batch != prevbatch)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                    prevbatch = r.Batch;
                }
                var bd = BatchImportContributions.AddContributionDetail(date, fund, r.Amount, r.CheckNo, r.Routing, r.Account);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
