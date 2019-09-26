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

        public static IEnumerable<object[]> Data_GetParentsIdsTest =>
            new List<object[]>
            {
                new object[] { GenerateChildrenWithParents(10), 10 }
            };

        public static IQueryable<Person> GenerateChildrenWithParents(int numberOfChilds)
        {
            List<Person> children = new List<Person>();
            for (int i = 0; i < numberOfChilds; i++)
            {
                var child = CreateChild();
                children.Add(child);
                var father = CreateParent(child);
                father.Family.HeadOfHousehold = father;
                children.Add(father);
                var mother = CreateParent(child);
                mother.Family.HeadOfHouseholdSpouseId = mother.PeopleId;
                children.Add(mother);
            }
            return children.AsQueryable();
        }

        private static Person CreateParent(Person child)
        {
            return new Person
            {
                Family = child.Family,
                FirstName = DatabaseTestBase.RandomString(),
                LastName = DatabaseTestBase.RandomString(),
                EmailAddress = DatabaseTestBase.RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.PrimaryAdult
            };        
        }

        public static Person CreateChild(Family family = null)
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
                PositionInFamilyId = PositionInFamily.Child,
            };
            return child;
        }
    }
}
