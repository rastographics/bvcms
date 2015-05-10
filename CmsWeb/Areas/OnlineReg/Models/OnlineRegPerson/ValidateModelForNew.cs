using System;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public void ValidateModelForNew(ModelStateDictionary modelstate, int i)
        {
            modelState = modelstate;
            Index = i;
            IsValidForNew = true; // Assume true until proven false

            ValidateBasic();
            ValidateBirthdate();
            if (!IsValidForNew)
                return;
            ValidateBirthdayRange();
            ValidatePhone();
            ValidateEmailForNew();
            if (!CanProceedWithThisAddress()) 
                return;
            ValidateGender();
            ValidateMarital();
            ValidateMembership();
            IsValidForContinue = IsValidForNew = modelState.IsValid;
        }

        private bool CanProceedWithThisAddress()
        {
            var isnewfamily = whatfamily == 3;
            if (!isnewfamily)
                return true;

            if (!AddressLineOne.HasValue() && RequiredAddr())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].AddressLineOne), "address required.");
            if(RequiredZip() && !ZipCode.HasValue() && !RequiredAddr())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ZipCode), "zip required.");

            if (!RequiredZip() || !AddressLineOne.HasValue())
                return true;

            var hasCityStateOrZip = (City.HasValue() && State.HasValue()) || ZipCode.HasValue();
            if (!hasCityStateOrZip)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ZipCode), "zip required (or \"na\")");


            var countryIsOK = modelState.IsValid && AddressLineOne.HasValue() && (Country == "United States" || !Country.HasValue());
            if (!countryIsOK)
                return false;

            var r = AddressVerify.LookupAddress(AddressLineOne, AddressLineTwo, City, State, ZipCode);
            if (r.Line1 == "error") 
                return true; // Address Validator is not available, skip check

            if (r.found == false)
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ZipCode),
                    r.address + ", to skip address check, Change the country to USA, Not Validated");
                ShowCountry = RequiredAddr();
                return false;
            }

            // populdate Address corrections
            if (r.Line1 != AddressLineOne)
                AddressLineOne = r.Line1;
            if (r.Line2 != (AddressLineTwo ?? ""))
                AddressLineTwo = r.Line2;
            if (r.City != (City ?? ""))
                City = r.City;
            if (r.State != (State ?? ""))
                State = r.State;
            if (r.Zip != (ZipCode ?? ""))
                ZipCode = r.Zip;

            return true;
        }

        private void ValidateMarital()
        {
            if (!married.HasValue && RequiredMarital())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].married), "Please specify marital status");
        }

        private void ValidateGender()
        {
            if (!gender.HasValue && RequiredGender())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].gender), "Please specify gender");
        }

        private void ValidateMembership()
        {
            var foundname = Parent.GetNameFor(mm => mm.List[Index].Found);

            if (MemberOnly())
            {
                modelState.AddModelError(foundname, "Sorry, must be a member of church");
            }
            else if (org != null && setting.ValidateOrgIds.Count > 0 && !Parent.SupportMissionTrip)
            {
                modelState.AddModelError(foundname, "Must be member of specified organization");
            }
        }

        private void ValidateBirthdate()
        {
            var dobname = Parent.GetNameFor(mm => mm.List[Index].DateOfBirth);
            DateTime dt;
            if (RequiredDOB() && DateOfBirth.HasValue() && !Util.BirthDateValid(bmon, bday, byear, out dt))
                modelState.AddModelError(dobname, "birthday invalid");
            else if (!birthday.HasValue && RequiredDOB())
                modelState.AddModelError(dobname, "birthday required");
            if (birthday.HasValue && NoReqBirthYear() == false && birthday.Value.Year == Util.SignalNoYear)
            {
                modelState.AddModelError(dobname, "BirthYear is required");
                IsValidForNew = false;
            }

            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
            if (orgid == Util.CreateAccountCode && age < minage)
                modelState.AddModelError(dobname, "must be {0} to create account".Fmt(minage));

            if (ComputesOrganizationByAge() && GetAppropriateOrg() == null)
                modelState.AddModelError(dobname, NoAppropriateOrgError);
        }
        private void ValidatePhone()
        {
            var n = 0;
            if (Phone.HasValue() && Phone.GetDigits().Length >= 10)
                n++;
            if (ShowAddress && HomePhone.HasValue() && HomePhone.GetDigits().Length >= 10)
                n++;

            if (RequiredPhone() && n == 0)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].Phone), "cell or home phone required");

            if (HomePhone.HasValue() && HomePhone.GetDigits().Length > 20)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].HomePhone), "homephone too long");
        }
        private void ValidateEmailForNew()
        {
            if (EmailAddress.HasValue())
                EmailAddress = EmailAddress.Trim();

            if (!EmailAddress.HasValue() || !Util.ValidEmail(EmailAddress))
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].person.EmailAddress),
                    "Please specify a valid email address.");
        }
    }
}
