using SharedTestFixtures;
using CmsData;
using Xunit;
using Shouldly;
using System;
using System.Linq;
using System.Globalization;
using CmsWeb.Models.ExtraValues;

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
                var f = new Family
                {
                    CreatedDate = DateTime.Now
                };
                db.Families.InsertOnSubmit(f);
                db.SubmitChanges();
                familyId = f.FamilyId;
                var head = MockPeople.CreateSavePerson(db, f);
                head.MaritalStatusId = 20;
                head.GenderId = 1; 
                db.SubmitChanges();
                var spouse = MockPeople.CreateSavePerson(db, f);
                spouse.MaritalStatusId = 20;
                spouse.GenderId = 2;
                db.SubmitChanges();                
            }
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var family = db.Families.FirstOrDefault(p => p.FamilyId == familyId);
                family.HeadOfHouseholdSpouseId.ShouldNotBeNull();
            }
        }

        [InlineData("PushPayKey","keyXYZ","20190101",null,null,null)]
        [Theory]
        public void Should_Insert_EV_Only_If_does_not_Exist(string key, string value, string datevalue, string text, int? intvalue, bool? bitvalue)
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var person = db.People.FirstOrDefault();
                int extraValId = db.AddExtraValueDataIfNotExist(person.PeopleId, key, value, DateTime.ParseExact(datevalue,"yyyyMMdd",CultureInfo.InvariantCulture), text, intvalue, bitvalue);
                int attempt2 = db.AddExtraValueDataIfNotExist(person.PeopleId, key, value, DateTime.ParseExact(datevalue, "yyyyMMdd", CultureInfo.InvariantCulture), text, intvalue, bitvalue);
                attempt2.ShouldBe(0);
                var m = new ExtraValueModel(extraValId, "People", "Standard");
                m.Delete(key);
            }
                

        }
    }
}
