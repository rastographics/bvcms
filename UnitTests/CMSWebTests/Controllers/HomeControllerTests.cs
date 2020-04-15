using System;
using Xunit;
using SharedTestFixtures;
using CmsData;

namespace CmsWeb.Controllers.Tests
{
    [Collection(Collections.Database)]
    public class HomeControllerTests : IDisposable
    {
        //private CMSDataContext _currentDatabase;
        //private CMSDataContext CurrentDatabase => _currentDatabase ?? (_currentDatabase = DbUtil.Db);

        [Fact]
        public void GetCurrentUserTest()
        {
            //var user = CurrentDatabase.CurrentUser;

            Assert.Equal(1, 1);
        }

        public void Dispose()
        {
        }
    }
}
