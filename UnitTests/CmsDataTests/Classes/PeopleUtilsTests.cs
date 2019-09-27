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
                new object[] { GenerateChildrenWithParents(10), 20 },
                new object[] { GeneratePeople(10, 5, 3), 28 }
            };

        public static IEnumerable<object[]> Data_GetPrimarySecundaryIdsTest =>
            new List<object[]>
            {
                new object[] { GeneratePeople(10, 5, 3), 8 }
            };

        private static IQueryable<Person> GeneratePeople(int numberOfChilden, int numberOfPrimary, int numberOfSecondary)
        {
            var people = GenerateChildrenWithParents(numberOfChilden).ToList();
            var primary = GenerateAdults(numberOfPrimary, PositionInFamily.PrimaryAdult);
            var secondary = GenerateAdults(numberOfSecondary, PositionInFamily.SecondaryAdult);
            people.AddRange(primary);
            people.AddRange(secondary);
            return people.AsQueryable();
        }

        private static List<Person> GenerateAdults(int numberOfAdults, int positionInFamily)
        {
            var adults = new List<Person>();
            for (int i = 0; i < numberOfAdults; i++)
            {
                adults.Add(CreatePerson(positionInFamily));
            }
            return adults;
        }

        public static IQueryable<Person> GenerateChildrenWithParents(int numberOfChildren)
        {
            List<Person> children = new List<Person>();
            for (int i = 0; i < numberOfChildren; i++)
            {
                var child = CreatePerson(PositionInFamily.Child);
                children.Add(child);
                var father = CreatePerson(PositionInFamily.PrimaryAdult, child.Family);
                father.Family.HeadOfHousehold = father;
                children.Add(father);
                var mother = CreatePerson(PositionInFamily.PrimaryAdult, child.Family);
                mother.Family.HeadOfHouseholdSpouseId = mother.PeopleId;
                children.Add(mother);
            }
            return children.AsQueryable();
        }

        public static Person CreatePerson(int positionInFamily, Family family = null)
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
