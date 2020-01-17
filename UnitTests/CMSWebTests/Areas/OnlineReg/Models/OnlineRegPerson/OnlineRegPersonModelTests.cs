using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Areas.OnlineReg.Models;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CmsWeb.Areas.OnlineReg.ModelsTests
{
    [Collection(Collections.Database)]
    public class OnlineRegPersonModelTests : DatabaseTestBase
    {
        [Fact]
        public void OnEnrollTest()
        {
            var context = ContextTestUtils.CreateMockHttpContext();
            var org = CreateOrganization();
            var person = CreatePerson();
            var om = OrganizationMember.AddOrgMember(db, org.OrganizationId, person.PeopleId, MemberTypeCode.Member, DateTime.Now, org.OrganizationName);
            org.UpdateRegSetting(new Settings
            {
                AskItems = new List<Ask>(new Ask[] {
                    new AskText { Name = "ImportantQuestion" },
                    new AskCheckboxes {
                        Name ="Options",
                        list = new List<AskCheckboxes.CheckboxItem>(new[] {
                            new AskCheckboxes.CheckboxItem {
                                Name = "Item 1"
                            }
                        })
                    }
                })
            });
            db.SubmitChanges();
            var notify = db.StaffPeopleForOrg(om.OrganizationId).First();

            var personModel = new OnlineRegModel(db)
            {
                Orgid = om.OrganizationId,
                UserPeopleId = om.PeopleId,
            };
            var model = new OnlineRegPersonModel(db)
            {
                orgid = om.OrganizationId,
                Parent = personModel,
                PeopleId = om.PeopleId,
                FirstName = om.Person.FirstName,
                LastName = om.Person.LastName,
                Checkbox = new List<string>(new[] { "Item 1" }),
                QuestionsOK = true,
                Text = new List<Dictionary<string, string>>(new[] { new Dictionary<string, string>() })
            };
            model.Text.First()["ImportantQuestion"] = "ImportantAnswer";
            var script = "OnEnrollTest";
            db.WriteContentPython(script, @"print(Data)");
            model.setting.OnEnrollScript = script;
            model.OnEnroll(om);
            model.ScriptResults.Trim().ShouldBe($@"{{
  ""PeopleId"": {om.PeopleId},
  ""OrganizationId"": {om.OrganizationId},
  ""OnlineNotifyId"": {notify.PeopleId},
  ""OnlineNotifyEmail"": ""{notify.EmailAddress}"",
  ""OnlineNotifyName"": ""{notify.Name}"",
  ""LoggedIn"": true,
  ""FirstName"": ""{person.FirstName}"",
  ""LastName"": ""{person.LastName}"",
  ""TextQuestion"": {{
    ""ImportantQuestion"": ""ImportantAnswer""
  }},
  ""Checkbox"": {{
    ""Item 1"": true
  }}
}}".Replace("\r", ""));
        }
    }
}
