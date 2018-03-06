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
    internal class HunterStreetImporter : IContributionBatchImporter
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

            csv.Read();
            csv.ReadHeader();
            csv.Read(); // skip first row which is the total row

            while (csv.Read())
            {
                var amount = csv[0];
                var routing = csv[1];
                var checkno = csv[2];
                var account = csv[3];
                var dt = csv[4].ToDate();

                if (!amount.HasValue() || !dt.HasValue)
                    continue;

                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(date, now);

                var bd = BatchImportContributions.AddContributionDetail(dt.Value, fid, amount, checkno, routing, account);
                bd.Contribution.PostingDate = now;

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId ?? 0;
        }
    }
}
