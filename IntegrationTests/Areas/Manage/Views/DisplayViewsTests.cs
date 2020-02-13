using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Manage.Views
{
    [Collection(Collections.Webapp)]
    public class DisplayViewsTests : AccountTestBase
    {
        [Theory, FeatureTest]
        [InlineData("Empty Template")]
        [InlineData("empty template")]
        [InlineData("emptytemplate")]
        [InlineData("EmptyTemplate")]
        public void Empty_Template_Can_Not_Be_Used(string TemplateName)
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            Open($"{rootUrl}Display/#tab_emailTemplates");

            WaitForElement("#emailTemplates > div.row.hidden-xs > div > div > a");
            Find(css: "#emailTemplates > div.row.hidden-xs > div > div > a").Click();

            WaitForElement("#newName-role", maxWaitTimeInSeconds: 3);
            Find(id: "newName-role").Clear();
            Find(id: "newName-role").SendKeys(TemplateName);

            var smallMessage = Find(css: "#new-modal-role > div > div > form > div.modal-body > div:nth-child(2) > div > div > small");
            smallMessage.ShouldNotBeNull();

            var inputPost = Find(id: "post-role");
            inputPost.GetAttribute("class").ShouldContain("disabled");
        }
    }
}
