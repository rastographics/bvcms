using CmsData;
using CmsData.Registration;
using CmsData.View;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        private List<OnlineRegQA> qlist = null;
        public void RepopulateRegistration(OrganizationMember om)
        {
            var reg = person.RecRegs.SingleOrDefault();
            if (reg == null)
            {
                reg = new RecReg();
                person.RecRegs.Add(reg);
            }
            foreach (var ask in setting.AskItems)
            {
                switch (ask.Type)
                {
                    case "AskSize":
                        shirtsize = om.ShirtSize;
                        break;
                    case "AskChurch":
                        otherchurch = reg.ActiveInAnotherChurch ?? false;
                        memberus = reg.Member ?? false;
                        break;
                    case "AskAllergies":
                        medical = reg.MedicalDescription;
                        break;
                    case "AskParents":
                        mname = reg.Mname;
                        fname = reg.Fname;
                        break;
                    case "AskEmContact":
                        emcontact = reg.Emcontact;
                        emphone = reg.Emphone;
                        break;
                    case "AskTylenolEtc":
                        tylenol = reg.Tylenol;
                        advil = reg.Advil;
                        robitussin = reg.Robitussin;
                        maalox = reg.Maalox;
                        break;
                    case "AskDoctor":
                        docphone = reg.Docphone;
                        doctor = reg.Doctor;
                        break;
                    case "AskCoaching":
                        coaching = reg.Coaching;
                        break;
                    case "AskInsurance":
                        insurance = reg.Insurance;
                        policy = reg.Policy;
                        break;
                    case "AskTickets":
                        ntickets = om.Tickets;
                        break;
                    case "AskYesNoQuestions":
                        PopulateYesNoChoices(om, ask);
                        break;
                    case "AskCheckboxes":
                        PopulateCheckboxChoices(om, ask);
                        break;
                    case "AskExtraQuestions":
                        PopulateExtraAnswers(om, ask);
                        break;
                    case "AskText":
                        PopulateTextAnswers(om, ask);
                        break;
                    case "AskMenu":
                        break;
                    case "AskDropdown":
                        PopulateDropdownChoices(om, ask);
                        break;
                    case "AskGradeOptions":
                        PopulateGradeChoice(om);
                        break;
                }
            }
        }

        private void PopulateGradeChoice(OrganizationMember om)
        {
            gradeoption = person.Grade.ToString();
            if (!setting.TargetExtraValues)
            {
                gradeoption = om.Grade.ToString();
            }
        }

        private void PopulateDropdownChoices(OrganizationMember om, Ask ask)
        {
            if (option == null)
            {
                option = new List<string>();
            }

            if (setting.TargetExtraValues)
            {
                foreach (var dd in ((AskDropdown)ask).list)
                {
                    if (person.GetExtra(dd.SmallGroup) == "true")
                    {
                        option.Add(dd.SmallGroup);
                    }
                }
            }
            else
            {
                foreach (var dd in ((AskDropdown)ask).list)
                {
                    if (om.IsInGroup(dd.SmallGroup))
                    {
                        option.Add(dd.SmallGroup);
                    }
                }
            }
        }

        private void FetchQuestionsAnswersList(OrganizationMember om)
        {
            if (qlist == null)
            {
                qlist = (from qu in DbUtil.Db.ViewOnlineRegQAs
                         where qu.PeopleId == om.PeopleId
                         where qu.OrganizationId == om.OrganizationId
                         select qu).ToList();
            }
        }
        private void PopulateTextAnswers(OrganizationMember om, Ask ask)
        {
            FetchQuestionsAnswersList(om);

            if (Text == null)
            {
                Text = new List<Dictionary<string, string>>();
            }

            var tx = new Dictionary<string, string>();
            Text.Add(tx);
            foreach (var q in ((AskText)ask).list)
            {
                if (setting.TargetExtraValues)
                {
                    var v = person.GetExtra(q.Question);
                    if (v.HasValue())
                    {
                        tx[q.Question] = v;
                    }
                }
                else if (qlist != null)
                {
                    var v = qlist.SingleOrDefault(qq => qq.Question == q.Question && qq.Type == "text");
                    if (v != null)
                    {
                        tx[q.Question] = v.Answer;
                    }
                }
            }
        }

        private void PopulateExtraAnswers(OrganizationMember om, Ask ask)
        {
            FetchQuestionsAnswersList(om);

            if (ExtraQuestion == null)
            {
                ExtraQuestion = new List<Dictionary<string, string>>();
            }

            var eq = new Dictionary<string, string>();
            ExtraQuestion.Add(eq);

            foreach (var q in ((AskExtraQuestions)ask).list)
            {
                if (setting.TargetExtraValues)
                {
                    var v = person.GetExtra(q.Question);
                    if (v.HasValue())
                    {
                        eq[q.Question] = v;
                    }
                }
                else if (qlist != null)
                {
                    var v = qlist.SingleOrDefault(qq => qq.Question == q.Question && qq.Type == "question");
                    if (v != null)
                    {
                        eq[q.Question] = v.Answer;
                    }
                }
            }
        }

        private void PopulateCheckboxChoices(OrganizationMember om, Ask ask)
        {
            if (setting.TargetExtraValues)
            {
                foreach (var ck in ((AskCheckboxes)ask).list)
                {
                    if (person.GetExtra(ck.SmallGroup).ToBool())
                    {
                        Checkbox.Add(ck.SmallGroup);
                    }
                }
            }
            else
            {
                foreach (var ck in ((AskCheckboxes)ask).list)
                {
                    if (om.IsInGroup(ck.SmallGroup))
                    {
                        Checkbox.Add(ck.SmallGroup);
                    }
                }
            }
        }

        private void PopulateYesNoChoices(OrganizationMember om, Ask ask)
        {
            if (setting.TargetExtraValues == false)
            {
                foreach (var yn in ((AskYesNoQuestions)ask).list)
                {
                    {
                        if (om.IsInGroup("Yes:" + yn.SmallGroup))
                        {
                            YesNoQuestion[yn.SmallGroup] = true;
                        }

                        if (om.IsInGroup("No:" + yn.SmallGroup))
                        {
                            YesNoQuestion[yn.SmallGroup] = false;
                        }
                    }
                }
            }
            else
            {
                foreach (var yn in ((AskYesNoQuestions)ask).list)
                {
                    if (person.GetExtra(yn.SmallGroup) == "Yes")
                    {
                        YesNoQuestion[yn.SmallGroup] = true;
                    }

                    if (person.GetExtra(yn.SmallGroup) == "No")
                    {
                        YesNoQuestion[yn.SmallGroup] = false;
                    }
                }
            }
        }
    }
}
