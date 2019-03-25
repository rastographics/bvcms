/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using CmsData;
using CmsData.Codes;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class OakbrookChurchImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            throw new NotImplementedException();
        }

        private static int? BatchProcessOakbrookChurch(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();

            BundleHeader bh = null;

            var qf = from f in DbUtil.Db.ContributionFunds
                     where f.FundStatusId == 1
                     orderby f.FundId
                     select f.FundId;

            while (csv.ReadNextRecord())
            {
                if (csv[16] == "Credit")
                {
                    if (bh != null)
                    {
                        BatchImportContributions.FinishBundle(bh);
                    }

                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                    continue;
                }
                if (bh == null)
                {
                    bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                }

                var bd = new BundleDetail
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Now,
                };

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
                rt = csv[11];
                ac = csv[13];
                ck = csv[14];
                bd.Contribution.ContributionAmount = csv[15].GetAmount();

                bd.Contribution.CheckNo = ck;
                var eac = Util.Encrypt(rt + "|" + ac);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                var pid = q.SingleOrDefault();
                if (pid != null)
                {
                    bd.Contribution.PeopleId = pid;
                }

                bd.Contribution.BankAccount = eac;
                bh.BundleDetails.Add(bd);
            }
            BatchImportContributions.FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
