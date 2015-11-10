using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using CmsData.API;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsData.OnlineRegSummaryText
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class OnlineRegPersonModel0
    {
        public OnlineRegPersonModel0(string xml = null)
        {
            if(xml != null)
                ReadXml(xml);
            if (ExtraQuestion == null)
                ExtraQuestion = new List<Dictionary<string, string>> {new Dictionary<string, string>()};
            if (Text == null)
                Text = new List<Dictionary<string, string>> {new Dictionary<string, string>()};
        }

        public static OnlineRegPersonModel0 CreateFromSettings(CMSDataContext db, int orgid)
        {
            var m = new OnlineRegPersonModel0();
            var settings = db.CreateRegistrationSettings(orgid);
            foreach (var ask in settings.AskItems)
            {
                switch (ask.Type)
                {
                    case "AskExtraQuestions":
                        var eq = (AskExtraQuestions)ask;
                        if(eq.UniqueId >= m.ExtraQuestion.Count)
                            m.ExtraQuestion.Add(new Dictionary<string, string>());
                        foreach (var q in eq.list)
                            m.ExtraQuestion[eq.UniqueId][q.Question] = "";
                        break;
                    case "AskText":
                        var tx = (AskText)ask;
                        if(tx.UniqueId >= m.Text.Count)
                            m.Text.Add(new Dictionary<string, string>());
                        foreach (var q in tx.list)
                            m.Text[tx.UniqueId][q.Question] = "";
                        break;
                }
            }
            return m;
        }
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
        public string WriteXml()
        {
            var optionsAdded = false;
            var checkoxesAdded = false;
            var w = new APIWriter();
            w.Start("OnlineRegPersonModel");

            foreach (PropertyInfo pi in typeof(OnlineRegPersonModel0).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "ExtraQuestion":
                        WriteExtraAnswers(w);
                        break;
                    case "Text":
                        WriteText(w);
                        break;
                    case "YesNoQuestion":
                        WriteYesNoChoices(w);
                        break;
                    case "option":
                        optionsAdded = WriteDropdownOptions(optionsAdded, w);
                        break;
                    case "Checkbox":
                        checkoxesAdded = WriteCheckboxChoices(checkoxesAdded, w);
                        break;
                    case "MenuItem":
                        WriteMenuChoices(w);
                        break;
                    default:
                        w.Add(pi.Name, pi.GetValue(this, null));
                        break;
                }
            }
            w.End();
            return w.ToString();
        }

        private bool WriteDropdownOptions(bool optionsAdded, APIWriter w)
        {
            if (option != null && option.Count > 0 && !optionsAdded)
                foreach (var o in option)
                    w.Add("option", o);
            optionsAdded = true;
            return optionsAdded;
        }

        private void WriteMenuChoices(APIWriter w)
        {
            if (MenuItem != null)
                for (var i = 0; i < MenuItem.Count; i++)
                    if (MenuItem[i] != null && MenuItem[i].Count > 0)
                        foreach (var q in MenuItem[i])
                        {
                            w.Start("MenuItem");
                            w.Attr("set", i);
                            w.Attr("name", q.Key);
                            w.Attr("number", q.Value);
                            w.End();
                        }
        }

        private bool WriteCheckboxChoices(bool checkoxesAdded, APIWriter w)
        {
            if (Checkbox != null && Checkbox.Count > 0 && !checkoxesAdded)
                foreach (var c in Checkbox)
                    w.Add("Checkbox", c);
            checkoxesAdded = true;
            return checkoxesAdded;
        }

        private void WriteYesNoChoices(APIWriter w)
        {
            if (YesNoQuestion != null && YesNoQuestion.Count > 0)
                foreach (var q in YesNoQuestion)
                {
                    w.Start("YesNoQuestion");
                    w.Attr("question", q.Key);
                    w.AddText(q.Value.ToString());
                    w.End();
                }
        }

        private void WriteText(APIWriter w)
        {
            if (Text != null)
                for (var i = 0; i < Text.Count; i++)
                    if (Text[i] != null && Text[i].Count > 0)
                        foreach (var q in Text[i])
                        {
                            w.Start("Text");
                            w.Attr("set", i);
                            w.Attr("question", q.Key);
                            w.AddText(q.Value);
                            w.End();
                        }
        }

        private void WriteExtraAnswers(APIWriter w)
        {
            if (ExtraQuestion != null)
                for (var i = 0; i < ExtraQuestion.Count; i++)
                    if (ExtraQuestion[i] != null && ExtraQuestion[i].Count > 0)
                        foreach (var q in ExtraQuestion[i])
                        {
                            w.Start("ExtraQuestion");
                            w.Attr("set", i);
                            w.Attr("question", q.Key);
                            w.AddText(q.Value);
                            w.End();
                        }
        }

        private void WriteFundItems(APIWriter w)
        {
            if (FundItem != null && FundItem.Count > 0)
                foreach (var f in FundItem.Where(ff => ff.Value > 0))
                {
                    w.Start("FundItem");
                    w.Attr("fund", f.Key);
                    w.AddText(f.Value.Value.ToString());
                    w.End();
                }
        }
    }
}