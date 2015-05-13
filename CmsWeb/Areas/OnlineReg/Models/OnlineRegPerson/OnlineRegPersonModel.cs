using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData;
using CmsData.API;
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
        public int? classid { get; set; }
        public int? PeopleId { get; set; }
        public bool? Found { get; set; }
        public bool IsNew { get; set; }
        public bool QuestionsOK { get; set; }
        public bool LoggedIn { get { return Parent.UserPeopleId > 0; } }
        public bool IsValidForExisting { get; set; }
        public bool ShowAddress { get; set; }
        public bool ShowCountry { get; set; }
        public bool CreatingAccount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        internal int? bmon, byear, bday;

        public string DateOfBirth
        {
            get
            {
                return Util.FormatBirthday(byear, bmon, bday);
            }
            set
            {
                bday = null;
                bmon = null;
                byear = null;
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    bday = dt.Day;
                    bmon = dt.Month;
                    if (Regex.IsMatch(value, @"\d+/\d+/\d+"))
                        byear = dt.Year;
                }
                else
                {
                    int n;
                    if (int.TryParse(value, out n))
                        if (n >= 1 && n <= 12)
                            bmon = n;
                        else
                            byear = n;
                }
            }
        }

        public bool DateValid()
        {
            return bmon.HasValue && byear.HasValue && bday.HasValue;
        }

        public string Phone
        {
            get { return phone.FmtFone(); }
            set { phone = value; }
        }
        public string HomePhone
        {
            get { return homephone.FmtFone(); }
            set { homephone = value; }
        }

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
        public string emphone { get; set; }

        [DisplayName("Insurance Carrier"), StringLength(100)]
        public string insurance { get; set; }

        [DisplayName("Policy / Group #"), StringLength(100)]
        public string policy { get; set; }

        [DisplayName("Family Physician"), StringLength(100)]
        public string doctor { get; set; }
        [DisplayName("Physician Phone"), StringLength(15)]
        public string docphone { get; set; }

        [DisplayName("Medical Issues"), StringLength(200)]
        public string medical { get; set; }

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
            Parent = HttpContext.Current.Items["OnlineRegModel"] as OnlineRegModel;
        }
        private void AfterSettingConstructor()
        {
            if (_setting == null)
                return;
            var ndd = setting.AskItems.Count(aa => aa.Type == "AskDropdown");
            if (ndd > 0 && option == null)
                option = new string[ndd].ToList();

            var neqsets = setting.AskItems.Count(aa => aa.Type == "AskExtraQuestions");
            if (neqsets > 0 && ExtraQuestion == null)
            {
                ExtraQuestion = new List<Dictionary<string, string>>();
                for (var i = 0; i < neqsets; i++)
                    ExtraQuestion.Add(new Dictionary<string, string>());
            }
            var ntxsets = setting.AskItems.Count(aa => aa.Type == "AskText");
            if (ntxsets > 0 && Text == null)
            {
                Text = new List<Dictionary<string, string>>();
                for (var i = 0; i < ntxsets; i++)
                    Text.Add(new Dictionary<string, string>());
            }
            var nmi = setting.AskItems.Count(aa => aa.Type == "AskMenu");
            if (nmi > 0 && MenuItem == null)
            {
                MenuItem = new List<Dictionary<string, int?>>();
                for (var i = 0; i < nmi; i++)
                    MenuItem.Add(new Dictionary<string, int?>());
            }

            var ncb = setting.AskItems.Count(aa => aa.Type == "AskCheckboxes");
            if (ncb > 0 && Checkbox == null)
                Checkbox = new List<string>();

            if (!Suggestedfee.HasValue && setting.AskVisible("AskSuggestedFee"))
                Suggestedfee = setting.Fee;
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
            get { return FirstName + " " + LastName + " <" + EmailAddress + ">"; }
        }

        public int? MenuItemValue(int i, string s)
        {
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

        private DateTime _Birthday;
        public DateTime? birthday
        {
            get
            {
                if (_Birthday == DateTime.MinValue)
                    Util.BirthDateValid(bmon, bday, byear, out _Birthday);
                return _Birthday == DateTime.MinValue ? (DateTime?)null : _Birthday;
            }
        }
        public int GetAge(DateTime bd)
        {
            int y = bd.Year;
            if (y == Util.SignalNoYear)
                return 0;
            if (y < 1000)
                if (y < 50)
                    y = y + 2000;
                else y = y + 1900;
            var dt = DateTime.Today;
            int age = dt.Year - y;
            if (dt.Month < bd.Month || (dt.Month == bd.Month && dt.Day < bd.Day))
                age--;
            return age;
        }
        public string RegistrantProblem;
        public string CancelText = "Cancel this person";
        internal int count;

        private Person _person;
        private string phone;
        private string homephone;

        public Person person
        {
            get
            {
                if (_person == null)
                    if (PeopleId.HasValue)
                    {
                        _person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                        count = 1;
                    }
                    else
                    {
                        //_Person = SearchPeopleModel.FindPerson(first, last, birthday, email, phone, out count);

                        var list = DbUtil.Db.FindPerson(FirstName, LastName, birthday, EmailAddress, Phone.GetDigits()).ToList();
                        count = list.Count;
                        if (count == 1)
                            _person = DbUtil.Db.LoadPersonById(list[0].PeopleId.Value);
                        if (_person != null)
                            PeopleId = _person.PeopleId;
                    }
                return _person;
            }
        }
        public void AddPerson(Person p, int entrypoint)
        {
            Family f;
            if (p == null)
                f = new Family
                {
                    AddressLineOne = AddressLineOne,
                    AddressLineTwo = AddressLineTwo,
                    CityName = City,
                    StateCode = State,
                    ZipCode = ZipCode,
                    CountryName = Country,
                    HomePhone = HomePhone,
                };
            else
                f = p.Family;

            var position = DbUtil.Db.ComputePositionInFamily(age, false, f.FamilyId) ?? 10;
            _person = Person.Add(f, position,
                null, FirstName.Trim(), null, LastName.Trim(), DateOfBirth, married == 20, gender ?? 0,
                    OriginCode.Enrollment, entrypoint);
            person.EmailAddress = EmailAddress.Trim();
            person.SendEmailAddress1 = true;
            person.CampusId = DbUtil.Db.Setting("DefaultCampusId", "").ToInt2();
            person.CellPhone = Phone.GetDigits();

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("OnlineReg AddPerson {0}".Fmt(person.PeopleId));
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
            PeopleId = person.PeopleId;
        }
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

        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException("The method or operation is not implemented.");
        }
        internal string GetOthersInTransaction(Transaction transaction)
        {
            var TransactionPeopleIds = transaction.OriginalTrans.TransactionPeople.Select(tt => tt.PeopleId);
            var q = from pp in DbUtil.Db.People
                where TransactionPeopleIds.Contains(pp.PeopleId)
                where pp.PeopleId != PeopleId
                select pp.Name;
            var others = string.Join(",", q.ToArray());
            return others;
        }

    }
}
