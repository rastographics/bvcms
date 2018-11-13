/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.IO;
using System.Linq;
using CmsData;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class Vanco2Importer : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true, '\t'))
                return BatchProcessVanco2(csv, date, fundid);
        }

        private static int? BatchProcessVanco2(CsvReader csv, DateTime date, int? fundid)
        {
            var fundList = (from f in DbUtil.Db.ContributionFunds
                            orderby f.FundId
                            select f.FundId).ToList();

            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid != null && fundList.Contains(fundid ?? 0) ? fundid ?? 0 : firstfund;

            while (csv.ReadNextRecord())
            {
                var routing = "0";
                var checkno = "0";
                var account = csv[0];
                var amount = csv[7];
                var fundText = csv[11];

                if (bh == null)
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);

                var f = DbUtil.Db.FetchOrCreateFund(fundText);
                var bd = BatchImportContributions.AddContributionDetail(date, f.FundId, amount, checkno, routing, account);

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}
