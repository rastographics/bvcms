/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.IO;
using CmsData;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class FbcFayettevilleImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
                return BatchProcessFbcFayetteville(csv, date, fundid);
        }

        private static int? BatchProcessFbcFayetteville(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            while (csv.ReadNextRecord())
            {
                if (csv[6].StartsWith("Total Checks"))
                    continue;
                var routing = csv[4];
                var account = csv[5];
                var checkno = csv[6];
                var amount = csv[7];

                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);

                var bd = BatchImportContributions.AddContributionDetail(date, fund, amount, checkno, routing, account);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}