using CmsData;
using CmsWeb.Models;
using SharedTestFixtures;
using Shouldly;
using System.Web.UI.WebControls;
using UtilityExtensions;
using Xunit;

namespace CMSWebTests.Models
{
    public class PeopleSearchModelTest
    {
        //This test ensures that search string without apostrophe retrieves surnames with apostrophe 
        [Fact]
        public void FetchPeople_ApostropheSearch_Test()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {   
                ContextTestUtils.CreateMockHttpContext();                
                var m = new PeopleSearchModel(db);
                                
                var p = MockPeople.CreateSavePerson(db, lastName: "O'Neil");
                m.m.name = "ONeil";
                var people = m.FetchPeople();
                people.ShouldNotBeNull();

                p.LastName = "Del'hi";
                m.m.name = "Delhi";
                people = m.FetchPeople();
                people.ShouldNotBeNull();
            }
        }
    }
}
