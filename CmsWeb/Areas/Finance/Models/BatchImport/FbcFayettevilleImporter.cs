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
            BundleHeader bundleHeader = null;
            var firstfund = BatchImportContributions.FirstFundId(); //TODO: use default fund id based on DBSetting w/ default to 1 if not set
            var fund = fundid ?? firstfund;

            while (csv.ReadNextRecord())
            {
                var isHeaderRow = csv[0].StartsWith("Date");

                if (isHeaderRow)
                {
                    continue;
                }

                var contributionDate = csv[1];
                var memberNumber = csv[2];
                var memberName = csv[3];
                var amount = csv[4];
                var checkNumber = csv[5];

                if(bundleHeader == null)
                {
                    bundleHeader = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                }

                var bundleDetails = BatchImportContributions.AddContributionDetail(date, fund, amount, checkNumber, "", "");
                bundleHeader.BundleDetails.Add(bundleDetails);
            }

            if(bundleHeader == null)
            {
                return null;
            }

            BatchImportContributions.FinishBundle(bundleHeader);
            return bundleHeader.BundleHeaderId;
        }
    }
}
