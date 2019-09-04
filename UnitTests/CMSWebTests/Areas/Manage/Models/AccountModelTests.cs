using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CMSWebTests;
using SharedTestFixtures;
using CMSWebTests.Support;
using Shouldly;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Membership;
using CmsData;

namespace CmsWeb.ModelsTests
{
    [Collection(Collections.Database)]
    public class AccountModelTests : ControllerTestBase
    {
        [Fact]
        public void AuthenticateMobileWithUsernameAndPasswordTest()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var idb = requestManager.CurrentImageDatabase;
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);

            var result = AccountModel.AuthenticateMobile(db, idb);

            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.IsValid.ShouldBeTrue();
            result.Status.ShouldBe(UserValidationStatus.Success);
        }

        [Fact]
        public void AuthenticateMobileWithSessionTokenTest()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var sessionToken = new ApiSession
            {
                CreatedDate = DateTime.Now,
                LastAccessedDate = DateTime.Now,
                SessionToken = Guid.NewGuid(),
                UserId = user.UserId,
            };
            db.ApiSessions.InsertOnSubmit(sessionToken);
            db.SubmitChanges();

            requestManager.CurrentHttpContext.Request.Headers["SessionToken"] = sessionToken.SessionToken.ToString();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);

            var result = AccountModel.AuthenticateMobile(requestManager.CurrentDatabase, requestManager.CurrentImageDatabase);

            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.IsValid.ShouldBeTrue();
            result.Status.ShouldBe(UserValidationStatus.Success);
        }
    }
}
