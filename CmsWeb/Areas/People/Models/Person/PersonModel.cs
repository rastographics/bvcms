using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;
using Picture = CmsData.Picture;

namespace CmsWeb.Areas.People.Models
{
    public class PersonModel
    {
        public BasicPersonInfo basic { get; set; }

        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public string[] StatusFlags { get; set; }

        public string Name { get; set; }
        public string MemberStatus { get; set; }

        private FamilyModel familyModel;
        public FamilyModel FamilyModel
        {
            get { return familyModel ?? (familyModel = new FamilyModel(PeopleId)); }
        }

        private IEnumerable<User> users;
        public IEnumerable<User> Users
        {
            get { return users ?? (users = Person.Users); }
        }

        public Picture Picture
        {
            get { return picture ?? new Picture(); }
            set { picture = value; }
        }
        public Picture FamilyPicture
        {
            get { return familypicture ?? new Picture(); }
            set { familypicture = value; }
        }

        public int AddressTypeId { get; set; }
        private AddressInfo primaryAddr;
        private AddressInfo otherAddr;
        private Picture picture;
        private Picture familypicture;
        public Person Person { get; set; }

        public AddressInfo PrimaryAddr
        {
            get
            {
                if (primaryAddr == null)
                    if (FamilyAddr.Preferred)
                        primaryAddr = FamilyAddr;
                    else if (PersonalAddr.Preferred)
                        primaryAddr = PersonalAddr;
                return primaryAddr;
            }
        }

        public AddressInfo OtherAddr
        {
            get
            {
                if (otherAddr == null)
                    if (FamilyAddr.Preferred)
                        otherAddr = PersonalAddr;
                    else if (PersonalAddr.Preferred)
                        otherAddr = FamilyAddr;
                return otherAddr;
            }
        }

        public AddressInfo FamilyAddr { get; set; }
        public AddressInfo PersonalAddr { get; set; }

        public string Email
        {
            get
            {
                if (Person.EmailAddress.HasValue())
                    return string.Format("<a href='mailto:{0}' target='_blank'>{0}</a>", Person.EmailAddress);
                return null;
            }
        }
        public string Cell
        {
            get
            {
                if (Person.CellPhone.HasValue())
                    return Person.CellPhone.FmtFone("C").Replace(" ", "&nbsp;");
                return null;
            }
        }
        public string HomePhone
        {
            get
            {
                if (Person.HomePhone.HasValue())
                    return Person.HomePhone.FmtFone("H").Replace(" ", "&nbsp;");
                return null;
            }
        }

