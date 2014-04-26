using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;
using CmsData;
using System.Text;
using System.Data.Linq;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public class SearchPersonModel
    {
        private static CodeValueModel cv = new CodeValueModel();

        public int index { get; set; }
        public string context { get; set; }
        public string first { get; set; }
        public string goesby { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
        public string title { get; set; }
        public string suffix { get; set; }
        public string dob { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int gender { get; set; }
        public int marital { get; set; }
        public int? campus { get; set; }
        public int? entrypoint { get; set; }
        public string displayname
        {
            get
            {
                if (goesby.HasValue() && goesby != first)
                    return "{0} ({1}) {2}".Fmt(first, goesby, last);
                return "{0} {1}".Fmt(first, last);
            }
        }

        public string homephone { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }

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

        public string genderdisplay
        {
            get { return cv.GenderCodesWithUnspecified().ItemValue(gender); }
        }

        public string marrieddisplay
        {
            get { return cv.MaritalStatusCodes99().ItemValue(marital); }
        }

        public IEnumerable<SelectListItem> TitleCodes()
        {
            return CodeValueModel.ConvertToSelect(cv.TitleCodes(), "Code");
        }

        public IEnumerable<SelectListItem> GenderCodes()
        {
            return CodeValueModel.ConvertToSelect(cv.GenderCodesWithUnspecified(), "Id");
        }

        public IEnumerable<SelectListItem> MaritalStatuses()
        {
            return CodeValueModel.ConvertToSelect(cv.MaritalStatusCodes99(), "Id");
        }

        public IEnumerable<SelectListItem> CampusCodes()
        {
            return CodeValueModel.ConvertToSelect(cv.AllCampuses0(), "Id");
        }

        public IEnumerable<SelectListItem> EntryPointCodes()
        {
            return CodeValueModel.ConvertToSelect(cv.EntryPoints(), "Id");
        }

        public IEnumerable<SelectListItem> StateCodes()
        {
            return CodeValueModel.ConvertToSelect(cv.GetStateListUnknown(), "Code");
        }

        public IEnumerable<SelectListItem> Countries()
        {
            var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList(), null);
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "" });
            return list;
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

        internal void ValidateModelForNew(ModelStateDictionary ModelState, bool checkaddress)
        {
            if (!first.HasValue())
                ModelState.AddModelError("name", "first name required");

            if (!last.HasValue())
                ModelState.AddModelError("name", "last name required");

            if (!birthday.HasValue && dob.NotEqual("na"))
                ModelState.AddModelError("dob", "valid birthday (or \"na\")");

            var d = phone.GetDigits().Length;
            if (d != 7 && d < 10 && phone.NotEqual("na"))
                ModelState.AddModelError("phone", "7 or 10+ digits (or \"na\")");

            int count = 0;
            PeopleSearchModel.FindPerson(first, last, birthday ?? DateTime.MinValue, string.Empty, phone.GetDigits(),
                                         out count);
            if (count > 0)
                ModelState.AddModelError("name", "name/dob already exists in db");

            if (!Util.ValidEmail(email) && email.NotEqual("na"))
                ModelState.AddModelError("email", "valid email address (or \"na\")");

            if (gender == 99)
                ModelState.AddModelError("gender", "specify gender");

            if (marital == 99)
                ModelState.AddModelError("marital", "specify marital status");

            if (checkaddress)
            {

                if (!address.HasValue())
                    ModelState.AddModelError("address", "address required (or \"na\")");

                var addrok = false;
                if (city.HasValue() && state.HasValue())
                    addrok = true;
                if (zip.HasValue())
                    addrok = true;
                if (city.Equal("na") && state.Equal("na") && zip.Equal("na"))
                    addrok = true;
                if (!addrok)
                    ModelState.AddModelError("zip", "city/state required or zip required (or \"na\" in all)");

                if (ModelState.IsValid
                    && address.NotEqual("na") && city.NotEqual("na") && state.NotEqual("na")
                    && (country.Equal("United States") || !country.HasValue()))
                {
                    var r = AddressVerify.LookupAddress(address, address2, city, state, zip);
                    if (r.Line1 != "error")
                    {
                        if (r.found == false)
                        {
                            ModelState.AddModelError("zip",
                                                     r.address +
                                                     ", if your address will not validate, change the country to 'USA, Not Validated'");
                            return;
                        }
                        if (r.Line1 != address)
                        {
                            ModelState.AddModelError("address", "address changed from '{0}'".Fmt(address));
                            address = r.Line1;
                        }
                        if (r.Line2 != (address2 ?? ""))
                        {
                            ModelState.AddModelError("address2", "address2 changed from '{0}'".Fmt(address2));
                            address2 = r.Line2;
                        }
                        if (r.City != (city ?? ""))
                        {
                            ModelState.AddModelError("city", "city changed from '{0}'".Fmt(city));
                            city = r.City;
                        }
                        if (r.State != (state ?? ""))
                        {
                            ModelState.AddModelError("state", "state changed from '{0}'".Fmt(state));
                            state = r.State;
                        }
                        if (r.Zip != (zip ?? ""))
                        {
                            ModelState.AddModelError("zip", "zip changed from '{0}'".Fmt(zip));
                            zip = r.Zip;
                        }
                    }
                }
            }
        }

        internal void AddPerson(int originid, int? entrypointid, int? campusid)
        {
            var na = "na";
            Family f = FamilyId > 0
                ? family
                : new Family
                {
                    HomePhone = homephone.GetDigits(),
                    AddressLineOne = address.Disallow(na),
                    AddressLineTwo = address2,
                    CityName = city.Disallow(na),
                    StateCode = state.Disallow(na),
                    ZipCode = zip.Disallow(na),
                    CountryName = country,
                };

            if (goesby != null)
                goesby = goesby.Trim();

            var position = DbUtil.Db.ComputePositionInFamily(age ?? -1, false, FamilyId) ?? 10;

            _Person = Person.Add(f, position,
                                 null, first.Trim(), goesby, last.Trim(), dob, false, gender,
                                 originid, entrypointid);
            if (title.HasValue())
                person.TitleCode = title;
            person.EmailAddress = email.Disallow(na);
            person.MaritalStatusId = marital;
            person.SuffixCode = suffix;
            person.MiddleName = middle;
            if (campusid == 0)
                campusid = null;
            person.CampusId = Util.PickFirst(campusid.ToString(), DbUtil.Db.Setting("DefaultCampusId", "")).ToInt2();
            if (person.CampusId == 0)
                person.CampusId = null;

            person.CellPhone = phone.GetDigits();
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("OldUI AddPerson {0}".Fmt(person.PeopleId));
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
            PeopleId = person.PeopleId;
        }

        internal void LoadFamily()
        {
            if (FamilyId < 0)
                return;
            homephone = family.HomePhone;
            address = family.AddressLineOne;
            address2 = family.AddressLineTwo;
            city = family.CityName;
            state = family.StateCode;
            zip = family.ZipCode;
        }

        public string ToolTip
        {
            get
            {
                return "{0} {1}|{2}|{3}|Birthday: {4}|c {5}|h {6}|{7}|Gender: {8}|Marital: {9}".Fmt(
                    goesby ?? first, last,
                    address,
                    CityStateZip,
                    birthday.FormatDate(),
                    phone.FmtFone(),
                    homephone.FmtFone(),
                    email,
                    genderdisplay,
                    marrieddisplay);
            }
        }

        public string CityStateZip
        {
            get { return "{0}, {1} {2}".Fmt(city, state, zip); }
        }

    }
}
