using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Models.SmsMessages;
using Dapper;
using Twilio.AspNet.Common;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models
{
    public class IncomingSmsModel
    {
        public CMSDataContext CurrentDatabase { get; set; }

        private string To { get; set; }
        private string From { get; set; }
        private string Body { get; set; }

        private SmsReceived row;
        private Person person;
        private string groupName;
        private SmsReplyWordsActionModel action;

        public IncomingSmsModel(CMSDataContext db, SmsRequest incomingMessage)
        {
            CurrentDatabase = db;
            To = incomingMessage.To;
            From = incomingMessage.From.StartsWith("+1")
                ? incomingMessage.From.Substring(2)
                : incomingMessage.From;
            Body = incomingMessage.Body.Trim();
            row = new SmsReceived
            {
                Body = Body,
                ToNumber = To,
                FromNumber = From,
                DateReceived = Util.Now,
            };

        }

        public Person FindPerson()
        {
            var per = (from p in CurrentDatabase.People
                          where p.CellPhone == From || p.HomePhone == From
                          select p).FirstOrDefault();
            row.FromPeopleId = per?.PeopleId;
            return per;
        }

        SmsReplyWordsModel model;
        public SmsReplyWordsModel FindGroup()
        {
            var m = new SmsReplyWordsModel(CurrentDatabase);
            var group = (from grp in CurrentDatabase.SMSGroups
                join num in CurrentDatabase.SMSNumbers on grp.Id equals num.GroupID
                where num.Number == To
                select grp).FirstOrDefault();
            if(group == null)
                throw new Exception($"could not find group from number {To}");
            m.GroupId = group.Id;
            groupName = group.Name;
            m.PopulateActions();
            row.ToGroupId = m.GroupId;
            return m;
        }

        public string ProcessAndRespond()
        {
            try
            {
                model = FindGroup();
                person = FindPerson();
            }
            catch (Exception e)
            {
                return GetError(e.Message);
            }

            var rval = "";
            foreach(var r in model.Actions)
            {
                action = r;
                if (!Body.Equal(action.Word))
                    continue;
                row.Action = action.Action;
                switch (action.Action)
                {
                    case "OptOut":
                        rval = GroupOptOut();
                        break;
                    case "OptIn":
                        rval = GroupOptIn();
                        break;
                    case "Attending":
                        rval = MarkAttendingIntention(AttendCommitmentCode.Attending);
                        break;
                    case "Regrets":
                        rval = MarkAttendingIntention(AttendCommitmentCode.Regrets);
                        break;
                    case "AddToOrg":
                        rval = AddToOrg();
                        break;
                    case "AddToOrgSg":
                        rval = AddToSmallGroup();
                        break;
                    case "SendAnEmail":
                        rval = SendAnEmail();
                        break;
                    case "RunScript":
                        rval = RunScript();
                        break;
                    default:
                        rval = GetError($"{action.Action} action not recognized for {action.Word} on number {To}");
                        break;
                }
            }
            if(rval.HasValue())
                return rval;
            //Reply word never found in loop, must be a regular text message
            return ReceivedTextNoAction();
        }

        public void SendNotices()
        {
            var q = from gm in CurrentDatabase.SMSGroupMembers
                where gm.GroupID == model.GroupId
                where gm.ReceiveNotifications == true
                select gm.User.Person;
            var subject = $"Received Text from {From}";
            var body = $@"From {person?.Name ?? "name unknown"} to {groupName} at {row.DateReceived}<br>
with message: <br/>{Body}<br>
<a href='{CurrentDatabase.CmsHost}/SmsMessages#{row.Id}'>Click this to goto message.</a><br><br>
They received: {row.ActionResponse}";
            foreach (var p in q)
            {
                CurrentDatabase.Email(Util.AdminMail, p, null, subject, body, false);
            }
        }

        private string GetActionReplyMessage()
        {
            row.ActionResponse = DoReplacments(Util.PickFirst(action.ReplyMessage, action.DefaultMessage));
            CurrentDatabase.SmsReceiveds.InsertOnSubmit(row);
            CurrentDatabase.SubmitChanges();
            SendNotices();
            if (action.ReplyMessage.Equal("NONE"))
                return string.Empty;
            return row.ActionResponse;
        }
        private string GetError(string message)
        {
            row.ErrorOccurred = true;
            row.ActionResponse = message;
            CurrentDatabase.SmsReceiveds.InsertOnSubmit(row);
            CurrentDatabase.SubmitChanges();
            SendNotices();
            return message;
        }

        private const string GetNoPersonMessage = "We don't recognize this number. Please contact the church office";

        private const string MatchCodeRe = "({[^}]*})";
        private string DoReplacments(string message)
        {
            var stringlist = Regex.Split(message, MatchCodeRe, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            var texta = new List<string>(stringlist);
            for (var i = 1; i < texta.Count; i ++)
                texta[i] = DoReplaceCode(texta[i]);

            return string.Join("", texta);
        }

        private string DoReplaceCode(string item)
        {
            if (!item.StartsWith("{"))
                return item;
            switch (item.ToLower())
            {
                case "{meetingid}":
                    return meeting.MeetingId.ToString();
                case "{meetingdate}":
                    return meeting.MeetingDate.FormatDateTm();
                case "{groupname}":
                    return groupName;
                case "{orgname}":
                    return organization.OrganizationName;
                case "{name}":
                    return person.Name;
                case "{smallgroup}":
                    return action.SmallGroup;
                case "{markedas}":
                    return markedas;
            }
            return item;
        }

        private string GroupOptOut()
        {
            if (person == null)
                return GetNoPersonMessage;
            var o = new SmsGroupOptOut
            {
                FromGroup = model.GroupId,
                ToPeopleId = person.PeopleId
            };
            CurrentDatabase.SmsGroupOptOuts.InsertOnSubmit(o);
            return GetActionReplyMessage();
        }
        private string GroupOptIn()
        {
            if (person == null)
                return GetNoPersonMessage;
            CurrentDatabase.Connection.Execute(
                "delete dbo.SmsGroupOptOut where FromGroup = @gid and ToPeopleId = @pid",
                new {gid = model.GroupId, pid = person.PeopleId});
            return GetActionReplyMessage();
        }
        private Meeting meeting;
        private string markedas;
        private string MarkAttendingIntention(int code)
        {
            if (person == null)
                return GetNoPersonMessage;
            try
            {
                row.Args = $"{{ \"MeetingId\": \"{action.MeetingId}\"}}";
                if (action.MeetingId == null)
                    throw new Exception("meetingid null");
                meeting = CurrentDatabase.Meetings.FirstOrDefault(mm => mm.MeetingId == action.MeetingId);
                if (meeting == null)
                    throw new Exception($"meetingid {action.MeetingId} not found");
                organization = CurrentDatabase.LoadOrganizationById(meeting.OrganizationId);
                Attend.MarkRegistered(CurrentDatabase, person.PeopleId, meeting.MeetingId, code);
                markedas = code == AttendCommitmentCode.Attending ? "Attending" : "Regrets";
            }
            catch (Exception e)
            {
                return GetError($"No Meeting on action {action.Action}, {e.Message}");
            }
            return GetActionReplyMessage();
        }

        private Organization organization;
        private string AddToOrg()
        {
            if (person == null)
                return GetNoPersonMessage;
            try
            {
                row.Args = $"{{ \"OrgId\": \"{action.OrgId}\"}}";
                if (action.OrgId == null)
                    throw new Exception("orgid null");
                organization = CurrentDatabase.LoadOrganizationById(action.OrgId);
                if (organization == null)
                    throw new Exception($"orgid {action.OrgId} not found");
                organization = CurrentDatabase.LoadOrganizationById(action.OrgId);
                OrganizationMember.InsertOrgMembers(CurrentDatabase, action.OrgId.Value, person.PeopleId, 220, Util.Now, null, false);
            }
            catch (Exception e)
            {
                return GetError($"Org not found on action {action.Action}, {e.Message}");
            }
            return GetActionReplyMessage();
        }

        private string AddToSmallGroup()
        {
            if (person == null)
                return GetNoPersonMessage;
            try
            {
                row.Args = $"{{ \"OrgId\": \"{action.OrgId}\", \"SmallGroup\": \"{action.SmallGroup}\"}}";
                if (action.OrgId == null)
                    throw new Exception("OrgId is null");
                organization = CurrentDatabase.LoadOrganizationById(action.OrgId);
                if (organization == null)
                    throw new Exception($"OrgId {action.OrgId} not found");
                if (action.SmallGroup == null)
                    throw new Exception("SmallGroup is null");
                var om = OrganizationMember.InsertOrgMembers(CurrentDatabase, action.OrgId.Value, person.PeopleId, 220, Util.Now, null, false);
                om.AddToGroup(CurrentDatabase, action.SmallGroup);
            }
            catch (Exception e)
            {
                return GetError(e.Message);
            }
            return GetActionReplyMessage();
        }

        private string SendAnEmail()
        {
            if (person == null)
                return GetNoPersonMessage;
            row.Args = $"{{ \"EmailId\": \"{action.EmailId}\"}}";
            if (action.EmailId == null)
                return GetError($"Email Draft not found for id {action.EmailId}");
            var email = CurrentDatabase.ContentFromID(action.EmailId.Value);
            CurrentDatabase.Email(DbUtil.AdminMail, person, email.Title, email.Body);
            return GetActionReplyMessage();
        }
        private string ReceivedTextNoAction()
        {
            CurrentDatabase.SmsReceiveds.InsertOnSubmit(row);
            CurrentDatabase.SubmitChanges();
            SendNotices();
            return string.Empty;
        }
        private string RunScript()
        {
            if (person == null)
                return GetNoPersonMessage;
            var m = new PythonModel(CurrentDatabase);
            var script = CurrentDatabase.ContentOfTypePythonScript(action.ScriptName);
            if (!script.HasValue())
                return GetError($"Script name {action.ScriptName} not found");
            m.DictionaryAdd("ToNumber", To);
            m.DictionaryAdd("ToGroupId", row.ToGroupId);
            m.DictionaryAdd("FromNumber", From);
            m.DictionaryAdd("Message", Body);
            m.DictionaryAdd("PeopleId", row.FromPeopleId);
            m.DictionaryAdd("Name", person.Name);
            m.DictionaryAdd("First", person.FirstName);
            m.DictionaryAdd("Last", person.LastName);
            var msg = Util.PickFirst(
                m.RunScript(script).Trim(),
                action.ReplyMessage,
                action.DefaultMessage);
            row.ActionResponse = DoReplacments(msg);
            CurrentDatabase.SmsReceiveds.InsertOnSubmit(row);
            CurrentDatabase.SubmitChanges();
            SendNotices();
            if(msg.Equal("NONE"))
                return String.Empty;
            return row.ActionResponse;
        }
    }
}
