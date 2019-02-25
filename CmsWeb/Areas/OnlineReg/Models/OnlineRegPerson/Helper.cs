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

namespace CmsWeb.Areas.OnlineReg.Models
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
                    _settings = HttpContextFactory.Current.Items["RegSettings"] as Dictionary<int, Settings>;
                    if (_settings == null)
                        Parent.ParseSettings();
                    _settings = HttpContextFactory.Current.Items["RegSettings"] as Dictionary<int, Settings>;
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
                    if (ManageSubscriptions() && masterorgid.HasValue)
                        return settings[masterorgid.Value];
                    if (org == null)
                        throw new Exception("no valid org");
                    if (settings == null)
                        throw new Exception("settings are null");
                    if (!settings.ContainsKey(org.OrganizationId))
                        settings[org.OrganizationId] = DbUtil.Db.CreateRegistrationSettings(org.OrganizationId);
                    _setting = settings[org.OrganizationId];
                    AfterSettingConstructor();
                }
                return _setting;
            }
        }

        public int? age
        {
            get
            {
                if (_person?.BirthDate != null)
                    return GetAge(_person.BirthDate.Value);
                if (birthday.HasValue)
                    return byear == null || birthday.Value.Year == Util.SignalNoYear
                        ? (int?)null
                        : GetAge(birthday.Value);
                return null;
            }
        }

        [DisplayName("Gender")]
        public string GenderDisplay => gender == 1 ? "Male" : gender == 2 ? "Female" : "not specified";

        [DisplayName("Marital")]
        public string MarriedDisplay => married == 10 ? "Single" : married == 20 ? "Married" : "not specified";

        public string SpecialGivingFundsHeader => DbUtil.Db.Setting("SpecialGivingFundsHeader", "Special Giving Funds");

        public bool UserSelectsOrganization()
        {
            return masterorgid.HasValue && masterorg.RegistrationTypeId == RegistrationTypeCode.UserSelects;
        }

        public bool ComputesOrganizationByAge()
        {
            return masterorgid.HasValue &&
                   masterorg.RegistrationTypeId == RegistrationTypeCode.ComputeOrgByAge;
        }

        public bool DisplaySelectedOrg()
        {
            return (UserSelectsOrganization() || ComputesOrganizationByAge()) && org != null;
        }

        public bool ShowCancelButton()
        {
            if (Parent.OnlyOneAllowed() || Parent.OnlineGiving() || Parent.ManagingSubscriptions() || IsCreateAccount())
                return false;
            // Show the Cancel link because we have either found the record or this is a new record
            return Found.HasValue || IsNew;
        }

        public bool ManageSubscriptions()
        {
            return masterorgid.HasValue && masterorg.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions;
        }

        public bool RecordFamilyAttendance()
        {
            return org != null && org.RegistrationTypeId == RegistrationTypeCode.RecordFamilyAttendance;
        }

        public bool ChooseVolunteerTimes()
        {
            return org?.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes;
        }

        public bool ManageGiving()
        {
            return org?.RegistrationTypeId == RegistrationTypeCode.ManageGiving;
        }

        public bool OnlineGiving()
        {
            return org?.RegistrationTypeId == RegistrationTypeCode.OnlineGiving;
        }

        public bool IsSpecialScript()
        {
            return org?.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript;
        }

        public bool IsSpecialReg()
        {
            return ManageSubscriptions()
                   || OnlinePledge()
                   || ManageGiving()
                   || Parent.ChoosingSlots();
        }

        public bool RegistrationFull()
        {
            if (org == null)
                return false;
            if (!Parent.SupportMissionTrip)
                IsFilled = org.RegLimitCount(DbUtil.Db) >= org.Limit;
            return IsFilled;
        }

        public bool CanRegisterInCommunityGroup(DateTime enrollmentCutoff)
        {
            if (PeopleId == null)
                return false;

            var db = DbUtil.DbReadOnly;

            var results = from om in db.OrganizationMembers
                join org in db.Organizations on om.OrganizationId equals org.OrganizationId
                where om.PeopleId == PeopleId && om.EnrollmentDate >= enrollmentCutoff && org.OrganizationType.Code == "CG"
                select om.PeopleId;
            
            return !results.Any();
        }

        public string GetSpecialScript()
        {
            if (org == null) return "Organization not found.";

            var settings = DbUtil.Db.CreateRegistrationSettings(org.OrganizationId);

            var body = DbUtil.Content(settings.SpecialScript, "Shell not found.");
            body = body.Replace("[action]", "/OnlineReg/SpecialRegistrationResults/" + org.OrganizationId, true);

            return body;
        }

        public bool OnlinePledge()
        {
            return org?.RegistrationTypeId == RegistrationTypeCode.OnlinePledge;
        }

        public bool MemberOnly()
        {
            if (org != null)
                return setting.MemberOnly;
            if (divid == null)
                return false;
            return settings.Values.Any(o => o.MemberOnly);
        }

        private bool? noorg = null;
        private Organization _org;
        public Organization org
        {
            get
            {
                if (noorg == true)
                    return null;
                if (orgid == null)
                {
                    _org = null;
                    _setting = null;
                }
                if (_org == null && orgid.HasValue)
                    _org = DbUtil.Db.LoadOrganizationById(orgid.Value);
                if (_org == null && (divid.HasValue || masterorgid.HasValue))
                {
                    if (ComputesOrganizationByAge())
                        _org = GetAppropriateOrg();
                }
                if (_org != null)
                    orgid = _org.OrganizationId;
                if (!orgid.HasValue)
                    noorg = true;
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
                    if (org.RegistrationTypeId == RegistrationTypeCode.UserSelects
                        || org.RegistrationTypeId == RegistrationTypeCode.ComputeOrgByAge
                        || org.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions)
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

        private Organization GetAppropriateOrg()
        {
            if (_org != null)
                return _org;
            NoAppropriateOrgError = null;

            if (!masterorgid.HasValue)
                return null;

            var cklist = masterorg.OrgPickList.Split(',').Select(o => o.ToInt()).ToList();
            var bestgender = gender ?? person?.GenderId;
            var q = from o in DbUtil.Db.Organizations
                    where cklist.Contains(o.OrganizationId)
                    where bestgender == null || o.GenderId == bestgender || (o.GenderId ?? 0) == 0
                    select o;

            var list = q.ToList();
            var q2 = from o in list
                     where BestBirthday >= o.BirthDayStart || o.BirthDayStart == null
                     where BestBirthday <= o.BirthDayEnd || o.BirthDayEnd == null
                     select o;

            var oo = q2.FirstOrDefault();

            if (oo == null)
            {
                NoAppropriateOrgError = person?.BirthDate != null 
                    ? "Sorry, cannot find an appropriate age group for the birthday we have on record for you" 
                    : "Sorry, cannot find an appropriate age group";
            }
            else if (oo.RegEnd.HasValue && Util.Now > oo.RegEnd)
                NoAppropriateOrgError = $"Sorry, registration has ended for {oo.OrganizationName}";
            else if (oo.OrganizationStatusId == OrgStatusCode.Inactive)
                NoAppropriateOrgError = $"Sorry, {oo.OrganizationName} is inactive";
            else if (oo.ClassFilled == true)
                NoAppropriateOrgError = $"Sorry, {oo.OrganizationName} is filled";
            else if (oo.RegistrationTypeId == RegistrationTypeCode.None)
                NoAppropriateOrgError = $"Sorry, {oo.OrganizationName} is not available";
            else if (oo.RegistrationClosed == true)
                NoAppropriateOrgError = $"Sorry, {oo.OrganizationName} is closed";
            if (NoAppropriateOrgError.HasValue())
                oo = null;

            return oo;
        }

        public bool Finished()
        {
            return FinishedFindingOrAddingRegistrant && QuestionsOK;
        }

        public bool NotFirst()
        {
            return Index > 0;
        }

        public bool ShowLogin
        {
            get
            {
                if (Parent.List.Count != 1) // must be on first person
                    return false;
                if (IsNew) // must not be a new person
                    return false;
                if (Found == true) // must not already have been found
                    return false;
                if (LoggedIn) // must not already be logged in
                    return false;
                if (Util.UserPeopleId > 0)
                    return false;
                if (QuestionsOK) // must not already be done with questions
                    return false;
                return true;
            }
        }

        public bool FinishedFindingOrAddingRegistrant
        {
            get
            {
                if (Parent.UserNeedsSelection && LastItem())
                    return false;
                if (Found == true && IsValidForExisting)
                    return true;
                if (org == null || IsFilled)
                    return false;
                if (IsFamily)
                    return IsValidForExisting;
                return IsNew && IsValidForNew;
            }
        }

        public bool HasQuestions()
        {
            if (org != null)
                if (org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
                    return false;
                else if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                    return false;
            return settings.Values.Any(s => s.AskItems.Any() || s.Deposit > 0);
        }

        public void CheckNotifyDiffEmails()
        {
            var orgname = org.OrganizationName;
            MailAddress ma = null;
            try
            {
                ma = new MailAddress(EmailAddress);
            }
            catch (Exception)
            {
            }
            if (ma == null) // no need to continue since they must have a good email on their record to have gotten this far.
                return;

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
                    .Replace("{phone}", DbUtil.Db.Setting("ChurchPhone", "ChurchPhone"));
                var subj = $"{orgname}, different email address than one on record";
                DbUtil.Db.Email(fromemail, person, Util.ToMailAddressList(EmailAddress), subj, msg, false);
                Log("DiffEmailFromRecord");
            }
            else
            {
                var msg = DbUtil.Db.ContentHtml("NoEmailMessage", Resource1.NoEmailMessage);
                msg = msg.Replace("{name}", person.Name)
                    .Replace("{first}", person.PreferredName, ignoreCase: true)
                    .Replace("{org}", orgname, ignoreCase: true)
                    .Replace("{phone}", DbUtil.Db.Setting("ChurchPhone", "ChurchPhone"));
                var subj = $"{orgname}, no email on your record";
                DbUtil.Db.Email(fromemail, person, Util.ToMailAddressList(EmailAddress), subj, msg, false);
                Log("NoEmailOnRecord");
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
                        Name = $"{FirstName} {LastName}",
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
                        Attend = (om != null && om.IsInGroup("Attending")) || forceattend
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
            var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList().Where(c => c.Code != "NA"), null);
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "" });
            return list;
        }
        public IEnumerable<SelectListItem> Campuses()
        {
            var campusids = (from cid in DbUtil.Db.Setting("CampusIds", "").Split(',')
                             where cid.HasValue()
                             select cid.ToInt()).ToArray();
            var qc = from c in DbUtil.Db.Campus
                where campusids.Length == 0 || campusids.Contains(c.Id)
                select c;
            qc = DbUtil.Db.Setting("SortCampusByCode") 
                ? qc.OrderBy(cc => cc.Code) 
                : qc.OrderBy(cc => cc.Description);
            var q = from c in qc
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Description,
                        Selected = c.Id == Campus.ToInt()
                    };
            var list = q.ToList();
            var text = RequiredCampus() 
                ? $"Choose {Util2.CampusLabel}" 
                : "Optional";
            list.Insert(0, new SelectListItem {Value = "0", Text = text});
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
                sb.Append($"{person.Name}({person.PeopleId},{person.Gender.Code},{person.MaritalStatus.Code}), Birthday: {person.DOB}({Person.AgeDisplay(person.Age, person.PeopleId)}), Phone: {Phone.FmtFone()}, {person.EmailAddress}, {EmailAddress}<br />\n");
                if (ShowAddress)
                    sb.AppendFormat("&nbsp;&nbsp;{0}; {1}<br />\n", person.PrimaryAddress, person.CityStateZip);
            }
            else
            {
                sb.Append($"{FirstName} {LastName}({gender},{married}), Birthday: {DateOfBirth}({age}), Phone: {Phone.FmtFone()}, {EmailAddress}<br />\n");
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

        public SelectListItem[] DesignatedDonationFund()
        {
            if (ShouldPullSpecificFund())
                return ReturnContributionForSetting();

            return new SelectListItem[0];
        }

        private SelectListItem[] ReturnContributionForSetting()
        {
            var fund = DbUtil.Db.ContributionFunds.SingleOrDefault(f => f.FundId == setting.DonationFundId);
            if (fund == null)
                throw new Exception($"DonationFundId {setting.DonationFundId} does not point to a fund");
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

        public static SelectListItem[] EntireFundList()
        {
            return (from f in GetAllOnlineFunds()
                select new SelectListItem
                {
                    Text = $"{f.FundName}",
                    Value = f.FundId.ToString()
                }).ToArray();
        }

        public static SelectListItem[] FullFundList(IList<string> defaultFundIds)
        {
            var fullList = FullFundList();
            var list = defaultFundIds.Select(id => fullList.SingleOrDefault(s => s.Value == id)).Where(fund => fund != null).ToList();
            list.AddRange(fullList.Where(f => !defaultFundIds.Contains(f.Value)));
            return list.ToArray();
        }

        public static SelectListItem[] FullFundList()
        {
            return (from f in GetAllOnlineFunds()
                    where (f.OnlineSort > 0)
                    select new SelectListItem
                    {
                        Text = $"{f.FundName}",
                        Value = f.FundId.ToString()
                    }).ToArray();
        }

        public static SelectListItem[] FundList()
        {
            return (from f in GetAllOnlineFunds()
                    where (f.OnlineSort > 0 && f.OnlineSort <= 99)
                    select new SelectListItem
                    {
                        Text = $"{f.FundName}",
                        Value = f.FundId.ToString()
                    }).ToArray();
        }

        public static SelectListItem[] SpecialFundList()
        {
            return (from f in GetAllOnlineFunds()
                    where f.OnlineSort > 99
                    select new SelectListItem
                    {
                        Text = $"{f.FundName}",
                        Value = f.FundId.ToString()
                    }).ToArray();
        }

        public static string GetFundName(int fundId)
        {
            var fund =  (from f in GetAllOnlineFunds()
                where f.FundId == fundId
                select f).SingleOrDefault();

            return fund?.FundName;
        }

        private static IQueryable<ContributionFund> GetAllOnlineFunds()
        {
            return from f in DbUtil.Db.ContributionFunds
                   where f.FundStatusId == 1
                   orderby f.OnlineSort
                   select f;
        }

        private PythonModel pythonModel;
        public PythonModel PythonModel => pythonModel ?? (pythonModel = HttpContextFactory.Current.Items["PythonEvents"] as PythonModel);

        private readonly Dictionary<string, string> _nameLookup = new Dictionary<string, string>()
        {
            {"first", "FirstName"},
            {"last", "LastName"},
            {"dob", "DateOfBirth"},
            {"phone", "Phone"},
            {"email", "EmailAddress"},

            {"homephone", "HomePhone"},
            {"OtherOK", "QuestionsOK"},
            {"address", "AddressLineOne"},
            {"address2", "AddressLineTwo"},
            {"city", "City"},
            {"state", "State"},
            {"zip", "ZipCode"},
            {"country", "Country"},
        };

        public string TranslateName(string name)
        {
            return _nameLookup.ContainsKey(name) ? _nameLookup[name] : name;
        }

        public bool NeedsToChooseClass()
        {
            // set org from class dropdown if applicable
            if (Parent.classid > 0)
                orgid = Parent.classid;

            // make sure orgid is set
            if (!orgid.HasValue && Parent.Orgid.HasValue)
                orgid = Parent.Orgid;

            Parent.UserNeedsSelection = UserSelectsOrganization() && orgid == null;
            if (Parent.UserNeedsSelection)
                IsValidForContinue = false;
            return Parent.UserNeedsSelection;
        }
        internal void DoGroupToJoin()
        {
            int grouptojoin = setting.GroupToJoin.ToInt();
            if (!PeopleId.HasValue)
                throw new Exception("PeopleId has no value in DoGroupToJoin");
            if (grouptojoin > 0)
            {
                OrganizationMember.InsertOrgMembers(DbUtil.Db, grouptojoin, PeopleId.Value, MemberTypeCode.Member,
                    Util.Now, null, false);
                DbUtil.Db.UpdateMainFellowship(grouptojoin);
                Log("AddedToOrg");
            }
        }
        private void AfterSettingConstructor()
        {
            if (_setting == null)
                return;
            InitializeOptionIfNeeded();

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

        private void InitializeOptionIfNeeded()
        {
            if (option != null)
                return;
            var ndd = setting.AskItems.Count(aa => aa.Type == "AskDropdown");
            if (ndd > 0)
                option = new string[ndd].ToList();
        }

        public void Log(string action)
        {
            DbUtil.LogActivity("OnlineReg " + action, masterorgid ?? orgid, PeopleId ?? Parent.UserPeopleId, Parent.DatumId);
        }

        public string Address()
        {
            var sb = new StringBuilder();
            sb.AppendNewLine(AddressLineOne);
            sb.AppendNewLine(AddressLineTwo);
            var csz = Util.FormatCSZ(City, State, ZipCode);
            sb.AppendNewLine(csz);
            return sb.ToString();
        }

        public bool NoPhoneEmailOnFind()
        {
            var o = masterorg ?? org;
            return o != null && o.GetExtra(DbUtil.Db, "NoPhoneEmailOnFind") == "true";
        }
        public int MinimumUserAge => DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();

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
        public string Gender => GenderId == 1 ? "Male" : "Female";
        public string Marital => MaritalId == 10 ? "Single" : "Married";
    }
}
