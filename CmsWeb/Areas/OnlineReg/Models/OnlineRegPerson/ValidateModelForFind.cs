using CmsData;
using CmsData.Codes;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        private DateTime? _bestbirthday;
        private ModelStateDictionary modelState;

        public DateTime? BestBirthday
        {
            get
            {
                if (_bestbirthday.HasValue)
                {
                    return _bestbirthday;
                }

                _bestbirthday = birthday;
                if (person?.BirthDate != null)
                {
                    _bestbirthday = person.BirthDate ?? birthday;
                }

                return _bestbirthday;
            }
        }

        public void ValidateModelForFind(ModelStateDictionary modelstate, int i, bool selectFromFamily = false)
        {
            if (IsValidForNew) // This should never be the case
            {
                throw new Exception("Unexpected onlinereg state: IsValidForNew is true and in ValidateModelForFind");
            }

            DbUtil.Db.SetNoLock();
            modelState = modelstate;
            IsValidForContinue = true; // true till proven false
            IsValidForExisting = true; // true till proven false
            Index = i;
            var foundname = Parent.GetNameFor(mm => mm.List[Index].Found);

            if (PeopleId > 0 && Parent.UserPeopleId.HasValue)
            {
                // from a register link or logged in, don't validate basic stuff
                Found = true;
                ValidateAgeRequirement();
                ValidateEmail();
            }
            else // validate basic stuff
            {
                if (selectFromFamily == false)
                {
                    PeopleId = null; // not found yet
                }

                if (IsFamily)
                {
                    foundname = "fammember-" + PeopleId;
                }

                ValidateBasic();
                if (!modelState.IsValid)
                {
                    IsValidForExisting = false;
                    return;
                }
                Found = person != null;
                ValidateAgeRequirement();
                ValidateEmail();

                if (!modelState.IsValid || count != 1)
                {
                    ValidateBirthdayRange(selectFromFamily);
                    return;
                }
            }

            PopulateExistingInformation();
            if (AlreadyPendingRegistrant(foundname))
            {
                return;
            }

            if (EmailRequiredForThisRegistration(foundname))
            {
                return;
            }

            if (NoAppropriateOrgFound(selectFromFamily))
            {
                return;
            }

            if (MustBeChurchMember(foundname))
            {
                return;
            }

            if (CannotSupportSelf(foundname))
            {
                return;
            }

            ValidForOrg(selectFromFamily, foundname);
        }

        private void ValidForOrg(bool selectFromFamily, string foundname)
        {
            if (org == null)
            {
                return;
            }

            if (NotValidForCreateAccountRegistration(foundname))
            {
                return;
            }

            if (AllowSenderToBecomeGoerToo())
            {
                return;
            }

            if (AlreadyRegisteredNotAllowed(foundname))
            {
                return;
            }

            if (MustBeMemberOfAnotherOrgToRegister(foundname))
            {
                return;
            }

            if (MustNotBeMemberOfAnotherOrg(foundname))
            {
                return;
            }

            ValidateBirthdayRange(selectFromFamily);
        }

        private void ValidateBasic()
        {
            if (!FirstName.HasValue())
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].FirstName), "first name required");
            }

            if (!LastName.HasValue())
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].LastName), "last name required");
            }

            var mindate = DateTime.Parse("1/1/1753");
            var HasOneOfThreeRequired = false;

            DateTime dt;
            if (DateOfBirth.HasValue() && !Util.BirthDateValid(bmon, bday, byear, out dt))
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth), "birthday invalid");
            }

            if (BestBirthday.HasValue && BestBirthday < mindate)
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth), "invalid date");
            }

            if (BestBirthday.HasValue && BestBirthday > mindate)
            {
                HasOneOfThreeRequired = true;
            }

            if (Util.ValidEmail(EmailAddress))
            {
                HasOneOfThreeRequired = true;
            }

            var d = Phone.GetDigits().Length;
            if (Phone.HasValue() && d >= 10)
            {
                HasOneOfThreeRequired = true;
            }

            if (d > 20)
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].Phone), "too many digits in phone");
            }

            if (!HasOneOfThreeRequired)
            {
                modelState.AddModelError("FORM", "we require one of valid birthdate, email or phone to find an Existing profile");
            }

            if (!Util.ValidEmail(EmailAddress) && (person == null || !Util.ValidEmail(person.EmailAddress)))
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].EmailAddress), "valid email required for registration confirmation");
            }

            if (Phone.HasValue() && d < 10)
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].Phone), "10+ digits required");
            }

            if (!modelState.IsValid)
            {
                Log("InvalidBasic");
            }
        }

        private void ValidateAgeRequirement()
        {
            if (ComputesOrganizationByAge() && !BestBirthday.HasValue)
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth), "birthday required");
            }

            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
            if (orgid == Util.CreateAccountCode && age < minage)
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth),
                    $"must be {minage} to create account");
            }

            if (!modelState.IsValid)
            {
                Log("InvalidAge");
            }
        }

        private void ValidateEmail()
        {
            if (!EmailAddress.HasValue() || !Util.ValidEmail(EmailAddress))
            {
                if (person == null)
                {
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].EmailAddress),
                        "Please specify a valid email address.");
                    Log("InvalidEmail");
                }
                else if (!Util.ValidEmail(person.EmailAddress))
                {
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].EmailAddress),
                        "No valid email address needed for confirmation on record");
                    Log("InvalidEmailOnRecord");
                }
            }
        }

        private void ValidateBirthdayRange(bool selectFromFamily)
        {
            if (ManageSubscriptions())
            {
                return;
            }

            if (org == null)
            {
                NoAppropriateOrgError = "No Appropriate Org Found";
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth), NoAppropriateOrgError);
                Log("No appropriate org");
            }
            else if (!BestBirthday.HasValue && (org.BirthDayStart.HasValue || org.BirthDayEnd.HasValue))
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth), "birthday required");
                RegistrantProblem = @"**Birthday is required for this registration**";
                Log("BirthdayRequired");
            }
            else if (BestBirthday.HasValue)
            {
                if ((org.BirthDayStart.HasValue && BestBirthday < org.BirthDayStart)
                    || (org.BirthDayEnd.HasValue && BestBirthday > org.BirthDayEnd))
                {
                    var name = selectFromFamily
                        ? "fammember-" + PeopleId
                        : Parent.GetNameFor(mm => mm.List[Index].DateOfBirth);
                    modelState.AddModelError(name, "birthday outside age allowed range");
                    RegistrantProblem = @"**Birthday is outside the allowed age range**";

                    Log("InvalidBirthdate");
                    IsValidForExisting = false;
                }
            }
            IsValidForExisting = IsValidForContinue = modelState.IsValid;
        }

        private void PopulateExistingInformation()
        {
            AddressLineOne = person.PrimaryAddress;
            City = person.PrimaryCity;
            State = person.PrimaryState;
            ZipCode = person.PrimaryZip;
            gender = person.GenderId;
            married = person.MaritalStatusId == 2 ? 2 : 1;
        }

        private bool AlreadyPendingRegistrant(string foundname)
        {
            if (Parent.List.Count(ii => ii.PeopleId == PeopleId) <= 1)
            {
                return false;
            }

            Log("AlreadyPending");
            modelState.AddModelError(foundname, "Person already in Pending Registration");
            CancelText = "Register a different person";
            RegistrantProblem = @"**Person already in Pending Registration**";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }

        private bool EmailRequiredForThisRegistration(string foundname)
        {
            var needemail = !person.EmailAddress.HasValue() &&
                             (ManageSubscriptions()
                              || orgid == Util.CreateAccountCode
                              || OnlineGiving()
                              || ManageGiving()
                              || OnlinePledge()
                              || !EmailAddress.HasValue() // register emai
                                 );
            if (!needemail)
            {
                return false;
            }

            modelState.AddModelError(foundname, "No Email Address on record");
            Log("NoEmailOnRecord");
            RegistrantProblem = @"
** No Email Address on Record**
We have found your record but we have no email address for you.
This means that we cannot proceed until we have that to protect your data.
Please call the church to resolve this before we can complete your information.";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }

        private bool NoAppropriateOrgFound(bool selectFromFamily = false)
        {
            if (!ComputesOrganizationByAge() || org != null)
            {
                return false;
            }

            NoAppropriateOrgError = "Sorry, cannot find an appropriate age group";
            RegistrantProblem = NoAppropriateOrgError;
            Log("NoAppropriateOrg");
            if (selectFromFamily)
            {
                modelState.AddModelError("fammember-" + person.PeopleId, NoAppropriateOrgError);
            }
            else
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].DateOfBirth), NoAppropriateOrgError);
            }

            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }

        private bool MustBeChurchMember(string foundname)
        {
            if (!MemberOnly() || person.MemberStatusId == MemberStatusCode.Member)
            {
                return false;
            }

            modelState.AddModelError(foundname, "Sorry, must be a member of church");
            Log("MustBeChurchMember");
            RegistrantProblem = @"**Sorry, must be a member of this church**";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }
        private bool CannotSupportSelf(string foundname)
        {
            if (!Parent.SupportMissionTrip || Parent.GoerId != person.PeopleId)
            {
                return false;
            }

            modelState.AddModelError(foundname, "Use a paylink to support self");
            Log("CannotSupportSelf");
            RegistrantProblem = @"**Sorry, Cannot support self from this page**";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }

        private bool NotValidForCreateAccountRegistration(string foundname)
        {
            if (org.RegistrationTypeId != RegistrationTypeCode.CreateAccount)
            {
                return false;
            }
#if DEBUG2
#else
            if (person.Users.Any())
            {
                modelState.AddModelError(foundname, "You already have an account");
                Log("AccountAlreadyExists");
                IsValidForContinue = false;
                IsValidForExisting = false;
                return true;
            }
            if (Util.ValidEmail(person.EmailAddress))
            {
                return false;
            }

            modelState.AddModelError(foundname, "You must have a valid email address on record");
            Log("NeedEmailOnRecordForAccount");
            RegistrantProblem = @"
We have found your record but we do not have a valid email for you.
For your protection, we cannot continue to create an account.
We can't use the one you enter online here since we can't be sure this is you.
Please call the church to resolve this before we can complete your account.
";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
#endif
        }

        private bool AllowSenderToBecomeGoerToo()
        {
            var om = org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == PeopleId);
            var senderWantsToBeGoerToo = (om != null
                                          && org.IsMissionTrip == true
                                          && !Parent.SupportMissionTrip
                                          && om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Sender")
                                          && om.OrgMemMemTags.All(mm => mm.MemberTag.Name != "Goer"));
            if (!senderWantsToBeGoerToo)
            {
                return false;
            }

            IsValidForContinue = true;
            IsValidForExisting = true;
            return true;
        }

        private bool AlreadyRegisteredNotAllowed(string foundname)
        {
            var om = org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == PeopleId);
            if (om == null || setting.AllowReRegister ||
                om.Organization.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes ||
                Parent.SupportMissionTrip)
            {
                return false;
            }

            modelState.AddModelError(foundname, "This person is already registered");
            Log("AlreadyRegistered");
            RegistrantProblem = @"**This person is already registered**";
            CancelText = "Register a different person";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }

        private bool MustBeMemberOfAnotherOrgToRegister(string foundname)
        {
            if (setting.ValidateOrgIds.Count <= 0 || Parent.SupportMissionTrip)
            {
                return false;
            }

            var reqmemberids = setting.ValidateOrgIds.Where(ii => ii > 0).ToList();
            if (reqmemberids.Count <= 0)
            {
                return false;
            }

            if (person.OrganizationMembers.Any(mm => reqmemberids.Contains(mm.OrganizationId)))
            {
                return false;
            }

            modelState.AddModelError(foundname, "Must be member of specified organization");
            Log("MustBeMemberOfSpecifiedOrg");
            RegistrantProblem = @"**Must be a member of specified organization to register**";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }

        private bool MustNotBeMemberOfAnotherOrg(string foundname)
        {
            if (setting.ValidateOrgIds.Count <= 0 || Parent.SupportMissionTrip)
            {
                return false;
            }

            var reqnomemberids = setting.ValidateOrgIds.Where(ii => ii < 0).ToList();
            if (reqnomemberids.Count <= 0)
            {
                return false;
            }

            if (!person.OrganizationMembers.Any(mm => reqnomemberids.Contains(-mm.OrganizationId)))
            {
                return false;
            }

            modelState.AddModelError(foundname, "Must not be a member of specified organization");
            Log("MustNotBeMemberOfSpecifiedOrg");
            RegistrantProblem = @"**Must not be a member of specified organization to register**";
            IsValidForContinue = false;
            IsValidForExisting = false;
            return true;
        }
    }
}
