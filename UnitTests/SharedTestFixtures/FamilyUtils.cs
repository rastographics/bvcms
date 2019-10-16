using CmsData;
using CmsData.Classes.ProtectMyMinistry;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharedTestFixtures
{
    public class FamilyUtils: DatabaseTestBase
    {
        public static Person CreateFamilyWithDeceasedMember()
        {
            CMSDataContext db = CMSDataContext.Create(DatabaseFixture.Host);

            var family = new Family();
            db.Families.InsertOnSubmit(family);
            db.SubmitChanges();

            var firstMember = new Person
            {
                Family = family,
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.PrimaryAdult,
            };
            db.People.InsertOnSubmit(firstMember);

            var deceasedMember = new Person
            {
                Family = family,
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.Child,
                IsDeceased = true,
                DeceasedDate = DateTime.Now
            };
            db.People.InsertOnSubmit(firstMember);
            db.SubmitChanges();

            return firstMember;
        }
    }
}
