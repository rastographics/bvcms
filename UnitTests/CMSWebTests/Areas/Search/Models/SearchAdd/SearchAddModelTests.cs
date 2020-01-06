using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace CMSWebTests.Areas.Search.Models.SearchAdd
{
    [Collection(Collections.Database)]
    public class SearchAddModelTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Move_PushPay_EVs_when_Reassigning_Contribution()
        {
            var PushPayKey = "PushPayKey";
            var KeyValue = RandomString();
            var db = DatabaseFixture.NewDbContext();
            User user = db.Users.Where(u => u.Username == "admin").FirstOrDefault();
            db.CurrentUser = user;
            SearchAddModel m = new SearchAddModel(db);

            Person from = db.People.OrderByDescending(p => p.PeopleId).FirstOrDefault();
            var existing = from.PeopleExtras.SingleOrDefault(ev => ev.Field == PushPayKey);

            if (existing != null)
            {
                db.PeopleExtras.DeleteOnSubmit(existing);
                db.SubmitChanges();
            }

            db.AddExtraValueDataIfNotExist(from.PeopleId, PushPayKey, null, null, KeyValue, null, null);

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
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
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
            db = db.Copy();
            var extraValue = db.PeopleExtras.SingleOrDefault(p => p.PeopleId == from.PeopleId && p.Field == PushPayKey && p.Data == KeyValue);
            extraValue.ShouldBe(null);

            // person contribution moved to should have the ev
            extraValue = db.PeopleExtras.SingleOrDefault(p => p.PeopleId == to.PeopleId && p.Field == PushPayKey && p.Data == KeyValue);
            extraValue.ShouldNotBe(null);

            // clean up
            db.PeopleExtras.DeleteOnSubmit(extraValue);
            db.Contributions.DeleteOnSubmit(db.Contributions.First(mm => mm.ContributionId == c.ContributionId));
            db.SubmitChanges();
        }
    }
}
