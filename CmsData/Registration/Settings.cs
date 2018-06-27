using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Xml.Serialization;
using CmsData;

namespace CmsData.Registration
{
    public partial class Settings
    {
        public List<Ask> AskItems { get; set; }
        public bool AskVisible(string name)
        {
            return AskItems.Find(aa => aa.Type == name) != null;
        }
        public Ask AskItem(string name)
        {
            return AskItems.Find(aa => aa.Type == name);
        }

        public void AddTextQuestion(string question)
        {
            var tx = AskItem("AskText") as AskText;
            if (tx == null)
            {
                tx = new AskText();
                AskItems.Add(tx);
            }
            var q = tx.list.SingleOrDefault(vv => vv.Question == question);
            if (q != null)
                return;
            q = new AskExtraQuestions.ExtraQuestion() { Question = question };
            tx.list.Add(q);
        }
        public void AddExtraQuestion(string question)
        {
            var eq = AskItem("AskExtraQuestions") as AskExtraQuestions;
            if (eq == null)
            {
                eq = new AskExtraQuestions();
                AskItems.Add(eq);
            }
            var q = eq.list.SingleOrDefault(vv => vv.Question == question);
            if (q != null)
                return;
            q = new AskExtraQuestions.ExtraQuestion() { Question = question };
            eq.list.Add(q);
        }
        public decimal? Deposit { get; set; }
        public string Shell { get; set; }
        public string ShellBs { get; set; }
        public decimal? Fee { get; set; }
        public decimal? ExtraFee { get; set; }
        public decimal? MaximumFee { get; set; }
        public bool ApplyMaxToOtherFees { get; set; }
        public bool AllowOnlyOne { get; set; }
        public bool TargetExtraValues { get; set; }
        public bool AllowReRegister { get; set; }
        public bool AllowSaveProgress { get; set; }
        public bool OtherFeesAddedToOrgFee { get; set; }
        public bool IncludeOtherFeesWithDeposit { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReminderSubject { get; set; }
        public string ReminderBody { get; set; }
        public string SupportSubject { get; set; }
        public string SupportBody { get; set; }
        public string SenderSubject { get; set; }
        public string SenderBody { get; set; }
        public string Terms { get; set; }
        public string ConfirmationTrackingCode { get; set; }

        public bool MemberOnly { get; set; }
        public bool AskDonation { get; set; }
        public string SpecialScript { get; set; }
        public string OnEnrollScript { get; set; }
        public string DonationLabel { get; set; }
        public string ExtraValueFeeName { get; set; }
        public bool NoReqBirthYear { get; set; }
        // ReSharper disable once InconsistentNaming
        public bool NotReqDOB { get; set; }
        public bool NotReqAddr { get; set; }
        public bool NotReqPhone { get; set; }
        public bool NotReqGender { get; set; }
        public bool NotReqMarital { get; set; }
        public bool NotReqZip { get; set; }
        public bool NotReqCampus { get; set; }
        public int? DonationFundId { get; set; }
        public string AccountingCode { get; set; }
        public int? TimeSlotLockDays { get; set; }
        public string GroupToJoin { get; set; }
        public bool AddAsProspect { get; set; }
        public int? TimeOut { get; set; }
        public bool DisallowAnonymous { get; set; }

        public string DonationFund()
        {
            return (from f in Db.ContributionFunds
                    where f.FundId == DonationFundId
                    select f.FundName).SingleOrDefault();
        }

        public string InstructionSelect { get; set; }
        public string InstructionFind { get; set; }
        public string InstructionOptions { get; set; }
        public string InstructionSpecial { get; set; }
        public string InstructionLogin { get; set; }
        public string InstructionSubmit { get; set; }
        public string InstructionSorry { get; set; }
        public string InstructionAll { get; set; }
        public string ThankYouMessage { get; set; }
        public string FinishRegistrationButton { get; set; }

        public List<OrgFee> OrgFees { get; set; }
        public List<AgeGroup> AgeGroups { get; set; }
        public TimeSlots TimeSlots { get; set; }
        public List<int> LinkGroupsFromOrgs { get; set; }
        public List<int> ValidateOrgIds { get; set; }

        public string ValidateOrgs
        {
            get { return string.Join(",", ValidateOrgIds); }
            set
            {
                if (value.HasValue())
                    ValidateOrgIds = (from i in value.Split(',')
                                      where i.ToInt() > 0 || i.ToInt() < 0
                                      select i.ToInt()).ToList();
                else
                    ValidateOrgIds = new List<int>();
            }
        }

        public class HtmlText2
        {
            public string Text { get; set; }
            public string Html { get; set; }
            public override string ToString()
            {
                if (Html.HasValue())
                    return Html;
                return Text;
            }
        }
        public class AgeGroup
        {
            public int Id { get; set; }
            [DisplayName("Sub-Group")]
            public string SmallGroup { get; set; }
            public int StartAge { get; set; }
            public int EndAge { get; set; }
            public decimal? Fee { get; set; }
        }

        public class OrgFee
        {
            public string Name;
            [DisplayName("Org Id")]
            public int? OrgId { get; set; }
            public decimal? Fee { get; set; }
        }

        public Organization org { get; set; }
        public int OrgId { get; set; }
        public CMSDataContext Db { get; set; }

        public Settings()
        {
            OrgFees = new List<OrgFee>();
            TimeSlots = new TimeSlots();
            AgeGroups = new List<AgeGroup>();
            LinkGroupsFromOrgs = new List<int>();
            ValidateOrgs = "";
            AskItems = new List<Ask>();
        }

        public void SetUniqueIds(string t)
        {
            var n = 0;
            foreach (var dd in AskItems.Where(aa => aa.Type == t))
                dd.UniqueId = n++;
        }

    }
}
