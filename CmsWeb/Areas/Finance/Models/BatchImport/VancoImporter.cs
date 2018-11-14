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

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class VancoImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            if (fromFile)
            {
                using (var csv = new CsvReader(new StringReader(text), false))
                {
                    return BatchProcessVanco(csv, date, fundid);
                }
            }

            using (var csv = new CsvReader(new StringReader(text), false, '\t'))
            {
                return BatchProcessVanco(csv, date, fundid);
            }
        }

        private static int? BatchProcessVanco(CsvReader csv, DateTime date, int? fundid)
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
                var amount = csv[1];
                var fundText = csv[3];
                var fundNum = 0;

                int.TryParse(fundText, out fundNum);

                if (bh == null)
                {
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                }

                BundleDetail bd;

                if (fundList.Contains(fundNum))
                {
                    bd = BatchImportContributions.AddContributionDetail(date, fundNum, amount, checkno, routing, account);
                }
                else
                {
                    bd = BatchImportContributions.AddContributionDetail(date, fund, amount, checkno, routing, account);
                    bd.Contribution.ContributionDesc = $"Used default fund (fund requested: {fundText})";
                }

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}
