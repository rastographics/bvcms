using System.Linq;
using System.Web.Mvc;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public void ValidateModelQuestions(ModelStateDictionary modelstate, int i)
        {
            modelState = modelstate;
            Index = i;
            if (Parent.SupportMissionTrip)
            {
                if (MissionTripGoerId == 0 && ((MissionTripSupportGeneral ?? 0) == 0))
                {
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].MissionTripGoerId), "Please select a participant");                    
                }
                if ((MissionTripSupportGoer ?? 0) == 0 && ((MissionTripSupportGeneral ?? 0) == 0))
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].MissionTripSupportGoer), "Please enter your gift amount");
                if ((MissionTripSupportGoer ?? 0) != 0 && MissionTripGoerId == 0)
                {
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].MissionTripGoerId), "Please select a participant");
                }
                QuestionsOK = modelState.IsValid;
                return;
            }
            if (RecordFamilyAttendance())
            {
                QuestionsOK = true;
                return;
            }
            foreach (var ask in setting.AskItems)
                switch (ask.Type)
                {
                    case "AskCoaching":
                        ValidateAskCoaching();
                        break;
                    case "AskCheckboxes":
                        ValidateAskCheckboxes(ask);
                        break;
                    case "AskDoctor":
                        ValidateAskDoctor();
                        break;
                    case "AskDropdown":
                        ValidateAskDropdown(ask);
                        break;
                    case "AskEmContact":
                        ValidateAskEmContact();
                        break;
                    case "AskExtraQuestions":
                        ValidateAskExtraQuestions(ask);
                        break;
                    case "AskGradeOptions":
                        ValidateGradeOptions();
                        break;
                    case "AskInsurance":
                        ValidateAskInsurance();
                        break;
                    case "AskParents":
                        ValidateAskParents();
                        break;
                    case "AskSMS":
                        ValidateAskSMS();
                        break;
                    case "AskSize":
                        ValidateAskSize();
                        break;
                    case "AskText":
                        ValidateAskText(ask);
                        break;
                    case "AskTylenolEtc":
                        ValidateAskTylenolEtc();
                        break;
                    case "AskTickets":
                        ValidateAskTickets();
                        break;
                    case "AskYesNoQuestions":
                        ValidateAskYesNoQuestions(ask);
                        break;
                }
            ValidatePaymentOption();
            QuestionsOK = modelState.IsValid;
            if (!QuestionsOK)
                Log("QuestionsRetry");
        }

        private void ValidateAskCoaching()
        {
            if (!coaching.HasValue)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].coaching), "please indicate");
        }

        private void ValidateAskCheckboxes(Ask ask)
        {
            var namecb = Parent.GetNameFor(mm => mm.List[Index].Checkbox[ask.UniqueId]);
            var cb = ((AskCheckboxes)ask);
            var cbcount = cb.CheckboxItemsChosen(Checkbox).Count();
            if (cb.Maximum > 0 && cbcount > cb.Maximum)
                modelState.AddModelError(namecb, $"Max of {cb.Maximum} exceeded");
            else if (cb.Minimum > 0 && (Checkbox == null || cbcount < cb.Minimum))
                modelState.AddModelError(namecb, $"Min of {cb.Minimum} required");
        }

        private void ValidateAskDoctor()
        {
            if (!doctor.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].doctor), "Doctor's name required");
            if (!docphone.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].docphone), "Doctor's phone # required");
        }

        private void ValidateAskDropdown(Ask ask)
        {
            string desc;
            var namedd = Parent.GetNameFor(mm => mm.List[Index].option[ask.UniqueId]);
            var sgi = ((AskDropdown)ask).SmallGroupChoice(option);
            if (sgi == null || !sgi.SmallGroup.HasValue())
                modelState.AddModelError(namedd, "please select an option");
            else if (((AskDropdown)ask).IsSmallGroupFilled(GroupTags, option, out desc))
                modelState.AddModelError(namedd, "limit reached for " + desc);
        }

        private void ValidateAskEmContact()
        {
            if (!emcontact.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].emcontact), "emergency contact required");
            if (!emphone.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].emphone), "emergency phone # required");
        }

        private void ValidateAskExtraQuestions(Ask ask)
        {
            var eq = (AskExtraQuestions)ask;
            if (setting.AskVisible("AnswersNotRequired") == false)
                for (var n = 0; n < eq.list.Count; n++)
                {
                    var a = eq.list[n];
                    if (ExtraQuestion == null || !ExtraQuestion[eq.UniqueId].ContainsKey(a.Question) ||
                        !ExtraQuestion[eq.UniqueId][a.Question].HasValue())
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ExtraQuestion[eq.UniqueId][a.Question]),
                            "please give some answer");
                }
        }

        private void ValidateGradeOptions()
        {
            if (gradeoption == "00")
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].gradeoption), "please select a grade option");
        }

        private void ValidateAskInsurance()
        {
            if (!insurance.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].insurance), "insurance carrier required");
            if (!policy.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].policy), "insurance policy # required");
        }

        private void ValidateAskParents()
        {
            if (!mname.HasValue() && !fname.HasValue())
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].fname),
                    "please provide either mother or father name");
            else
            {
                string mfirst, mlast;
                Util.NameSplit(mname, out mfirst, out mlast);
                if (mname.HasValue() && !mfirst.HasValue())
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].mname), "provide first and last names");
                string ffirst, flast;
                Util.NameSplit(fname, out ffirst, out flast);
                if (fname.HasValue() && !ffirst.HasValue())
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].fname), "provide first and last names");
            }
        }

        private void ValidateAskSize()
        {
            if (shirtsize == "0")
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].shirtsize), "please select a shirt size");
        }

        private void ValidateAskSMS()
        {
            if (!sms.HasValue)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].sms), "please indicate");
        }

        private void ValidateAskText(Ask ask)
        {
            var tx = (AskText)ask;
            if (setting.AskVisible("AnswersNotRequired") == false)
                for (var n = 0; n < tx.list.Count; n++)
                {
                    var a = tx.list[n];
                    if (Text == null || !Text[tx.UniqueId].ContainsKey(a.Question) ||
                        !Text[tx.UniqueId][a.Question].HasValue())
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].Text[tx.UniqueId][a.Question]),
                            "please give some answer");
                }
        }

        private void ValidateAskTickets()
        {
            if ((ntickets ?? 0) == 0)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].ntickets), "please enter a number of tickets");
        }

        private void ValidateAskTylenolEtc()
        {
            if (!tylenol.HasValue)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].tylenol), "please indicate");
            if (!advil.HasValue)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].advil), "please indicate");
            if (!maalox.HasValue)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].maalox), "please indicate");
            if (!robitussin.HasValue)
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].robitussin), "please indicate");
        }

        private void ValidateAskYesNoQuestions(Ask ask)
        {
            for (var n = 0; n < ((AskYesNoQuestions)ask).list.Count; n++)
            {
                var a = ((AskYesNoQuestions)ask).list[n];
                if (YesNoQuestion == null || !YesNoQuestion.ContainsKey(a.SmallGroup))
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].YesNoQuestion[a.SmallGroup]),
                        "please select yes or no");
            }
        }

        private void ValidatePaymentOption()
        {
            var totalAmount = TotalAmount();
            if (setting.Deposit > 0 && totalAmount > 0)
                if (!paydeposit.HasValue)
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].paydeposit), "please indicate");
                else
                {
                    var amountToPay = AmountToPay();
                    if (paydeposit == true && amountToPay > totalAmount)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[Index].paydeposit),
                            "Cannot use deposit since total due is less");
                }
            if (OnlineGiving() && totalAmount <= 0)
                modelState.AddModelError("form", "Gift amount required");
        }
    }
}
