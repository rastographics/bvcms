using IntegrationTests.Support;
using Shouldly;
using System.Drawing;
using Xunit;
using SharedTestFixtures;
using System.Linq;
using OpenQA.Selenium;

namespace IntegrationTests.Areas.Finance.Views.PostBundle
{
    [Collection(Collections.Webapp)]
    public class PostBundleViewsTests : AccountTestBase
    {
        [Fact]
        public void Should_Split_Contribution()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();
            Wait(3);
            Open($"{rootUrl}Bundles/");
            Find(text: "Create New Bundle").Click();
            Find(id: "Bundle_TotalCash").SendKeys("1000");
            Find(text: "Save").Click();

            var bundleHeaderId = db.BundleHeaders.OrderByDescending(b => b.CreatedDate).FirstOrDefault().BundleHeaderId;

            Open($"{rootUrl}PostBundle/{bundleHeaderId}");
            Find(xpath: "//input[@id='pid']").SendKeys("1");
            Find(xpath: "//input[@id='amt']").SendKeys("100");            
            Find(xpath: "(//a[contains(text(),'Update')])[4]").Click();
            Wait(3);
            Find(xpath: "//table[@id='bundle']/tbody/tr/td[8]/div/button").Click();
            Find(xpath: "//table[@id='bundle']/tbody/tr/td[8]/div/ul/li[2]/a").Click();
            Find(id: "amt-split").SendKeys("10 10 10 10 10");
            Find(id: "split-submit").Click();
            Wait(4);
            var r1 = Find(xpath: "//a[contains(text(),'50.00')]");
            r1.ShouldNotBeNull();
            var r2 = Find(css: "tr:nth-child(2) > .name");
            r2.ShouldNotBeNull();
            var r3 = Find(css: "tr:nth-child(3) > .name");
            r3.ShouldNotBeNull();
            var r4 = Find(css: "tr:nth-child(4) > .name");
            r4.ShouldNotBeNull();
            var r5 = Find(css: "tr:nth-child(5) > .name");
            r5.ShouldNotBeNull();
            var r6 = Find(css: "tr:nth-child(6) > .name");
            r6.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Split_Contribution_One_By_One()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();
            Wait(3);
            Open($"{rootUrl}Bundles/");
            Find(text: "Create New Bundle").Click();
            Find(id: "Bundle_TotalCash").SendKeys("1000");
            Find(text: "Save").Click();

            var bundleHeaderId = db.BundleHeaders.OrderByDescending(b => b.CreatedDate).FirstOrDefault().BundleHeaderId;

            Open($"{rootUrl}PostBundle/{bundleHeaderId}");
            Find(xpath: "//input[@id='pid']").SendKeys("1");
            Find(xpath: "//input[@id='amt']").SendKeys("100");
            Find(xpath: "(//a[contains(text(),'Update')])[4]").Click();
            Wait(3);
            Find(xpath: "//table[@id='bundle']/tbody/tr/td[8]/div/button").Click();
            Find(xpath: "//table[@id='bundle']/tbody/tr/td[8]/div/ul/li[2]/a").Click();
            Find(id: "amt-split").SendKeys("1 2 3 4 5");
            Find(id: "split-submit").Click();
            Wait(4);
            var r1 = Find(xpath: "//a[contains(text(),'85.00')]");
            r1.ShouldNotBeNull();
            var r2 = Find(css: "tr:nth-child(2) > .amt");
            r2.GetAttribute("val").ShouldBe("5.00");            
            var r3 = Find(css: "tr:nth-child(3) > .amt");
            r3.GetAttribute("val").ShouldBe("4.00");
            var r4 = Find(css: "tr:nth-child(4) > .amt");
            r4.GetAttribute("val").ShouldBe("3.00");
            var r5 = Find(css: "tr:nth-child(5) > .amt");
            r5.GetAttribute("val").ShouldBe("2.00");
            var r6 = Find(css: "tr:nth-child(6) > .amt");
            r6.GetAttribute("val").ShouldBe("1.00");
        }
    }
}
