using System;
using System.Text;
using CmsWeb.Areas.Public.Models;
using CMSWebTests.Properties;
using CMSWebTests.Support;
using SharedTestFixtures;
using Shouldly;
using Twilio.AspNet.Common;
using Xunit;
using Dapper;
using UtilityExtensions;

namespace CMSWebTests.Areas.Public.Models
{
    [Collection(Collections.Database)]
    public class IncomingSmsModelTests : ControllerTestBase
    {
        [Theory]
        [InlineData("YES")]
        [InlineData("NO")]
        [InlineData("NO", "MeetingId", 123456789)]
        [InlineData("JOIN")]
        [InlineData("JOIN", "OrgId", 123456789)]
        [InlineData("Flowers")]
        [InlineData("Flowers", "SmallGroup", null)]
        public void ProcessIncomingTextMessage(string word, string field = null, object value = null)
        {
            var number = "+12055836839";
            var meetingdt = DateTime.Today.AddDays(-1).AddHours(10);
            var from = "+19014888888";
            var cn = db.Connection;

            CleanDatabase();
            PrepareDatabase();

            switch (word.ToUpper())
            {
                case "YES":
                    TestActionShouldBe($"David Carroll has been marked as Attending to App Testing Org for {meetingdt}");
                    break;
                case "NO":
                    TestActionShouldBe(field == null
                        ? $"David Carroll has been marked as Regrets to App Testing Org for {meetingdt}"
                        : $"No Meeting on action Regrets, meetingid {value} not found");
                    break;
                case "JOIN":
                    TestActionShouldBe(field == null
                        ? $"David Carroll has been added to App Testing Org"
                        : $"Org not found on action AddToOrg, orgid 123456789 not found");
                    break;
                case "FLOWERS":
                    TestActionShouldBe(field == null
                        ? $"David Carroll has been added to Flowers group for App Testing Org"
                        : $"SmallGroup is null");
                    break;
            }
            // cleanup
            CleanDatabase();

            void PrepareDatabase()
            {
                var cell = from.Substring(2);
                var replywords = ReplyWordsJson();
                db.Connection.Execute(@"
insert SmsGroups (Name, Description) values ('TestGroup', '');
declare @gid int = @@Identity;
insert SmsGroupMembers (GroupId, UserId) values (@gid, 1);
insert SmsGroupMembers (GroupId, UserId) values (@gid, 2);
insert SmsNumbers (GroupID,Number,LastUpdated,ReplyWords) Values (@gid, @number, @meetingdt, @replywords)
", new { number, meetingdt, replywords });
                db.Connection.Execute("UPDATE dbo.People SET CellPhone = @cell WHERE PeopleId = 2", new { cell });
            }

            string ReplyWordsJson()
            {
                var meeting = CmsData.Meeting.FetchOrCreateMeeting(db, 36, meetingdt);
                string json = Encoding.Default.GetString(CMSWebTestsResources.SmsReplyWords);
                return json.Replace("{meetingid}", meeting.MeetingId.ToString());
            }

            void CleanDatabase()
            {
                cn.Execute(@"
delete dbo.SmsItems;
delete dbo.SmsList;
delete dbo.SMSNumbers;
delete dbo.SMSGroupMembers
delete dbo.SMSGroups;
delete dbo.Attend where OrganizationId = 36
delete dbo.Meetings where OrganizationId = 36

delete dbo.OrgMemMemTags
from dbo.OrgMemMemTags mt
join dbo.MemberTags t on t.Id = mt.MemberTagId and t.OrgId = mt.OrgId
where mt.OrgId = 36 and t.Name = 'flowers'

delete dbo.MemberTags where Name = 'flowers' and OrgId = 36
");
            }
            void TestActionShouldBe(string correctmessage)
            {
                var request = new SmsRequest { To = number, From = from, Body = word };
                var incoming = new IncomingSmsModel(db, request);
                var model = incoming.FindGroup();
                if (field != null)
                {
                    var action = model.Actions.Find(vv => vv.Word == word);
                    switch (field)
                    {
                        case "MeetingId":
                            action.MeetingId = (int?)value;
                            break;
                        case "OrgId":
                            action.OrgId = (int?)value;
                            break;
                        case "SmallGroup":
                            action.SmallGroup = (string)value;
                            break;
                    }
                    model.Save();
                }
                var actualmessage = incoming.ProcessAndRespond();
                actualmessage.ShouldBe(correctmessage);
            }
        }
    }
}
