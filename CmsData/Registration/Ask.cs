using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;

namespace CmsData.Registration
{
    [Serializable]
    public class Ask : IXmlSerializable
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int UniqueId { get; set; }

        public Ask(string type)
        {
            Type = type;
        }

        public static Ask ParseAsk(Parser parser)
        {
            var r = new Ask(parser.curr.kw.ToString());
            parser.GetBool();
            return r;
        }

        public virtual void Output(StringBuilder sb)
        {
            Settings.AddValueCk(0, sb, Type, true);
        }

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

        public virtual XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            w.Start(Type);
            w.End();
        }
    }
}
