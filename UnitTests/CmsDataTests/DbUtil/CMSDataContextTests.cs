using SharedTestFixtures;
using CmsData;
using Xunit;
using Shouldly;
using System;
using System.Linq;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class CMSDataContextTests
    {
        [Fact]
        public void PeopleQuery2_Parse_Test()
        {
            var query = @"PledgeBalance( FundId='10102' ) <= 0.00
    AND PledgeAmountBothJointHistory( StartDate='2/1/2016', EndDate='11/29/2018', FundIdOrNullForAll='10102' ) >= 1
    AND HasManagedGiving( FundIdOrBlank='10102' ) = 1[True]
    AND HasPeopleExtraField <> 'RiseRecurAutoStopped'
    AND CampusId = 2[West Side]";
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            db.PeopleQuery2(query);
        }

        [Fact]
        public static void Should_Get_Setting()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var expected = "1";
                var setting = MockSettings.CreateSaveSetting(db, "PostContributionPledgeFunds", "1");
                var actual = db.GetSetting("PostContributionPledgeFunds", "");
                actual.ShouldBe(expected);
            }
        }

        [Fact]
        public void Should_Set_SpouseId_In_Family()
        {
            int familyId;
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                familyId = MockPeople.CreateSaveCouple(db);
            }
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var family = db.Families.FirstOrDefault(p => p.FamilyId == familyId);
                family.HeadOfHouseholdSpouseId.ShouldNotBeNull();
            }
        }

        [Fact]
        public void Should_ShouldUpdateAllSpouseId()
        {
            int familyId;
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                familyId = MockPeople.CreateSaveCouple(db);
            }
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var family = db.Families.FirstOrDefault(p => p.FamilyId == familyId);
                family.HeadOfHouseholdSpouseId = null;
                db.SubmitChanges();
            }
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                db.UpdateAllSpouseId();
                var family = db.Families.FirstOrDefault(p => p.FamilyId == familyId);
                family.HeadOfHouseholdSpouseId.ShouldNotBeNull();
            }
        }
    }
}
