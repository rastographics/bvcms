using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;
using CmsData;
using UtilityExtensions;
using System.Linq;
using OpenQA.Selenium;

namespace IntegrationTests.Areas.Search.Models
{
    [Collection(Collections.Webapp)]
    public class SavedQueryInfoTest : AccountTestBase
    {
        [Fact]        
        public void Should_Update_Model()
        {
            using (var db = CMSDataContext.Create(Util.Host))
            {
                username = RandomString();
                password = RandomString();
                var user = CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin" });
                Login();
                Open($"{rootUrl}SavedQueryList");
                WaitForElement(".edit-saved-query.btn", 5);
                Find(text: "Edit").Click();

                WaitForElement("input#Name", 5);
                var preName = Find(css: "input#Name").GetAttribute("Value");
                var preOwner = Find(css: "input#Owner").GetAttribute("Value");
                var preIsPublic = Find(css: "input#Ispublic").GetAttribute("checked");
                var queryID = Find(css: "div.modal-body").FindElement(By.Id("QueryId")).GetAttribute("value");

                Find(css: "input#Name").SendKeys("Test");
                Find(css: "input#Owner").SendKeys("Test");
                Find(css: "input#Ispublic").Click();
                Find(text: "Save").Click();


                var rowTD = Find(css: $@"tr#row-{queryID} > a").FindElements(By.TagName("td"));
                var x = rowTD[0].FindElement(By.TagName("a")).Text;
                var x1 = rowTD[1].Text;
                var x2 = rowTD[2].FindElement(By.TagName("i"));
                //.Text.ShouldBe(preName + "Test");
                //Find(css: "input#Owner").GetAttribute("Value").ShouldBe(preOwner + "Test");
                //Find(css: "input#Ispublic").GetAttribute("checked").ShouldBe((preIsPublic is null) ? "cheked" : null);

                //Find(css: "input#Owner").

                //var queryBeforeUpdate = db.LoadQueryById2(qry.Id);
                //SavedQueryInfo sqi = new SavedQueryInfo {                    
                //    Ispublic = true,                    
                //    Name = "test-case",
                //    Owner = qry.Owner,
                //    QueryId = qry.Id                    
                //    };

                //sqi.UpdateModel(db);

                //var queryAfterUpdate = db.LoadQueryById2(qry.Id);
                //queryAfterUpdate.Name.ShouldBe("test-case");
                //queryAfterUpdate.Ispublic.ShouldBe(true);
                //queryAfterUpdate.Owner.ShouldBe(qry.Owner);
                //queryAfterUpdate.QueryId.ShouldBe(qry.Id);

            }
                
        }
    }
}
