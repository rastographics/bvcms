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
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Amount,Account,Serial,RoutingNumber,TransmissionDate,DepositTotal"))
                using (var csv = new CsvReader(new StringReader(text), hasHeaders:true))
                    return BatchProcessHollyCreek(csv, date, fundid);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Transaction Date,Status,Payment Type,Name on Account,Transaction Number,Ref. Number,Customer Number,Operation,Location Name,Amount,Check #"))
                using (var csv = new CsvReader(new StringReader(text), hasHeaders:true))
                    return BatchProcessJackHenry(csv, date, fundid);
            if (text.Substring(0, Math.Min(text.Length, 300)).Contains("textbox32,textbox30,textbox26,textbox22,textbox10,textbox7,DepositStatus,textbox3,SourceLocation,textbox4,submittedByValue,CaptureSequence,Sequence,AmountType,Amount,Serial,Account_1,RoutingNumber,AnalysisStatus,IsOverridden"))
                using (var csv = new CsvReader(new StringReader(text), hasHeaders:true))
                    return BatchProcessMetropolitan(csv, date, fundid);
            if(text.Substring(0, Math.Min(text.Length, 200)).Contains("Deposit Date,Account Number,Check Number,Check Amount,Routing Number"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessRedeemer(csv, date, fundid);
            if(text.Substring(0, Math.Min(text.Length, 200)).Contains("Id,Date,Name,Donor Address,Donor City,Donor State,Donor Zip,Donor Id,Donor Email,Gross Amount,Net Amount,Fee,Number,Keyword,Status"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessKindred(csv, date, fundid);
            if(!fromFile && text.Substring(0, Math.Min(text.Length, 200)).Contains("Customer ID\tMember Name\tPhone\tEmail\tTransaction Type\tProcess Date\tSettlement Date\tAmount\tReturn Date\tReturn/Fail Reason\tFund ID\tFund Name\tFund Text Message\tFrequency"))
                using (var csv = new CsvReader(new StringReader(text), true, '\t'))
                    return BatchProcessVanco2(csv, date, fundid);
            if(text.Substring(0, Math.Min(text.Length, 200)).Contains("Date,Fund,Final Amount,Final Micr,Ck,"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessTeaysValley(csv, date, fundid);
            if(text.Substring(0, Math.Min(text.Length, 200)).Contains("Financial_Institution,Corporate_ID,Corporate_Name,Processing_Date,Deposit_Account,Site_ID,Deposit_ID,Deposit_Receipt_Time,ISN,Account_Number,Routing_and_Transit,Serial_Number,Tran_Code,Amount,"))
                using (var csv = new CsvReader(new StringReader(text), hasHeaders:true))
                    return BatchProcessGraceCC(csv, date, fundid);

            switch (DbUtil.Db.Setting("BankDepositFormat", "none").ToLower())
            {
                case "fcchudson":
                    using (var csv = new CsvReader(new StringReader(text), true, '\t'))
                        return BatchProcessFcchudson(csv, date, fundid);
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
                case "stewardshiptechnology":
                    if (fromFile)
                        using (var csv = new CsvReader(new StringReader(text), false))
                            return BatchProcessStewardshipTechnology(csv, date, fundid);
                    using (var csv = new CsvReader(new StringReader(text), false, '\t'))
                        return BatchProcessStewardshipTechnology(csv, date, fundid);
                case "firststate":
                    using (var csv = new CsvReader(new StringReader(text), true, '\t'))
                            return BatchProcessFirstState(csv, date, fundid);
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
            if (text.Substring(0, 40).Contains("Report Date,Report Requestor"))
            {
                DbUtil.LogActivity("BatchProcessRegions");
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessRegions(csv, date, fundid);
            }

            if (text.StartsWith("From MICR :"))
                return BatchProcessMagTek(text, date);

            if (text.StartsWith("Financial_Institution"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessSunTrust(csv, date, fundid);

            if (text.StartsWith("TOTAL DEPOSIT AMOUNT"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessChase(csv, date, fundid);

            //if (text.StartsWith("Report Date,Report Requestor"))
            //    using (var csv = new CsvReader(new StringReader(text), true))
            //        return BatchProcessSunTrust2(csv, date, fundid);

            if (text.Contains("ProfileID"))
                using (var csv = new CsvReader(new StringReader(text), true))
                    return BatchProcessServiceU(csv, date);

            throw new Exception("unsupported import file");
        }

        private class depositRecord
        {
            public DateTime? date { get; set; }
            public string batch { get; set; }
            public int peopleid { get; set; }
            public string routing { get; set; }
            public string account { get; set; }
            public string checkno { get; set; }
            public string amount { get; set; }
            public string type { get; set; }
            public int row { get; set; }
            public bool valid { get; set; }
            public string description { get; set; }
        }
        private static BundleHeader GetBundleHeader(DateTime date, DateTime now, int? btid = null)
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
            bh.BundleHeaderTypeId = btid ?? BundleTypeCode.ChecksAndCash;
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
        private static BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, int peopleid)
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
            bd.Contribution.PeopleId = peopleid;
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

