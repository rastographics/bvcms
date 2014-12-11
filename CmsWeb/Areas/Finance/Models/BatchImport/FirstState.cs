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
        public static int? BatchProcessFirstState(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = FirstFundId();

            var list = new List<depositRecord>();
/*
    Deposit Item
    Sequence #
    Item Date
    Item Status
    Customer Name
    Routing / Account #
    Check #
    Amount
    Deposit As
    Amount Source
    Image Quality Pass
    Scanned Count
*/
            while (csv.ReadNextRecord())
            {
                string rt, ac;
                var a = csv[5].Split('/');
                list.Add(new depositRecord()
                {
                    date = csv[2].ToDate(),
                    routing = a[0].Trim(),
                    account = a[1].Trim(),
                    amount = csv[7],
                    checkno = csv[6],
                    row = csv[1].ToInt(),
                    valid = csv[3].Trim() == "Deposited"
                });
            }
            DateTime? prevbatch = null;
            foreach(var r in list.Where(rr => rr.valid).OrderBy(rr => rr.row))
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

                bd = AddContributionDetail(r.date ?? date, fundid ?? firstfund, r.amount, r.checkno, r.routing, r.account);
                bh.BundleDetails.Add(bd);
            }
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}