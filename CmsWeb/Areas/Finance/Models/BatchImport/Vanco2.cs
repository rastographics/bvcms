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
        public static int? BatchProcessVanco2(CsvReader csv, DateTime date, int? fundid)
        {
            var fundList = (from f in DbUtil.Db.ContributionFunds
                            orderby f.FundId
                            select f.FundId).ToList();

            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = FirstFundId();
            var fund = fundid != null && fundList.Contains(fundid ?? 0) ? fundid ?? 0 : firstfund;

            while (csv.ReadNextRecord())
            {
                var routing = "0";
                var checkno = "0";
                var account = csv[0];
                var amount = csv[7];
                var fundText = csv[11];

                if (bh == null)
                    bh = GetBundleHeader(date, DateTime.Now);

                var f = DbUtil.Db.FetchOrCreateFund(fundText);
                var bd = AddContributionDetail(date, f.FundId, amount, checkno, routing, account);

                bh.BundleDetails.Add(bd);
            }

            FinishBundle(bh);

            return bh.BundleHeaderId;
        }
    }
}