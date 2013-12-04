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
        private static int? BatchProcessFcchudson(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<depositRecord>();
            while (csv.ReadNextRecord())
                list.Add(new depositRecord()
                {
                    batch = csv[0],
                    routing = csv[1],
                    account = csv[2],
                    amount = csv[3],
                    checkno = csv[4],
                    type = csv[5],
                });
            var q = from r in list
                    where r.type == "Check"
                    select r;
            var prevbatch = "";
            foreach (var r in q)
            {
                if (r.batch != prevbatch)
                {
                    if (bh != null)
                        FinishBundle(bh);
                    bh = GetBundleHeader(r.batch.ToDate().Value, DateTime.Now);
                    prevbatch = r.batch;
                }
                var bd = AddContributionDetail(date, fund, r.amount, r.checkno, r.routing, r.account);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}