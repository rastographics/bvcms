/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using CmsData;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class JackHenryImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
            {
                return BatchProcessJackHenry(csv, date, fundid);
            }
        }

        private static int? BatchProcessJackHenry(CsvReader csv, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var firstfund = BatchImportContributions.FirstFundId();
            var fund = fundid ?? firstfund;

            var list = new List<DepositRecord>();
            var r = 2;
            while (csv.ReadNextRecord())
            {
                var peopleid = csv[6].ToInt();
                list.Add(new DepositRecord()
                {
                    PeopleId = peopleid,
                    Amount = csv[9],
                    CheckNo = csv[10],
                    Row = r++,
                    Valid = DbUtil.Db.People.Any(pp => pp.PeopleId == peopleid),
                });
            }
            if (list.Any(vv => vv.Valid == false))
            {
                throw new Exception("The following rows had peopleids that were not found<br>\n" +
                    string.Join(",", (list.Where(vv => vv.Valid == false).Select(vv => vv.Row)).ToArray()));
            }
            foreach (var i in list)
            {
                if (bh == null)
                {
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                }

                var bd = BatchImportContributions.AddContributionDetail(date, fund, i.Amount, i.CheckNo, i.Routing, i.PeopleId);
                bh.BundleDetails.Add(bd);
            }
            if (bh == null)
            {
                return null;
            }

            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
