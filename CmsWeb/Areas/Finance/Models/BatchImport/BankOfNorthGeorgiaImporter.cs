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
    internal class BankOfNorthGeorgiaImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            return BatchProcessBankOfNorthGeorgia(text, date, fundid);
        }

        private static int? BatchProcessBankOfNorthGeorgia(string text, DateTime date, int? fundid)
        {
            BundleHeader bh = null;
            var sr = new StringReader(text);
            var line = "";
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
                        BatchImportContributions.FinishBundle(bh);
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
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


                var rt = csv[14];
                var ac = csv[20];
                var ck = csv[17];
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
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
