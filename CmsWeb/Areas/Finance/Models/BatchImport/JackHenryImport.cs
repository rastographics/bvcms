/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class BatchImportContributions
    {
        private static int? BatchProcessJackHenry(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<depositRecord>();
            var r = 2;
            while (csv.ReadNextRecord())
            {
                var peopleid = csv[6].ToInt();
                list.Add(new depositRecord()
                {
                    peopleid = peopleid,
                    amount = csv[9],
                    checkno = csv[10],
                    row = r++,
                    valid = DbUtil.Db.People.Any(pp => pp.PeopleId == peopleid),
                });
            }
            if (list.Any(vv => vv.valid == false))
            {
                throw new Exception("The following rows had peopleids that were not found<br>\n" +
                    string.Join(",", (list.Where(vv => vv.valid == false).Select(vv => vv.row)).ToArray()));
            }
            foreach (var i in list)
            {
                if (bh == null)
                    bh = GetBundleHeader(date, DateTime.Now);

                var bd = AddContributionDetail(date, fund, i.amount, i.checkno, i.routing, i.peopleid);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
                return null;
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}