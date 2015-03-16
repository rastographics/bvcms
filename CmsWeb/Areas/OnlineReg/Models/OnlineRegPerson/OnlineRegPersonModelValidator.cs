using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsWeb.Models
{
    internal static class OnlineRegPersonModelValidator
    {
        public static void ValidateModelForFind(OnlineRegPersonModel model, ModelStateDictionary modelState, OnlineRegModel m, int i, bool selectFromFamily = false)
        {
            model.IsValidForContinue = true; // true till proven false
            if (model.UserSelectsOrganization())
                if ((model.classid ?? 0) == 0)
                {
                    var nameclassid = model.Parent.GetNameFor(mm => mm.List[i].classid);
                    const string pleaseChooseAGroupEvent = "please choose a group/event";
                    if (model.IsFamily)
                        modelState.AddModelError(nameclassid, pleaseChooseAGroupEvent);
                    else
                        modelState.AddModelError(nameclassid, pleaseChooseAGroupEvent);
                    model.IsValidForExisting = modelState.IsValid;
                    return;
                }

            var dobname = model.Parent.GetNameFor(mm => mm.List[i].DateOfBirth);
            var foundname = model.Parent.GetNameFor(mm => mm.List[i].Found);

            if (model.IsFamily)
                foundname = "fammember-" + model.PeopleId;

            if (!model.PeopleId.HasValue)
                ValidBasic(model, modelState, i);

            if (model.ComputesOrganizationByAge() && !model.birthday.HasValue)
                modelState.AddModelError(dobname, "birthday required");

            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();

            if (model.orgid == Util.CreateAccountCode && model.age < minage)
                modelState.AddModelError(dobname, "must be {0} to create account".Fmt(minage));

            if (!model.IsFamily && (!model.EmailAddress.HasValue() || !Util.ValidEmail(model.EmailAddress)))
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].person.EmailAddress), "Please specify a valid email address.");

            if (modelState.IsValid)
            {
                model.Found = model.person != null;
                if (model.count == 1)
                {
                    model.AddressLineOne = model.person.PrimaryAddress;
                    model.City = model.person.PrimaryCity;
                    model.State = model.person.PrimaryState;
                    model.ZipCode = model.person.PrimaryZip;
                    model.gender = model.person.GenderId;
                    model.married = model.person.MaritalStatusId == 2 ? 2 : 1;

                    if (!model.person.EmailAddress.HasValue() &&
                        (model.ManageSubscriptions()
                            || model.orgid == Util.CreateAccountCode
                            || model.OnlineGiving()
                            || model.ManageGiving()
                            || model.OnlinePledge()
                        ))
                    {
                        modelState.AddModelError(foundname, "No Email Address on record");
                        model.NotFoundText = @"<strong>No Email Address on Record</strong><br/>
We have found your record but we have no email address for you.<br/>
This means that we cannot proceed until we have that to protect your data.<br/>
Please call the church to resolve this before we can complete your information.";
                        model.IsValidForContinue = false;
                    }
                    else if (model.ComputesOrganizationByAge() && model.org == null)
                    {
                        var msg = model.NoAppropriateOrgError ?? "Sorry, no approprate org";
                        if (selectFromFamily)
                            modelState.AddModelError("age-" + model.person.PeopleId, msg);
                        else
                            modelState.AddModelError(dobname, msg);
                        model.IsValidForContinue = false;
                    }
                    else if (model.MemberOnly() && model.person.MemberStatusId != MemberStatusCode.Member)
                    {
                        modelState.AddModelError(foundname, "Sorry, must be a member of church");
                        model.NotFoundText = @"<strong>Sorry, must be a member of this church</strong>";
                        model.IsValidForContinue = false;
                    }
                    else if (model.org != null)
                    {
                        var om = model.org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == model.PeopleId);
                        if (model.org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
                        {
#if DEBUG
#else
                            if (model.person.Users.Count() > 0)
                            {
                                modelState.AddModelError(foundname, "You already have an account");
                                model.IsValidForContinue = false;
                            }
                            if (!Util.ValidEmail(model.person.EmailAddress))
                            {
                                modelState.AddModelError(foundname, "You must have a valid email address on record");
                                model.NotFoundText = @"We have found your record but we do not have a valid email for you.<br/>
For your protection, we cannot continue to create an account.<br />
We can't use the one you enter online here since we can't be sure this is you.<br />
Please call the church to resolve this before we can complete your account.<br />";
                                model.IsValidForContinue = false;
                            }
#endif
                        }
                        else if (om != null && model.org.IsMissionTrip == true
                            && !model.Parent.SupportMissionTrip
                            && om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Sender")
                            && !om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer"))
                        {
                            model.IsValidForContinue = true;
                        }
                        else if (om != null && model.setting.AllowReRegister == false
                            && om.Organization.RegistrationTypeId != RegistrationTypeCode.ChooseVolunteerTimes
                            && !model.Parent.SupportMissionTrip)
                        {
                            modelState.AddModelError(foundname, "This person is already registered");
                            model.NotFoundText = @"<strong>This person is already registered</strong>";
                            model.CancelText = "Register a different person";
                            model.IsValidForContinue = false;
                        }
                        else if (model.setting.ValidateOrgIds.Count > 0 && !model.Parent.SupportMissionTrip)
                        {
                            var reqmemberids = model.setting.ValidateOrgIds.Where(ii => ii > 0).ToList();
                            if (reqmemberids.Count > 0)
                                if (!model.person.OrganizationMembers.Any(mm => reqmemberids.Contains(mm.OrganizationId)))
                                {
                                    modelState.AddModelError(foundname, "Must be member of specified organization");
                                    model.NotFoundText = @"<strong>Must be a member of specified organization to register</strong>";
                                    model.IsValidForContinue = false;
                                }
                            var reqnomemberids = model.setting.ValidateOrgIds.Where(ii => ii < 0).ToList();
                            if (reqnomemberids.Count > 0)
                                if (model.person.OrganizationMembers.Any(mm => reqnomemberids.Contains(-mm.OrganizationId)))
                                {
                                    modelState.AddModelError(foundname, "Must not be a member of specified organization");
                                    model.NotFoundText = @"<strong>Must not be a member of specified organization to register</strong>";
                                    model.IsValidForContinue = false;
                                }
                        }
                    }
                    if (m.List.Count(ii => ii.PeopleId == model.PeopleId) > 1)
                    {
                        modelState.AddModelError(foundname, "Person already in Pending Registration");
                        model.CancelText = "Register a different person";
                        model.NotFoundText = @"<strong>Person already in Pending Registration</strong>";
                        model.IsValidForContinue = false;
                    }
                }
                else if (model.count > 1)
                {
                    modelState.AddModelError(foundname, "More than one match, sorry");
                    model.NotFoundText = @"<strong>MORE THAN ONE MATCH</strong><br />
We have found more than one record that matches your information
This is an unexpected error and we don't know which one is you.
Please call the church to resolve this before we can complete your registration.";
                    model.IsValidForContinue = false;
                }
                else if (model.count == 0)
                {
                    modelState.AddModelError(foundname, "record not found");
                    model.NotFoundText = @" <strong>RECORD NOT FOUND</strong><br />
                            We were not able to find you in our database. Please check the information you entered.
                            <ul>
                                <li>If everything looks good, select ""New Profile""</li>
                                <li>If you make a correction, select ""Search Again""</li>
                            </ul>";
                }
            }
            ValidateBirthdayRange(model, modelState, i);
            model.IsValidForExisting = modelState.IsValid;
        }

        public static void ValidateModelForNew(OnlineRegPersonModel model, ModelStateDictionary modelState, int i)
        {
            var dobname = model.Parent.GetNameFor(mm => mm.List[i].DateOfBirth);
            var foundname = model.Parent.GetNameFor(mm => mm.List[i].Found);

            ValidBasic(model, modelState, i);

            DateTime dt;
            if (model.RequiredDOB() && model.DateOfBirth.HasValue() && !Util.BirthDateValid(model.bmon, model.bday, model.byear, out dt))
                modelState.AddModelError(dobname, "birthday invalid");
            else if (!model.birthday.HasValue && model.RequiredDOB())
                modelState.AddModelError(dobname, "birthday required");
            if (model.birthday.HasValue && model.NoReqBirthYear() == false && model.birthday.Value.Year == Util.SignalNoYear)
            {
                modelState.AddModelError(dobname, "BirthYear is required");
                model.IsValidForNew = false;
                return;
            }

            var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
            if (model.orgid == Util.CreateAccountCode && model.age < minage)
                modelState.AddModelError(dobname, "must be {0} to create account".Fmt(minage));

            if (model.ComputesOrganizationByAge() && model.GetAppropriateOrg() == null)
                modelState.AddModelError(dobname, model.NoAppropriateOrgError);

            ValidateBirthdayRange(model, modelState, i);
            var n = 0;
            if (model.Phone.HasValue() && model.Phone.GetDigits().Length >= 10)
                n++;
            if (model.ShowAddress && model.HomePhone.HasValue() && model.HomePhone.GetDigits().Length >= 10)
                n++;

            if (model.RequiredPhone() && n == 0)
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].Phone), "cell or home phone required");

            if (model.HomePhone.HasValue() && model.HomePhone.GetDigits().Length > 20)
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].HomePhone), "homephone too long");

            if (model.EmailAddress.HasValue())
                model.EmailAddress = model.EmailAddress.Trim();

            if (!model.EmailAddress.HasValue() || !Util.ValidEmail(model.EmailAddress))
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].person.EmailAddress), "Please specify a valid email address.");

            var isnewfamily = model.whatfamily == 3;
            if (isnewfamily)
            {
                if (!model.AddressLineOne.HasValue() && model.RequiredAddr())
                    modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].AddressLineOne), "address required.");
                if (model.RequiredZip() && model.AddressLineOne.HasValue())
                {
                    var addrok = model.City.HasValue() && model.State.HasValue();
                    if (model.ZipCode.HasValue())
                        addrok = true;

                    if (!addrok)
                        modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].ZipCode), "zip required (or \"na\")");

                    if (modelState.IsValid && model.AddressLineOne.HasValue()
                        && (model.Country == "United States" || !model.Country.HasValue()))
                    {
                        var r = AddressVerify.LookupAddress(model.AddressLineOne, model.AddressLineTwo, model.City, model.State, model.ZipCode);
                        if (r.Line1 != "error")
                        {
                            if (r.found == false)
                            {
                                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].ZipCode), r.address + ", to skip address check, Change the country to USA, Not Validated");
                                model.ShowCountry = true;
                                return;
                            }
                            if (r.Line1 != model.AddressLineOne)
                                model.AddressLineOne = r.Line1;
                            if (r.Line2 != (model.AddressLineTwo ?? ""))
                                model.AddressLineTwo = r.Line2;
                            if (r.City != (model.City ?? ""))
                                model.City = r.City;
                            if (r.State != (model.State ?? ""))
                                model.State = r.State;
                            if (r.Zip != (model.ZipCode ?? ""))
                                model.ZipCode = r.Zip;
                        }
                    }
                }
            }

            if (!model.gender.HasValue && model.RequiredGender())
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].gender), "Please specify gender");
            if (!model.married.HasValue && model.RequiredMarital())
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].married), "Please specify marital status");

            if (model.MemberOnly())
            {
                modelState.AddModelError(foundname, "Sorry, must be a member of church");
            }
            else if (model.org != null && model.setting.ValidateOrgIds.Count > 0 && !model.Parent.SupportMissionTrip)
            {
                modelState.AddModelError(foundname, "Must be member of specified organization");
            }

            model.IsValidForNew = modelState.IsValid;
            model.IsValidForContinue = modelState.IsValid;
        }

        public static void ValidateModelForOther(OnlineRegPersonModel model, ModelStateDictionary modelState, int i)
        {
            if (model.Parent.SupportMissionTrip)
            {
                model.OtherOK = modelState.IsValid;
                return;
            }
            if (model.RecordFamilyAttendance())
            {
                model.OtherOK = true;
                return;
            }
            foreach (var ask in model.setting.AskItems)
                switch (ask.Type)
                {
                    case "AskEmContact":
                        if (!model.emcontact.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].emcontact), "emergency contact required");
                        if (!model.emphone.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].emphone), "emergency phone # required");
                        break;

                    case "AskInsurance":
                        if (!model.insurance.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].insurance), "insurance carrier required");
                        if (!model.policy.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].policy), "insurance policy # required");
                        break;

                    case "AskDoctor":
                        if (!model.doctor.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].doctor), "Doctor's name required");
                        if (!model.docphone.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].docphone), "Doctor's phone # required");
                        break;

                    case "AskTylenolEtc":
                        if (!model.tylenol.HasValue)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].tylenol), "please indicate");
                        if (!model.advil.HasValue)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].advil), "please indicate");
                        if (!model.maalox.HasValue)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].maalox), "please indicate");
                        if (!model.robitussin.HasValue)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].robitussin), "please indicate");
                        break;

                    case "AskSize":
                        if (model.shirtsize == "0")
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].shirtsize), "please select a shirt size");
                        break;

                    case "AskCoaching":
                        if (!model.coaching.HasValue)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].coaching), "please indicate");
                        break;

                    case "AskSMS":
                        if (!model.sms.HasValue)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].sms), "please indicate");
                        break;

                    case "AskDropdown":
                        string desc;
                        var namedd = model.Parent.GetNameFor(mm => mm.List[i].option[ask.UniqueId]);
                        var sgi = ((AskDropdown)ask).SmallGroupChoice(model.option);
                        if (sgi == null || !sgi.SmallGroup.HasValue())
                            modelState.AddModelError(namedd, "please select an option");
                        else if (((AskDropdown)ask).IsSmallGroupFilled(model.GroupTags, model.option, out desc))
                            modelState.AddModelError(namedd, "limit reached for " + desc);
                        break;

                    case "Askperson.Parents":
                        if (!model.mname.HasValue() && !model.fname.HasValue())
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].fname), "please provide either mother or father name");
                        else
                        {
                            string mfirst, mlast;
                            Util.NameSplit(model.mname, out mfirst, out mlast);
                            if (model.mname.HasValue() && !mfirst.HasValue())
                                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].mname), "provide first and last names");
                            string ffirst, flast;
                            Util.NameSplit(model.fname, out ffirst, out flast);
                            if (model.fname.HasValue() && !ffirst.HasValue())
                                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].fname), "provide first and last names");
                        }
                        break;

                    case "AskTickets":
                        if ((model.ntickets ?? 0) == 0)
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].ntickets), "please enter a number of tickets");
                        break;

                    case "AskYesNoQuestions":
                        for (var n = 0; n < ((AskYesNoQuestions)ask).list.Count; n++)
                        {
                            var a = ((AskYesNoQuestions)ask).list[n];
                            if (model.YesNoQuestion == null || !model.YesNoQuestion.ContainsKey(a.SmallGroup))
                                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].YesNoQuestion[a.SmallGroup]), "please select yes or no");
                        }
                        break;

                    case "AskExtraQuestions":
                        var eq = (AskExtraQuestions)ask;
                        if (model.setting.AskVisible("AnswersNotRequired") == false)
                            for (var n = 0; n < eq.list.Count; n++)
                            {
                                var a = eq.list[n];
                                if (model.ExtraQuestion == null || !model.ExtraQuestion[eq.UniqueId].ContainsKey(a.Question) ||
                                    !model.ExtraQuestion[eq.UniqueId][a.Question].HasValue())
                                    modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].ExtraQuestion[eq.UniqueId][a.Question]), "please give some answer");
                            }
                        break;

                    case "AskText":
                        var tx = (AskText)ask;
                        if (model.setting.AskVisible("AnswersNotRequired") == false)
                            for (var n = 0; n < tx.list.Count; n++)
                            {
                                var a = tx.list[n];
                                if (model.Text == null || !model.Text[tx.UniqueId].ContainsKey(a.Question) ||
                                    !model.Text[tx.UniqueId][a.Question].HasValue())
                                    modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].Text[tx.UniqueId][a.Question]), "please give some answer");
                            }
                        break;

                    case "AskCheckboxes":
                        var namecb = model.Parent.GetNameFor(mm => mm.List[i].Checkbox[ask.UniqueId]);
                        var cb = ((AskCheckboxes)ask);
                        var cbcount = cb.CheckboxItemsChosen(model.Checkbox).Count();
                        if (cb.Max > 0 && cbcount > cb.Max)
                            modelState.AddModelError(namecb, "Max of {0} exceeded".Fmt(cb.Max));
                        else if (cb.Min > 0 && (model.Checkbox == null || cbcount < cb.Min))
                            modelState.AddModelError(namecb, "Min of {0} required".Fmt(cb.Min));
                        break;

                    case "AskGradeOptions":
                        if (model.gradeoption == "00")
                            modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].gradeoption), "please select a grade option");
                        break;
                }
            var totalAmount = model.TotalAmount();
            if (model.setting.Deposit > 0)
                if (!model.paydeposit.HasValue)
                    modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].paydeposit), "please indicate");
                else
                {
                    var amountToPay = model.AmountToPay();
                    if (model.paydeposit == true && amountToPay > totalAmount)
                        modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].paydeposit),
                            "Cannot use deposit since total due is less");
                }
            if (model.OnlineGiving() && totalAmount <= 0)
                modelState.AddModelError("form", "Gift amount required");

            model.OtherOK = modelState.IsValid;
        }

        private static void ValidateBirthdayRange(OnlineRegPersonModel model, ModelStateDictionary modelState, int i)
        {
            if (model.org == null) return;
            if (!model.birthday.HasValue && (model.org.BirthDayStart.HasValue || model.org.BirthDayEnd.HasValue))
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].DateOfBirth), "birthday required");
            else if (model.birthday.HasValue)
            {
                if ((model.org.BirthDayStart.HasValue && model.birthday < model.org.BirthDayStart)
                    || (model.org.BirthDayEnd.HasValue && model.birthday > model.org.BirthDayEnd))
                    modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].DateOfBirth), "birthday outside age allowed range");
            }
        }

        private static void ValidBasic(OnlineRegPersonModel model, ModelStateDictionary modelState, int i)
        {
            if (!model.FirstName.HasValue())
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].FirstName), "first name required");

            if (!model.LastName.HasValue())
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].LastName), "last name required");

            var mindate = DateTime.Parse("1/1/1753");
            var n = 0;

            if (model.birthday.HasValue && model.birthday < mindate)
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].DateOfBirth), "invalid date");

            if (model.birthday.HasValue && model.birthday > mindate)
                n++;

            if (Util.ValidEmail(model.EmailAddress))
                n++;

            var d = model.Phone.GetDigits().Length;
            if (model.Phone.HasValue() && d >= 10)
                n++;

            if (d > 20)
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].Phone), "too many digits in phone");

            if (n == 0)
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].DateOfBirth), "we require one of valid birthdate, email or phone to find your record");

            if (!Util.ValidEmail(model.EmailAddress))
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].EmailAddress), "valid email required");

            if (model.Phone.HasValue() && d < 10)
                modelState.AddModelError(model.Parent.GetNameFor(mm => mm.List[i].Phone), "10+ digits required");
        }
    }
}
