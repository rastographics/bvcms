using CmsData;
using CmsData.Classes.ProtectMyMinistry;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CmsDataTests
{
    public class PeopleUtilsTests
    {
        [Theory]
        [MemberData(nameof(Data_GetParentsIdsTest))]
        public void GetParentsIdsTest(IQueryable<Person> q, int expected)
        {
            var actual = PeopleUtils.GetParentsIds(q);
            actual.Count().ShouldBe(expected);
        }

        //[Theory]
        //[MemberData(nameof(Data_GetPrimarySecundaryIdsTest))]
        //public void GetPrimarySecundaryIdsTest(IQueryable<Person> q, int expected)
        //{
        //    var actual = PeopleUtils.GetPrimarySecundaryIds(q);
        //    actual.Count().ShouldBe(expected);
        //}

        public static IEnumerable<object[]> Data_GetParentsIdsTest =>
            new List<object[]>
            {
                new object[] { GenerateChildrenWithParents(0, 10), 20 },
                new object[] { GeneratePeople(10, 5, 3), 28 }
            };

        public static IEnumerable<object[]> Data_GetPrimarySecundaryIdsTest =>
            new List<object[]>
            {
                new object[] { GeneratePeople(10, 5, 3), 8 }
            };

        private static IQueryable<Person> GeneratePeople(int numberOfChilden, int numberOfPrimary, int numberOfSecondary)
        {
            var id = 0;
            var people = GenerateChildrenWithParents(id, numberOfChilden).ToList();
            id += numberOfChilden;
            var primary = GenerateAdults(id, numberOfPrimary, PositionInFamily.PrimaryAdult);
            id += numberOfPrimary;
            var secondary = GenerateAdults(id, numberOfSecondary, PositionInFamily.SecondaryAdult);
            people.AddRange(primary);
            people.AddRange(secondary);
            return people.AsQueryable();
        }

        private static List<Person> GenerateAdults(int initialId, int numberOfAdults, int positionInFamily)
        {
            var id = initialId;
            var adults = new List<Person>();
            for (int i = 0; i < numberOfAdults; i++)
            {
                adults.Add(CreatePerson(id, positionInFamily));
                id++;
            }
            return adults;
        }

        public static IQueryable<Person> GenerateChildrenWithParents(int initialId, int numberOfChildren)
        {
            var id = initialId;
            List<Person> children = new List<Person>();
            for (int i = 0; i < numberOfChildren; i++)
            {
                var child = CreatePerson(id, PositionInFamily.Child);
                children.Add(child);
                var father = CreatePerson(id, PositionInFamily.PrimaryAdult, child.Family);
                father.Family.HeadOfHousehold = father;
                children.Add(father);
                var mother = CreatePerson(id, PositionInFamily.PrimaryAdult, child.Family);
                mother.Family.HeadOfHouseholdSpouseId = mother.PeopleId;
                children.Add(mother);
                id++;
            }
            return children.AsQueryable();
        }

        public static Person CreatePerson(int id, int positionInFamily, Family family = null)
        {
            if (family == null)
            {
                family = new Family()
                {
                    CreatedDate = DateTime.Now
                };
            }
            var child = new Person
            {
                PeopleId = id,
                Family = family,
                FirstName = DatabaseTestBase.RandomString(),
                LastName = DatabaseTestBase.RandomString(),
                EmailAddress = DatabaseTestBase.RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = positionInFamily,
            };
            return child;
        }
    }
}
