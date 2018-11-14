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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class RegionsImporter : IContributionBatchImporter
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
            var curbundle = 0;

            var bh = BatchImportContributions.GetBundleHeader(date, DateTime.Now);

            Regex re = new Regex(
                @"(?<g1>d(?<rt>.*?)d\sc(?<ac>.*?)(?:c|\s)(?<ck>.*?))$
		|(?<g2>d(?<rt>.*?)d(?<ck>.*?)(?:c|\s)(?<ac>.*?)c[\s!]*)$
		|(?<g3>d(?<rt>.*?)d(?<ac>.*?)c(?<ck>.*?$))
		|(?<g4>c(?<ck>.*?)c\s*d(?<rt>.*?)d(?<ac>.*?)c\s*$)
		", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            int fieldCount = csv.FieldCount;
            var cols = csv.GetFieldHeaders();

            while (csv.ReadNextRecord())
            {
                if (!csv[12].Contains("Check"))
                {
                    continue;
                }

                var bd = new CmsData.BundleDetail
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
                string ac = null, rt = null;
                for (var c = 1; c < fieldCount; c++)
                {
                    switch (cols[c].ToLower())
                    {
                        case "deposit number":
                            curbundle = csv[c].ToInt();
                            if (prevbundle == -1)
                            {
                                prevbundle = curbundle;
                            }
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
                            var m = re.Match(csv[c]);
                            rt = m.Groups["rt"].Value;
                            ac = m.Groups["ac"].Value;
                            bd.Contribution.CheckNo = m.Groups["ck"].Value.Truncate(20);
                            break;
                    }
                }
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
