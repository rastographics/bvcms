/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using CmsData.Codes;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class FirstStateImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true, '\t'))
                return BatchProcessFirstState(csv, date, fundid);
        }

        private static int? BatchProcessFirstState(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();

            var list = new List<DepositRecord>();
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
                var a = csv[5].Split('/');
                list.Add(new DepositRecord()
                {
                    Date = csv[2].ToDate(),
                    Routing = a[0].Trim(),
                    Account = a[1].Trim(),
                    Amount = csv[7],
                    CheckNo = csv[6],
                    Row = csv[1].ToInt(),
                    Valid = csv[3].Trim() == "Deposited"
                });
            }
            DateTime? prevbatch = null;
            foreach(var r in list.Where(rr => rr.Valid).OrderBy(rr => rr.Row))
            {
                if (r.Date != prevbatch)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(r.Date ?? date, DateTime.Now, BundleTypeCode.Online);
                    bh.DepositDate = r.Date;
                    prevbatch = r.Date;
                }

                BundleDetail bd;

                bd = BatchImportContributions.AddContributionDetail(r.Date ?? date, fundid ?? firstfund, r.Amount, r.CheckNo, r.Routing, r.Account);
                bh.BundleDetails.Add(bd);
            }
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}