using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData.Codes;
using CmsWeb;

namespace CmsWeb.Models
{
    public partial class OnlineRegPersonModel
    {
        public bool IsValidForContinue { get; set; }
        private void ValidateBirthdayRange(ModelStateDictionary ModelState)
        {
            var i = Index();
            if (org != null)
                if (!birthday.HasValue && (org.BirthDayStart.HasValue || org.BirthDayEnd.HasValue))
                    ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].dob), "birthday required");
                else if (birthday.HasValue)
                {
                    if ((org.BirthDayStart.HasValue && birthday < org.BirthDayStart)
                        || (org.BirthDayEnd.HasValue && birthday > org.BirthDayEnd))
                        ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].dob), "birthday outside age allowed range");
                }
        }
        private void ValidBasic(ModelStateDictionary ModelState)
        {
            var i = Index();
            if (!first.HasValue())
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].first), "first name required");
            if (!last.HasValue())
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].last), "last name required");

            var mindate = DateTime.Parse("1/1/1753");
            int n = 0;
            if (birthday.HasValue && birthday < mindate)
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].dob), "invalid date");
            if (birthday.HasValue && birthday > mindate)
                n++;
            if (Util.ValidEmail(email))
                n++;
            var d = phone.GetDigits().Length;
            if (phone.HasValue() && d >= 10)
                n++;
            if (d > 20)
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].phone), "too many digits in phone");

            if (n == 0)
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].dob), "we require one of valid birthdate, email or phone to find your record");

            if (!Util.ValidEmail(email))
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].email), "valid email required");
            if (phone.HasValue() && d < 10)
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].phone), "10+ digits required");
        }
        public void ValidateModelForFind(ModelStateDictionary ModelState, OnlineRegModel m, bool selectfromfamily = false)
        {
            var i = Index();
            IsValidForContinue = true; // true till proven false
            if (UserSelectsOrganization())
                if ((classid ?? 0) == 0)
                {
                    var nameclassid = Parent.GetNameFor(mm => mm.List[i].classid);
                    const string pleaseChooseAGroupEvent = "please choose a group/event";
                    if (IsFamily)
                        ModelState.AddModelError(nameclassid, pleaseChooseAGroupEvent);
                    else
                        ModelState.AddModelError(nameclassid, pleaseChooseAGroupEvent);
                    IsValidForExisting = ModelState.IsValid;
                    return;
                }
            var dobname = Parent.GetNameFor(mm => mm.List[i].dob);
            var foundname = Parent.GetNameFor(mm => mm.List[i].Found);
            if (!PeopleId.HasValue)
                ValidBasic(ModelState);
            if (ComputesOrganizationByAge() && !birthday.HasValue)
                ModelState.AddModelError(dobname, "birthday required");
            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
            if (orgid == Util.CreateAccountCode && age < minage)
                ModelState.AddModelError(dobname, "must be {0} to create account".Fmt(minage));
            if (!IsFamily && (!email.HasValue() || !Util.ValidEmail(email)))
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].email), "Please specify a valid email address.");
            if (ModelState.IsValid)
            {
                Found = person != null;
                if (count == 1)
                {
                    address = person.PrimaryAddress;
                    city = person.PrimaryCity;
                    state = person.PrimaryState;
                    zip = person.PrimaryZip;
                    gender = person.GenderId;
                    married = person.MaritalStatusId == 2 ? 2 : 1;

                    if (!person.EmailAddress.HasValue() &&
                        (ManageSubscriptions()
                            || orgid == Util.CreateAccountCode
                            || OnlineGiving()
                            || ManageGiving()
                            || OnlinePledge()
                        ))
                    {
                        ModelState.AddModelError(foundname, "No Email Address on record");
                        NotFoundText = @"We have found your record but we have no email address for you.<br/>
This means that we cannot proceed until we have that to protect your data.<br/>
Please call the church to resolve this before we can complete your information.";
                        IsValidForContinue = false;
                    }
                    else if (ComputesOrganizationByAge() && org == null)
                    {
                        var msg = NoAppropriateOrgError ?? "Sorry, no approprate org";
                        if (selectfromfamily)
                            ModelState.AddModelError("age-" + person.PeopleId, msg);
                        else
                            ModelState.AddModelError(dobname, msg);
                        IsValidForContinue = false;
                    }
                    else if (MemberOnly() && person.MemberStatusId != MemberStatusCode.Member)
                    {
                        ModelState.AddModelError(foundname, "Sorry, must be a member of church");
                        IsValidForContinue = false;
                    }
                    else if (org != null)
                    {
                        var om = org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == PeopleId);
                        if (org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
                        {
#if DEBUG
#else
                            if (person.Users.Count() > 0)
                            {
                                ModelState.AddModelError(foundname, "You already have an account");
                                IsValidForContinue = false;
                            }
                            if (!Util.ValidEmail(person.EmailAddress))
                            {
                                ModelState.AddModelError(foundname, "You must have a valid email address on record");
                                NotFoundText = @"We have found your record but we do not have a valid email for you.<br/>
For your protection, we cannot continue to create an account.<br />
We can't use the one you enter online here since we can't be sure this is you.<br />
Please call the church to resolve this before we can complete your account.<br />";
                                IsValidForContinue = false;
                            }
#endif
                        }
                        else if (om != null && org.IsMissionTrip == true
                            && !Parent.SupportMissionTrip
                            && om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Sender")
                            && !om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer"))
                        {
                            IsValidForContinue = true;
                        }
                        else if (om != null && setting.AllowReRegister == false
                            && om.Organization.RegistrationTypeId != RegistrationTypeCode.ChooseVolunteerTimes
                            && !Parent.SupportMissionTrip)
                        {
                            ModelState.AddModelError(foundname, "This person is already registered");
                            IsValidForContinue = false;
                        }
                        else if (setting.ValidateOrgIds.Count > 0)
                        {
                            var reqmemberids = setting.ValidateOrgIds.Where(ii => ii > 0).ToList();
                            if (reqmemberids.Count > 0)
                                if (!person.OrganizationMembers.Any(mm => reqmemberids.Contains(mm.OrganizationId)))
                                {
                                    ModelState.AddModelError(foundname, "Must be member of specified organization");
                                    IsValidForContinue = false;
                                }
                            var reqnomemberids = setting.ValidateOrgIds.Where(ii => ii < 0).ToList();
                            if (reqnomemberids.Count > 0)
                                if (person.OrganizationMembers.Any(mm => reqnomemberids.Contains(-mm.OrganizationId)))
                                {
                                    ModelState.AddModelError(foundname, "Must be not be member of specified organization");
                                    IsValidForContinue = false;
                                }
                        }
                    }
                    if (m.List.Count(ii => ii.PeopleId == PeopleId) > 1)
                    {
                        ModelState.AddModelError(foundname, "Person already in Pending Registrations");
                        IsValidForContinue = false;
                    }
                }
                else if (count > 1)
                {
                    ModelState.AddModelError(foundname, "More than one match, sorry");
                    NotFoundText = @"We have found more than one record that matches your information
This is an unexpected error and we don't know which one is you.
Please call the church to resolve this before we can complete your registration.";
                    IsValidForContinue = false;
                }
                else if (count == 0)
                {
                    ModelState.AddModelError(foundname, "record not found");
                    NotFoundText =
@"The first and last name in addition to either birthday, email, or phone,
must match a record in the system.
Please search with a different email, phone, or birthday.";
                }
            }
            ValidateBirthdayRange(ModelState);
            IsValidForExisting = ModelState.IsValid;
        }
        public bool IsValidForNew { get; set; }
        internal void ValidateModelForNew(ModelStateDictionary ModelState)
        {
            var i = Index();
            var dobname = Parent.GetNameFor(mm => mm.List[i].dob);
            var foundname = Parent.GetNameFor(mm => mm.List[i].Found);
            var isnewfamily = whatfamily == 3;
            ValidBasic(ModelState);
            DateTime dt;
            if (RequiredDOB() && dob.HasValue() && !Util.BirthDateValid(bmon, bday, byear, out dt))
                ModelState.AddModelError(dobname, "birthday invalid");
            else if (!birthday.HasValue && RequiredDOB())
                ModelState.AddModelError(dobname, "birthday required");
            if(birthday.HasValue && setting.NoReqBirthYear == false && birthday.Value.Year == Util.SignalNoYear)
                ModelState.AddModelError(dobname, "BirthYear is required");

            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
            if (orgid == Util.CreateAccountCode && age < minage)
                ModelState.AddModelError(dobname, "must be {0} to create account".Fmt(minage));

            if (ComputesOrganizationByAge() && GetAppropriateOrg() == null)
                ModelState.AddModelError(dobname, NoAppropriateOrgError);

            ValidateBirthdayRange(ModelState);
            int n = 0;
            if (phone.HasValue() && phone.GetDigits().Length >= 10)
                n++;
            if (ShowAddress && homephone.HasValue() && homephone.GetDigits().Length >= 10)
                n++;

            if (RequiredPhone() && n == 0)
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].phone), "cell or home phone required");

            if (homephone.HasValue() && homephone.GetDigits().Length > 20)
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].homephone), "homephone too long");

            if (email.HasValue())
                email = email.Trim();
            if (!email.HasValue() || !Util.ValidEmail(email))
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].email), "Please specify a valid email address.");
            if (isnewfamily)
            {
                if (!address.HasValue() && RequiredAddr())
                    ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].address), "address required.");
                if (RequiredZip() && address.HasValue())
                {
                    var addrok = city.HasValue() && state.HasValue();
                    if (zip.HasValue())
                        addrok = true;
                    if (!addrok)
                        ModelState.AddModelError("zip", "city/state required or zip required (or \"na\" in all)");

                    if (ModelState.IsValid && address.HasValue()
                        && (country == "United States" || !country.HasValue()))
                    {
                        var r = AddressVerify.LookupAddress(address, address2, city, state, zip);
                        if (r.Line1 != "error")
                        {
                            if (r.found == false)
                            {
                                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].zip), r.address + ", if your address will not validate, change the country to 'USA, Not Validated'");
                                return;
                            }
                            if (r.Line1 != address)
                            {
                                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].address), "address changed from '{0}'".Fmt(address));
                                address = r.Line1;
                            }
                            if (r.Line2 != (address2 ?? ""))
                            {
                                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].address2), "address2 changed from '{0}'".Fmt(address2));
                                address2 = r.Line2;
                            }
                            if (r.City != (city ?? ""))
                            {
                                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].city), "city changed from '{0}'".Fmt(city));
                                city = r.City;
                            }
                            if (r.State != (state ?? ""))
                            {
                                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].state), "state changed from '{0}'".Fmt(state));
                                state = r.State;
                            }
                            if (r.Zip != (zip ?? ""))
                            {
                                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].zip), "zip changed from '{0}'".Fmt(zip));
                                zip = r.Zip;
                            }
                        }
                    }
                }
            }

            if (!gender.HasValue && RequiredGender())
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].gender), "Please specify gender");
            if (!married.HasValue && RequiredMarital())
                ModelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].married), "Please specify marital status");

            if (MemberOnly())
                ModelState.AddModelError(foundname, "Sorry, must be a member of church");
            else if (org != null && setting.ValidateOrgIds.Count > 0)
                ModelState.AddModelError(foundname, "Must be member of specified organization");

            IsValidForNew = ModelState.IsValid;
        }
        public void ValidateModelForOther(ModelStateDictionary modelState)
        {
            var i = Index();
            if (Parent.SupportMissionTrip)
            {
                OtherOK = true;
                return;
            }
            foreach (var ask in setting.AskItems)
                switch (ask.Type)
                {
                    case "AskEmContact":
                        if (!emcontact.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].emcontact), "emergency contact required");
                        if (!emphone.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].emphone), "emergency phone # required");
                        break;
                    case "AskInsurance":
                        if (!insurance.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].insurance), "insurance carrier required");
                        if (!policy.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].policy), "insurance policy # required");
                        break;
                    case "AskDoctor":
                        if (!doctor.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].doctor), "Doctor's name required");
                        if (!docphone.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].docphone), "Doctor's phone # required");
                        break;
                    case "AskTylenolEtc":
                        if (!tylenol.HasValue)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].tylenol), "please indicate");
                        if (!advil.HasValue)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].advil), "please indicate");
                        if (!maalox.HasValue)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].maalox), "please indicate");
                        if (!robitussin.HasValue)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].robitussin), "please indicate");
                        break;
                    case "AskSize":
                        if (shirtsize == "0")
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].shirtsize), "please select a shirt size");
                        break;
                    case "AskCoaching":
                        if (!coaching.HasValue)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].coaching), "please indicate");
                        break;
                    case "AskSMS":
                        if (!sms.HasValue)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].sms), "please indicate");
                        break;
                    case "AskDropdown":
                        string desc;
                        string namedd = Parent.GetNameFor(mm => mm.List[i].option[ask.UniqueId]);
                        var sgi = ((AskDropdown)ask).SmallGroupChoice(option);
                        if (sgi == null || !sgi.SmallGroup.HasValue())
                            modelState.AddModelError(namedd, "please select an option");
                        else if (((AskDropdown)ask).IsSmallGroupFilled(GroupTags, option, out desc))
                            modelState.AddModelError(namedd, "limit reached for " + desc);
                        break;
                    case "AskParents":
                        if (!mname.HasValue() && !fname.HasValue())
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].fname), "please provide either mother or father name");
                        else
                        {
                            string mfirst, mlast;
                            Util.NameSplit(mname, out mfirst, out mlast);
                            if (mname.HasValue() && !mfirst.HasValue())
                                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].mname), "provide first and last names");
                            string ffirst, flast;
                            Util.NameSplit(fname, out ffirst, out flast);
                            if (fname.HasValue() && !ffirst.HasValue())
                                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].fname), "provide first and last names");
                        }
                        break;
                    case "AskTickets":
                        if ((ntickets ?? 0) == 0)
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].ntickets), "please enter a number of tickets");
                        break;
                    case "AskYesNoQuestions":
                        var ynname = Parent.GetNameFor(mm => mm.List[i]);
                        for (int n = 0; n < ((AskYesNoQuestions)ask).list.Count; n++)
                        {
                            var a = ((AskYesNoQuestions)ask).list[n];
                            if (YesNoQuestion == null || !YesNoQuestion.ContainsKey(a.SmallGroup))
                                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].YesNoQuestion[a.SmallGroup]), "please select yes or no");
                        }
                        break;
                    case "AskExtraQuestions":
                        var eq = (AskExtraQuestions)ask;
                        if (setting.AskVisible("AnswersNotRequired") == false)
                            for (int n = 0; n < eq.list.Count; n++)
                            {
                                var a = eq.list[n];
                                if (ExtraQuestion == null || !ExtraQuestion[eq.UniqueId].ContainsKey(a.Question) ||
                                    !ExtraQuestion[eq.UniqueId][a.Question].HasValue())
                                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].ExtraQuestion[eq.UniqueId][a.Question]), "please give some answer");
                            }
                        break;
                    case "AskCheckboxes":
                        string namecb = Parent.GetNameFor(mm => mm.List[i].Checkbox[ask.UniqueId]);
                        var cb = ((AskCheckboxes)ask);
                        if (cb.Max > 0 && cb.CheckboxItemsChosen(Checkbox).Count() > cb.Max)
                            modelState.AddModelError(namecb, "Max of {0} exceeded".Fmt(cb.Max));
                        else if (cb.Min > 0 && (Checkbox == null || Checkbox.Count < cb.Min))
                            modelState.AddModelError(namecb, "Min of {0} required".Fmt(cb.Min));
                        break;
                    case "AskGradeOptions":
                        if (gradeoption == "00")
                            modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].gradeoption), "please select a grade option");
                        break;
                }
            var totalAmount = TotalAmount();
            if (setting.Deposit > 0)
                if (!paydeposit.HasValue)
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].paydeposit), "please indicate");
                else
                {
                    var amountToPay = AmountToPay();
                    if (paydeposit == true && amountToPay > totalAmount)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[i].paydeposit),
                            "Cannot use deposit since total due is less");
                }
            if (OnlineGiving() && totalAmount <= 0)
                modelState.AddModelError("form", "Gift amount required");

            OtherOK = modelState.IsValid;
        }
    }
}