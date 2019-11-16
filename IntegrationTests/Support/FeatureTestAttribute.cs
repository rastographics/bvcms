using System.Reflection;
using Xunit.Sdk;

namespace IntegrationTests.Support
{
    public class FeatureTestAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            FeatureTestBase.Current?.ClearCookies();
            FeatureTestBase.Current?.MaximizeWindow();
        }

        public override void After(MethodInfo methodUnderTest)
        {
            FeatureTestBase.Current?.SaveScreenshot(methodUnderTest.Name);
            FeatureTestBase.Current?.ShouldNotHaveScriptError();
        }
    }
}
