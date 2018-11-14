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
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class ServiceUImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text), true))
            {
                return Import(csv, date, fundid);
            }
        }

        private static int? Import(CsvReader csv, DateTime date, int? fundid)
        {
            var cols = csv.GetFieldHeaders();
            var now = DateTime.Now;

            var bh = BatchImportContributions.GetBundleHeader(date, now);

            while (csv.ReadNextRecord())
            {
                string ac = null, oth = null, first = null, last = null, addr = null, name = null, email = null;
                var dt = date;
                for (var c = 1; c < csv.FieldCount; c++)
                {
                    var col = cols[c].Trim();
                    switch (col)
                    {
                        case "Date Entered":
                            dt = csv[c].ToDate() ?? date;
                            break;
                        case "ProfileID":
                            ac = csv[c];
                            break;
                        case "First Name":
                            first = csv[c];
                            break;
                        case "Last Name":
                            last = csv[c];
                            break;
                        case "Full Name":
                            name = csv[c];
                            break;
                        case "Address":
                            addr = csv[c];
                            break;
                        case "Email Address":
                            email = csv[c];
                            break;
                        case "Designation for &quot;Other&quot;":
                            oth = csv[c];
                            break;
                    }
                }
                if (ac.ToInt() == 0)
                {
                    ac = email;
                }

                var eac = Util.Encrypt(ac);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                var pid = q.SingleOrDefault();
                string bankac = null;
                ExtraDatum ed = null;
                if (pid == null)
                {
                    bankac = eac;
                    string person;
                    if (last.HasValue())
                    {
                        person = $"{last}, {first}; {addr}";
                    }
                    else
                    {
                        person = $"{name}; {addr}";
                    }

                    ed = new ExtraDatum { Data = person, Stamp = Util.Now };
                }
                BundleDetail bd = null;
                var defaultfundid = DbUtil.Db.Setting("DefaultFundId", "1").ToInt();
                for (var c = 0; c < csv.FieldCount; c++)
                {
                    var col = cols[c].Trim();
                    if (col != "Amount" && !col.Contains("Comment") && csv[c].StartsWith("$") && csv[c].GetAmount() > 0)
                    {
                        var fid = FindFund(col);
                        bd = CreateContribution(date, fid ?? defaultfundid);
                        bd.Contribution.ContributionAmount = csv[c].GetAmount();
                        if (col == "Other")
                        {
                            col = oth;
                        }

                        if (!fundid.HasValue)
                        {
                            bd.Contribution.ContributionDesc = col;
                        }

                        if (ac.HasValue())
                        {
                            bd.Contribution.BankAccount = bankac;
                        }

                        bd.Contribution.PeopleId = pid;
                        bh.BundleDetails.Add(bd);
                        if (ed != null)
                        {
                            bd.Contribution.ExtraDatum = ed;
                        }
                    }
                }
            }
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }

        private static void FinishBundle(BundleHeader bh)
        {
            bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            DbUtil.Db.SubmitChanges();
        }

        private static int? FindFund(string s)
        {
            var qf = from f in DbUtil.Db.ContributionFunds
                     where f.FundName == s
                     select f;
            var fund = qf.FirstOrDefault();
            return fund?.FundId;
        }

        private static BundleDetail CreateContribution(DateTime date, int fundid)
        {
            var bd = new BundleDetail
            {
                CreatedBy = Util.UserId,
                CreatedDate = Util.Now
            };
            bd.Contribution = new Contribution
            {
                CreatedBy = Util.UserId,
                CreatedDate = Util.Now,
                ContributionDate = date,
                FundId = fundid,
                ContributionStatusId = 0,
                ContributionTypeId = ContributionTypeCode.CheckCash
            };
            return bd;
        }
    }
}
