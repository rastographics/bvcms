using SharedTestFixtures;
using System;
using System.Text;

namespace CMSWebTests.Support
{
    public class ControllerTestBase : DatabaseTestBase
    {
        protected string BasicAuthenticationString(string username, string password)
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        }
    }
}
