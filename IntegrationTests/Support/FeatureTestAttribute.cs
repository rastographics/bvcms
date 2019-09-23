using System;
using System.Reflection;
using Xunit.Sdk;

namespace IntegrationTests.Support
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FeatureTestAttribute : BeforeAfterTestAttribute
    {
        private bool takeScreenshot;

        public FeatureTestAttribute()
        {
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            try
            {
                base.Before(methodUnderTest);
            }
            catch
            {
                takeScreenshot = true;
                throw;
            }
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (takeScreenshot)
            {
                FeatureTestBase.Current?.SaveScreenshot();
            }
            base.After(methodUnderTest);
        }
    }
}
