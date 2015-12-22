/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.IO;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class AnchorBibleQbImporter : IContributionBatchImporter
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

            while (csv.Read())
            {
                var dt = csv[1].ToDate() ?? date;
                var amount = csv[4];
                if (!amount.HasValue())
                    continue;

                var pid = csv[2].ToInt();
                var fund = csv[5].ToInt2() ?? fid;

                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(dt, DateTime.Now);
                var bd = BatchImportContributions.AddContributionDetail(dt, fund, amount, pid);
                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}