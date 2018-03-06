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
    internal class KindredImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return BatchProcessKindred(csv, date, fundid);
        }

        private static int? BatchProcessKindred(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<DepositRecord>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
                if(csv[14] == "Completed")
                {
                    var desc = csv[13].HasValue()
                        ? $"keyword={csv[13]}; {csv[2]}; {csv[3]}, {csv[4]}, {csv[5]} {csv[6]}; {csv[8]}" 
                        : $"{csv[2]}; {csv[3]}, {csv[4]}, {csv[5]} {csv[6]}; {csv[8]}";
                    list.Add(new DepositRecord()
                    {
                        Date = csv[1].ToDate(),
                        Account = csv[7],
                        Amount = csv[9],
                        CheckNo = csv[12],
                        Description= desc,
                    });
                }
            var q = from r in list
                    select r;
            foreach (var r in q)
            {
                var dt = DateTime.Parse("1/1/1980");
                if (r.Date.HasValue)
                    dt = r.Date.Value;
                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(dt.Date, DateTime.Now);
                var bd = BatchImportContributions.AddContributionDetail(dt, fund, r.Amount, r.CheckNo, "", r.Account);
                bd.Contribution.ContributionDesc = r.Description;
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
