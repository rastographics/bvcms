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
        public static int? BatchProcessBankOfNorthGeorgia(string text, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var sr = new StringReader(text);
            string line = "";
            do
            {
                line = sr.ReadLine();
                if (line == null)
                    return null;
            } while (!line.Contains("Item ID"));
            var sep = ',';
            if (line.Contains("Item ID\t"))
                sep = '\t';

            for (; ; )
            {
                line = sr.ReadLine();
                if (line == null)
                    break;
                line = line.TrimStart();
                var csv = line.Split(sep);
                if (!csv[6].HasValue())
                    continue;

                if (csv[21] == "VDP")
                {
                    if (bh != null)
                        FinishBundle(bh);
                    bh = GetBundleHeader(date, DateTime.Now);
                    continue;
                }

                var bd = new BundleDetail
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Now,
                };
                var qf = from f in DbUtil.Db.ContributionFunds
                         where f.FundStatusId == 1
                         orderby f.FundId
                         select f.FundId;

                bd.Contribution = new Contribution
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Now,
                    ContributionDate = date,
                    FundId = fundid ?? qf.First(),
                    ContributionStatusId = 0,
                    ContributionTypeId = ContributionTypeCode.CheckCash,
                };


                string ck, rt, ac;
                rt = csv[14];
                ac = csv[20];
                ck = csv[17];
                bd.Contribution.ContributionAmount = csv[9].GetAmount();

                bd.Contribution.CheckNo = ck;
                var eac = Util.Encrypt(rt + "|" + ac);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                var pid = q.SingleOrDefault();
                if (pid != null)
                    bd.Contribution.PeopleId = pid;
                bd.Contribution.BankAccount = eac;
                bh.BundleDetails.Add(bd);
            }
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}