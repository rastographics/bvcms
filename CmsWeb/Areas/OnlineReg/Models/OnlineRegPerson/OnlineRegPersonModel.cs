using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Xml.Serialization;
using CmsWeb.Controllers;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.OnlineReg.Models
{
    [Serializable]
    public partial class OnlineRegPersonModel : IXmlSerializable
    {
        // IsValidForContinue = false means that there is some reason registrant cannot proceed to the questions page
        public bool IsValidForContinue { get; set; }
        public bool IsValidForNew { get; set; }

        public bool InMobileAppMode { get { return MobileAppMenuController.InMobileAppMode; } }
        public int? orgid { get; set; }
        public int? masterorgid { get; set; }
        public int? divid { get; set; }
        public int? PeopleId { get; set; }
        public bool? Found { get; set; }
        public bool IsNew { get; set; }
        public bool QuestionsOK { get; set; }

        private bool? loggedin;
        public bool LoggedIn
        {
            set { loggedin = value; }
            get
            {
                if(!loggedin.HasValue)
                    loggedin = Parent.UserPeopleId > 0;
                return loggedin ?? false;
            }
        }
        public bool IsValidForExisting { get; set; }
        public bool ShowAddress { get; set; }
        public bool ShowCountry { get; set; }
        public bool CreatingAccount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }

        // used for both cell and home phone (when a new family is created), otherwise just cell
        public string Phone
        {
            get { return phone.FmtFone(); }
            set { phone = value; }
        }

        public string Campus { get; set; }

        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        [DisplayName("State Abbr")]
        public string State { get; set; }
        [DisplayName("Postal Code")]
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public int? gender { get; set; }
        public int? married { get; set; }
        public bool IsFilled { get; set; }
        public string shirtsize { get; set; }

        [DisplayName("Emergency Contact"), StringLength(100)]
        public string emcontact { get; set; }

        [DisplayName("Emergency Phone"), StringLength(50)]
        public string emphone
        {
            get { return _emphone; }
            set { _emphone = value.Truncate(50); }
        }

        [DisplayName("Insurance Carrier"), StringLength(100)]
        public string insurance { get; set; }

        [DisplayName("Policy / Group #"), StringLength(100)]
        public string policy { get; set; }

        [DisplayName("Family Physician"), StringLength(100)]
        public string doctor { get; set; }
        [DisplayName("Physician Phone"), StringLength(15)]
        public string docphone { get; set; }

        [DisplayName("Allergies"), StringLength(200)]
        public string medical
        {
            get { return _medical; }
            set { _medical = value.Truncate(200); }
        }

        [DisplayName("Mother's Name (first last)"), StringLength(80)]
        public string mname { get; set; }
        [DisplayName("Father's Name (first last)"), StringLength(80)]
        public string fname { get; set; }

        public bool memberus { get; set; }
        public bool otherchurch { get; set; }

        [DisplayName("Interested in Coaching")]
        public bool? coaching { get; set; }

        [DisplayName("Receive Text Messages")]
        public bool? sms { get; set; }

        [DisplayName("Tylenol")]
        public bool? tylenol { get; set; }

        [DisplayName("Advil")]
        public bool? advil { get; set; }

        [DisplayName("Maalox")]
        public bool? maalox { get; set; }

        [DisplayName("Robitussin")]
        public bool? robitussin { get; set; }

        public bool? paydeposit { get; set; }
        public string request { get; set; }
        public string grade { get; set; }
        public int? ntickets { get; set; }

        public string gradeoption { get; set; }
        public bool IsFamily { get; set; }

        public int? MissionTripGoerId { get; set; }
        public bool MissionTripNoNoticeToGoer { get; set; }
        public decimal? MissionTripSupportGoer { get; set; }
        public decimal? MissionTripSupportGeneral { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? Suggestedfee { get; set; }
         
        public List<FamilyAttendInfo> FamilyAttend { get; set; }
        public Dictionary<int, decimal?> FundItem { get; set; }
        public Dictionary<string, string> SpecialTest { get; set; }
        public List<Dictionary<string, string>> ExtraQuestion { get; set; }
        public List<Dictionary<string, string>> Text { get; set; }
        public Dictionary<string, bool?> YesNoQuestion { get; set; }
        public List<string> option { get; set; }
        public List<string> Checkbox { get; set; }
        public List<Dictionary<string, int?>> MenuItem { get; set; }

        public OnlineRegPersonModel()
        {
            YesNoQuestion = new Dictionary<string, bool?>();
            FundItem = new Dictionary<int, decimal?>();
            Parent = HttpContextFactory.Current.Items["OnlineRegModel"] as OnlineRegModel;
        }

        public OnlineRegModel Parent;

        public int Index { get; set; }

        public bool LastItem()
        {
            return Index == Parent.List.Count - 1;
        }
        public bool NotLast()
        {
            return Index < Parent.List.Count - 1;
        }

        public bool SawExistingAccount;
        public bool CannotCreateAccount;
        public bool CreatedAccount;

        public string EmailAddress { get; set; }
        public string fromemail
        {
            get
            {
                if (!IsNew && !EmailAddress.HasValue())
                    return $"{FirstName} {LastName} <{person.EmailAddress}>";
                return $"{FirstName} {LastName} <{EmailAddress}>";
            }
        }

        public int? MenuItemValue(int i, string s)
        {
            if (s == null)
                return null;
            if (MenuItem[i].ContainsKey(s))
                return MenuItem[i][s];
            return null;
        }

        public decimal? FundItemValue(int n)
        {
            if (FundItem.ContainsKey(n))
                return FundItem[n];
            return null;
        }

        public string ExtraQuestionAnswer(int id, string question)
        {
            if(ExtraQuestion[id].ContainsKey(question))
                return ExtraQuestion[id][question];
            return "n/a";
        }
        public string TextAnswer(int id, string question)
        {
            if(Text[id].ContainsKey(question))
                return Text[id][question];
            return "n/a";
        }

        public string RegistrantProblem;
        public string CancelText = "Cancel this person";
        internal int count;

        private string phone;
        private string _emphone;
        private string _medical;

        public bool IsCreateAccount()
        {
            if (org != null)
            {
                if (org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
                    return true;
                if ((org.IsMissionTrip ?? false) && !Parent.SupportMissionTrip)
                    return true;
            }
            return CreatingAccount;
        }
        public bool IsMissionTrip()
        {
            return org != null && (org.IsMissionTrip ?? false);
        }

        public bool IsCommunityGroup()
        {
            return org != null && org.OrganizationType?.Code == "CG";
        }
    }
}
