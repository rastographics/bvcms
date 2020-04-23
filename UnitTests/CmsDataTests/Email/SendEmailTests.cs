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
        [Fact]
        public void Send_Email_Without_From_Address_Does_Not_Throw_Exception()
        {
            bool success = true;

            try
            {
                var from = Util.TryGetMailAddress("");
                var person = db.People.Where(p => p.PeopleId == 1).FirstOrDefault();

                db.EmailFinanceInformation(from, person, null, "This is a test", "This is a test");
            }
            catch
            {
                success = false;
            }

            Assert.True(success == true);
        }
    }
}