        public PersonModel(int id)
        {
            var flags = DbUtil.Db.Setting("StatusFlags", "F04,F01,F02,F03");
            var isvalid = Regex.IsMatch(flags, @"\A(F\d\d,{0,1})(,F\d\d,{0,1})*\z", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            var i = (from pp in DbUtil.Db.People
                     let spouse = (from sp in pp.Family.People where sp.PeopleId == pp.SpouseId select sp.Name).SingleOrDefault()
                     where pp.PeopleId == id
                     select new
                     {
                         pp,
                         pp.Picture,
                         f = pp.Family,
                         FamilyPicture = pp.Family.Picture,
                         memberStatus = pp.MemberStatus.Description,
                     }).FirstOrDefault();
            if (i == null)
                return;

            string statusflags;
            if (isvalid)
                statusflags = string.Join(",", from s in DbUtil.Db.StatusFlagsPerson(id).ToList()
                                                where s.RoleName == null || HttpContext.Current.User.IsInRole(s.RoleName)
                                                select s.Name);
            else
                statusflags = "invalid setting in status flags";

            Person = i.pp;
            var p = Person;
            var fam = i.f;

            MemberStatus = i.memberStatus;
            PeopleId = p.PeopleId;
            AddressTypeId = p.AddressTypeId;
            FamilyId = p.FamilyId;
            Name = p.Name;
            Picture = i.Picture;
            FamilyPicture = i.FamilyPicture;
            StatusFlags = (statusflags ?? "").Split(',');

            basic = new BasicPersonInfo(p.PeopleId);

            FamilyAddr = new AddressInfo
            {
                Name = "FamilyAddr",
                PeopleId = p.PeopleId,
                person = p,
                AddressLineOne = fam.AddressLineOne,
                AddressLineTwo = fam.AddressLineTwo,
                CityName = fam.CityName,
                ZipCode = fam.ZipCode,
                BadAddress = fam.BadAddressFlag ?? false,
                StateCode = new CodeInfo(fam.StateCode, "State"),
                Country = new CodeInfo(fam.CountryName, "Country"),
                ResCode = new CodeInfo(fam.ResCodeId, "ResCode"),
                FromDt = fam.AddressFromDate,
                ToDt = fam.AddressToDate,
                Preferred = p.AddressTypeId == 10,
            };
            PersonalAddr = new AddressInfo
            {
                Name = "PersonalAddr",
                PeopleId = p.PeopleId,
                person = p,
                AddressLineOne = p.AddressLineOne,
                AddressLineTwo = p.AddressLineTwo,
                CityName = p.CityName,
                StateCode = new CodeInfo(p.StateCode, "State"),
                Country = new CodeInfo(p.CountryName, "Country"),
                ResCode = new CodeInfo(p.ResCodeId, "ResCode"),
                ZipCode = p.ZipCode,
                BadAddress = p.BadAddressFlag ?? false,
                FromDt = p.AddressFromDate,
                ToDt = p.AddressToDate,
                Preferred = p.AddressTypeId == 30,
            };
        }
        public string CheckView()
        {
            if (Person == null)
                return "person not found";
            if (!HttpContext.Current.User.IsInRole("Access"))
                if (Person == null || !Person.CanUserSee)
                    return "no access";

            if (Util2.OrgMembersOnly)
            {
                var omotag = DbUtil.Db.OrgMembersOnlyTag2();
                if (!DbUtil.Db.TagPeople.Any(pt => pt.PeopleId == PeopleId && pt.Id == omotag.Id))
                {
                    DbUtil.LogActivity("Trying to view person: {0}".Fmt(Person.Name));
                    return "<h3 style='color:red'>{0}</h3>\n<a href='{1}'>{2}</a>"
                            .Fmt("You must be a member one of this person's organizations to have access to this page",
                                "javascript: history.go(-1)", "Go Back");
                }
            }
            else if (Util2.OrgLeadersOnly)
            {
                var olotag = DbUtil.Db.OrgLeadersOnlyTag2();
                if (!DbUtil.Db.TagPeople.Any(pt => pt.PeopleId == PeopleId && pt.Id == olotag.Id))
                {
                    DbUtil.LogActivity("Trying to view person: {0}".Fmt(Person.Name));
                    return "<h3 style='color:red'>{0}</h3>\n<a href='{1}'>{2}</a>"
                            .Fmt("You must be a leader of one of this person's organizations to have access to this page",
                                "javascript: history.go(-1)", "Go Back");
                }
            }
            return null;
        }

        internal void UpdateEnvelopeOption(string name, int option)
        {
            var db = DbUtil.Db;
            name = name + "Id";
            int? opt = option;
            if (opt == 0)
                opt = null;
            Person.UpdateValue(name, opt);
            Person.LogChanges(db);
            var sp = db.LoadPersonById(Person.SpouseId ?? 0);
            if (sp != null)
                if (opt == StatementOptionCode.Joint || opt == StatementOptionCode.Individual)
                {
                    sp.UpdateValue(name, opt);
                    sp.LogChanges(db);
                }
                else if (name == "ContributionOptionsId" && sp.ContributionOptionsId == StatementOptionCode.Joint)
                {
                    sp.UpdateValue(name, null);
                    sp.LogChanges(db);
                }
                else if (name == "EnvelopeOptionsId" && sp.EnvelopeOptionsId == StatementOptionCode.Joint)
                {
                    sp.UpdateValue(name, null);
                    sp.LogChanges(db);
                }
            db.SubmitChanges();
        }

        internal void UpdateElectronicStatement(bool tf)
        {
            var db = DbUtil.Db;
            Person.UpdateValue("ElectronicStatement", tf);
            var sp = db.LoadPersonById(Person.SpouseId ?? 0);
            if (sp != null && Person.ContributionOptionsId == StatementOptionCode.Joint)
            {
                sp.UpdateValue("ElectronicStatement", tf);
                sp.LogChanges(db);
            }
            Person.LogChanges(db);
            db.SubmitChanges();
        }
    }
}
