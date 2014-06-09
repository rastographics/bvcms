/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using UtilityExtensions;
using CmsData;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Models
{
    public partial class BatchImportContributions
    {
        public static int? BatchProcessSilverdale(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = FirstFundId();
            var fund = fundid ?? firstfund;

            while (csv.ReadNextRecord())
            {
                var excludecol = csv[12] == "Virtual Credit Item";
                var routing = csv[18];
                var account = csv[19];
                var amount = csv[20];
                var checkno = csv[24];
                if (!checkno.HasValue() && account.Count(c => c == ' ') == 1)
                {
                    var a = account.Split(' ');
                    account = a[1];
                    checkno = a[0];
                }

                if (excludecol)
                {
                    if (bh != null) FinishBundle(bh);
                    bh = GetBundleHeader(date, DateTime.Now);
                    continue;
                }
                if (bh == null)
                    bh = GetBundleHeader(date, DateTime.Now);

                var bd = AddContributionDetail(date, fund, amount, checkno, routing, account);
                bh.BundleDetails.Add(bd);
            }
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}