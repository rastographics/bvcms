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
    public class SavedQueryInfoTests : AccountTestBase
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

                //Edit Saved Search Query
                Open($"{rootUrl}SavedQueryList");
                WaitForElement($@"div#contact-footer", 5);
                var trID = Find(css: $@"table#resultsTable > tbody > tr:first-child").GetAttribute("id");

                var preName = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(1) > a").Text;
                var preOwner = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(2)").Text;
                var preIsPublic = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(3) > i.fa.fa-check");
                var preIsPublicBool = preIsPublic is null ? false : true;
                var editbtn = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(6)");
                editbtn.Click();

                WaitForElement("input#Ispublic", 5);

                //Update values
                var txtName = Find(css: "input#Name");
                var txtOwner = Find(css: "input#Owner");
                var cboIsPublic = Find(css: "input#Ispublic");
                var savebtn = Find(text: "Save");

                txtName.Clear();
                txtName.SendKeys("NameTest");
                txtOwner.Clear();
                txtOwner.SendKeys("OwnerTest");
                cboIsPublic.Click();
                savebtn.Click();

                WaitForElement($@"div#contact-footer", 5);

                //Test update
                var postName = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(1) > a").Text;
                var postOwner = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(2)").Text;
                var postIsPublic = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(3) > i.fa.fa-check");
                var postIsPublicBool = postIsPublic is null ? false : true;
                var editbtn2 = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(6)");

                postName.ShouldBe("NameTest");
                postOwner.ShouldBe("OwnerTest");
                postIsPublicBool.ShouldBe(!preIsPublicBool);

                //RollBack
                editbtn2.Click();
                WaitForElement("input#Ispublic", 5);

                txtName = Find(css: "input#Name");
                txtOwner = Find(css: "input#Owner");
                cboIsPublic = Find(css: "input#Ispublic");
                savebtn = Find(text: "Save");

                txtName.Clear();
                txtName.SendKeys(preName);
                txtOwner.Clear();
                txtOwner.SendKeys(preOwner);
                cboIsPublic.Click();
                savebtn.Click();

                WaitForElement($@"div#contact-footer", 5);

                //Test rollback
                postName = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(1) > a").Text;
                postOwner = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(2)").Text;
                postIsPublic = Find(css: $@"table#resultsTable > tbody > tr#{trID} > td:nth-child(3) > i.fa.fa-check");
                postIsPublicBool = postIsPublic is null ? false : true;

                postName.ShouldBe(preName);
                postOwner.ShouldBe(preOwner);
                postIsPublicBool.ShouldBe(preIsPublicBool);
            }
        }
    }
}
