using CmsData;
using CmsData.Codes;
using System;
using System.Linq;

namespace SharedTestFixtures
{
    public class MockPeople
    {
        public static Person CreateSavePerson(CMSDataContext db, Family family = null)
        {
            if (family == null)
            {
                family = new Family()
                {
                    CreatedDate = DateTime.Now
                };
                db.Families.InsertOnSubmit(family);
                db.SubmitChanges();
            }
            var person = new Person
            {
                Family = family,
                FirstName = DatabaseTestBase.RandomString(),
                LastName = DatabaseTestBase.RandomString(),
                EmailAddress = DatabaseTestBase.RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.PrimaryAdult,
            };
            db.People.InsertOnSubmit(person);
            db.SubmitChanges();

            return person;
        }
       
        public static int CreateSaveCouple(CMSDataContext db)
        {
            var f = new Family
            {
                CreatedDate = DateTime.Now
            };
            db.Families.InsertOnSubmit(f);
            db.SubmitChanges();
            var familyId = f.FamilyId;
            var head = CreateSavePerson(db, f);
            head.MaritalStatusId = 20;
            head.GenderId = 1;
            db.SubmitChanges();
            var spouse = CreateSavePerson(db, f);
            spouse.MaritalStatusId = 20;
            spouse.GenderId = 2;
            db.SubmitChanges();

            return familyId;
        }

        public static void DeleteMockPerson(CMSDataContext db, Person person)
        {
            var family = db.Families.FirstOrDefault(f => f.FamilyId == person.FamilyId);
            db.Families.DeleteOnSubmit(family);
            db.People.DeleteOnSubmit(person);
            db.SubmitChanges();
        }
    }
}
