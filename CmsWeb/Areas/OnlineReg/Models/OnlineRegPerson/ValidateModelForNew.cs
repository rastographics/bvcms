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
            IsValidForContinue = true; // Assume true until proven false

            ValidateBasic();
            ValidateBirthdate();
            if (!IsValidForNew)
                return;
            ValidateBirthdayRange(selectFromFamily: false);
            ValidatePhone();
            ValidateEmailForNew();
            if (!CanProceedWithThisAddress())
            {
                IsValidForContinue = false;
                return;
            }
            ValidateCampus();
            ValidateGender();
            ValidateMarital();
            ValidateMembership();
            IsValidForContinue = IsValidForNew = modelState.IsValid;
        }

        public bool CanProceedWithThisAddress()
        {
            if (!AddressLineOne.HasValue() && RequiredAddr())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].AddressLineOne), "address required.");
            if (RequiredZip() && !ZipCode.HasValue() && !RequiredAddr())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ZipCode), "zip required.");

            if (!RequiredZip() || !AddressLineOne.HasValue())
                return true;

            var hasCityStateOrZip = (City.HasValue() && State.HasValue()) || ZipCode.HasValue();
            if (!hasCityStateOrZip)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ZipCode), "zip required (or \"na\")");

            if (!modelState.IsValid)
            {
                Log("AddressNotValid");
                return false;
            }
            var countryIsOK = modelState.IsValid && AddressLineOne.HasValue() && (Country == "United States" || !Country.HasValue());
            if (!countryIsOK)
                return true; // not going to validate address

            var r = AddressVerify.LookupAddress(AddressLineOne, AddressLineTwo, City, State, ZipCode);
            if (r.Line1 == "error")
                return true; // Address Validator is not available, skip check

            if (r.found == false)
                return true; // not going to bother them to ask for better address

            // populate Address corrections
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
            if (married.HasValue || !RequiredMarital())
                return;
            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].married), "Please specify marital status");
            Log("MaritalRequired");
        }

        private void ValidateGender()
        {
            if (gender.HasValue || !RequiredGender())
                return;
            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].gender), "Please specify gender");
            Log("GenderRequired");
        }
        private void ValidateCampus()
        {
            if (!ShowCampusOnRegistration || !RequiredCampus() || Campus.ToInt() > 0)
                return;
            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].Campus), $"Please choose {Util2.CampusLabel}.");
            Log("CampusRequired");
        }

        private void ValidateMembership()
        {
            var foundname = Parent.GetNameFor(mm => mm.List[Index].Found);

            if (MemberOnly())
            {
                modelState.AddModelError(foundname, "Sorry, must be a member of church");
                Log("MustBeChurchMember");
            }
            else if (org != null && setting.ValidateOrgIds.Count > 0 && !Parent.SupportMissionTrip)
            {
                modelState.AddModelError(foundname, "Must be member of specified organization");
                Log("MustBeSpecifiedOrgMember");
            }
        }

        private void ValidateBirthdate()
        {
            var dobname = Parent.GetNameFor(mm => mm.List[Index].DateOfBirth);
            DateTime dt;
            if (RequiredDOB() && DateOfBirth.HasValue() && !Util.BirthDateValid(bmon, bday, byear, out dt))
                modelState.AddModelError(dobname, "birthday invalid");
            else if (!BestBirthday.HasValue && RequiredDOB())
                modelState.AddModelError(dobname, "birthday required");
            if (BestBirthday.HasValue && BirthYearRequired() && BestBirthday.Value.Year == Util.SignalNoYear)
            {
                modelState.AddModelError(dobname, "BirthYear is required");
                Log("BirthYearRequired");
                IsValidForNew = false;
            }

            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
            if (orgid == Util.CreateAccountCode && age < minage)
            {
                modelState.AddModelError(dobname, $"must be {minage} to create account");
                Log("UnderAgeForAccount");
            }
            if (ComputesOrganizationByAge() && GetAppropriateOrg() == null)
            {
                modelState.AddModelError(dobname, NoAppropriateOrgError);
                Log("NoAppropriateOrg");
            }
        }
        private void ValidatePhone()
        {
            if (PhoneOK)
                return;
            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].Phone), "cell or home phone required");
            Log("NeedPhone");
        }

        public bool PhoneOK
        {
            get
            {
                var hasrequired = !RequiredPhone() || Phone.HasValue();
                var lengthok = !Phone.HasValue() || Phone.GetDigits().Length >= 10;
                return hasrequired && lengthok;
            }
        }

        private void ValidateEmailForNew()
        {
            if (EmailAddress.HasValue())
                EmailAddress = EmailAddress.Trim();

            if (EmailAddress.HasValue() && Util.ValidEmail(EmailAddress))
                return;

            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].person.EmailAddress),
                "Please specify a valid email address.");
            Log("NeedValidEmail");
        }
    }
}
