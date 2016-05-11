using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CmsData;
using CmsData.Codes;
using CmsData.Resource;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class PersonModel
    {
        private FamilyModel familyModel;
        private Picture familypicture;
        private AddressInfo otherAddr;
        private Picture picture;
        private AddressInfo primaryAddr;
        private IEnumerable<User> users;

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
                         memberStatus = pp.MemberStatus.Description
                     }).FirstOrDefault();
            if (i == null)
                return;

            string statusflags;
            if (isvalid)
                statusflags = string.Join(",", from s in DbUtil.Db.StatusFlagsPerson(id).ToList()
                                               where s.RoleName == null || HttpContext.Current.User.IsInRole(s.RoleName)
                                               orderby s.TokenID
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
                Preferred = p.AddressTypeId == 10
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
                Preferred = p.AddressTypeId == 30
            };
        }

        public BasicPersonInfo basic { get; set; }
        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public string[] StatusFlags { get; set; }
        public string Name { get; set; }
        public string MemberStatus { get; set; }

        public FamilyModel FamilyModel => familyModel ?? (familyModel = new FamilyModel(PeopleId));

        public IEnumerable<User> Users => users ?? (users = Person.Users);

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
                    return $"<a href='mailto:{Person.EmailAddress}' target='_blank'>{Person.EmailAddress}</a>";
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

        public string CheckView()
        {
            if (Person == null)
                return "person not found";
            if (!HttpContext.Current.User.IsInRole("Access"))
                if (Person == null || !Person.CanUserSee)
                    return "no access";

            if (Util2.OrgLeadersOnly)
            {
                var olotag = DbUtil.Db.OrgLeadersOnlyTag2();
                if (!DbUtil.Db.TagPeople.Any(pt => pt.PeopleId == PeopleId && pt.Id == olotag.Id))
                {
                    DbUtil.LogActivity($"Trying to view person: {Person.Name}");
                    return $"<h3 style='color:red'>{"You must be a leader of one of this person's organizations to have access to this page"}</h3>\n<a href='{"javascript: history.go(-1)"}'>{"Go Back"}</a>";
                }
            }
            return null;
        }

        public List<CmsData.Resource.Resource> Resources
        {
            get
            {
                return new List<CmsData.Resource.Resource>()
                {
                    new CmsData.Resource.Resource
                    {
                        Name = "Church Guidebook",
                        Type = ResourceType.Pdf,
                        UpdatedTime = DateTime.Now.AddDays(-33)
                    },
                    new CmsData.Resource.Resource
                    {
                        Name = "South America Mission Goals",
                        OrgId = 1,
                        Type = ResourceType.Pdf,
                        UpdatedTime = DateTime.Now.AddDays(-22)
                    },
                    new CmsData.Resource.Resource
                    {
                        Name = "Baseball Team Roster",
                        OrgId = 1,
                        Type = ResourceType.Spreadsheet,
                        UpdatedTime = DateTime.Now.AddDays(-52)
                    }
                };
            }
        }
    }
}
