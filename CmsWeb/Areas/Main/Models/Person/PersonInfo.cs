using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CmsData;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models.PersonPage
{
    public class PersonInfo
    {
        public BasicPersonInfo basic { get; set; }
        public MemberInfo member { get; set; }
        public GrowthInfo growth { get; set; }
        public MemberNotesInfo membernotes { get; set; }

        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public int? SpouseId { get; set; }
        public string StatusFlags { get; set; }

        public bool Deceased { get; set; }
        public string Name { get; set; }
        public int? SmallPicId { get; set; }

        public int AddressTypeId { get; set; }
        private AddressInfo _PrimaryAddr;
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
        public AddressInfo FamilyAddr { get; set; }
        public AddressInfo PersonalAddr { get; set; }
        public static PersonInfo GetPersonInfo(int? id)
        {
            var flags = DbUtil.Db.Setting("StatusFlags", "F04,F01,F02,F03");
            var isvalid = Regex.IsMatch(flags, @"\A(F\d\d,{0,1})(,F\d\d,{0,1})*\z", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			var i = (from pp in DbUtil.Db.People
					 let spouse = (from sp in pp.Family.People where sp.PeopleId == pp.SpouseId select sp.Name).SingleOrDefault()
					 where pp.PeopleId == id
					 select new
					 {
						 pp,
						 f = pp.Family,
                         spouse,
						 pp.Picture.SmallId,
					 }).FirstOrDefault();
            if (i == null)
                return null;
			var p = i.pp;
			var fam = i.f;
            var statusflags = isvalid
                ? DbUtil.Db.StatusFlagsForPerson(id, flags)
                : "invalid setting in status flags";

            var pi = new PersonInfo
            {
                PeopleId = p.PeopleId,
                AddressTypeId = p.AddressTypeId,
                Deceased = p.IsDeceased ?? false,
                FamilyId = p.FamilyId,
                Name = p.Name,
                SmallPicId = i.SmallId,
                SpouseId = p.SpouseId,
                StatusFlags = statusflags,

                member = new MemberInfo
                {
                    PeopleId = p.PeopleId,
                    BaptismSchedDate = p.BaptismSchedDate,
                    BaptismTypeId = p.BaptismTypeId ?? 0,
                    BaptismStatusId = p.BaptismStatusId ?? 0,
                    BaptismDate = p.BaptismDate,
                    DecisionDate = p.DecisionDate,
                    DecisionTypeId = p.DecisionTypeId ?? 0,
                    DropDate = p.DropDate,
                    DropTypeId = p.DropCodeId,
                    EnvelopeOptionId = p.EnvelopeOptionsId ?? 0,
                    StatementOptionId = p.ContributionOptionsId ?? 0,
                    JoinTypeId = p.JoinCodeId,
                    NewChurch = p.OtherNewChurch,
                    PrevChurch = p.OtherPreviousChurch,
                    NewMemberClassDate = p.NewMemberClassDate,
                    NewMemberClassStatusId = p.NewMemberClassStatusId ?? 0,
                    MemberStatusId = p.MemberStatusId,
                    JoinDate = p.JoinDate,
                },
                basic = new BasicPersonInfo
                {
                    PeopleId = p.PeopleId,
                    person = p,
                    Age = p.Age.ToString(),
                    Birthday = p.DOB,
                    CampusId = p.CampusId ?? 0,
                    CellPhone = p.CellPhone.FmtFone(),
                    DeceasedDate = p.DeceasedDate,
                    DoNotCallFlag = p.DoNotCallFlag,
                    DoNotMailFlag = p.DoNotMailFlag,
                    DoNotVisitFlag = p.DoNotVisitFlag,
                    EmailAddress = p.EmailAddress,
                    SendEmailAddress1 = p.SendEmailAddress1 ?? true,
                    EmailAddress2 = p.EmailAddress2,
                    SendEmailAddress2 = p.SendEmailAddress2 ?? false,
                    Employer = p.EmployerOther,
                    First = p.FirstName,
                    GenderId = p.GenderId,
                    Created = p.CreatedDate,
                    Grade = p.Grade.ToString(),
                    HomePhone = p.Family.HomePhone,
                    JoinDate = p.JoinDate,
                    Last = p.LastName,
                    AltName = p.AltName,
                    Maiden = p.MaidenName,
                    MaritalStatusId = p.MaritalStatusId,
                    MemberStatusId = p.MemberStatusId,
                    Middle = p.MiddleName,
                    NickName = p.NickName,
                    Occupation = p.OccupationOther,
                    School = p.SchoolOther,
                    Spouse = i.spouse,
                    Suffix = p.SuffixCode,
                    Title = p.TitleCode,
                    WeddingDate = p.WeddingDate,
                    WorkPhone = p.WorkPhone,
                    ReceiveSMS = p.ReceiveSMS,
                },
                growth = new GrowthInfo
                {
                    PeopleId = p.PeopleId,
                    InterestPointId = p.InterestPointId ?? 0,
                    OriginId = p.OriginId ?? 0,
                    EntryPointId = p.EntryPointId ?? 0,
                    ChristAsSavior = p.ChristAsSavior,
                    Comments = p.Comments,
                    InterestedInJoining = p.InterestedInJoining,
                    MemberAnyChurch = p.MemberAnyChurch,
                    PleaseVisit = p.PleaseVisit,
                    SendInfo = p.InfoBecomeAChristian,
                },
                membernotes = new MemberNotesInfo
                {
                    PeopleId = p.PeopleId,
                    LetterStatusId = p.LetterStatusId ?? 0,
                    LetterReceived = p.LetterDateReceived,
                    LetterRequested = p.LetterDateRequested,
                    LetterNotes = p.LetterStatusNotes,
                },
                FamilyAddr = new AddressInfo
                {
                    Name = "FamilyAddr",
                    PeopleId = p.PeopleId,
                    person = p,
                    Address1 = fam.AddressLineOne,
                    Address2 = fam.AddressLineTwo,
                    City = fam.CityName,
                    State = fam.StateCode,
                    Zip = fam.ZipCode,
                    Country = fam.CountryName,
                    BadAddress = fam.BadAddressFlag,
                    ResCodeId = fam.ResCodeId ?? 0,
                    FromDt = fam.AddressFromDate,
                    ToDt = fam.AddressToDate,
                    Preferred = p.AddressTypeId == 10,
                },
                PersonalAddr = new AddressInfo
                {
                    Name = "PersonalAddr",
                    PeopleId = p.PeopleId,
                    person = p,
                    Address1 = p.AddressLineOne,
                    Address2 = p.AddressLineTwo,
                    City = p.CityName,
                    State = p.StateCode,
                    Zip = p.ZipCode,
                    Country = p.CountryName,
                    BadAddress = p.BadAddressFlag,
                    ResCodeId = p.ResCodeId ?? 0,
                    FromDt = p.AddressFromDate,
                    ToDt = p.AddressToDate,
                    Preferred = p.AddressTypeId == 30,
                },
                //familymembers = i.familymembers
            };
            return pi;
        }
    }
}
