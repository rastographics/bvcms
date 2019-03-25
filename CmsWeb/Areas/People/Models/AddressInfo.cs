using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class AddressInfo
    {
        public int PeopleId
        {
            get { return _peopleId; }
            set
            {
                if (_peopleId != value)
                    person = DbUtil.Db.LoadPersonById(value);
                _peopleId = value;
            }
        }

        public CmsData.Person person { get; set; }

        public string Name { get; set; }
        public string OtherName
        {
            get
            {
                switch (Name)
                {
                    case "PersonalAddr": return "FamilyAddr";
                    case "FamilyAddr": return "PersonalAddr";
                }
                return "";
            }
        }
        public string OtherDisplay
        {
            get
            {
                switch (Name)
                {
                    case "PersonalAddr": return "Family";
                    case "FamilyAddr": return "Personal";
                }
                return "Address";
            }
        }
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
                        default:
                            return true;
                    }
                }
                return _CanUserEditAddress.Value;
            }
        }

        public string Display
        {
            get
            {
                switch (Name)
                {
                    case "PersonalAddr": return "Personal";
                    case "FamilyAddr": return "Family";
                }
                return "Address";
            }
        }

        [DisplayName("Address Line 1"), RemoveNA]
        public string AddressLineOne { get; set; }

        [DisplayName("Address Line 2"), RemoveNA]
        public string AddressLineTwo { get; set; }

        [RemoveNA]
        public string CityName { get; set; }

        public CodeInfo StateCode { get; set; }

        [RemoveNA]
        public string ZipCode { get; set; }

        public CodeInfo Country { get; set; }

        public string AddrCityStateZip()
        {
            var sb = new StringBuilder();
            sb.AppendLine(AddressLineOne);
            if (AddressLineTwo.HasValue())
                sb.AppendLine(AddressLineTwo);
            sb.AppendLine(Util.FormatCSZ(CityName, StateCode.Value, ZipCode));
            return sb.ToString();
        }
        public string AddrCityStateZipLine()
        {
            var sb = new StringBuilder();
            sb.Append(AddressLineOne);
            if (AddressLineTwo.HasValue())
                sb.Append(", " + AddressLineTwo);
            if (sb.Length > 0)
                sb.Append(", " + Util.FormatCSZ(CityName, StateCode.Value, ZipCode));
            return sb.ToString();
        }
        public string MapAddrCityStateZip()
        {
            return AddrCityStateZip().Replace("\n", ",");
        }
        public string CityStateZip()
        {
            return Util.FormatCSZ(CityName, StateCode.Value, ZipCode);
        }

        public AddressVerify.AddressResult Result { get; set; }

        [DisplayName("Bad Address Flag")]
        public bool BadAddress { get; set; }
        public string BadAddressClass => BadAddress ? "badaddress" : "";

        [DisplayName("Resident Code")]
        public CodeInfo ResCode { get; set; }

        [DisplayName("Primary Address")]
        public bool Preferred { get; set; }

        [DisplayName("From Date")]
        public DateTime? FromDt { get; set; }

        [DisplayName("To Date")]
        public DateTime? ToDt { get; set; }

        public static IEnumerable<SelectListItem> ResCodes()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.ResidentCodesWithZero(), "Id");
        }
        public static IEnumerable<SelectListItem> States()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.GetStateList(), "Code");
        }
        public static IEnumerable<SelectListItem> Countries
        {
            get
            {
                var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList().Where(c => c.Code != "NA"), null);
                list.Insert(0, new SelectListItem {Text = "(not specified)", Value = ""});
                return list;
            }
        }

        public AddressInfo()
        {
            Result = new AddressVerify.AddressResult();
            StateCode = new CodeInfo("", "State");
            Country = new CodeInfo("", "Country");
        }
        public AddressInfo(string address1, string address2, string city, string state, string zip, string country)
        {
            Result = new AddressVerify.AddressResult();
            AddressLineOne = address1;
            AddressLineTwo = address2;
            CityName = city;
            StateCode = new CodeInfo(state, "State");
            ZipCode = zip;
            Country = new CodeInfo(country, "Country");
        }

        public static AddressInfo GetAddressInfo(int id, string typeid)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, p);
            var a = new AddressInfo { PeopleId = id };
            switch (typeid)
            {
                case "PrimaryAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.AddressLineOne = p.PrimaryAddress;
                    a.AddressLineTwo = p.PrimaryAddress2;
                    a.BadAddress = p.PrimaryBadAddrFlag == 1;
                    a.CityName = p.PrimaryCity;
                    a.ZipCode = p.PrimaryZip;
                    a.StateCode = new CodeInfo(p.PrimaryState, "State");
                    a.Country = new CodeInfo(p.PrimaryCountry, "Country");
                    a.ResCode = new CodeInfo(p.PrimaryResCode, "ResCode");
                    break;
                case "FamilyAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.AddressLineOne = p.Family.AddressLineOne;
                    a.AddressLineTwo = p.Family.AddressLineTwo;
                    a.FromDt = p.Family.AddressFromDate;
                    a.ToDt = p.Family.AddressToDate;
                    a.BadAddress = p.Family.BadAddressFlag ?? false;
                    a.CityName = p.Family.CityName;
                    a.ZipCode = p.Family.ZipCode;
                    a.StateCode = new CodeInfo(p.Family.StateCode, "State");
                    a.Country = new CodeInfo(p.Family.CountryName, "Country");
                    a.ResCode = new CodeInfo(p.Family.ResCodeId, "ResCode");
                    a.Preferred = p.AddressTypeId == 10;
                    break;
                case "PersonalAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.AddressLineOne = p.AddressLineOne;
                    a.AddressLineTwo = p.AddressLineTwo;
                    a.FromDt = p.AddressFromDate;
                    a.ToDt = p.AddressToDate;
                    a.BadAddress = p.BadAddressFlag ?? false;
                    a.CityName = p.CityName;
                    a.ZipCode = p.ZipCode;
                    a.StateCode = new CodeInfo(p.StateCode, "State");
                    a.Country = new CodeInfo(p.CountryName, "Country");
                    a.ResCode = new CodeInfo(p.ResCodeId, "ResCode");
                    a.Preferred = p.AddressTypeId == 30;
                    break;
            }
            return a;
        }
        public void SetAddressInfo()
        {
            AddressLineOne = Result.Line1;
            AddressLineTwo = Result.Line2;
            CityName = Result.City;
            ZipCode = Result.Zip;
            StateCode = new CodeInfo(Result.State, "State");
        }

        public bool? Addrok { get; set; }
        public string Error { get; set; }
        public bool Saved { get; set; }
        public bool ResultChanged { get; set; }
        public bool ResultNotFound => Result.found == false || Result.error.HasValue();

        public bool IsValid
        {
            get
            {
                if (Result.found == null) // not checked, don't report as invalid yet
                    return Addrok != false;
                return Addrok == true && !ResultChanged && !ResultNotFound;
            }
        }

        public void UpdateAddress(ModelStateDictionary modelState, bool forceSave = false)
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var f = p.Family;
            var rc = DbUtil.Db.FindResCode(ZipCode, Country.Value);
            ResCode = new CodeInfo(rc, "ResCode");

            switch (Name)
            {
                case "FamilyAddr":
                    UpdateValue(f, "AddressLineOne", AddressLineOne);
                    UpdateValue(f, "AddressLineTwo", AddressLineTwo);
                    UpdateValue(f, "AddressFromDate", FromDt);
                    UpdateValue(f, "AddressToDate", ToDt);
                    UpdateValue(f, "CityName", CityName);
                    UpdateValue(f, "StateCode", StateCode.Value);
                    UpdateValue(f, "ZipCode", ZipCode ?? "");
                    UpdateValue(f, "CountryName", Country.Value);
                    UpdateValue(f, "ResCodeId", rc);
                    if (Preferred)
                        UpdateValue(p, "AddressTypeId", 10);
                    UpdateValue(f, "BadAddressFlag", BadAddress);
                    break;
                case "PersonalAddr":
                    UpdateValue(p, "AddressLineOne", AddressLineOne);
                    UpdateValue(p, "AddressLineTwo", AddressLineTwo);
                    UpdateValue(f, "AddressFromDate", FromDt);
                    UpdateValue(p, "AddressToDate", ToDt);
                    UpdateValue(p, "CityName", CityName);
                    UpdateValue(p, "StateCode", StateCode.Value);
                    UpdateValue(p, "ZipCode", ZipCode ?? "");
                    UpdateValue(p, "CountryName", Country.Value);
                    UpdateValue(p, "ResCodeId", rc);
                    if (Preferred)
                        UpdateValue(p, "AddressTypeId", 30);
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
                DbUtil.LogActivity($"Update Address for: {person.Name}");
            }
            catch (InvalidOperationException ex)
            {
                Error = ex.Message;
                return;
            }
            Saved = true;

            if (!HttpContextFactory.Current.User.IsInRole("Access"))
                if (psb.Count > 0 || fsb.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var c in psb)
                        sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
                    foreach (var c in fsb)
                        sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
                    var np = DbUtil.Db.GetNewPeopleManagers();
                    if(np != null)
                        DbUtil.Db.EmailRedacted(p.FromEmail, np,
                            "Address Info Changed",
                            $"{Util.UserName} changed the <a href='{DbUtil.Db.ServerLink($"/Person2/{PeopleId}")}'>following information</a>:<br />\n<table>{sb}</table>");
                }
        }

        public string GetNameFor<M, P>(M model, Expression<Func<M, P>> ex)
        {
            return ExpressionHelper.GetExpressionText(ex);
        }
        public bool ValidateAddress(ModelStateDictionary modelState)
        {
            //            if (!Address1.HasValue())
            //                modelState.AddModelError(this.GetNameFor(m => m.Address1), "Street Address Required");
            //            if ((!City.HasValue() || !State.Value.HasValue()) && !Zip.HasValue())
            //                modelState.AddModelError(this.GetNameFor(m => m.Zip), "Require either Zip Code or City/State");

            Addrok = modelState.IsValid;
            if (Addrok == false)
                return false;

            if ((Country.Value == "United States"))
            {
                Result = AddressVerify.LookupAddress(AddressLineOne, AddressLineTwo, CityName, StateCode.Value, ZipCode);
                const string alertdiv = @" <div class=""alert {0}"">{1}</div>";
                if (Result.Line1 == "error")
                    Error = string.Format(alertdiv, "alert-danger", "<h4>Network Error</h4>");
                else if (ResultNotFound)
                    Error = string.Format(alertdiv, "alert-danger", $"<h4>Address Not Validated</h4><h6>{Result.error}</h6>");
                else if (Result.Changed(AddressLineOne, AddressLineTwo, CityName, StateCode.Value, ZipCode))
                {
                    var msg = @"<h4>Address Found and Adjusted by USPS</h4><h6>What you entered</h6>"
                              + AddrCityStateZip().Replace("\n", "<br/>\n");
                    ResultChanged = true;
                    var rc = DbUtil.Db.FindResCode(Result.Zip, Country.Value);
                    ResCode = new CodeInfo(rc, "ResCode");
                    SetAddressInfo();
                    Error = string.Format(alertdiv, "alert-success", msg);
                }
            }
            return !Error.HasValue();
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
        private int _peopleId;

        private void UpdateValue(CmsData.Person p, string field, object value)
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
