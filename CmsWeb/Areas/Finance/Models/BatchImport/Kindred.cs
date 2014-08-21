/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using CmsData;
using CmsData.Codes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Models
{
    public partial class BatchImportContributions
    {
        private static int? BatchProcessKindred(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<depositRecord>();
            while (csv.ReadNextRecord())
                if(csv[14] == "Completed")
                    list.Add(new depositRecord()
                    {
                        date = csv[1].ToDate(),
                        account = csv[7],
                        amount = csv[9],
                        
                    });
            var q = from r in list
                    select r;
            foreach (var r in q)
            {
                var dt = DateTime.Parse("1/1/1980");
                if (r.date.HasValue)
                    dt = r.date.Value;
                if (bh == null)
                    bh = GetBundleHeader(dt.Date, DateTime.Now);
                var bd = AddContributionDetail(dt, fund, r.amount, csv[12], "", r.account);
                bd.Contribution.ContributionDesc = "{0}; {1}, {2}, {3} {4}; {5}".Fmt(
                    csv[2], csv[3], csv[4], csv[5], csv[6], csv[8]);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}