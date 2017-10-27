using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Elmah;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        private int eqset;
        private int menuset;
        private int txset;

        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) return;

            eqset = 0;
            txset = 0;
            menuset = 0;
            var optionN = 0;

            foreach (var e in x.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "FundItem":
                        ReadFundItem(e);
                        break;
                    case "FamilyAttend":
                        ReadFamilyAttend(e);
                        break;
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
                        ReadDropdownOption(e, optionN++);
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
                    case "CreatingAccount":
                        CreatingAccount = e.Value.ToBool();
                        break;
                    case "SpecialTest":
                        ReadSpecialTest(e);
                        break;
                    default:
                        if (Util.SetPropertyFromText(this, TranslateName(name), e.Value) == false)
                        {
                            ErrorSignal.FromCurrentContext().Raise(new Exception("OnlineRegPerson Missing name:" + name));
                            Log($"Error:Missing({name})");
                        }
                        break;
                }
            }
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private string GetAttr(XElement e, string name)
        {
            var a = e.Attribute(name);
            return a?.Value ?? string.Empty;
        }

        private void ReadDropdownOption(XElement e, int n)
        {
            InitializeOptionIfNeeded();
            option[n] = e.Value;
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
            var cnt = setting.AskItems.Count(vv => vv.IsAskText);
            if (Text == null)
            {
                Text = new List<Dictionary<string, string>>();
                for(var n = 0; n < cnt; n++)
                    Text.Add(new Dictionary<string, string>());
            }
            var txsetattr = e.Attribute("set");
            if (txsetattr != null)
                txset = txsetattr.Value.ToInt();

            while (Text.Count <= txset)
                Text.Add(new Dictionary<string, string>());

            var tx = e.Attribute("question");
            if (tx != null)
                Text[txset].Add(tx.Value, e.Value);
        }

        private void ReadExtraQuestion(XElement e)
        {
            var cnt = setting.AskItems.Count(vv => vv.IsAskExtraQuestions);
            if (ExtraQuestion == null)
            {
                ExtraQuestion = new List<Dictionary<string, string>>();
                for(var n = 0; n < cnt; n++)
                    ExtraQuestion.Add(new Dictionary<string, string>());
            }
            var eqsetattr = e.Attribute("set");
            if (eqsetattr != null)
                eqset = eqsetattr.Value.ToInt();

            if (ExtraQuestion.Count <= eqset)
                ExtraQuestion.Add(new Dictionary<string, string>());

            var eq = e.Attribute("question");
            if (eq != null)
                ExtraQuestion[eqset].Add(eq.Value, e.Value);
        }

        private void ReadSpecialTest(XElement e)
        {
            if (SpecialTest == null)
                SpecialTest = new Dictionary<string, string>();
            var key = e.Attribute("key");
            if (key != null)
                SpecialTest.Add(key.Value, e.Value);
        }

        private void ReadFamilyAttend(XElement e)
        {
            var fa = new FamilyAttendInfo();
            fa.PeopleId = GetAttr(e, "PeopleId").ToInt2();
            fa.Attend = GetAttr(e, "Attend").ToBool();
            fa.Name = GetAttr(e, "Name");
            fa.Birthday = GetAttr(e, "Birthday");
            fa.Email = GetAttr(e, "Email");
            fa.MaritalId = GetAttr(e, "MaritalId").ToInt2();
            fa.GenderId = GetAttr(e, "GenderId").ToInt2();
            if (FamilyAttend == null)
                FamilyAttend = new List<FamilyAttendInfo>();
            FamilyAttend.Add(fa);
        }

        private void ReadFundItem(XElement e)
        {
            if (FundItem == null)
                FundItem = new Dictionary<int, decimal?>();
            var fu = e.Attribute("fund");
            if (fu != null)
                FundItem.Add(fu.Value.ToInt(), e.Value.ToDecimal());
        }
    }
}
