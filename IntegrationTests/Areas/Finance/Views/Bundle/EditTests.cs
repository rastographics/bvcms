using IntegrationTests.Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Areas.Finance.Views.Bundle
{
    [Collection("WebApp Collection")]
    public class EditTests : AccountTestBase
    {
        [Fact]
        public void Should_Open_Datepicker_On_Mobile_Resolutions()
        {
            driver.Manage().Window.Size = new Size(768, 1024);

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            Open($"{rootUrl}Bundles/");
        }
    }
}
