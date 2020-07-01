using CmsData;
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
        public List<User> ValidUsers { get; set; } = new List<User>();
        public MockCMSMembershipProvider()
        {
        }

        public bool ValidUser { get; set; }

        public override bool ValidateUser(string username, string password)
        {
            return ValidUsers.Any(u => u.Username == username && u.Password == password) || ValidUser;
        }

        public override void SetAuthCookie(string username, bool createPersistentCookie = true)
        {
            //do nothing
        }
    }
}
