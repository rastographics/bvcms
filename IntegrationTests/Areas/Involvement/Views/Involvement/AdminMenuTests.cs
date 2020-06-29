using CmsData;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Involvement.Views.Involvement
{
    [Collection(Collections.Webapp)]
    public class AdminMenuTests : AccountTestBase
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AdminMenu_Should_Show_Venues_When_User_In_VenueBuilder_Role(bool isUserInVenuesRole)
        {
            username = RandomString();
            password = RandomString();

            if (isUserInVenuesRole)
            {
                var db = CMSDataContext.Create(DatabaseFixture.Host);
                db.ExecuteCommand("SET IDENTITY_INSERT Roles ON; INSERT INTO Roles (RoleName, RoleId, hardwired, Priority) VALUES('VenueBuilder', 9999, 1, NULL);");
                var user = CreateUser(username, password, roles: new string[] { "Admin", "Access", "Edit", "VenueBuilder" });
            }
            else
            {
                // Per requirements (5/13/2020), even being in the "Admin" role in itself is not sufficient to display
                //    the Venues menu.  A user must explicitly be in the "VenueBuilder" role.
                var user = CreateUser(username, password, roles: new string[] { "Admin", "Access", "Edit" });
            }

            WaitForPageLoad();
            Login();

            RepeatUntil(() =>
            {
                Find(text: "Administration")?.Click();
                Wait(3);
            },
            () => Find(text: "Venues") != null);

            var venuesText = Find(text: "Venues");

            if (isUserInVenuesRole)
            {
                db.ExecuteCommand("DELETE FROM UserRole where RoleId = 9999");
                db.ExecuteCommand("DELETE FROM Roles where RoleName = 'VenueBuilder' AND RoleId = 9999");
            }

            if (isUserInVenuesRole)
            {
                venuesText.ShouldNotBeNull();
            }
            else
            {
                venuesText.ShouldBeNull();
            }
        }
    }
}
