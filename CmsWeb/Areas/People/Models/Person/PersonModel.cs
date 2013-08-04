using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing;
using UtilityExtensions;
using Picture = CmsData.Picture;

namespace CmsWeb.Areas.People.Models.Person
{
    public class PersonModel
    {
        public BasicPersonInfo basic { get; set; }
        public MemberInfo member { get; set; }
        public GrowthInfo growth { get; set; }
        public MemberNotesInfo membernotes { get; set; }

        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public int? SpouseId { get; set; }
        public string[] StatusFlags { get; set; }

        public bool Deceased { get; set; }
        public string Name { get; set; }

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
                     }).FirstOrDefault();
            if (i == null)
                return;

            Person = i.pp;
            var p = Person;
            var fam = i.f;

            PeopleId = p.PeopleId;
            AddressTypeId = p.AddressTypeId;
            Deceased = p.IsDeceased ?? false;
            FamilyId = p.FamilyId;
            Name = p.Name;
            Picture = i.Picture;
            SpouseId = p.SpouseId;
            StatusFlags = (i.statusflags ?? "").Split(',');

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
            };
            basic = new BasicPersonInfo
            {
                PeopleId = p.PeopleId,
                person = p,
                Age = p.Age.ToString(),
                Birthday = p.DOB,
                Mobile = new CellPhoneInfo(p.CellPhone.FmtFone(), p.ReceiveSMS),
                DeceasedDate = p.DeceasedDate,
                DoNotCallFlag = p.DoNotCallFlag,
                DoNotMailFlag = p.DoNotMailFlag,
                DoNotVisitFlag = p.DoNotVisitFlag,
                PrimaryEmail = new EmailInfo(p.EmailAddress, p.SendEmailAddress1 ?? true),
                AltEmail = new EmailInfo(p.EmailAddress2, p.SendEmailAddress2 ?? false),
                Campus = new CodeInfo(p.CampusId, "Campus"),
                Gender = new CodeInfo(p.GenderId, "Gender"),
                Marital = new CodeInfo(p.MaritalStatusId, "Marital"),
                MemberStatus = new CodeInfo(p.MemberStatusId, "MemberStatus"),
                FamilyPosition = new CodeInfo(p.PositionInFamilyId, "FamilyPosition"),
                Employer = p.EmployerOther,
                FirstName = p.FirstName,
                Created = p.CreatedDate,
                Grade = p.Grade.ToString(),
                HomePhone = p.Family.HomePhone,
                JoinDate = p.JoinDate,
                LastName = p.LastName,
                AltName = p.AltName,
                FormerName = p.MaidenName,
                MemberStatusId = p.MemberStatusId,
                MiddleName = p.MiddleName,
                GoesBy = p.NickName,
                Occupation = p.OccupationOther,
                School = p.SchoolOther,
                Spouse = i.spouse,
                Suffix = p.SuffixCode,
                Title = new CodeInfo(p.TitleCode, "Title"),
                WeddingDate = p.WeddingDate.FormatDate(),
                Work = p.WorkPhone.FmtFone(),
                ReceiveSMS = p.ReceiveSMS,
            };
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
            };
            membernotes = new MemberNotesInfo
            {
                PeopleId = p.PeopleId,
                LetterStatusId = p.LetterStatusId ?? 0,
                LetterReceived = p.LetterDateReceived,
                LetterRequested = p.LetterDateRequested,
                LetterNotes = p.LetterStatusNotes,
            };
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

        public void Reverse(string field, string value, string pf)
        {
            var sb = new StringBuilder();
            switch(pf)
            {
                case "p":
                    Person.UpdateValueFromText(sb, field, value);
                    Person.LogChanges(DbUtil.Db, sb, Util.UserPeopleId.Value);
                    break;
                case "f":
                    Person.Family.UpdateValueFromText(sb, field, value);
                    Person.Family.LogChanges(DbUtil.Db, sb, Person.PeopleId, Util.UserPeopleId.Value);
                    break;
            }
            DbUtil.Db.SubmitChanges();
        }
        public bool FieldEqual(CmsData.Person p, string field, string value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            if (!Util.HasProperty(p, field))
                return false;
            var o = Util.GetProperty(p, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            var p2 = new CmsData.Person();
	        Util.SetPropertyFromText(p2, field, value);
            var o2 = Util.GetProperty(p2, field);
            if (o == o2)
                return true;
            if (o.IsNull() && o2.IsNotNull())
                return false;
            return o.Equals(o2);
        }
        public bool FieldEqual(Family f, string field, string value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            if (!Util.HasProperty(f, field))
                return false;
            var o = Util.GetProperty(f, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            var f2 = new Family();
            Util.SetPropertyFromText(f2, field, value);
            var o2 = Util.GetProperty(f2, field);
            if (o == o2)
                return true;
            if (o.IsNull() && o2.IsNotNull())
                return false;
            return o.Equals(o2);
        }
        public bool FieldEqual(string pf, string field, string value)
        {
            switch (pf)
            {
                case "p":
					if (field == "Picture")
						return false;
                    return FieldEqual(this.Person, field, value);
                case "f":
                    return FieldEqual(this.Person.Family, field, value);
            }
            return false;
        }

        public class ChangeLogInfo
        {
            public string User { get; set; }
            public string FieldSet { get; set; }
            public DateTime? Time { get; set; }
            public string Field { get; set; }
            public string Before { get; set; }
            public string After { get; set; }
            public string pf { get; set; }
            public bool Reversable { get; set; }
        }
        private string PersonOrFamily(string FieldSet)
        {
            switch (FieldSet)
            {
                case "HomePhone":
                case "Basic Info":
                case "PersonalAddr":
                    return "p";
                case "Family":
                case "FamilyAddr":
                    return "f";
            }
            return "n";
        }
        public List<ChangeLogInfo> details(ChangeLog log, string name)
        {
            var list = new List<ChangeLogInfo>();
            var re = new Regex("<tr><td>(?<field>[^<]+)</td><td>(?<before>[^<]*)</td><td>(?<after>[^<]*)</td></tr>", RegexOptions.Singleline);
            Match matchResult = re.Match(log.Data);
            var FieldSet = log.Field;
            var pf = PersonOrFamily(FieldSet);
            DateTime? Time = log.Created;
            while (matchResult.Success)
            {
                var After = matchResult.Groups["after"].Value;
                var Field = matchResult.Groups["field"].Value;
                var c = new ChangeLogInfo
                {
                    User = name,
                    FieldSet = FieldSet,
                    Time = Time,
                    Field = Field,
                    Before = matchResult.Groups["before"].Value,
                    After = After,
                    pf = pf,
                    Reversable = FieldEqual(pf, Field, After)
                };
                list.Add(c);
                FieldSet = "";
                name = "";
                Time = null;
                matchResult = matchResult.NextMatch();
            }
            return list;
        }
//        public IEnumerable<ChangeLogInfo> GetChangeLogs()
//        {
//            var list = (from c in DbUtil.Db.ChangeLogs
//                        let userp = DbUtil.Db.People.SingleOrDefault(u => u.PeopleId == c.UserPeopleId)
//                        where c.PeopleId == Person.PeopleId || c.FamilyId == Person.FamilyId
//                        where userp != null
//                        select new { userp, c }).ToList();
//            var q = from i in list
//                    orderby i.c.Created descending
//                    from d in details(i.c, i.userp.Name)
//                    select d;
//            return q;
//        }
    }
}
