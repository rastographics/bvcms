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
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class BatchImportContributions
    {
        public static int? BatchProcess(string text, DateTime date, int? fundid, bool fromFile)
        {
            var importer = FindMatchingImporter(text, fromFile);

            DbUtil.LogActivity($"BatchProcess: {importer.GetType().Name}");

            return importer.RunImport(text, date, fundid, fromFile);
        }

        public static BundleHeader GetBundleHeader(DateTime date, DateTime now, int? btid = null)
        {
            var opentype = DbUtil.Db.Roles.Any(rr => rr.RoleName == "FinanceDataEntry")
                ? BundleStatusCode.OpenForDataEntry
                : BundleStatusCode.Open;
            var bh = new BundleHeader
            {
                BundleHeaderTypeId = BundleTypeCode.PreprintedEnvelope,
                BundleStatusId = opentype,
                ContributionDate = date,
                CreatedBy = Util.UserId,
                CreatedDate = now,
                FundId = DbUtil.Db.Setting("DefaultFundId", "1").ToInt()
            };
            DbUtil.Db.BundleHeaders.InsertOnSubmit(bh);
            bh.BundleHeaderTypeId = btid ?? BundleTypeCode.ChecksAndCash;
            return bh;
        }

        public static void FinishBundle(BundleHeader bh)
        {
            bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            DbUtil.Db.SubmitChanges();
        }

        public static int FirstFundId()
        {
            var firstfund = (from f in DbUtil.Db.ContributionFunds
                             where f.FundStatusId == 1
                             orderby f.FundId
                             select f.FundId).First();
            return firstfund;
        }

        public static BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, string account)
        {
            var bd = NewBundleDetail(date, fundid, amount);
            bd.Contribution.CheckNo = checkno;
            int? pid = null;
            if (account.HasValue() && !account.Contains("E+"))
            {
                var eac = Util.Encrypt(routing + "|" + account);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                pid = q.SingleOrDefault();
                bd.Contribution.BankAccount = eac;
            }
            if (pid > 0)
                bd.Contribution.PeopleId = pid;
            return bd;
        }

        public static BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, int peopleid)
        {
            var bd = NewBundleDetail(date, fundid, amount);
            bd.Contribution.CheckNo = checkno;
            bd.Contribution.PeopleId = peopleid;
            return bd;
        }
        public static BundleDetail AddContributionDetail(DateTime date, int fundid, string amount, int peopleid)
        {
            var bd = NewBundleDetail(date, fundid, amount);
            bd.Contribution.PeopleId = peopleid;
            return bd;
        }

        public static BundleDetail NewBundleDetail(DateTime date, int fundid, string amount)
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
                ContributionTypeId = ContributionTypeCode.CheckCash,
                ContributionAmount = amount.GetAmount()
            };
            return bd;
        }

        private static IContributionBatchImporter FindMatchingImporter(string text, bool fromFile)
        {
            var subtext = text.Substring(0, Math.Min(text.Length, 300));

            if (subtext.Contains("Amount,Account,Serial,RoutingNumber,TransmissionDate,DepositTotal"))
                return new HollyCreekImporter();

            if (subtext.Contains("Transaction Date,Status,Payment Type,Name on Account,Transaction Number,Ref. Number,Customer Number,Operation,Location Name,Amount,Check #", ignoreCase: true))
                return new JackHenryImporter();

            if (subtext.Contains("textbox32,textbox30,textbox26,textbox22,textbox10,textbox7,DepositStatus,textbox3,SourceLocation,textbox4,submittedByValue,CaptureSequence,Sequence,AmountType,Amount,Serial,Account_1,RoutingNumber,AnalysisStatus,IsOverridden"))
                return new MetropolitanImporter();

            if (subtext.Contains("Deposit Date,Account Number,Check Number,Check Amount,Routing Number"))
                return new RedeemerImporter();

            if (subtext.Contains("Id,Date,Name,Donor Address,Donor City,Donor State,Donor Zip,Donor Id,Donor Email,Gross Amount,Net Amount,Fee,Number,Keyword,Status"))
                return new KindredImporter();

            if (subtext.Contains("Reference number,Gateway,Gross amount,Fees,Net amount,Type,Currency code,Donor external identifier,Donor first name,Donor last name"))
                return new Text2Give();

            if (!fromFile && subtext.Contains("Customer ID\tMember Name\tPhone\tEmail\tTransaction Type\tProcess Date\tSettlement Date\tAmount\tReturn Date\tReturn/Fail Reason\tFund ID\tFund Name\tFund Text Message\tFrequency"))
                return new Vanco2Importer();

            if (subtext.Contains("Date,Fund,Final Amount,Final Micr,Ck,"))
                return new TeaysValleyImporter();

            if (subtext.Contains("Financial_Institution,Corporate_ID,Corporate_Name,Processing_Date,Deposit_Account,Site_ID,Deposit_ID,Deposit_Receipt_Time,ISN,Account_Number,Routing_Transit,Serial_Number,Tran_Code,Amount,"))
                return new GraceCcImporter();

            if (subtext.Contains("\"Sequence #\",\"Item Date\",\"Status\",\"Customer Name\",\"Routing / Account #\",\"Check #\",\"Amount\",\"Deposit As\",\"Amount Source\",\"Image Quality Pass\",\"Scanned Count\""))
                return new EnonImporter();

            if (subtext.Contains("Sequence #,Item Date,Status,Customer Name,Routing / Account #,Check #,Amount,Deposit As,Amount Source,Image Quality Pass,Scanned Count"))
                return new EnonImporter();

            if (subtext.Contains("Type,Date,Member ID,Account,Amount,Fund"))
                return new AnchorBibleQbImporter();

            if (subtext.Contains("#Item Number,Item Date,Aux On Us,Route/Transit,Bank On Us,Amount,Deposit Number,Customer Name,Account Number,Account Name,Deposit Date"))
                return new TulsaFbcImporter();

            if (subtext.Contains("ItemId,Deposit_Tracking_No,Sequence,TransmissionID,TransmissionDate,ItemTypeName,CaptureSequence,Amount,IsCancelled,CapturedDate,UserEdited,MicrAuxOnUs,MicrOnUs,MicrOnUsA,MicrOnUsB,MicrOnUsC,MicrEPC,Account,Serial,TranCode,RoutingNumber,AnalysisStatus,CaptureUserName,AccountName"))
                return new CspcImporter();

            if (subtext.Contains("AMOUNT,FRB,CHECK NUMBER,ACCOUNT NUMBER,CAPTUREDATE"))
                return new HunterStreetImporter();

            if (subtext.Contains("Funded  Date,Trans. #,Trans. Date,Payer,Designation,Payment,Last 4,Gross Amount,Fees Withheld,Net Amount,Misc,Merchant Order #,Batch Id"))
                return new AbundantLifeImporter();

            if (subtext.Contains("Type,Date,Num,Name,Memo,Class,Split,Amount"))
                return new ChristLutheranVailImporter();

            if (subtext.Contains("!TRNS	TRNSID	TRNSTYPE	DATE	ACCNT	NAME	AMOUNT	DOCNUM	MEMO	CLASS	PAYMETH	PONUM	ADDR1	ADDR2	ADDR3	ADDR4	ADDR5	SADDR1	SADDR2	SADDR3	SADDR4	SADDR5	TOPRINT"))
                return new SimpleGiveImporter();

            if (subtext.Contains("date,time,transaction_id,transfer_id,transfer_date,transfer_net,first_name,last_name,email,give_amount,net_amount,fee_amount,status,member_id,campus_id,fund,fund_id,cause,cause_id,refund_amount,fund_code"))
                return new SubSplashImporter();
            if (subtext.Contains("date,transaction_id,transfer_id,transfer_date,transfer_net,first_name,last_name,email,give_amount,net_amount,fee_amount,status,member_id,campus_id,fund,fund_id,cause,cause_id,refund_amount"))
                return new SubSplashImporter();

            if (subtext.Contains("Posting Date,Account #,Deposit Ticket Sequence #,Deposit Amount,Debit Account,Item Serial #,Item Sequence #,Item Amount"))
                return new LongViewHeightsImporter();

            if (subtext.Contains("Site ID,Customer Name,Deposit ID,Processing Date,Deposit Account,Deposit Report,Batch ID,Transaction IDs,Type,AUX/Serial,RIC,RT,WAUX/FLD4,Account,Check,Amount,Item Type"))
                return new ForestvilleImporter();

            if (subtext.Contains("paymentID,orderID,userID,terminalID,terminalType,transType,transDate,settleDate,transID,batchNum,apiResponse,paymentType,paymentTotal,splitPayment,cash,credit,lastfour,nameonCard,echeck,checknum,giftcard,amountpaid,balance,cartid,status,firstName,lastName"))
                return new ClearGiveImporter();

            switch (DbUtil.Db.Setting("BankDepositFormat", "none").ToLower())
            {
                case "crossroadsbaptist":
                    return new CrossroadsBaptistImporter();
                case "stonyrunfriends":
                    return new StonyRunFriendsImporter();
                case "fcchudson":
                    return new FcchudsonImporter();
                case "fbcfayetteville":
                    return new FbcFayettevilleImporter();
                case "ebcfamily":
                    return new EbcfamilyImporter();
                case "vanco":
                    return new VancoImporter();
                case "stewardshiptechnology":
                    return new StewardshipTechnologyImporter();
                case "firststate":
                    return new FirstStateImporter();
                case "silverdale":
                    return new SilverdaleImporter();
                case "oakbrookchurch":
                    return new OakbrookChurchImporter();
                case "bankofnorthgeorgia":
                    return new BankOfNorthGeorgiaImporter();
                case "fbcstark":
                    return new FbcStark2Importer();
                case "discovercrosspoint":
                    return new DiscoverCrossPointImporter();
                case "regionsimporter2":
                    return new RegionsImporter2();
            }

            if (text.Substring(0, 40).Contains("Report Date,Report Requestor"))
                return new RegionsImporter();

            if (text.StartsWith("From MICR :"))
                return new MagtekImporter();

            if (text.StartsWith("Financial_Institution"))
                return new SunTrustImporter();

            if (text.StartsWith("TOTAL DEPOSIT AMOUNT"))
                return new ChaseImporter();

            if (text.Substring(0, text.IndexOf(Environment.NewLine, StringComparison.Ordinal)).Contains("TransmissionDate,MerchantName,DepositDate,Account,DepositTotal,DebitCount,DepositStatus,TrackingNo,SourceLocation,CreatedBy,submittedByValue,CaptureSequence,Sequence,AmountType,Amount,Serial,AccountNo,RoutingNumber,AnalysisStatus,OverrideIndicator"))
                return new CapitalCityImporter();

            if (text.StartsWith("1") && text.Substring(0, text.IndexOf(Environment.NewLine, StringComparison.Ordinal)).Length == 94)
                return new AchImporter();

            if (text.Contains("ProfileID"))
                return new ServiceUImporter();

            if (text.StartsWith("Id,Recipient,Date,Time,Currency,Amount,Status,Payment Method,Payer Name,Email address,Mobile Number,Source,Method"))
                return new PushPayImporter();

            throw new Exception("unsupported import file");
        }
    }
}
