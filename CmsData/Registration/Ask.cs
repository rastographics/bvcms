using System.Collections.Generic;
using System.Xml.Linq;
using CmsData.API;

namespace CmsData.Registration
{
    public class Ask
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int UniqueId { get; set; }

        public Ask(string type)
        {
            Type = type;
        }
        public virtual void WriteXml(APIWriter w)
        {
            w.Start(Type);
            w.End();
        }
        public static Ask ReadXml(XElement ele)
        {
            var ask = ele.Name.ToString();
            switch (ask)
            {
                case "AskCheckboxes":
                    return AskCheckboxes.ReadXml(ele);
                case "AskDropdown":
                    return AskDropdown.ReadXml(ele);
                case "AskExtraQuestions":
                    return AskExtraQuestions.ReadXml(ele);
                case "AskGradeOptions":
                    return AskGradeOptions.ReadXml(ele);
                case "AskHeader":
                    return AskHeader.ReadXml(ele);
                case "AskInstruction":
                    return AskInstruction.ReadXml(ele);
                case "AskMenu":
                    return AskMenu.ReadXml(ele);
                case "AskRequest":
                    return AskRequest.ReadXml(ele);
                case "AskSize":
                    return AskSize.ReadXml(ele);
                case "AskSuggestedFee":
                    return AskSuggestedFee.ReadXml(ele);
                case "AskText":
                    return AskText.ReadXml(ele);
                case "AskTickets":
                    return AskTickets.ReadXml(ele);
                case "AskYesNoQuestions":
                    return AskYesNoQuestions.ReadXml(ele);
                default:
                    return new Ask(ask);
            }
        }

        public bool IsAskAllergies => Type == "AskAllergies";
        public bool IsAskCheckboxes => Type == "AskCheckboxes";
        public bool IsAskChurch => Type == "AskChurch";
        public bool IsAskCoaching => Type == "AskCoaching";
        public bool IsAskDoctor => Type == "AskDoctor";
        public bool IsAskDropdown { get { return Type == "AskDropdown";  } }
        public bool IsAskEmContact => Type == "AskEmContact";
        public bool IsAskExtraQuestions => Type == "AskExtraQuestions";
        public bool IsAskGradeOptions => Type == "AskGradeOptions";
        public bool IsAskHeader => Type == "AskHeader";
        public bool IsAskInstruction => Type == "AskInstruction";
        public bool IsAskInsurance => Type == "AskInsurance";
        public bool IsAskMenu => Type == "AskMenu";
        public bool IsAskParents => Type == "AskParents";
        public bool IsAskRequest => Type == "AskRequest";
        public bool IsAskSize => Type == "AskSize";
        public bool IsAskSms => Type == "AskSMS";
        public bool IsAskSuggestedFee => Type == "AskSuggestedFee";
        public bool IsAskText => Type == "AskText";
        public bool IsAskTickets => Type == "AskTickets";
        public bool IsAskTylenolEtc => Type == "AskTylenolEtc";
        public bool IsAskYesNoQuestions => Type == "AskYesNoQuestions";

        public virtual List<string> SmallGroups()
        {
            return new List<string>();
        }

        public virtual string Help { get { return HelpDictionary[Type]; } }

        private static readonly Dictionary<string, string> HelpDictionary = new Dictionary<string, string>
        {
            {"AskAllergies", @"Displays a multi-line text box to enter any medical information."},
            {"AnswersNotRequired", @"Textbox like questions do not require answers."},
            {"AskCoaching", @"Asks whether the parent is interested in coaching or not."},
            {"AskDoctor", @"Asks for Doctor's name and Phone Number."},
            {"AskEmContact", @"Asks for the name and phone number for an emergency contact."},
            {"AskInsurance", @"
Displays two questions: Insurance name and policy number.
Good for camp or ball teams where you might a participant might get hurt.
"},
            {"AskParents", @"Displays two text boxes asking for mother and/or father's names."},
            {"AskSMS", @"Displays yes/no radio buttons for opting in to SMS."},
            {"AskTylenolEtc", @"Asks whether it is ok to give a child Tylenol, Advil, Robitussin, or Maalox."},
            {"AskChurch", @"
Ask whether they are a member here, or active in another church.
Good for indicating whether they are a prospect or not.
"},
        };

    }
}
