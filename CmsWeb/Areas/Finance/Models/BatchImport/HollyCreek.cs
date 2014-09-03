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
        public static int? BatchProcessHollyCreek(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = FirstFundId();
            var fund = fundid ?? firstfund;

            // 0 Amount, 1 Account, 2 Serial, 3 RoutingNumber, 4 TransmissionDate, 5 DepositTotal"))
            while (csv.ReadNextRecord())
            {
                var amount = csv[0];
                var account = csv[1];
                var checkno = csv[2];
                var routing = csv[3];
                if (bh == null)
                    bh = GetBundleHeader(date, DateTime.Now);
                var bd = AddContributionDetail(date, fund, amount, checkno, routing, account);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}