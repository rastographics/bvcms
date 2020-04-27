using CmsData;
using CmsData.Codes;
using DocuSign.eSign.Model;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests.Email
{
    [Collection(Collections.Database)]
    public class SendEmailTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(-1, true)]
        public void Email_Finance_Information_Without_From_Address_No_Exception(int peopleId, bool expected)
        {
            bool success = true;

            try
            {
                var from = Util.TryGetMailAddress("");
                var person = db.People.Where(p => p.PeopleId == peopleId).FirstOrDefault();

                db.EmailFinanceInformation(from, person, null, "This is a test", "This is a test");
            }
            catch
            {
                success = false;
            }

            Assert.Equal(expected, success);
        }
    }
}
