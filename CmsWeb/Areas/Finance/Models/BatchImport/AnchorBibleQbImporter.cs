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
    internal class AnchorBibleQbImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
                return BatchProcess(csv, date, fundid);
        }

        private class Row
        {
            public int N { get; set; }
            public int Pid { get; set; }
            public DateTime Dt { get; set; }
            public string Amount { get; set; }
            public int Fund { get; set; }
        }

        private static int? BatchProcess(CsvReader csv, DateTime date, int? fundid)
        {
            var prevdt = DateTime.MinValue;
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var rows = new List<Row>();

            var n = 0;
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                n += 1;
                if (!csv[4].HasValue())
                    continue;
                var row = new Row
                {
                    N = n,
                    Dt = csv[1].ToDate() ?? date,
                    Amount = csv[4],
                    Pid = csv[2].ToInt(),
                    Fund = csv[5].ToInt2() ?? fid
                };
                rows.Add(row);
            }

            var q = from row in rows
                    orderby row.Dt, row.N
                    select row;

            foreach(var row in q)
            {
                if (bh == null || row.Dt != prevdt)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(row.Dt, DateTime.Now);
                    prevdt = row.Dt;
                }
                var bd = BatchImportContributions.AddContributionDetail(row.Dt, row.Fund, row.Amount, row.Pid);
                bh.BundleDetails.Add(bd);
                bd.Contribution.PostingDate = date;
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId ?? 1;
        }
    }
}
