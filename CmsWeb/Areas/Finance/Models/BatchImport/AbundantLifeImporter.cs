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
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class AbundantLifeImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), hasHeaders:false))
                return BatchProcessAbundantLife(csv, date, fundid);
        }

        private static int? BatchProcessAbundantLife(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var prevbatch = "";
            while (csv.ReadNextRecord())
            {
                var batch = csv[5];
                if (bh == null || batch != prevbatch)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                    prevbatch = batch;
                    continue; // the first row of a batch is a total row
                }
                var amount = csv[3];
                var routing = csv[8];
                var account = csv[9];
                var checkno = csv[10];
                var bd = BatchImportContributions.AddContributionDetail(date, fid, amount, checkno, routing, account);
                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId;
        }
    }
}