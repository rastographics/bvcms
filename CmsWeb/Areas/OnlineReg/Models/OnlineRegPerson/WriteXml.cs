using System.Linq;
using System.Reflection;
using System.Xml;
using CmsData.API;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {

        public void WriteXml(XmlWriter writer)
        {
            var optionsAdded = false;
            var checkoxesAdded = false;
            var w = new APIWriter(writer);

            foreach (PropertyInfo pi in typeof(OnlineRegPersonModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "SpecialTest":
                        WriteSpecialTest(w);
                        break;
                    case "FundItem":
                        WriteFundItems(w);
                        break;
                    case "FamilyAttend":
                        WriteFamilyAttend(w);
                        break;
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
                    case "MissionTripGoerId":
                        if (Parent.SupportMissionTrip)
                            w.Add(pi.Name, MissionTripGoerId);
                        break;
                    case "IsFilled":
                        if (IsFilled)
                            w.Add(pi.Name, IsFilled);
                        break;
                    case "CreatingAccount":
                        if (CreatingAccount)
                            w.Add(pi.Name, CreatingAccount);
                        break;
                    case "MissionTripNoNoticeToGoer":
                        if (MissionTripNoNoticeToGoer)
                            w.Add(pi.Name, MissionTripNoNoticeToGoer);
                        break;
                    case "memberus":
                        if (memberus)
                            w.Add(pi.Name, memberus);
                        break;
                    case "otherchurch":
                        if (otherchurch)
                            w.Add(pi.Name, otherchurch);
                        break;
                    case "nochurch":
                        if (nochurch)
                            w.Add(pi.Name, nochurch);
                        break;
                    default:
                        w.Add(pi.Name, pi.GetValue(this, null));
                        break;
                }
            }
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

        private void WriteFamilyAttend(APIWriter w)
        {
            if (FamilyAttend != null && FamilyAttend.Count > 0)
                foreach (var f in FamilyAttend)
                {
                    w.Start("FamilyAttend");
                    w.Attr("PeopleId", f.PeopleId);
                    w.Attr("Name", f.Name);
                    w.Attr("Attend", f.Attend);
                    w.Attr("Birthday", f.Birthday);
                    w.Attr("GenderId", f.GenderId);
                    w.Attr("MaritalId", f.MaritalId);
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

        private void WriteSpecialTest(APIWriter w)
        {
            if (SpecialTest != null)
                foreach (var d in SpecialTest)
                {
                    w.Start("SpecialTest");
                    w.Attr("key", d.Key);
                    w.AddText(d.Value);
                    w.End();
                }
        }
    }
}
