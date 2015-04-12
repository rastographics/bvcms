/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class BatchImportContributions
    {
        public static int? BatchProcessEnon(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var fid = fundid ?? FirstFundId();

            while (csv.ReadNextRecord())
            {
                var dt = csv[2].ToDate();
                var amount = csv[7];
                if (!amount.HasValue() || !dt.HasValue)
                    continue;

                var account = csv[5];
                var checkno = csv[6];

                if (bh == null)
                    bh = GetBundleHeader(dt.Value, DateTime.Now);

                var bd = AddContributionDetail(date, fid, amount, checkno, "", account);

                bh.BundleDetails.Add(bd);
            }

            FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}