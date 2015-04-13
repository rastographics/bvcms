/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class EbcfamilyImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), false))
                return BatchProcessEbcfamily(csv, date, fundid);
        }

        private static int? BatchProcessEbcfamily(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<DepositRecord>();
            while (csv.ReadNextRecord())
                list.Add(new DepositRecord()
                {
                    Batch = csv[0],
                    Routing = csv[1],
                    Account = csv[2],
                    CheckNo = csv[3],
                    Amount = csv[4],
                });
            var q = from r in list
                    where r.Batch.ToLower().Contains("contribution")
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