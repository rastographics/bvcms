using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using System.Web.Mvc;
using System.Text;
using System.Net.Mail;
using CmsData.Codes;
using Settings = CmsData.Registration.Settings;

namespace CmsWeb.Models
{
    public partial class OnlineRegPersonModel
    {
        private Dictionary<int, Settings> _settings;
        public Dictionary<int, Settings> settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = HttpContext.Current.Items["RegSettings"] as Dictionary<int, Settings>;
                    if (_settings == null)
                        Parent.ParseSettings();
                    _settings = HttpContext.Current.Items["RegSettings"] as Dictionary<int, Settings>;
                }
                return _settings;
            }
        }


        private Settings _setting;
        public Settings setting
        {
            get
            {
                if (_setting == null)
                {
                    if (org == null)
                        throw new Exception("no valid org");
                    if (settings == null)
                        throw new Exception("settings are null");
                    if (!settings.ContainsKey(org.OrganizationId))
                        settings[org.OrganizationId] = new Settings(org.RegSetting, DbUtil.Db, org.OrganizationId);
                    _setting = settings[org.OrganizationId];
                    AfterSettingConstructor();
                }
                return _setting;
            }
        }

        public int age
        {
            get
            {
                if (_person != null && _person.BirthDate.HasValue)
                    return GetAge(_person.BirthDate.Value);
                if (birthday.HasValue)
                    return GetAge(birthday.Value);
                return 0;
            }
        }

        [DisplayName("Gender")]
        public string GenderDisplay
        {
            get { return gender == 1 ? "Male" : gender == 2 ? "Female" : "not specified"; }
        }

        [DisplayName("Marital")]
        public string MarriedDisplay
        {
            get { return married == 10 ? "Single" : married == 20 ? "Married" : "not specified"; }
        }

        public string SpecialGivingFundsHeader
        {
            get { return DbUtil.Db.Setting("SpecialGivingFundsHeader", "Special Giving Funds"); }
        }

        public IEnumerable<Organization> GetOrgsInDiv()
        {
            return from o in DbUtil.Db.Organizations
                where o.DivOrgs.Any(di => di.DivId == divid)
                select o;
        }

        private bool RegistrationType(int typ)
        {
            if (divid == null)
                return false;
            var q = from o in GetOrgsInDiv()
                where o.RegistrationTypeId == typ
                select o;
            return q.Any();
        }

        public bool ShowCancelButton()
        {
            if (Parent.OnlyOneAllowed() || Parent.OnlineGiving() || Parent.ManagingSubscriptions() || IsCreateAccount())
                return false;
            // Show the Cancel link because we have either found the record or this is a new record
            return Found.HasValue || IsNew;
        }

        public bool UserSelectsOrganization()
        {
            return masterorgid.HasValue && masterorg.RegistrationTypeId == RegistrationTypeCode.UserSelectsOrganization2;
        }

        public bool ComputesOrganizationByAge()
        {
            return masterorgid.HasValue &&
                   masterorg.RegistrationTypeId == RegistrationTypeCode.ComputeOrganizationByAge2;
        }

        public bool ManageSubscriptions()
        {
            return masterorgid.HasValue && masterorg.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions2;
        }

        public bool RecordFamilyAttendance()
        {
            return org != null && org.RegistrationTypeId == RegistrationTypeCode.RecordFamilyAttendance;
        }

        public bool ChooseVolunteerTimes()
        {
            return RegistrationType(RegistrationTypeCode.ChooseVolunteerTimes);
        }

        public bool ManageGiving()
        {
            return RegistrationType(RegistrationTypeCode.ManageGiving);
        }

        public bool OnlineGiving()
        {
            if (org != null)
                return org.RegistrationTypeId == RegistrationTypeCode.OnlineGiving;
            return false;
        }

        public bool IsSpecialScript()
        {
            if (org != null)
                return org.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript;
            return false;
        }

        public string GetSpecialScript()
        {
            if (org == null) return "Organization not found.";

            var settings = new Settings(org.RegSetting, DbUtil.Db, org.OrganizationId);

            var body = DbUtil.Content(settings.SpecialScript, "Shell not found.");
            body = body.Replace("[action]", "/OnlineReg/SpecialRegistrationResults/" + org.OrganizationId, true);

            return body;
        }

        public bool OnlinePledge()
        {
            if (org != null)
                return org.RegistrationTypeId == RegistrationTypeCode.OnlinePledge;
            return false;
        }

        public bool NoReqBirthYear()
        {
            if (org != null)
                return setting.NoReqBirthYear;
            return settings.Values.Any(o => o.NoReqBirthYear);
        }

        public bool MemberOnly()
        {
            if (org != null)
                return setting.MemberOnly;
            if (divid == null)
                return false;
            return settings.Values.Any(o => o.MemberOnly);
        }

        private Organization _org;
        public Organization org
        {
            get
            {
                if (_org == null && orgid.HasValue)
                {
                    if (orgid == Util.CreateAccountCode)
                        _org = OnlineRegModel.CreateAccountOrg();
                    else
                        _org = DbUtil.Db.LoadOrganizationById(orgid.Value);
                }
                if (_org == null && classid.HasValue)
                    _org = DbUtil.Db.LoadOrganizationById(classid.Value);
                if (_org == null && (divid.HasValue || masterorgid.HasValue) && (Found == true || IsValidForNew))
                {
                    if (ComputesOrganizationByAge())
                        _org = GetAppropriateOrg();
                }
                if (_org != null)
                    orgid = _org.OrganizationId;
                return _org;
            }
        }

        private Organization _masterorg;
        public Organization masterorg
        {
            get
            {
                if (_masterorg != null)
                    return _masterorg;

                if (masterorgid.HasValue)
                    _masterorg = DbUtil.Db.LoadOrganizationById(masterorgid.Value);
                else
                {
                    if (org.RegistrationTypeId == RegistrationTypeCode.UserSelectsOrganization2
                        || org.RegistrationTypeId == RegistrationTypeCode.ComputeOrganizationByAge2
                        || org.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions2)
                    {
                        _masterorg = org;
                        masterorgid = orgid;
                        orgid = null;
                        _org = null;
                    }
                }
                return _masterorg;
            }
        }

        public string NoAppropriateOrgError;

        internal Organization GetAppropriateOrg()
        {
            NoAppropriateOrgError = null;

            var bestbirthdate = birthday;

            if (person != null && person.BirthDate.HasValue)
                bestbirthdate = person.BirthDate;

            if (!masterorgid.HasValue)
                return null;

            var cklist = masterorg.OrgPickList.Split(',').Select(o => o.ToInt()).ToList();
            var q = from o in DbUtil.Db.Organizations
                    where cklist.Contains(o.OrganizationId)
                    where gender == null || o.GenderId == gender || (o.GenderId ?? 0) == 0
                    select o;

            var list = q.ToList();
            var q2 = from o in list
                     where bestbirthdate >= o.BirthDayStart || o.BirthDayStart == null
                     where bestbirthdate <= o.BirthDayEnd || o.BirthDayEnd == null
                     select o;

            var oo = q2.FirstOrDefault();

            if (oo == null)
                NoAppropriateOrgError = "Sorry, cannot find an appropriate age group";
            else if (oo.RegEnd.HasValue && DateTime.Now > oo.RegEnd)
                NoAppropriateOrgError = "Sorry, registration has ended for {0}".Fmt(oo.OrganizationName);
            else if (oo.OrganizationStatusId == OrgStatusCode.Inactive)
                NoAppropriateOrgError = "Sorry, {0} is inactive".Fmt(oo.OrganizationName);
            else if (oo.ClassFilled == true)
                NoAppropriateOrgError = "Sorry, {0} is filled".Fmt(oo.OrganizationName);
            else if (oo.RegistrationTypeId == RegistrationTypeCode.None)
                NoAppropriateOrgError = "Sorry, {0} is not available".Fmt(oo.OrganizationName);
            else if (oo.RegistrationClosed == true)
                NoAppropriateOrgError = "Sorry, {0} is closed".Fmt(oo.OrganizationName);
            if (NoAppropriateOrgError.HasValue())
                oo = null;

            return oo;
        }

        public bool Finished()
        {
            return ShowDisplay() && OtherOK;
        }

        public bool NotFirst()
        {
            return Index > 0;
        }

        public bool ShowDisplay()
        {
            if (Found == true && IsValidForExisting)
                return true;
            if (org == null || IsFilled)
                return false;
            if (IsFamily)
                return IsValidForExisting;
            return IsNew && IsValidForNew;
        }

        public bool AnyOtherInfo()
        {
            if (org != null)
                if (org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
                    return false;
                else if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                    return false;
            return settings.Values.Any(s => s.AskItems.Any() || s.Deposit > 0);
        }

        public static void CheckNotifyDiffEmails(Person person, string fromemail, string regemail, string orgname,
            string phone)
        {
            MailAddress ma = null;
            try
            {
                ma = new MailAddress(regemail);
            }
            catch (Exception)
            {
            }
            if (ma != null)
            {
                /* If one of the email addresses we have on record
                 * is the same as the email address given in registration
                 * then no problem, (not different) */
                if (person.EmailAddress.HasValue() &&
                    string.Compare(ma.Address, person.EmailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                    return;
                if (person.EmailAddress2.HasValue() &&
                    String.Compare(ma.Address, person.EmailAddress2, StringComparison.OrdinalIgnoreCase) == 0)
                    return;
                /* So now we check to see if anybody in the famiy
                 * has the email address used in registration
                 * if so then that is OK too. */
                var flist = from fm in person.Family.People
                            where fm.PositionInFamilyId == PositionInFamily.PrimaryAdult
                            select fm;
                foreach (var fm in flist)
                {
                    if (fm.EmailAddress.HasValue() &&
                        string.Compare(ma.Address, fm.EmailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                        return;
                    if (fm.EmailAddress2.HasValue() &&
                        string.Compare(ma.Address, fm.EmailAddress2, StringComparison.OrdinalIgnoreCase) == 0)
                        return;
                }
            }
            if (!phone.HasValue())
                phone = DbUtil.Db.Setting("ChurchPhone", "(sorry, no phone # in settings)");

            /* so now we have a different email address than the one on record
             * we need to notify them */
            if (person.EmailAddress.HasValue() || person.EmailAddress2.HasValue())
            {
                var msg = DbUtil.Db.ContentHtml("DiffEmailMessage", Resource1.DiffEmailMessage);
                msg = msg.Replace("{name}", person.Name, ignoreCase: true)
                    .Replace("{first}", person.PreferredName, ignoreCase: true)
                    .Replace("{org}", orgname, ignoreCase: true)
                    .Replace("{phone}", phone, ignoreCase: true);
                var subj = "{0}, different email address than one on record".Fmt(orgname);
                DbUtil.Db.Email(fromemail, person, Util.ToMailAddressList(regemail), subj, msg, false);
            }
            else
            {
                var msg = DbUtil.Db.ContentHtml("NoEmailMessage", Resource1.NoEmailMessage);
                msg = msg.Replace("{name}", person.Name)
                    .Replace("{first}", person.PreferredName, ignoreCase: true)
                    .Replace("{org}", orgname, ignoreCase: true)
                    .Replace("{phone}", phone.FmtFone(), ignoreCase: true);
                var subj = "{0}, no email on your record".Fmt(orgname);
                DbUtil.Db.Email(fromemail, person, Util.ToMailAddressList(regemail), subj, msg, false);
            }
        }

        public OrganizationMember GetOrgMember()
        {
            if (org != null)
                return DbUtil.Db.OrganizationMembers.SingleOrDefault(m2 =>
                    m2.PeopleId == PeopleId && m2.OrganizationId == org.OrganizationId);
            return null;
        }

        public void PopulateFamilyMemberAttends()
        {
            if (IsNew)
            {
                FamilyAttend = new List<FamilyAttendInfo>()
                {
                    new FamilyAttendInfo()
                    {
                        Name = "{0} {1}".Fmt(FirstName, LastName),
                        PeopleId = -1,
                        Attend = true
                    }
                };
                return;
            }
            var q = from p in person.Family.People
                let om =
                    p.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == p.PeopleId && mm.OrganizationId == orgid)
                let forceattend = p.PeopleId == PeopleId
                orderby p.PositionInFamilyId, p.GenderId, p.Age descending
                select new FamilyAttendInfo()
                {
                    PeopleId = p.PeopleId,
                    Name = p.Name,
                    Age = p.Age,
                    Attend = (om != null && om.IsInGroup("Attended")) || forceattend
                };
            FamilyAttend = q.ToList();
        }

        public IEnumerable<SelectListItem> StateCodes()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.GetStateListUnknown(), "Code");
        }

        public IEnumerable<SelectListItem> Countries()
        {
            var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList(), null);
            list.Insert(0, new SelectListItem {Text = "(not specified)", Value = ""});
            return list;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (org == null)
                return string.Empty;
            sb.AppendFormat("Org: {0}<br/>\n", org.OrganizationName);
            if (PeopleId.HasValue)
            {
                sb.AppendFormat("{0}({1},{2},{3}), Birthday: {4}({5}), Phone: {6}, {7}, {8}<br />\n".Fmt(
                    person.Name, person.PeopleId, person.Gender.Code, person.MaritalStatus.Code,
                    person.DOB, person.Age, Phone.FmtFone(), person.EmailAddress, EmailAddress));
                if (ShowAddress)
                    sb.AppendFormat("&nbsp;&nbsp;{0}; {1}<br />\n", person.PrimaryAddress, person.CityStateZip);
            }
            else
            {
                sb.AppendFormat("{0} {1}({2},{3}), Birthday: {4}({5}), Phone: {6}, {7}<br />\n".Fmt(
                    FirstName, LastName, gender, married,
                    DateOfBirth, age, Phone.FmtFone(), EmailAddress));
                if (ShowAddress)
                    sb.AppendFormat("&nbsp;&nbsp;{0}; {1}<br />\n", AddressLineOne, City);
            }
            return sb.ToString();
        }

        public SelectListItem[] Funds()
        {
            if (ShouldPullSpecificFund())
                return ReturnContributionForSetting();

            return FundList();
        }

        public SelectListItem[] AllFunds()
        {
            if (ShouldPullSpecificFund())
                return ReturnContributionForSetting();

            return FullFundList();
        }

        public SelectListItem[] SpecialFunds()
        {
            if (ShouldPullSpecificFund())
                return ReturnContributionForSetting();

            return SpecialFundList();
        }

        private SelectListItem[] ReturnContributionForSetting()
        {
            var fund = DbUtil.Db.ContributionFunds.SingleOrDefault(f => f.FundId == setting.DonationFundId);
            return new[]
            {
                new SelectListItem
                {
                    Text = fund.FundName,
                    Value = fund.FundId.ToString()
                }
            };
        }

        public bool ShouldPullSpecificFund()
        {
            return Parent.OnlineGiving()
                   && !Parent.AskDonation()
                   && setting.DonationFundId.HasValue;
        }

        public static SelectListItem[] FullFundList()
        {
            return (from f in GetAllOnlineFunds()
                    where (f.OnlineSort > 0)
                    select new SelectListItem
                    {
                        Text = "{0}".Fmt(f.FundName),
                        Value = f.FundId.ToString()
                    }).ToArray();
        }

        public static SelectListItem[] FundList()
        {
            return (from f in GetAllOnlineFunds()
                    where (f.OnlineSort > 0 && f.OnlineSort <= 99)
                    select new SelectListItem
                    {
                        Text = "{0}".Fmt(f.FundName),
                        Value = f.FundId.ToString()
                    }).ToArray();
        }

        public static SelectListItem[] SpecialFundList()
        {
            return (from f in GetAllOnlineFunds()
                    where f.OnlineSort > 99
                    select new SelectListItem
                    {
                        Text = "{0}".Fmt(f.FundName),
                        Value = f.FundId.ToString()
                    }).ToArray();
        }

        private static IQueryable<ContributionFund> GetAllOnlineFunds()
        {
            return from f in DbUtil.Db.ContributionFunds
                   where f.FundStatusId == 1
                   orderby f.OnlineSort
                   select f;
        }

        private PythonEvents _pythonEvents;
        public PythonEvents PythonEvents
        {
            get { return _pythonEvents ?? (_pythonEvents = HttpContext.Current.Items["PythonEvents"] as PythonEvents); }
        }

        private readonly Dictionary<string, string> NameLookup = new Dictionary<string, string>()
        {
            {"first", "FirstName"},
            {"middle", "MiddleName"},
            {"last", "LastName"},
            {"suffix", "Suffix"},
            {"dob", "DateOfBirth"},
            {"phone", "Phone"},
            {"email", "EmailAddress"},

            {"homephone", "HomePhone"},
            {"address", "AddressLineOne"},
            {"address2", "AddressLineTwo"},
            {"city", "City"},
            {"state", "State"},
            {"zip", "ZipCode"},
            {"country", "Country"},
        };

        public string TranslateName(string name)
        {
            return NameLookup.ContainsKey(name) ? NameLookup[name] : name;
        }

    }

    public class FamilyAttendInfo
    {
        public int? PeopleId { get; set; }
        public bool Attend { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public int? GenderId { get; set; }
        public int? MaritalId { get; set; }
        public string Gender { get { return GenderId == 1 ? "Male" : "Female"; } }
        public string Marital { get { return MaritalId == 10 ? "Single" : "Married"; } }
    }
}