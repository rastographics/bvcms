using CmsData;
using CmsData.Codes;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class RegionsImporter2 : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
            {
                return BatchProcessRegions(csv, date, fundid);
            }
        }

        private static int? BatchProcessRegions(CsvReader csv, DateTime date, int? fundid)
        {
            var prevbundle = -1;

            var bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);

            var re = new Regex(
                @"(?<g1>d(?<rt>.*?)d\sc(?<ac>.*?)(?:c|\s)(?<ck>.*?))$
        |(?<g2>d(?<rt>.*?)d(?<ck>.*?)(?:c|\s)(?<ac>.*?)c[\s!]*)$
        |(?<g3>d(?<rt>.*?)d(?<ac>.*?)c(?<ck>.*?$))
        |(?<g4>c(?<ck>.*?)c\s*d(?<rt>.*?)d(?<ac>.*?)c\s*$)
        ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            var fieldCount = csv.FieldCount;
            var cols = csv.GetFieldHeaders();

            while (csv.ReadNextRecord())
            {

                if (!csv[12].Contains("Check"))
                {
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

                string accountNumber = null;
                string routingNumber = null;
                string checkingNumber = null;
                string micr = null;

                for (var c = 1; c < fieldCount; c++)
                {
                    switch (cols[c].ToLower())
                    {
                        case "deposit number":
                            var curbundle = csv[c].ToInt();
                            if (curbundle != prevbundle)
                            {
                                if (curbundle == 3143)
                                {
                                    foreach (var i in bh.BundleDetails)
                                    {
                                        Debug.WriteLine(i.Contribution.ContributionDesc);
                                        Debug.WriteLine(i.Contribution.BankAccount);
                                    }
                                }

                                BatchImportContributions.FinishBundle(bh);
                                bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);
                                prevbundle = curbundle;
                            }
                            break;
                        case "post amount":
                            bd.Contribution.ContributionAmount = csv[c].GetAmount();
                            break;
                        case "micr":
                            micr = csv[c];
                            break;
                        case "r/t":
                            routingNumber = csv[c];
                            break;
                        case "account number":
                            accountNumber = csv[c];
                            break;
                        case "check number":
                            checkingNumber = csv[c];
                            break;
                    }
                }

                if (string.IsNullOrEmpty(routingNumber) ||
                    string.IsNullOrEmpty(accountNumber) ||
                    string.IsNullOrEmpty(checkingNumber))
                {
                    var m = re.Match(micr);
                    routingNumber = m.Groups["rt"].Value;
                    accountNumber = m.Groups["ac"].Value;
                    bd.Contribution.CheckNo = m.Groups["ck"].Value.Truncate(20);
                }
                else
                {
                    bd.Contribution.CheckNo = checkingNumber;
                }

                var eac = Util.Encrypt(routingNumber + "|" + accountNumber);
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
