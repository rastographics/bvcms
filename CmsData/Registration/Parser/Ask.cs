using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public Ask ParseAsk()
        {
            var r = new Ask(curr.kw.ToString());
            GetBool();
            return r;
        }

        public void OutputAsk(StringBuilder sb, Ask ask)
        {
            switch (ask.Type)
            {
                case "AskSuggestedFee":
                    Output(sb, ask as AskSuggestedFee);
                    break;
                case "AskCheckboxes":
                    Output(sb, ask as AskCheckboxes);
                    break;
                case "AskDropdown":
                    Output(sb, ask as AskDropdown);
                    break;
                case "AskExtraQuestions":
                    Output(sb, ask as AskExtraQuestions);
                    break;
                case "AskGradeOptions":
                    Output(sb, ask as AskGradeOptions);
                    break;
                case "AskHeader":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskInstruction":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskMenu":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskRequest":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskSize":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskText":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskTickets":
                    Output(sb, ask as AskHeader);
                    break;
                case "AskYesNoQuestions":
                    Output(sb, ask as AskHeader);
                    break;
                default:
                    AddValueCk(0, sb, ask.Type, true);
                    break;
            }
        }
    }
}
