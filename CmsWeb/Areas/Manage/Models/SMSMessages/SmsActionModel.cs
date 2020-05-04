using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using RestSharp.Extensions;

namespace CmsWeb.Areas.Manage.Models.SMSMessages
{
    public class SmsActionModel
    {
        public string Word { get; set; }
        public string Action { get; set; }

        public int? MeetingId { get; set; }
        public int? OrgId { get; set; }
        public int? EmailId { get; set; }
        public string SmallGroup { get; set; }
        public string ScriptName { get; set; }

        // Metadata
        [JsonIgnore] public string Description { get; set; }
        [JsonIgnore] public string Arg1Name { get; set; }
        [JsonIgnore] public string Arg1Description { get; set; }
        [JsonIgnore] public string Arg2Name { get; set; }
        [JsonIgnore] public string Arg2Description { get; set; }

        public object Arg1Value
        {
            get
            {
                switch (Arg1Name)
                {
                    case "MeetingId":
                        return MeetingId;
                    case "OrgId":
                        return OrgId;
                    case "EmailId":
                        return EmailId;
                    case "ScriptName":
                        return ScriptName;
                }
                return null;
            }
        }
        public object Arg2Value
        {
            get
            {
                if (Arg2Name == "SmallGroup")
                    return SmallGroup;
                return null;
            }
        }

        public static List<SmsActionModel> StandardActions = new List<SmsActionModel>
            {
                New("OptOut", "Opt Out"),
                New("Attending", "Attending", "MeetingId", "Meeting Id"),
                New("Regrets", "Regrets", "MeetingId", "Meeting Id"),
                New("AddToOrg", "Add To Organization", "OrgId", "Organization Id"),
                New("AddToOrgSg", "Add To Smallgroup", "OrgId", "Organization Id", "SmallGroup", "Small Group Name"),
                New("SendAnEmail", "Send an Email", "EmailId", "Email Id"),
                New("RunScript", "Run Python Script", "ScriptName", "Script Name"),
            };

        public static SmsActionModel New(string action, string description, string arg1 = null, string argdesc1 = null, string arg2 = null, string argdesc2 = null)
        {
            return new SmsActionModel
            {
                Action = action,
                Description = description,
                Arg1Name = arg1,
                Arg1Description = argdesc1,
                Arg2Name = arg2,
                Arg2Description = argdesc2,
            };
        }
        public MvcHtmlString ActionOptions()
        {
            var sb = new StringBuilder($"<option value=''>(choose action)</option>\n");
            foreach (var a in StandardActions)
            {
                sb.AppendLine($"<option value='{a.Action}' {Selected(a.Action)}>{a.Description}</option>");
            }
            return new MvcHtmlString(sb.ToString());

            string Selected(string action)
            {
                if (action == Action)
                    return "selected='selected'";
                return "";
            }
        }

        public MvcHtmlString Input1(int n)
        {
            if (Arg1Name.HasValue())
                return new MvcHtmlString($"<input name='Actions[{n}].{Arg1Name}' placeholder='{Arg1Description}' type='text' class='form-control' value='{Arg1Value}' />");
            return MvcHtmlString.Empty;
        }
        public MvcHtmlString Input2(int n)
        {
            if (Arg2Name.HasValue())
                return new MvcHtmlString($"<input name='Actions[{n}].{Arg2Name}' placeholder='{Arg2Description}' type='text' class='form-control' value='{Arg2Value}' />");
            return MvcHtmlString.Empty;
        }

        public void PopulateMetaData()
        {
            var t = StandardActions.Find(vv => vv.Action == Action);
            Description = t.Description;
            Arg1Name = t.Arg1Name;
            Arg2Name = t.Arg2Name;
            Arg1Description = t.Arg1Description;
            Arg2Description = t.Arg2Description;
        }
    }
}
