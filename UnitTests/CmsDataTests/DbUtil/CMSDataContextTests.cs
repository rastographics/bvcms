using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsData.Tests
{
    [Collection(Collections.Database)]
    public class CMSDataContextTests : DatabaseTestBase
    {
        [Fact]
        public void PeopleQuery2_Parse_Test()
        {
            var query = @"PledgeBalance( FundId='10102' ) <= 0.00
    AND PledgeAmountBothJointHistory( StartDate='2/1/2016', EndDate='11/29/2018', FundIdOrNullForAll='10102' ) >= 1
    AND HasManagedGiving( FundIdOrBlank='10102' ) = 1[True]
    AND HasPeopleExtraField <> 'RiseRecurAutoStopped'
    AND CampusId = 2[West Side]";
            db.PeopleQuery2(query);
        }

        [Fact]
        public void SessionValues_Insert_Fetch_Delete()
        {
            var sessionId = RandomString();
            var name = RandomString();
            var value = RandomString();
            db.SessionValues.DeleteAllOnSubmit(
                db.SessionValues.Where(v => v.SessionId == sessionId && v.Name == name));
            db.SessionValues.FirstOrDefault(v => v.Name == name && v.SessionId == sessionId).ShouldBeNull();
            db.SessionValues.InsertOnSubmit(new SessionValue
            {
                SessionId = sessionId,
                Name = name,
                Value = value
            });
            db.SubmitChanges();

            var newdb = db.Copy();
            var sessionValue = newdb.SessionValues.FirstOrDefault(v => v.Name == name && v.SessionId == sessionId);
            sessionValue.ShouldNotBeNull();
            sessionValue.Value.ShouldBe(value);

            newdb.SessionValues.DeleteAllOnSubmit(
                newdb.SessionValues.Where(v => v.SessionId == sessionId && v.Name == name));
            newdb.SubmitChanges();

            sessionValue = newdb.SessionValues.FirstOrDefault(v => v.Name == name && v.SessionId == sessionId);
            sessionValue.ShouldBeNull();
        }

        [Fact]
        public void Should_Get_Setting()
        {
            var expected = "1";
            var setting = MockSettings.CreateSaveSetting(db, "PostContributionPledgeFunds", "1");
            var actual = db.GetSetting("PostContributionPledgeFunds", "");
            actual.ShouldBe(expected);
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

        [InlineData("PushPayKey", "keyXYZ", null, null, null)]
        [Theory]
        public void Should_Insert_EV_Only_If_does_not_Exist(string key, string value, string text, int? intvalue, bool? bitvalue)
        {
            var datevalue = DateTime.Now;
            var person = db.People.FirstOrDefault();
            int extraValId = db.AddExtraValueDataIfNotExist(person.PeopleId, key, value, datevalue, text, intvalue, bitvalue);
            db.SubmitChanges();
            int attempt2 = db.AddExtraValueDataIfNotExist(person.PeopleId, key, value, datevalue, text, intvalue, bitvalue);
            attempt2.ShouldBe(0);
            db.SubmitChanges();

            var extraValue = db.PeopleExtras.SingleOrDefault(p => p.PeopleId == person.PeopleId && p.Field == key && p.Instance == extraValId);
            extraValue.ShouldNotBe(null);

            db.PeopleExtras.DeleteOnSubmit(extraValue);
            db.SubmitChanges();
        }

        [Fact]
        public void Should_Return_Null_Transaction()
        {
            var currentDate = Util.Now;

            var randomName = RandomString();
            var randomFirst = RandomString();
            var randomLast = RandomString();

            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var t1 = new Transaction
                {
                    Name = randomName,
                    First = randomFirst,
                    MiddleInitial = "m",
                    Last = randomLast,
                    Suffix = "sufix",
                    Donate = 0,
                    Amtdue = 0,
                    Amt = 10,
                    Emails = Util.FirstAddress("email@gmail.com").Address,
                    Testing = true,
                    Description = "description",
                    OrgId = 1,
                    Url = "url",
                    Address = "address",
                    TransactionGateway = "transnational",
                    City = "city",
                    State = "state",
                    Zip = "12345",
                    DatumId = 0,
                    Phone = "1234567890",
                    OriginalId = null,
                    Financeonly = false,
                    TransactionDate = currentDate,
                    PaymentType = "C",
                    LastFourCC = "1234",
                    LastFourACH = null
                };

                var tran = db.CreateTransaction(t1, 10, 0, "transnational");
                tran.ShouldNotBeNull();
            }

            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var t2 = new Transaction
                {
                    Name = randomName,
                    First = randomFirst,
                    MiddleInitial = "m",
                    Last = randomLast,
                    Suffix = "sufix",
                    Donate = 0,
                    Amtdue = 0,
                    Amt = 10,
                    Emails = Util.FirstAddress("email@gmail.com").Address,
                    Testing = true,
                    Description = "description",
                    OrgId = 1,
                    Url = "url",
                    Address = "address",
                    TransactionGateway = "transnational",
                    City = "city",
                    State = "state",
                    Zip = "12345",
                    DatumId = 0,
                    Phone = "1234567890",
                    OriginalId = null,
                    Financeonly = false,
                    TransactionDate = currentDate.AddMinutes(1),
                    PaymentType = "C",
                    LastFourCC = "1234",
                    LastFourACH = null
                };

                var tran2 = db.CreateTransaction(t2, 10, 0, "transnational");
                tran2.ShouldBeNull();
            }
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("1337", 1)]
        [InlineData("2,4,6,", 3)]
        [InlineData("2,147,483,647", 4)]
        [InlineData("2,4,6,,8,,,10", 5)]
        [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,", 100)]
        [InlineData("0,2147483647", 2)]
        [InlineData("1999,Feast", 1)]
        public void SplitIntsTest(string list, int count)
        {
            var ints = db.SplitInts(list).ToList();
            ints.Count().ShouldBe(count);
        }

        [Fact]
        public void SetOrgLeadersOnlyTest()
        {
            var user = CreateUser(RandomString(), RandomString());
            var person = user.Person;
            var personInFamily = CreatePerson(person.Family);
            var personInOrg = CreatePerson();
            var personNotInOrg = CreatePerson();
            var org = CreateOrganization();
            CreateOrganizationMember(org.OrganizationId, personInOrg.PeopleId);
            var leader = CreateOrganizationMember(org.OrganizationId, person.PeopleId);
            leader.MemberTypeId = MemberTypeCode.Leader;
            db.SubmitChanges();

            db.CurrentUser = user;
            db.CurrentSessionId = RandomString(32);

            db.SetOrgLeadersOnly();

            var tag = db.OrgLeadersOnlyTag2();

            var everyone = tag.People(db);
            var ids = everyone.Select(p => p.PeopleId).ToArray();

            ids.ShouldContain(person.PeopleId);
            ids.ShouldContain(personInFamily.PeopleId);
            ids.ShouldContain(personInOrg.PeopleId);
            ids.ShouldNotContain(personNotInOrg.PeopleId);
        }

        [Fact]
        public void UseTicketedDbSetting_Enabled_Should_Return_TicketedEvent()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            db.ExecuteCommand("DELETE FROM Setting where id = 'UseTicketed'");
            db.ExecuteCommand("INSERT INTO Setting VALUES('UseTicketed', 'true', NULL)");
            var codes = RegistrationTypeCode.GetCodePairs(db);

            var ticketEventCode = codes.Where(x => x.Key.Equals(RegistrationTypeCode.TicketedEvent)).ToList();
            ticketEventCode.Count.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void UseTicketedDbSetting_Disabled_Should_Not_Return_TicketedEvent()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            db.ExecuteCommand("DELETE FROM Setting where id = 'UseTicketed'");
            db.ExecuteCommand("INSERT INTO Setting VALUES('UseTicketed', 'false', NULL)");

            var codes = RegistrationTypeCode.GetCodePairs(db);

            var ticketEventCode = codes.Where(x => x.Key.Equals(RegistrationTypeCode.TicketedEvent)).ToList();
            ticketEventCode.Count.ShouldBeEquivalentTo(0);
        }
        
        [Fact]
        public void NextSecurityCodeTest()
        {
            db.NextSecurityCode().Count().ShouldBe(1);
        }

        [Fact]
        public void NextSecurityCode_Uniqueness_Test()
        {
            var code1 = db.NextSecurityCode().Single();
            var code2 = db.NextSecurityCode().Single();

            code1.Code.ShouldNotBe(code2.Code);
        }
    }
}
