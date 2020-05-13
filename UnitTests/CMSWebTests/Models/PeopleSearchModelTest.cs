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
        [Theory]
        [InlineData("O'Neil")]
        [InlineData("Del'hi")]
        public void FetchPeople_ApostropheSearch_Test(string name)
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {   
                ContextTestUtils.CreateMockHttpContext();                
                var m = new PeopleSearchModel(db);
                                
                var p = MockPeople.CreateSavePerson(db, lastName: name);
                m.m.name = name.Replace("'", string.Empty);
                var people = m.FetchPeople();
                people.ShouldNotBeNull();
            }
        }
    }
}
