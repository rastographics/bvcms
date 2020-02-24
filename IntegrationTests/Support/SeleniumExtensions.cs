using OpenQA.Selenium;

namespace IntegrationTests.Support
{
    public static class SeleniumExtensions
    {
        public static IWebElement Parent(this IWebElement element)
        {
            return element.FindElement(By.XPath("./.."));
        }
    }
}
