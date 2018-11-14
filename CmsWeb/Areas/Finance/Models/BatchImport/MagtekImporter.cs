/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using CmsData;
using CmsData.Codes;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class MagtekImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            return BatchProcessMagTek(text, date);
        }

        private static int? BatchProcessMagTek(string lines, DateTime date)
        {
            var now = DateTime.Now;
            var bh = new BundleHeader
            {
                BundleHeaderTypeId = BundleTypeCode.ChecksAndCash,
                BundleStatusId = BundleStatusCode.Open,
                ContributionDate = date,
                CreatedBy = Util.UserId,
                CreatedDate = now,
                FundId = DbUtil.Db.Setting("DefaultFundId", "1").ToInt()
            };
            DbUtil.Db.BundleHeaders.InsertOnSubmit(bh);

            var re = new Regex(
@"(T(?<rt>[\d?]+)T(?<ac>[\d ?]*)U\s*(?<ck>[\d?]+))|
(CT(?<rt>[\d?]+)A(?<ac>[\d ?]*)C(?<ck>[\d?]+)M)",
                RegexOptions.IgnoreCase);
            var m = re.Match(lines);
            while (m.Success)
            {
                var rt = m.Groups["rt"].Value;
                var ac = m.Groups["ac"].Value;
                var ck = m.Groups["ck"].Value;
                var bd = new BundleDetail
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = now,
                };
                bh.BundleDetails.Add(bd);
                var qf = from f in DbUtil.Db.ContributionFunds
                         where f.FundStatusId == 1
                         orderby f.FundId
                         select f.FundId;

                bd.Contribution = new Contribution
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = now,
                    ContributionDate = date,
                    FundId = qf.First(),
                    ContributionStatusId = 0,
                    ContributionTypeId = ContributionTypeCode.CheckCash,
                };
                bd.Contribution.ContributionDesc = ck;
                var eac = Util.Encrypt(rt + "," + ac);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                var pid = q.SingleOrDefault();
                if (pid != null)
                {
                    bd.Contribution.PeopleId = pid;
                }

                bd.Contribution.BankAccount = eac;
                bd.Contribution.ContributionDesc = ck;

                m = m.NextMatch();
            }
            bh.TotalChecks = 0;
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            DbUtil.Db.SubmitChanges();
            return bh.BundleHeaderId;
        }
    }
}
