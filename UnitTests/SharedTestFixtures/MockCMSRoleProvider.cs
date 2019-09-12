using CmsWeb.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockCMSRoleProvider : CMSRoleProvider
    {
        public MockCMSRoleProvider()
        {
        }

        public List<string> UserRoles = new List<string>();

        public override bool IsUserInRole(string username, string rolename) => UserRoles.Contains(rolename);
    }
}
