/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.IO;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class ForestvilleImporter : IContributionBatchImporter
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
            var list = new List<DepositRecord>();

            bh = BatchImportContributions.GetBundleHeader(date, now);
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var Amount = csv["Amount"];
                var Date = csv["Processing Date"].ToDate() ?? DateTime.Today;
                var Account = csv["Account"];
                var Routing = csv["RT"];
                var CheckNo = csv["Check"];

                var bd = BatchImportContributions.AddContributionDetail(Date, fid, Amount, CheckNo, Routing, Account);
                bd.Contribution.PostingDate = now;
                bh.BundleDetails.Add(bd);
            }
            BatchImportContributions.FinishBundle(bh);
            return bh?.BundleHeaderId ?? 0;
        }

    }
}
