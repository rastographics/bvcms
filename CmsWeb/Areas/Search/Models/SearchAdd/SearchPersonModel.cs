using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using NPOI.SS.Formula;
using UtilityExtensions;
using CmsData;
using System.Text;
using System.Data.Linq;
using CmsData.Codes;

namespace CmsWeb.Areas.Search.Models
{
    public class SearchPersonModel
    {
        public int index { get; set; }
        public string context { get; set; }

        [StringLength(25), Required(ErrorMessage = "required, or put 'na' if not known")]
        public string First { get; set; }

        [StringLength(15)]
        public string GoesBy { get; set; }

        [StringLength(15)]
        public string Middle { get; set; }

        [StringLength(100), Required(ErrorMessage = "required")]
        public string Last { get; set; }

        public CodeInfo Title { get; set; }

        [StringLength(10)]
        public string Suffix { get; set; }

        [DisplayName("Birthday")]
        public string dob { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20), EmailAddress]
        public string Email { get; set; }

        [UnallowedCode("99", ErrorMessage = "specify gender (or unknown)")]
        public CodeInfo Gender { get; set; }

        [UnallowedCode("99", ErrorMessage = "specify marital status (or unknown)")]
        public CodeInfo MaritalStatus { get; set; }

        public CodeInfo Campus { get; set; }

        [UnallowedCode("0", ErrorMessage = "specify an Entry Point")]
        public CodeInfo EntryPoint { get; set; }

        [StringLength(20)]
        public string HomePhone { get; set; }

        private DateTime? _Birthday;

        public DateTime? birthday
        {
            get
            {
                DateTime dt;
                if (!_Birthday.HasValue && dob.NotEqual("na"))
                    if (Util.DateValid(dob, out dt))
                        _Birthday = dt;
                return _Birthday;
            }
        }

        public int? age
        {
            get
            {
                if (birthday.HasValue)
                    return birthday.Value.AgeAsOf(Util.Now);
                return null;
            }
        }

        public bool IsNew
        {
            get { return !PeopleId.HasValue; }
        }

        public int FamilyId { get; set; }
        private Family _family;
        public Family family
        {
            get
            {
                if (_family == null && FamilyId > 0)
                    _family = DbUtil.Db.Families.Single(f => f.FamilyId == FamilyId);
                return _family;
            }
        }

        public int? PeopleId { get; set; }
        private Person _Person;
        public Person person
        {
            get
            {
                if (_Person == null && PeopleId.HasValue)
                    _Person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                return _Person;
            }
        }

        public AddressInfo AddressInfo { get; set; }

        public string PotentialDuplicate { get; set; }

        internal void CheckDuplicate()
        {
            var pids = DbUtil.Db.FindPerson(First, Last, birthday, null, Phone.GetDigits()).Select(pp => pp.PeopleId).ToList();
            var q = from p in DbUtil.Db.People
                    where pids.Contains(p.PeopleId)
                    select new { p.PeopleId, p.Name, p.PrimaryAddress, p.Age, };
            var sb = new StringBuilder();
            foreach (var p in q)
            {
                if (sb.Length == 0)
                    sb.AppendLine("<ul>\n");
                sb.AppendFormat("<li><a href=\"/Person2/{1}\" target=\"_blank\">{0}</a> ({1}), {2}, age:{3}</li>\n".Fmt(p.Name, p.PeopleId, p.PrimaryAddress, p.Age));
            }
            if (sb.Length > 0)
                PotentialDuplicate = sb + "</ul>\n";
        }

        public bool isNewFamily { get; set; }

        internal void AddPerson(int originid, int? entrypointid, int? campusid)
        {
            Family f;
            string na = "na";
            if (FamilyId > 0)
                f = family;
            else
                f = new Family
                        {
                            HomePhone = HomePhone.GetDigits(),
                            AddressLineOne = AddressInfo.Address1.Disallow(na),
                            AddressLineTwo = AddressInfo.Address2,
                            CityName = AddressInfo.City.Disallow(na),
                            StateCode = AddressInfo.State.Value.Disallow(na),
                            ZipCode = AddressInfo.Zip.Disallow(na),
                            CountryName = AddressInfo.Country.Value,
                        };

            if (GoesBy != null)
                GoesBy = GoesBy.Trim();
            var position = PositionInFamily.Child;
            if (!birthday.HasValue)
                position = PositionInFamily.PrimaryAdult;
            if (age >= 18)
                if (f.People.Count(per =>
                                   per.PositionInFamilyId == PositionInFamily.PrimaryAdult)
                    < 2)
                    position = PositionInFamily.PrimaryAdult;
                else
                    position = PositionInFamily.SecondaryAdult;

            _Person = CmsData.Person.Add(f, position,
                                 null, First.Trim(), GoesBy, Last.Trim(), dob, false, Gender.Value.ToInt(),
                                 originid, entrypointid);
            if (Title.Value.HasValue())
                person.TitleCode = Title.Value;
            person.EmailAddress = Email.Disallow(na);
            person.MaritalStatusId = MaritalStatus.Value.ToInt();
            person.SuffixCode = Suffix;
            person.MiddleName = Middle;
            if (campusid == 0)
                campusid = null;
            person.CampusId = Util.PickFirst(campusid.ToString(), DbUtil.Db.Setting("DefaultCampusId", "")).ToInt2();
            if (person.CampusId == 0)
                person.CampusId = null;

            person.CellPhone = Phone.GetDigits();
            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
            PeopleId = person.PeopleId;
        }

        public void LoadAddress()
        {
            if (FamilyId <= 0)
            {
                AddressInfo = new AddressInfo();
                HomePhone = "";
#if DEBUG
                AddressInfo.Address1 = "235 revere";
                AddressInfo.Zip = "38018";
#endif
                return;
            }
            var f = family;
            AddressInfo = new AddressInfo(f.AddressLineOne, f.AddressLineTwo, f.CityName, f.StateCode, f.ZipCode, f.CountryName);
            HomePhone = f.HomePhone;
        }

        //public string ToolTip
        //{
        //    get
        //    {
        //        return "{0} {1}|{2}|{3}|Birthday: {4}|c {5}|h {6}|{7}|Gender: {8}|Marital: {9}".Fmt(
        //            GoesBy ?? First, Last,
        //            AddressInfo.Address,
        //            CityStateZip,
        //            birthday.FormatDate(),
        //            Phone.FmtFone(),
        //            HomePhone.FmtFone(),
        //            Email,
        //            Gender.ToString(),
        //            Marital.ToString());
        //    }
        //}

        public string CityStateZip
        {
            get { return "{0}, {1} {2}".Fmt(AddressInfo.City, AddressInfo.State.Value, AddressInfo.Zip); }
        }

        public bool BeenValidated { get; set; }

//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            var results = new List<ValidationResult>();
//
//            if (BeenValidated)
//                return results;
//
//            if (Email == null || (Email != "na" && !Util.ValidEmail(Email)))
//                results.Add(ModelError("valid email address (or na)", "Email"));
//
//            var d = Phone.GetDigits().Length;
//            if(Phone == null || (Phone != "na" && d != 7 && d < 10))
//                results.Add(ModelError("Phone requires 7 or 10+ digits (or na)", "Phone"));
//
//            DateTime dt;
//            if(dob == null || dob != "na" && DateTime.TryParse(dob, out dt))
//                results.Add(ModelError("enter a valid date or na", "dob"));
//
//            return results;
//        }

        private ValidationResult ModelError(string message, string field)
        {
            return new ValidationResult(message, new [] { field });
        }
    }
}
