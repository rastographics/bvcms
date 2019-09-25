using System.Reflection;
using Xunit.Sdk;

namespace IntegrationTests.Support
{
    public class FeatureTestAttribute : BeforeAfterTestAttribute
    {   
        public override void After(MethodInfo methodUnderTest)
        {
            FeatureTestBase.Current?.SaveScreenshot(methodUnderTest.Name);
        }
    }
}
