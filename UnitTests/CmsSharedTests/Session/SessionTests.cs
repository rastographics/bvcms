using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using CMSShared.Session;
using Xunit;

namespace CmsSharedTests.Session
{
    [Collection(Collections.Database)]
    public class SessionTests : DatabaseTestBase
    {
        /// <summary>
        /// If the value is missing in the session cache for a valid key,
        ///     force a retrieval of the value for that key from the database.
        /// Tests: CmsSessionProvider.cs > FetchSessionValue()
        /// </summary>
        [Fact]
        public void Should_Get_Value_If_Not_In_Cache()
        {
            CmsSessionProvider sessionProvider = new CmsSessionProvider(db);
            string sessionId = DatabaseTestBase.RandomString();
            sessionProvider.CurrentSessionId = sessionId;
            sessionProvider.Clear();
            sessionProvider.Add("TESTKEY", "TEST VALUE");

            string value = sessionProvider.Get<string>("TESTKEY");

            // test retrieval from cache
            Assert.Equal("TEST VALUE", value);

            sessionProvider.CurrentSessionId = null;
            sessionProvider.Clear(); // only clears cache
            sessionProvider.CurrentSessionId = sessionId;

            // test retrieval from DB (key/value in cache no longer exists)
            value = sessionProvider.Get<string>("TESTKEY");
            Assert.Equal("TEST VALUE", value);
        }
    }
}
