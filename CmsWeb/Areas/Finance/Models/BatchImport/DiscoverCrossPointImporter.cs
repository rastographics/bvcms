/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.IO;
using System.Linq;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class DiscoverCrossPointImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            return BatchProcessDiscoverCrosspoint(text, date, fundid);
        }

        private static int? BatchProcessDiscoverCrosspoint(string text, DateTime date, int? fundid)
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

                var dt = csv[2].ToDate().Value;

                if (dt != prevdt)
                {
                    if (bh != null)
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(dt, DateTime.Now);
                    prevdt = dt;
                }
                bd.Contribution.ContributionAmount = csv[1].ToDecimal();

                var ck = csv[3];
                var rt = csv[4];
                var ac = csv[0];

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
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
