using Xunit;
using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTestFixtures;
using CmsData.Codes;
using Shouldly;

namespace CmsData.Tests
{
    [Collection(Collections.Database)]
    public class ContributionTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(100, "This is my first note.")]
        [InlineData(1200, "This is my second note.")]
        public void CreateContributionWithNote(decimal contributed, string notes)
        {
            var person = CreatePerson();
            var fromDate = new DateTime(2019, 1, 1);
            var fund = MockFunds.CreateSaveFund(db, true);
            var bundleHeader = MockContributions.CreateSaveBundle(db);
            MockContributions.CreateSaveContribution(db, bundleHeader, fromDate, contributed, person.PeopleId, ContributionTypeCode.CheckCash, fund.FundId, ContributionStatusCode.Recorded, notes);

            var contribution = db.Contributions.Where(c => c.PeopleId == person.PeopleId && c.Notes == notes).FirstOrDefault();
            contribution.Notes.ShouldBe(notes);
        }
    }
}
