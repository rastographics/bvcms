/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class StewardshipTechnologyImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            if (fromFile)
            {
                using (var csv = new CsvReader(new StringReader(text), false))
                {
                    return BatchProcessStewardshipTechnology(csv, date, fundid);
                }
            }

            using (var csv = new CsvReader(new StringReader(text), false, '\t'))
            {
                return BatchProcessStewardshipTechnology(csv, date, fundid);
            }
        }

        private static int? BatchProcessStewardshipTechnology(CsvReader csv, DateTime date, int? fundid)
        {
            var fundList = (from f in DbUtil.Db.ContributionFunds
                            select new
                            {
                                f.FundId,
                                f.FundName
                            }).ToList();

            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();

            var list = new List<DepositRecord>();
            csv.ReadNextRecord();
            while (csv.ReadNextRecord())
            {
                list.Add(new DepositRecord()
                {
                    Date = csv[1].ToDate(),
                    Account = csv[6],
                    Amount = csv[2],
                    CheckNo = csv[0],
                    Type = csv[3],
                });
            }

            DateTime? prevbatch = null;
            foreach (var r in list.OrderBy(rr => rr.Date))
            {
                if (r.Date != prevbatch)
                {
                    if (bh != null)
                    {
                        BatchImportContributions.FinishBundle(bh);
                    }

                    bh = BatchImportContributions.GetBundleHeader(r.Date ?? date, DateTime.Now, BundleTypeCode.Online);
                    bh.DepositDate = r.Date;
                    prevbatch = r.Date;
                }

                BundleDetail bd;

                var fid = (from f in fundList
                           where f.FundName == r.Type
                           select f.FundId).SingleOrDefault();
                if (fid > 0)
                {
                    bd = BatchImportContributions.AddContributionDetail(r.Date ?? date, fid, r.Amount, r.CheckNo, "", r.Account);
                }
                else
                {
                    bd = BatchImportContributions.AddContributionDetail(r.Date ?? date, fundid ?? firstfund, r.Amount, r.CheckNo, "", r.Account);
                    bd.Contribution.ContributionDesc = $"Used default fund (fund requested: {r.Type})";
                }
                bh.BundleDetails.Add(bd);
            }
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
