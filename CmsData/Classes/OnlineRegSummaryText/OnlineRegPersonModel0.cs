using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsData.OnlineRegSummaryText
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class OnlineRegPersonModel0
    {
        public int? MissionTripGoerId { get; set; }
        public Settings setting { get; set; }
        public int PeopleId { get; set; }
        public int orgid { get; set; }
        public int? tranid { get; set; }
        public string gradeoption { get; set; }
        public Person person { get; set; }
        public string AgeGroup()
        {
            foreach (var i in setting.AgeGroups)
                if (person.Age >= i.StartAge && person.Age <= i.EndAge)
                    return i.SmallGroup;
            return string.Empty;
        }
        public decimal? MissionTripSupportGeneral { get; set; }
        public bool SupportMissionTrip => MissionTripGoerId > 0 || MissionTripSupportGeneral > 0;
        public Dictionary<int, decimal?> FundItem { get; set; }
        public List<Dictionary<string, string>> ExtraQuestion { get; set; }
        public List<Dictionary<string, string>> Text { get; set; }
        public Dictionary<string, bool?> YesNoQuestion { get; set; }
        public List<string> option { get; set; }
        public List<string> Checkbox { get; set; }
        public List<Dictionary<string, int?>> MenuItem { get; set; }
        public Dictionary<string, string> GradeOptions(Ask ask)
        {
            var d = ((AskGradeOptions)ask).list.ToDictionary(k => k.Code.ToString(), v => v.Description);
            d.Add("00", "(please select)");
            return d;
        }

        public bool? advil { get; set; }
        public bool? tylenol { get; set; }
        public bool? maalox { get; set; }
        public bool? robitussin { get; set; }

        public void ReadXml(string s)
        {
            var x = XDocument.Parse(s);
            if (x.Root == null) return;

            eqset = 0;
            txset = 0;
            menuset = 0;

            foreach (var e in x.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "ExtraQuestion":
                        ReadExtraQuestion(e);
                        break;
                    case "Text":
                        ReadText(e);
                        break;
                    case "YesNoQuestion":
                        ReadYesNoChoices(e);
                        break;
                    case "option":
                        ReadDropdownOption(e);
                        break;
                    case "Checkbox":
                        ReadCheckboxChoice(e);
                        break;
                    case "MenuItem":
                        ReadMenuItemChoice(e);
                        break;
                    case "MissionTripGoerId":
                        MissionTripGoerId = e.Value.ToInt();
                        break;
                    default:
                        var tf = Util.SetPropertyFromText(this, name, e.Value);
                        if (tf)
                            Debug.WriteLine("here");
                            
                        break;
                }
            }
        }

        private int eqset;
        private int menuset;
        private int txset;

        private string GetAttr(XElement e, string name)
        {
            var a = e.Attribute(name);
            return a?.Value ?? string.Empty;
        }

        private void ReadDropdownOption(XElement e)
        {
            if (option == null)
                option = new List<string>();
            option.Add(e.Value);
        }

        private void ReadMenuItemChoice(XElement e)
        {
            if (MenuItem == null)
                MenuItem = new List<Dictionary<string, int?>>();
            var menusetattr = e.Attribute("set");
            if (menusetattr != null)
                menuset = menusetattr.Value.ToInt();
            while (MenuItem.Count - 1 < menuset)
                MenuItem.Add(new Dictionary<string, int?>());
            var aname = e.Attribute("name");
            var number = e.Attribute("number");
            if (aname != null && number != null)
                MenuItem[menuset].Add(aname.Value, number.Value.ToInt());
        }

        private void ReadCheckboxChoice(XElement e)
        {
            if (Checkbox == null)
                Checkbox = new List<string>();
            Checkbox.Add(e.Value);
        }

        private void ReadYesNoChoices(XElement e)
        {
            if (YesNoQuestion == null)
                YesNoQuestion = new Dictionary<string, bool?>();
            var ynq = e.Attribute("question");
            if (ynq != null)
                YesNoQuestion.Add(ynq.Value, e.Value.ToBool());
        }

        private void ReadText(XElement e)
        {
            if (Text == null)
                Text = new List<Dictionary<string, string>>();
            var txsetattr = e.Attribute("set");
            if (txsetattr != null)
                txset = txsetattr.Value.ToInt();
            if (Text.Count == txset)
                Text.Add(new Dictionary<string, string>());
            var tx = e.Attribute("question");
            if (tx != null)
                Text[txset].Add(tx.Value, e.Value);
        }

        private void ReadExtraQuestion(XElement e)
        {
            if (ExtraQuestion == null)
                ExtraQuestion = new List<Dictionary<string, string>>();
            var eqsetattr = e.Attribute("set");
            if (eqsetattr != null)
                eqset = eqsetattr.Value.ToInt();
            if (ExtraQuestion.Count == eqset)
                ExtraQuestion.Add(new Dictionary<string, string>());
            var eq = e.Attribute("question");
            if (eq != null)
                ExtraQuestion[eqset].Add(eq.Value, e.Value);
        }
        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}