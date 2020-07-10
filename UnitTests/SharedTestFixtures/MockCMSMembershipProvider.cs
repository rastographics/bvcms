using CmsData;
using CmsWeb.Membership;
using System.Collections.Generic;
using System.Linq;

namespace SharedTestFixtures
{
    public class MockCMSMembershipProvider : CMSMembershipProvider
    {
        public List<User> ValidUsers { get; set; } = new List<User>();
        protected override string GetValidationKey() => "0FFE1ABD1A08215353C233D6E009613E95EEC4253832A761AF28FF37AC5A150C";
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
