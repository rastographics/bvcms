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
using System.Text.RegularExpressions;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class Text2Give : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return BatchProcessText2Give(csv, date, fundid);
        }

        private static int? BatchProcessText2Give(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<DepositRecord>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
                if(csv[28].Equal("collected"))
                {
                    var dt = Regex.Match(csv[24] ?? "", @"(\d{1,2}/\d{1,2}/\d{4} \d+:\d+:\d+ [A-P]M)").Value.ToDate();
                    var desc = $"{csv[8]}, {csv[9]}, {csv[10]}, {csv[16]}, {csv[17]}";
                    list.Add(new DepositRecord()
                    {
                        Date = dt,
                        Account = csv[18],
                        Amount = csv[2],
                        CheckNo = csv[0],
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
