using CmsData;
using CmsData.API;
using System.Linq;
using System.Reflection;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        private void BuildDynamicData(PythonModel pe, OrganizationMember om)
        {
            pe.DictionaryAdd("PeopleId", PeopleId ?? 0);
            pe.DictionaryAdd("OrganizationId", om.OrganizationId);
            var notifyIds = DbUtil.Db.StaffPeopleForOrg(om.OrganizationId);
            pe.DictionaryAdd("OnlineNotifyId", notifyIds[0].PeopleId);
            pe.DictionaryAdd("OnlineNotifyEmail", notifyIds[0].EmailAddress);
            pe.DictionaryAdd("OnlineNotifyName", notifyIds[0].Name);
            var props = typeof(OnlineRegPersonModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pi in props.Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "ExtraQuestion":
                        AddQuestions(pe);
                        break;
                    case "Text":
                        AddText(pe);
                        break;
                    case "YesNoQuestion":
                        AddYesNo(pe);
                        break;
                    case "option":
                        AddDropDownOptions(pe);
                        break;
                    case "Checkbox":
                        AddCheckboxes(pe);
                        break;
                    case "MenuItem":
                        AddMenuItems(pe);
                        break;
                    case "ScriptResults":
                        if (ScriptResults.HasValue())
                        {
                            pe.DictionaryAdd(pi.Name, ScriptResults);
                        }

                        break;
                    case "memberus":
                        if (memberus)
                        {
                            pe.DictionaryAdd("MemberUs", memberus);
                        }

                        break;
                    case "otherchurch":
                        if (otherchurch)
                        {
                            pe.DictionaryAdd("OtherChurch", otherchurch);
                        }

                        break;
                    case "LoggedIn":
                        pe.DictionaryAdd(pi.Name, LoggedIn);
                        break;
                    case "FirstName":
                        pe.DictionaryAdd(pi.Name, FirstName);
                        break;
                    case "LastName":
                        pe.DictionaryAdd(pi.Name, LastName);
                        break;
                }
            }
        }

        private void AddQuestions(PythonModel pe)
        {
            if (pe.DataHas("ExtraQuestion"))
            {
                return;
            }

            if (ExtraQuestion == null || ExtraQuestion.Count == 0)
            {
                return;
            }

            var questions = new DynamicData();
            pe.DictionaryAdd("ExtraQuestion", questions);
            foreach (var dict in ExtraQuestion)
            {
                foreach (var q in dict)
                {
                    questions.AddValue(q.Key, q.Value);
                }
            }
        }

        private void AddText(PythonModel pe)
        {
            if (pe.DataHas("TextQuestion"))
            {
                return;
            }

            if (Text == null || Text.Count == 0)
            {
                return;
            }

            var textquestions = new DynamicData();
            pe.DictionaryAdd("TextQuestion", textquestions);
            foreach (var dict in Text)
            {
                foreach (var q in dict)
                {
                    textquestions.AddValue(q.Key, q.Value);
                }
            }
        }

        private void AddYesNo(PythonModel pe)
        {
            if (pe.DataHas("YesNoQuestion"))
            {
                return;
            }

            if (YesNoQuestion == null || YesNoQuestion.Count == 0)
            {
                return;
            }

            var yesnoquestions = new DynamicData();
            pe.DictionaryAdd("YesNoQuestion", yesnoquestions);
            foreach (var dict in YesNoQuestion)
            {
                yesnoquestions.AddValue(dict.Key, dict.Value ?? false);
            }
        }

        private void AddCheckboxes(PythonModel pe)
        {
            if (pe.DataHas("Checkbox"))
            {
                return;
            }

            if (Checkbox == null)
            {
                return;
            }

            var checkboxes = new DynamicData();
            pe.DictionaryAdd("Checkbox", checkboxes);
            foreach (var c in Checkbox)
            {
                checkboxes.AddValue(c, true);
            }
        }

        private void AddMenuItems(PythonModel pe)
        {
            if (pe.DataHas("MenuItem"))
            {
                return;
            }

            if (MenuItem == null)
            {
                return;
            }

            var menuitems = new DynamicData();
            pe.DictionaryAdd("MenuItem", menuitems);
            foreach (var dict in MenuItem)
            {
                foreach (var q in dict)
                {
                    menuitems.AddValue(q.Key, q.Value ?? 0);
                }
            }
        }

        private void AddDropDownOptions(PythonModel pe)
        {
            if (pe.DataHas("DropdownOption"))
            {
                return;
            }

            if (option == null)
            {
                return;
            }

            var options = new DynamicData();
            pe.DictionaryAdd("DropdownOption", options);
            foreach (var o in option)
            {
                options.AddValue(o, true);
            }
        }
    }
}
