using System;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Setup.Models;
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

        public IncomingSmsModel(CMSDataContext db, SmsRequest incomingMessage)
        {
            CurrentDatabase = db;
            To = incomingMessage.To;
            From = incomingMessage.From.Substring(2);
            Body = incomingMessage.Body;
        }

        public Person FindPerson()
        {
            var person = (from p in CurrentDatabase.People
                where p.CellPhone == From || p.HomePhone == From
                select p).FirstOrDefault();
            if (person == null)
            {
                throw new Exception($"could not find person with the number {From}");
            }
            return person;
        }
        public SmsReplyWordsModel FindNumber()
        {
            var model = new SmsReplyWordsModel(CurrentDatabase) {Number = To};
            model.PopulateNumber();
            return model;

        }
        public string ProcessAndRespond()
        {
            SmsReplyWordsModel number;
            Person person;
            try
            {
                number = FindNumber();
                person = FindPerson();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            foreach (var r in number.Actions)
            {
                if (!Body.Equal(r.Word))
                    continue;
                switch (r.Action)
                {
                    case "OptOut":
                        break; // todo: Optout
                    case "Attending":
                        return MarkAttendingIntention(r, AttendCommitmentCode.Attending);
                    case "Regrets":
                        return MarkAttendingIntention(r, AttendCommitmentCode.Regrets);
                    case "AddToOrg":
                        return AddToOrg(r);
                    case "AddToOrgSg":
                        return AddToSmallGroup(r);
                    case "SendAnEmail":
                        return SendAnEmail(r);
                    case "RunScript":
                        break; // todo: add RunScript
                    default:
                        return $"{r.Action} action not recognized for {r.Word} on number {To}";
                }
            }
            //Reply word never found in loop
            return $"{Body} reply word not recognized for number {To}";

            string MarkAttendingIntention(SmsActionModel r, int code)
            {
                Meeting meeting = null;
                Organization o = null;
                string markedas = null;
                try
                {
                    if (r.MeetingId == null)
                        throw new Exception("meetingid null");
                    meeting = CurrentDatabase.Meetings.FirstOrDefault(mm => mm.MeetingId == r.MeetingId);
                    if (meeting == null)
                        throw new Exception($"meetingid {r.MeetingId} not found");
                    o = CurrentDatabase.LoadOrganizationById(meeting.OrganizationId);
                    Attend.MarkRegistered(CurrentDatabase, person.PeopleId, r.MeetingId.Value, code);
                    markedas = code == AttendCommitmentCode.Attending ? "Attending" : "Regrets";
                }
                catch (Exception e)
                {
                    return $"No Meeting on action {r.Action}, {e.Message}";
                }
                return $"{person.Name} has been marked as {markedas} to {o.OrganizationName} for {meeting.MeetingDate}";
            }

            string AddToOrg(SmsActionModel r)
            {
                Organization o = null;
                try
                {
                    if (r.OrgId == null)
                        throw new Exception("orgid null");
                    o = CurrentDatabase.LoadOrganizationById(r.OrgId);
                    if (o == null)
                        throw new Exception($"orgid {r.OrgId} not found");
                    o = CurrentDatabase.LoadOrganizationById(r.OrgId);
                    OrganizationMember.InsertOrgMembers(CurrentDatabase, r.OrgId.Value, person.PeopleId, 220, Util.Now, null, false);
                }
                catch(Exception e)
                {
                    return $"Org not found on action {r.Action}, {e.Message}";
                }
                return $"{person.Name} has been added to {o.OrganizationName}";
            }

            string AddToSmallGroup(SmsActionModel r)
            {
                Organization o = null;
                try
                {
                    if(r.OrgId == null)
                        throw new Exception("OrgId is null");
                    o = CurrentDatabase.LoadOrganizationById(r.OrgId);
                    if (o == null)
                        throw new Exception($"OrgId {r.OrgId} not found");
                    if (r.SmallGroup == null)
                        throw new Exception($"SmallGroup is null");
                    var om = OrganizationMember.InsertOrgMembers(CurrentDatabase, r.OrgId.Value, person.PeopleId, 220, Util.Now, null, false);
                    om.AddToGroup(CurrentDatabase, r.SmallGroup);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                return $"{person.Name} has been added to {r.SmallGroup} group for {o.OrganizationName}";
            }

            string SendAnEmail(dynamic r)
            {
                int draftid = r.EmailId;
                var email = CurrentDatabase.ContentFromID(draftid);
                CurrentDatabase.Email(DbUtil.AdminMail, person, email.Title, email.Body);
                return $"email sent to {person.Name}";
            }
        }
    }
}
