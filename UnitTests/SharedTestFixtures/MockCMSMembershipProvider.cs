using CmsWeb.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockCMSMembershipProvider : CMSMembershipProvider
    {
        public MockCMSMembershipProvider()
        {
        }

        public bool ValidUser { get; set; }

        public override bool ValidateUser(string username, string password)
        {
            return ValidUser;
        }

        public override void SetAuthCookie(string username, bool createPersistentCookie = true)
        {
            //do nothing
        }
    }
}
