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
        public static int? BatchProcessFbcStark(string text, DateTime date, int? fundid)
        {
            var prevdt = DateTime.MinValue;
            BundleHeader bh = null;
            var sr = new StringReader(text);
            for (; ; )
            {
                var line = sr.ReadLine();
                if (line == null)
                    break;
                var csv = line.Split(',');
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

                var dtint = csv[3].ToLong();
                var y = (int)(dtint % 10000);
                var m = (int)(dtint / 1000000);
                var d = (int)(dtint / 10000) % 100;
                var dt = new DateTime(y, m, d);

                if (dt != prevdt)
                {
                    if (bh != null)
                        FinishBundle(bh);
                    bh = GetBundleHeader(dt, DateTime.Now);
                    prevdt = dt;
                }
                bd.Contribution.ContributionAmount = csv[5].GetAmount() / 100;

                string ck, rt, ac;
                ck = csv[4];
                rt = csv[6];
                ac = csv[7];

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