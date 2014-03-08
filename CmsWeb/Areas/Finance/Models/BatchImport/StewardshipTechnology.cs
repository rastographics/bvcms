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
        public static int? BatchProcessStewardshipTechnology(CsvReader csv, DateTime date, int? fundid)
        {
            var fundList = (from f in DbUtil.Db.ContributionFunds
                            select new 
                            {
                                f.FundId,
                                f.FundName
                            }).ToList();

            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = FirstFundId();

            var list = new List<depositRecord>();
            csv.ReadNextRecord();
            while (csv.ReadNextRecord())
                list.Add(new depositRecord()
                {
                    date = csv[1].ToDate(),
                    account = csv[6],
                    amount = csv[2],
                    checkno = csv[0],
                    type = csv[3],
                });
            DateTime? prevbatch = null;
            foreach(var r in list.OrderBy(rr => rr.date))
            {
                if (r.date != prevbatch)
                {
                    if (bh != null)
                        FinishBundle(bh);
                    bh = GetBundleHeader(r.date ?? date, DateTime.Now, BundleTypeCode.Online);
                    bh.DepositDate = r.date;
                    prevbatch = r.date;
                }

                BundleDetail bd;

                var fid = (from f in fundList
                    where f.FundName == r.type
                    select f.FundId).SingleOrDefault();
                if(fid > 0)
                    bd = AddContributionDetail(r.date ?? date, fid, r.amount, r.checkno, "", r.account);
                else
                {
                    bd = AddContributionDetail(r.date ?? date, fundid ?? firstfund, r.amount, r.checkno, "", r.account);
                    bd.Contribution.ContributionDesc = "Used default fund (fund requested: {0})".Fmt(r.type);
                }
                bh.BundleDetails.Add(bd);
            }
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}