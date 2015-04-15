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
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class RedeemerImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
                return BatchProcessRedeemer(csv, date, fundid);
        }

        private static int? BatchProcessRedeemer(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<DepositRecord>();
            while (csv.ReadNextRecord())
                list.Add(new DepositRecord()
                {
                    Batch = csv[0],
                    Account = csv[1],
                    CheckNo = csv[2],
                    Amount = csv[3],
                    Routing = csv[4],
                });
            var q = from r in list
                    select r;
            var prevbatch = "";
            foreach (var r in q)
            {
                if (r.Batch != prevbatch)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(r.Batch.ToDate().Value, DateTime.Now);
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