using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;
using System.Data.Linq;
using System.Text;

namespace CmsWeb.Models.PersonPage
{
    public class AddressInfo
    {
        private CodeValueModel cv = new CodeValueModel();

        public int PeopleId { get; set; }
        public Person person { get; set; }

        public string Name { get; set; }
        private bool? _CanUserEditAddress;
        public bool CanUserEditAddress
        {
            get
            {
                if (!_CanUserEditAddress.HasValue)
                {
                    switch (Name)
                    {
                        case "PersonalAddr":
                            _CanUserEditAddress = person.CanUserEditBasic;
                            break;
                        case "FamilyAddr":
                            _CanUserEditAddress = person.CanUserEditFamilyAddress;
                            break;
                    }
                }
                return _CanUserEditAddress.Value;
            }
        }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string CityStateZip()
        {
            return Util.FormatCSZ(City, State, Zip);
        }
        public string CityStateZip4()
        {
            return Util.FormatCSZ4(City, State, Zip);
        }
        public string AddrCityStateZip()
        {
            return Address1 + " " + CityStateZip();
        }
        public string Addr2CityStateZip()
        {
            return Address2 + " " + CityStateZip();
        }
        public bool? BadAddress { get; set; }
        public int? ResCodeId { get; set; }
        public string ResCode
        {
            get { return CodeValueModel.ResidentCodesWithZero().ItemValue(ResCodeId ?? 0); }
        }
        public bool Preferred { get; set; }
        public DateTime? FromDt { get; set; }
        public DateTime? ToDt { get; set; }

