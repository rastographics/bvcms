using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;
using System.Data.Linq;
using CmsData;

namespace IntegrationTests.Areas.Involvement.Views.Involvement
{
    [Collection(Collections.Webapp)]
    public class AdminMenuTests : AccountTestBase
    {
        [Fact]
        public void AdminMenu_Should_Show_Venues_When_User_In_VenueBuilder_Role()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            db.ExecuteCommand("SET IDENTITY_INSERT Roles ON; INSERT INTO Roles (RoleName, RoleId, hardwired, Priority) VALUES('VenueBuilder', 9999, 1, NULL);");

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "VenueBuilder" });
            WaitForPageLoad();
            Login();
            WaitForPageLoad();
            int cnt = 0;

            while (true)
            {
                var adminMenu = Find(text: "Administration");

                if (adminMenu != null)
                {
                    adminMenu.Click();
                    break;
                }
                else
                {
                    if (cnt > 20)
                        break;
                }

                Wait(2);
                cnt++;
            }

            Wait(3);

            var venuesMenu = Find(text: "Venues");
            db.ExecuteCommand("DELETE FROM UserRole where RoleId = 9999");
            db.ExecuteCommand("DELETE FROM Roles where RoleName = 'VenueBuilder' AND RoleId = 9999");
            venuesMenu.ShouldNotBeNull();
        }

        [Fact]
        public void AdminMenu_Should_Not_Show_Venues_When_User_Not_In_VenueBuilder_Role()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Edit", "Access" });
            WaitForPageLoad();
            Login();
            WaitForPageLoad();

            int cnt = 0;

            while (true)
            {
                var adminMenu = Find(text: "Administration");

                if (adminMenu != null)
                {
                    adminMenu.Click();
                    break;
                }
                else
                {
                    if (cnt > 20)
                        break;
                }

                Wait(2);
                cnt++;
            }

            Wait(3);

            var venuesMenu = Find(text: "Venues");
            venuesMenu.ShouldBeNull();
        }

        [Fact]
        public void AdminMenu_Should_Show_Venues_When_User_In_Admin_Role()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Admin", "Access", "Edit" });
            Wait(3);
            Login();

            WaitForPageLoad();
            int cnt = 0;

            while (true)
            {
                var adminMenu = Find(text: "Administration");

                if (adminMenu != null)
                {
                    adminMenu.Click();
                    break;
                }
                else
                {
                    if (cnt > 20)
                        break;
                }

                Wait(2);
                cnt++;
            }

            Wait(3);

            var venuesMenu = Find(text: "Venues");
            venuesMenu.ShouldNotBeNull();
        }
    }
}
