using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class SmsReplyWordsActionModel
    {
        public string Word { get; set; }
        public string Action { get; set; }

        public int? MeetingId { get; set; }
        public int? OrgId { get; set; }
        public int? EmailId { get; set; }
        public string SmallGroup { get; set; }
        public string ScriptName { get; set; }
        public string ReplyMessage { get; set; }

        // Metadata
        [JsonIgnore] public string Description { get; set; }
        [JsonIgnore] public string Arg1Name { get; set; }
        [JsonIgnore] public string Arg1Description { get; set; }
        [JsonIgnore] public string Arg2Name { get; set; }
        [JsonIgnore] public string Arg2Description { get; set; }

        [JsonIgnore]
        public string DefaultMessage
        {
            get
            {
                var a = StandardActions.Find(vv => vv.Action == Action);
                return a.ReplyMessage;
            }
        }

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

        public static string OptOutMesssage = "{name} has been opted out of {groupname}";
        public static string OptInMesssage = "{name} has been opted in to {groupname}";
        public static string MarkAttendMessage = "{name} has been marked as {markedas} to {orgname} for {meetingdate}";
        public static string RecordAttendanceMessage = "Recorded Attendance to {orgname} on {meetingdate} for {name}";
        public static string AddToOrgMessage = "{name} has been added to {orgname}";
        public static string AddToOrgSgMessage = "{name} has been added to {smallgroup} group for {orgname}";
        public static string SendAnEmailMessage = "email sent to {name}";
        public static string SendReplyOnlyMessage = "No default message exists for this action";
        public static string RunScriptMessage = "script run";

        public static List<SmsReplyWordsActionModel> StandardActions = new List<SmsReplyWordsActionModel>
            {
                New("OptOut", "Opt Out", reply: OptOutMesssage),
                New("OptIn", "Opt In", reply: OptInMesssage),
                New("Attending", "Attending", "MeetingId", "Meeting Id", reply: MarkAttendMessage),
                New("Regrets", "Regrets", "MeetingId", "Meeting Id", reply: MarkAttendMessage),
                New("RecordAttend", "Record Attendance", "MeetingId", "Meeting Id", reply: RecordAttendanceMessage),
                New("AddToOrg", "Add To Organization", "OrgId", "Organization Id", reply: AddToOrgMessage),
                New("AddToOrgSg", "Add To Smallgroup", "OrgId", "Organization Id", "SmallGroup", "Small Group Name", reply: AddToOrgSgMessage),
                New("SendAnEmail", "Send an Email", "EmailId", "Email Id", reply: SendAnEmailMessage),
                New("SendReplyOnly", "Send a Reply", reply: SendReplyOnlyMessage),
                New("RunScript", "Run Python Script", "ScriptName", "Script Name", reply: RunScriptMessage),
            };

        public static SmsReplyWordsActionModel New(string action, string description, string arg1 = null, string argdesc1 = null, string arg2 = null, string argdesc2 = null, string reply = null)
        {
            return new SmsReplyWordsActionModel
            {
                Action = action,
                Description = description,
                Arg1Name = arg1,
                Arg1Description = argdesc1,
                Arg2Name = arg2,
                Arg2Description = argdesc2,
                ReplyMessage = reply
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

        public void PopulateMetaData()
        {
            var t = StandardActions.Find(vv => vv.Action == Action);
            if (t == null)
                return;
            Description = t.Description;
            Arg1Name = t.Arg1Name;
            Arg2Name = t.Arg2Name;
            Arg1Description = t.Arg1Description;
            Arg2Description = t.Arg2Description;
        }
    }
}
