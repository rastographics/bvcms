using CmsWeb.Areas.Search.Models;
using CmsData;
using SharedTestFixtures;
using Xunit;
using Shouldly;
using System.Linq;
using CmsWeb.Code;
using CmsData.Codes;
using System;

namespace CMSWebTests.Areas.Search.Models.SearchAdd
{
    [Collection(Collections.Database)]
    public class SearchAddModelTests
    {
        [Fact]
        public void Should_Move_PushPay_EVs_when_Reassigning_Contribution()
        {
            var db = DatabaseFixture.NewDbContext();
            User user = db.Users.Where(u => u.Username == "admin").FirstOrDefault();
            db.CurrentUser = user;
            SearchAddModel m = new SearchAddModel(db);

            Person from = db.People.FirstOrDefault();
            var existing = from.PeopleExtras.SingleOrDefault(ev => ev.Field == "PushPayKey");

            if (existing != null)
            {
                db.PeopleExtras.DeleteOnSubmit(existing);
            }

            db.AddExtraValueDataIfNotExist(from.PeopleId, "PushPayKey", null, null, "test", null, null);

            Contribution c = new Contribution
            {
                PeopleId = from.PeopleId,
                ContributionDate = DateTime.Now,
                ContributionAmount = 1,
                ContributionTypeId = ContributionTypeCode.Online,
                ContributionStatusId = ContributionStatusCode.Recorded,
                Origin = ContributionOriginCode.PushPay,
                CreatedDate = DateTime.Now,
                FundId = 1,
                MetaInfo = "unit test"
            };
            db.Contributions.InsertOnSubmit(c);

            Person to = new Person
            {
                Family = db.Families.FirstOrDefault(),
                FirstName = DatabaseTestBase.RandomString(),
                LastName = DatabaseTestBase.RandomString(),
                EmailAddress = DatabaseTestBase.RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.PrimaryAdult,
            };

            db.People.InsertOnSubmit(to);
            db.SubmitChanges();

            var pp = new PendingPersonModel(db);
            pp.CopyPropertiesFrom(to);
            pp.LoadAddress();
            m.PendingList.Add(pp);
            m.AddContributor(c.ContributionId, OriginCode.Contribution);

            // person contribution moved from should not have the ev
            var extraValue = db.PeopleExtras.SingleOrDefault(p => p.PeopleId == from.PeopleId && p.Field == "PushPayKey" && p.Data == "test");
            extraValue.ShouldBe(null);

            // person contribution moved to should have the ev
            extraValue = db.PeopleExtras.SingleOrDefault(p => p.PeopleId == to.PeopleId && p.Field == "PushPayKey" && p.Data == "test");
            extraValue.ShouldNotBe(null);

            // clean up
            db.PeopleExtras.DeleteOnSubmit(extraValue);
            db.People.DeleteOnSubmit(to);
            db.Contributions.DeleteOnSubmit(c);
            db.SubmitChanges();
        }
    }
}
