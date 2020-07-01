using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;
using System.Data.Linq;
using CmsData;

namespace IntegrationTests.Areas.Involvement.Views.Navigation
{
    [Collection(Collections.Webapp)]
    public class DbSettingsTests : AccountTestBase
    {
        [Fact]
        public void UseNewInvolvementCreation_DbSetting_Should_Show_Involvement_Menu_When_Enabled()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            db.ExecuteCommand("DELETE FROM Setting where id = 'UseNewInvolvementCreation'");
            db.ExecuteCommand("INSERT INTO Setting VALUES('UseNewInvolvementCreation', 'true', NULL)");
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Admin", "Edit", "Access", "Checkin" });
            Login();
            Wait(3);

            var invMenu = Find(text: "Involvements");
            invMenu.ShouldNotBeNull();
        }

        [Fact]
        public void UseNewInvolvementCreation_DbSetting_Should_Not_Show_Involvement_Menu_When_Disabled()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            db.ExecuteCommand("DELETE FROM Setting where id = 'UseNewInvolvementCreation'");
            db.ExecuteCommand("INSERT INTO Setting VALUES('UseNewInvolvementCreation', 'false', NULL)");
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Admin", "Edit", "Access", "Checkin" });
            Login();
            Wait(3);

            var invMenu = Find(text: "Involvements");
            invMenu.ShouldBeNull();
        }
    }
}
