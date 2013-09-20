using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using Picture = CmsData.Picture;

namespace CmsWeb.Areas.People.Models.Person
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

        public int AddressTypeId { get; set; }
        private AddressInfo _PrimaryAddr;
        private AddressInfo _OtherAddr;
        private Picture picture;
        public CmsData.Person Person { get; set; }

        public AddressInfo PrimaryAddr
        {
            get
            {
                if (_PrimaryAddr == null)
                    if (FamilyAddr.Preferred)
                        _PrimaryAddr = FamilyAddr;
                    else if (PersonalAddr.Preferred)
                        _PrimaryAddr = PersonalAddr;
                return _PrimaryAddr;
            }
        }

        public AddressInfo OtherAddr
        {
            get
            {
                if (_OtherAddr == null)
                    if (FamilyAddr.Preferred)
                        _OtherAddr = PersonalAddr;
                    else if (PersonalAddr.Preferred)
                        _OtherAddr = FamilyAddr;
                return _OtherAddr;
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
            var i = (from pp in DbUtil.Db.People
                     let spouse = (from sp in pp.Family.People where sp.PeopleId == pp.SpouseId select sp.Name).SingleOrDefault()
                     let statusflags = DbUtil.Db.StatusFlags(flags).Single(sf => sf.PeopleId == id).StatusFlags
                     where pp.PeopleId == id
                     select new
                     {
                         pp,
                         f = pp.Family,
                         spouse,
                         pp.Picture,
                         statusflags,
                         memberStatus = pp.MemberStatus.Description,
                     }).FirstOrDefault();
            if (i == null)
                return;

            Person = i.pp;
            var p = Person;
            var fam = i.f;

            MemberStatus = i.memberStatus;
            PeopleId = p.PeopleId;
            AddressTypeId = p.AddressTypeId;
            FamilyId = p.FamilyId;
            Name = p.Name;
            Picture = i.Picture;
            StatusFlags = (i.statusflags ?? "").Split(',');

            basic = new BasicPersonInfo(p.PeopleId);

            FamilyAddr = new AddressInfo
            {
                Name = "FamilyAddr",
                PeopleId = p.PeopleId,
                person = p,
                Address1 = fam.AddressLineOne,
                Address2 = fam.AddressLineTwo,
                City = fam.CityName,
                Zip = fam.ZipCode,
                BadAddress = fam.BadAddressFlag,
                State = new CodeInfo(fam.StateCode, "State"),
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
                Address1 = p.AddressLineOne,
                Address2 = p.AddressLineTwo,
                City = p.CityName,
                State = new CodeInfo(p.StateCode, "State"),
                Country = new CodeInfo(p.CountryName, "Country"),
                ResCode = new CodeInfo(p.ResCodeId, "ResCode"),
                Zip = p.ZipCode,
                BadAddress = p.BadAddressFlag,
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
    }
}