        public static IEnumerable<SelectListItem> ResCodes()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.ResidentCodesWithZero(), "Id");
        }
        public static IEnumerable<SelectListItem> States()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.GetStateList(), "Code");
        }
        public static IEnumerable<SelectListItem> Countries()
        {
            var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList(), null);
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "" });
            return list;
        }
        public static AddressInfo GetAddressInfo(int id, string typeid)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            var a = new AddressInfo();
            a.person = p;
            switch (typeid)
            {
                case "FamilyAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.Address1 = p.Family.AddressLineOne;
                    a.Address2 = p.Family.AddressLineTwo;
                    a.ToDt = p.Family.AddressToDate;
                    a.BadAddress = p.Family.BadAddressFlag;
                    a.City = p.Family.CityName;
                    a.State = p.Family.StateCode;
                    a.Zip = p.Family.ZipCode;
                    a.Country = p.Family.CountryName;
                    a.ResCodeId = p.Family.ResCodeId;
                    a.Preferred = p.AddressTypeId == 10;
                    break;
                case "PersonalAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.Address1 = p.AddressLineOne;
                    a.Address2 = p.AddressLineTwo;
                    a.ToDt = p.AddressToDate;
                    a.BadAddress = p.BadAddressFlag;
                    a.City = p.CityName;
                    a.State = p.StateCode;
                    a.Zip = p.ZipCode;
                    a.Country = p.CountryName;
                    a.ResCodeId = p.ResCodeId;
                    a.Preferred = p.AddressTypeId == 30;
                    break;
            }
            return a;
        }

        public void UpdateAddress(ModelStateDictionary ModelState)
        {
            if (ResCodeId == 0)
                ResCodeId = null;
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var f = p.Family;

            var addrok = false;
            if (City.HasValue() && State.HasValue())
                addrok = true;
            if (Zip.HasValue())
                addrok = true;
            if (!City.HasValue() && !State.HasValue() && !Zip.HasValue())
                addrok = true;
            if (!addrok)
                ModelState.AddModelError("zip", "city/state required or zip required (or \"na\" in all)");

            if (Address1.HasValue() && (City.HasValue() || State.HasValue() || Zip.HasValue())
                && (Country == "United States" || !Country.HasValue()))
            {
                var r = AddressVerify.LookupAddress(Address1, Address2, City, State, Zip);
                if (r.Line1 != "error")
                {
                    if (r.found == false)
                    {
                        ModelState.AddModelError("zip", r.address + ", if your address will not validate, change the country to 'USA, Not Validated'");
                        return;
                    }
                    if (r.Line1 != Address1)
                    {
                        ModelState.AddModelError("address1", "address changed from '{0}'".Fmt(Address1));
                        Address1 = r.Line1;
                    }
                    if (r.Line2 != (Address2 ?? ""))
                    {
                        ModelState.AddModelError("address2", "address2 changed from '{0}'".Fmt(Address2));
                        Address2 = r.Line2;
                    }
                    if (r.City != (City ?? ""))
                    {
                        ModelState.AddModelError("city", "city changed from '{0}'".Fmt(City));
                        City = r.City;
                    }
                    if (r.State != (State ?? ""))
                    {
                        ModelState.AddModelError("state", "state changed from '{0}'".Fmt(State));
                        State = r.State;
                    }
                    if (r.Zip != (Zip ?? ""))
                    {
                        ModelState.AddModelError("zip", "zip changed from '{0}'".Fmt(Zip));
                        Zip = r.Zip;
                    }
                    if (!ModelState.IsValid)
                        return;
                }
            }
            

            switch (Name)
            {
                case "FamilyAddr":
                    UpdateValue(f, "AddressLineOne", Address1);
                    UpdateValue(f, "AddressLineTwo", Address2);
                    UpdateValue(f, "AddressToDate", ToDt);
                    UpdateValue(f, "CityName", City);
                    UpdateValue(f, "StateCode", State);
                    UpdateValue(f, "ResCodeId", ResCodeId);
                    UpdateValue(f, "ZipCode", Zip ?? "");
                    UpdateValue(f, "CountryName", Country);
                    if (Preferred)
                        UpdateValue(p, "AddressTypeId", 10);
                    if (fsb.Count > 0)
                        BadAddress = false;
                    UpdateValue(f, "BadAddressFlag", BadAddress);
                    break;
                case "PersonalAddr":
                    UpdateValue(p, "AddressLineOne", Address1);
                    UpdateValue(p, "AddressLineTwo", Address2);
                    UpdateValue(p, "AddressToDate", ToDt);
                    UpdateValue(p, "CityName", City);
                    UpdateValue(p, "StateCode", State);
                    UpdateValue(p, "ResCodeId", ResCodeId);
                    UpdateValue(p, "ZipCode", Zip ?? "");
                    UpdateValue(p, "CountryName", Country);
                    if (Preferred)
                        UpdateValue(p, "AddressTypeId", 30);
                    if (psb.Count > 0)
                        BadAddress = false;
                    UpdateValue(p, "BadAddressFlag", BadAddress);
                    break;
            }
            if (psb.Count > 0)
            {
                var c = new ChangeLog
                {
                    UserPeopleId = Util.UserPeopleId.Value,
                    PeopleId = PeopleId,
                    Field = Name,
                    Created = Util.Now
                };
                DbUtil.Db.ChangeLogs.InsertOnSubmit(c);
                c.ChangeDetails.AddRange(psb);
            }
            if (fsb.Count > 0)
            {
                var c = new ChangeLog
                {
                    FamilyId = p.FamilyId,
                    UserPeopleId = Util.UserPeopleId.Value,
                    PeopleId = PeopleId,
                    Field = Name,
                    Created = Util.Now
                };
                DbUtil.Db.ChangeLogs.InsertOnSubmit(c);
                c.ChangeDetails.AddRange(fsb);
            }
            try
            {
                DbUtil.Db.SubmitChanges();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }

            var sb = new StringBuilder();
            foreach (var c in psb)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
            foreach (var c in fsb)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
            if (!HttpContext.Current.User.IsInRole("Access"))
                if (psb.Count > 0 || fsb.Count > 0)
                {
                    DbUtil.Db.EmailRedacted(p.FromEmail, DbUtil.Db.GetNewPeopleManagers(),
                        "Address Info Changed",
                        "{0} changed the following information:<br />\n<table>{1}</table>"
                        .Fmt(Util.UserName, sb.ToString()));
                }
        }
        private List<ChangeDetail> fsb = new List<ChangeDetail>();
        private void UpdateValue(Family f, string field, object value)
        {
            var o = Util.GetProperty(f, field);
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            fsb.Add(new ChangeDetail(field, o, value));
            Util.SetProperty(f, field, value);
        }
        private List<ChangeDetail> psb = new List<ChangeDetail>();
        private void UpdateValue(Person p, string field, object value)
        {
            var o = Util.GetProperty(p, field);
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            psb.Add(new ChangeDetail(field, o, value));
            Util.SetProperty(p, field, value);
        }
        public static IEnumerable<SelectListItem> StateCodes()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.GetStateList(), "Code");
        }
    }
}
