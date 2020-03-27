using CmsData;
using CmsData.Codes;
using System;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class FbcStark2Importer : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            return BatchProcessFbcStark2(text, date, fundid);
        }

        private static int? BatchProcessFbcStark2(string text, DateTime date, int? fundid)
        {
            var db = DbUtil.Db;
            var prevdt = DateTime.MinValue;
            BundleHeader bh = null;
            var sr = new StringReader(text);
            string line = "";
            do
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    return null;
                }
            } while (!line.StartsWith("Batch ID"));
            var sep = ',';
            if (line.StartsWith("Batch ID\t"))
            {
                sep = '\t';
            }

            for (; ; )
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }

                var csv = line.Split(sep);
                var bd = new BundleDetail
                {
                    CreatedBy = db.UserId,
                    CreatedDate = DateTime.Now,
                };
                var qf = from f in db.ContributionFunds
                         where f.FundStatusId == 1
                         orderby f.FundId
                         select f.FundId;

                bd.Contribution = new Contribution
                {
                    CreatedBy = db.UserId,
                    CreatedDate = DateTime.Now,
                    ContributionDate = date,
                    FundId = fundid ?? qf.First(),
                    ContributionStatusId = 0,
                    ContributionTypeId = ContributionTypeCode.CheckCash,
                };

                var s = csv[3];
                var m = s.Substring(0, 2).ToInt();
                var d = s.Substring(2, 2).ToInt();
                var y = s.Substring(4, 2).ToInt() + 2000;
                var dt = new DateTime(y, m, d);

                if (dt != prevdt)
                {
                    if (bh != null)
                    {
                        BatchImportContributions.FinishBundle(bh);
                    }

                    bh = BatchImportContributions.GetBundleHeader(dt, DateTime.Now);
                    prevdt = dt;
                }

                var rt = csv[7];
                var ac = csv[8];
                var ck = csv[9];
                bd.Contribution.ContributionAmount = csv[10].GetAmount();

                bd.Contribution.CheckNo = ck;
                var eac = Util.Encrypt(rt + "|" + ac);
                var q = from kc in db.CardIdentifiers
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
