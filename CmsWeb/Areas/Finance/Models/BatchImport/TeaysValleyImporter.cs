/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using CmsData;
using LumenWorks.Framework.IO.Csv;
using System;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class TeaysValleyImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
            {
                return BatchProcessTeaysValley(csv, date, fundid);
            }
        }

        private static int? BatchProcessTeaysValley(CsvReader csv, DateTime date, int? fundid)
        {
            var fundList = (from f in DbUtil.Db.ContributionFunds
                            orderby f.FundId
                            select f.FundId).ToList();

            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            while (csv.ReadNextRecord())
            {
                var dt = csv[0].ToDate();
                var amount = csv[2];
                if (!amount.HasValue() || !dt.HasValue)
                {
                    continue;
                }

                var fid = csv[1].ToInt2() ?? fund;
                var account = csv[3];
                var checkno = csv[4];

                if (!fundList.Contains(fid))
                {
                    fid = firstfund;
                }

                if (bh == null)
                {
                    bh = BatchImportContributions.GetBundleHeader(dt.Value, DateTime.Now);
                }

                var bd = BatchImportContributions.AddContributionDetail(date, fid, amount, checkno, "", account);

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}
