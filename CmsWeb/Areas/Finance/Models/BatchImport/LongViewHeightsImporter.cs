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
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class LongViewHeightsImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return Import(csv, date, fundid);
        }

        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var list = new List<DepositRecord>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
                list.Add(new DepositRecord()
                {
                    Batch = csv[0],
                    Account = csv[4],
                    CheckNo = csv[5],
                    Amount = csv[7],
                });
            var q = from r in list
                    select r;
            var prevbatch = "";
            foreach (var r in q)
            {
                if (r.Batch != prevbatch)
                {
                    if (bh == null)
                        bh = BatchImportContributions.GetBundleHeader(r.Batch.ToDate() ?? date, DateTime.Now);
                    prevbatch = r.Batch;
                }
                var bd = BatchImportContributions.AddContributionDetail(date, fid, r.Amount, r.CheckNo, r.Routing, r.Account);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
