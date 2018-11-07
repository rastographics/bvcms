/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.IO;
using CmsData;
using CmsData.Codes;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class AbundantLifeImporter : IContributionBatchImporter
    {
        enum Columns
        {
            FundedDate,
            TransNumber,
            TransDate,
            Payer,
            Designation,
            Payment,
            Last4,
            GrossAmount,
            FeesWithheld,
            NetAmount,
            Misc,
            MerchantOrderNumber,
            BatchId
        };
        

        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
                return BatchProcessAbundantLife(csv, date, fundid);
        }

        private static int? BatchProcessAbundantLife(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            csv.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;

            var fid = fundid ?? BatchImportContributions.FirstFundId();

            var prevbatch = "";

            while (csv.ReadNextRecord())
            {
                if (!csv[Columns.TransNumber.ToInt()].HasValue())
                {
                    continue; // skip summary rows
                }

                var batch = csv[Columns.BatchId.ToInt()];
                if (bh == null || batch != prevbatch)
                {
                    if (bh != null) {
                        BatchImportContributions.FinishBundle(bh);
                    }
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                    prevbatch = batch;
                }

                var amount = csv[Columns.GrossAmount.ToInt()];

                var bd = new BundleDetail
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Now
                };

                bd.Contribution = new Contribution
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Now,
                    ContributionDate = date,
                    FundId = fid,
                    ContributionStatusId = ContributionStatusCode.Recorded,
                    ContributionTypeId = ContributionTypeCode.CheckCash,
                    ContributionAmount = amount.GetAmount()
                };

                bh.BundleDetails.Add(bd);
            }

            BatchImportContributions.FinishBundle(bh);

            return bh?.BundleHeaderId;
        }
    }
}
