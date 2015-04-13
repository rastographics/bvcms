/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class BatchImportContributions
    {
        public static int? BatchProcess(string text, DateTime date, int? fundid, bool fromFile)
        {
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Amount,Account,Serial,RoutingNumber,TransmissionDate,DepositTotal"))
                return new HollyCreekImporter().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Transaction Date,Status,Payment Type,Name on Account,Transaction Number,Ref. Number,Customer Number,Operation,Location Name,Amount,Check #"))
                return new JackHenryImporter().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 300)).Contains("textbox32,textbox30,textbox26,textbox22,textbox10,textbox7,DepositStatus,textbox3,SourceLocation,textbox4,submittedByValue,CaptureSequence,Sequence,AmountType,Amount,Serial,Account_1,RoutingNumber,AnalysisStatus,IsOverridden"))
                return new MetropolitanImporter().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Deposit Date,Account Number,Check Number,Check Amount,Routing Number"))
                return new RedeemerImporter().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Id,Date,Name,Donor Address,Donor City,Donor State,Donor Zip,Donor Id,Donor Email,Gross Amount,Net Amount,Fee,Number,Keyword,Status"))
                return new KindredImporter().RunImport(text, date, fundid, fromFile);
            if (!fromFile && text.Substring(0, Math.Min(text.Length, 200)).Contains("Customer ID\tMember Name\tPhone\tEmail\tTransaction Type\tProcess Date\tSettlement Date\tAmount\tReturn Date\tReturn/Fail Reason\tFund ID\tFund Name\tFund Text Message\tFrequency"))
                return new Vanco2Importer().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Date,Fund,Final Amount,Final Micr,Ck,"))
                return new TeaysValleyImporter().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Financial_Institution,Corporate_ID,Corporate_Name,Processing_Date,Deposit_Account,Site_ID,Deposit_ID,Deposit_Receipt_Time,ISN,Account_Number,Routing_and_Transit,Serial_Number,Tran_Code,Amount,"))
                return new GraceCcImporter().RunImport(text, date, fundid, fromFile);
            if (text.Substring(0, Math.Min(text.Length, 200)).Contains("Deposit Item,Sequence #,Item Date,Item Status,Customer Name,Routing / Account #,Check #,Amount,Deposit As,Amount Source,Image Quality Pass,Scanned Count"))
                return new EnonImporter().RunImport(text, date, fundid, fromFile);

            switch (DbUtil.Db.Setting("BankDepositFormat", "none").ToLower())
            {
                case "fcchudson":
                    return new FcchudsonImporter().RunImport(text, date, fundid, fromFile);
                case "fbcfayetteville":
                    return new FbcFayettevilleImporter().RunImport(text, date, fundid, fromFile);
                case "ebcfamily":
                    return new EbcfamilyImporter().RunImport(text, date, fundid, fromFile);
                case "vanco":
                    return new VancoImporter().RunImport(text, date, fundid, fromFile);
                case "stewardshiptechnology":
                    return new StewardshipTechnologyImporter().RunImport(text, date, fundid, fromFile);
                case "firststate":
                    return new FirstStateImporter().RunImport(text, date, fundid, fromFile);
                case "silverdale":
                    return new SilverdaleImporter().RunImport(text, date, fundid, fromFile);
                case "oakbrookchurch":
                    return new OakbrookChurchImporter().RunImport(text, date, fundid, fromFile);
                case "bankofnorthgeorgia":
                    return new BankOfNorthGeorgiaImporter().RunImport(text, date, fundid, fromFile);
                case "fbcstark":
                    return new FbcStark2Importer().RunImport(text, date, fundid, fromFile);
                case "discovercrosspoint":
                    return new DiscoverCrossPointImporter().RunImport(text, date, fundid, fromFile);
            }

            if (text.Substring(0, 40).Contains("Report Date,Report Requestor"))
            {
                DbUtil.LogActivity("BatchProcessRegions");
                return new RegionsImporter().RunImport(text, date, fundid, fromFile);
            }

            if (text.StartsWith("From MICR :"))
                return new MagtekImporter().RunImport(text, date, fundid, fromFile);

            if (text.StartsWith("Financial_Institution"))
                return new SunTrustImporter().RunImport(text, date, fundid, fromFile);

            if (text.StartsWith("TOTAL DEPOSIT AMOUNT"))
                return new ChaseImporter().RunImport(text, date, fundid, fromFile);

            if (text.StartsWith("1") && text.Substring(0, text.IndexOf(Environment.NewLine, StringComparison.Ordinal)).Length == 94)
                return new AchImporter().RunImport(text, date, fundid, fromFile);

            throw new Exception("unsupported import file");
        }

        internal static BundleHeader GetBundleHeader(DateTime date, DateTime now, int? btid = null)
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

        internal static void FinishBundle(BundleHeader bh)
        {
            bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            DbUtil.Db.SubmitChanges();
        }

        internal static int FirstFundId()
        {
            var firstfund = (from f in DbUtil.Db.ContributionFunds
                             where f.FundStatusId == 1
                             orderby f.FundId
                             select f.FundId).First();
            return firstfund;
        }

        internal static BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, string account)
        {
            var bd = new BundleDetail
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now
            };
            bd.Contribution = new Contribution
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now,
                ContributionDate = date,
                FundId = fundid,
                ContributionStatusId = 0,
                ContributionTypeId = ContributionTypeCode.CheckCash
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

        internal static BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, int peopleid)
        {
            var bd = new BundleDetail
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now
            };
            bd.Contribution = new Contribution
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now,
                ContributionDate = date,
                FundId = fundid,
                ContributionStatusId = 0,
                ContributionTypeId = ContributionTypeCode.CheckCash
            };
            bd.Contribution.ContributionAmount = amount.GetAmount();
            bd.Contribution.CheckNo = checkno;
            bd.Contribution.PeopleId = peopleid;
            return bd;
        }
    }
}