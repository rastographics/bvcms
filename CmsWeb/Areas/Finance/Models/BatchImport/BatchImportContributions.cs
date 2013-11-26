/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using UtilityExtensions;
using CmsData;
using CmsData.Codes;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Collections.Generic;

namespace CmsWeb.Models
{
    public partial class BatchImportContributions
    {
        public static int? BatchProcess(string text, DateTime date, int? fundid, bool fromFile)
        {
            var defaulthost = DbUtil.Db.Setting("DefaultHost", "");
            var dh = new List<string>()
            {
                "https://bellevue.bvcms.com",
                "https://northmobile.bvcms.com"
            };
            if (dh.Contains(defaulthost))
                using (var csv = new CsvReader(new StringReader(text), true))
                {
                    var names = csv.GetFieldHeaders();
                    if (names.Contains("ProfileID"))
                        return BatchProcessServiceU(csv, date);
                    return BatchProcessRegions(csv, date, fundid);
                }

            switch (DbUtil.Db.Setting("BankDepositFormat", "none").ToLower())
            {
                case "fcchudson":
                    using (var csv = new CsvReader(new StringReader(text), true, '\t'))
                        return BatchProcessFcchudson(csv, date, fundid);
                case "redeemer":
                    using (var csv = new CsvReader(new StringReader(text), true))
                        return BatchProcessRedeemer(csv, date, fundid);
                case "fbcfayetteville":
                    using (var csv = new CsvReader(new StringReader(text), true))
                        return BatchProcessFbcFayetteville(csv, date, fundid);
                case "ebcfamily":
                    using (var csv = new CsvReader(new StringReader(text), false))
                        return BatchProcessEbcfamily(csv, date, fundid);
                case "vanco":
                            if (fromFile)
                                using (var csv = new CsvReader(new StringReader(text), false))
                                    return BatchProcessVanco(csv, date, fundid);
                            using (var csv = new CsvReader(new StringReader(text), false, '\t'))
                                return BatchProcessVanco(csv, date, fundid);
                case "silverdale":
                    using (var csv = new CsvReader(new StringReader(text), true))
                        return BatchProcessSilverdale(csv, date, fundid);
                case "oakbrookchurch":
                    using (var csv = new CsvReader(new StringReader(text), true))
                        return BatchProcessOakbrookChurch(csv, date, fundid);
                case "bankofnorthgeorgia":
                    return BatchProcessBankOfNorthGeorgia(text, date, fundid);
                case "fbcstark":
                    return BatchProcessFbcStark2(text, date, fundid);
                case "discovercrosspoint":
                    return BatchProcessDiscoverCrosspoint(text, date, fundid);

            }

            if (text.StartsWith("From MICR :"))
                return BatchProcessMagTek(text, date);
            if (text.StartsWith("Financial_Institution"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessSunTrust(csv, date, fundid);
            if (text.StartsWith("Report Date,Report Requestor"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessSunTrust2(csv, date, fundid);
            if (text.StartsWith("TOTAL DEPOSIT AMOUNT"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessChase(csv, date, fundid);

            throw new Exception("missing deposit format");
        }

        private class depositRecord
        {
            public string batch { get; set; }
            public string routing { get; set; }
            public string account { get; set; }
            public string checkno { get; set; }
            public string amount { get; set; }
            public string type { get; set; }
        }
        private static BundleHeader GetBundleHeader(DateTime date, DateTime now)
        {
            var bh = new BundleHeader
                        {
                            BundleHeaderTypeId = BundleTypeCode.PreprintedEnvelope,
                            BundleStatusId = BundleStatusCode.Open,
                            ContributionDate = date,
                            CreatedBy = Util.UserId,
                            CreatedDate = now,
                            FundId = DbUtil.Db.Setting("DefaultFundId", "1").ToInt()
                        };
            DbUtil.Db.BundleHeaders.InsertOnSubmit(bh);
            bh.BundleStatusId = BundleStatusCode.Open;
            bh.BundleHeaderTypeId = BundleTypeCode.ChecksAndCash;
            return bh;
        }

        private static void FinishBundle(BundleHeader bh)
        {
            bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            DbUtil.Db.SubmitChanges();
        }

        private static int FirstFundId()
        {
            var firstfund = (from f in DbUtil.Db.ContributionFunds
                             where f.FundStatusId == 1
                             orderby f.FundId
                             select f.FundId).First();
            return firstfund;
        }

        private static BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, string account)
        {
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
                    FundId = fundid,
                    ContributionStatusId = 0,
                    ContributionTypeId = ContributionTypeCode.CheckCash,
                };
            bd.Contribution.ContributionAmount = amount.GetAmount();
            bd.Contribution.CheckNo = checkno;
            var eac = Util.Encrypt(routing + "|" + account);
            var q = from kc in DbUtil.Db.CardIdentifiers
                    where kc.Id == eac
                    select kc.PeopleId;
            var pid = q.SingleOrDefault();
            if (pid != null)
                bd.Contribution.PeopleId = pid;
            bd.Contribution.BankAccount = eac;
            return bd;
        }

        private static int? FindFund(string s)
        {
            var qf = from f in DbUtil.Db.ContributionFunds
                     where f.FundName == s
                     select f;
            var fund = qf.FirstOrDefault();
            if (fund == null)
                return null;
            return fund.FundId;
        }

        private static BundleDetail CreateContribution(DateTime date, int fundid)
        {
            var bd = new CmsData.BundleDetail
            {
                CreatedBy = Util.UserId,
                CreatedDate = Util.Now,
            };
            bd.Contribution = new Contribution
            {
                CreatedBy = Util.UserId,
                CreatedDate = Util.Now,
                ContributionDate = date,
                FundId = fundid,
                ContributionStatusId = 0,
                ContributionTypeId = ContributionTypeCode.CheckCash,
            };
            return bd;
        }
    }
}

