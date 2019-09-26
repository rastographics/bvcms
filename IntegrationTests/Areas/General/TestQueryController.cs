using CmsData;
using IntegrationTests.Support;
using Shouldly;
using UtilityExtensions;
using Xunit;
using IntegrationTests.Resources;
using SharedTestFixtures;

namespace IntegrationTests.Areas.Manage
{
    [Collection(Collections.Webapp)]
    public class TestQueryController : AccountTestBase
    {
        /// <summary>
        /// This verifies that when QueryModel m is bound in POST operation
        /// that it no longer needs to have m.Db = CurrentDatabase
        /// as this is now done in SmartBinder with an inherited class called DbBinder
        /// This is applicable wherever this pattern is found on any model binding that needs CurrentDatabase
        /// The POST ActionResult exercised is QueryController.Results(QueryModel m)
        /// </summary>
        [Fact, FeatureTest]
        public void DbInQueryModelShouldBeInstantiatedByModelBinding()
        {
            username = RandomString();
            password = RandomString();
            CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin" });
            Login();
            Open($"{rootUrl}QueryCode?code=Age>65");
            WaitForElement("input[id=totcnt]");
            PageSource.ShouldContain("David Carroll");
        }
    }
}
