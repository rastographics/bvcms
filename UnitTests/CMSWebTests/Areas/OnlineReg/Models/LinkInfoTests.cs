using Xunit;
using System.Collections;
using System.Net.Http;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using System.Collections.Generic;
using Shouldly;
using UtilityExtensions;
using SharedTestFixtures;
using CmsData;
using System;

namespace CMSWebTests.Areas.OnlineReg
{
    [Collection(Collections.Database)]
    public class LinkInfoTests
    {
        [Theory]
        [InlineData("SendLink", "Landing")]
        public void Should_Get_LinkInfo(string link, string from)
        {
            Guid id;
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                OneTimeLink otl = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = $"0,0,0,{"supportlink"}:1",
                    Used = false,
                    Expires = DateTime.Now.AddDays(1)
                };
                db.OneTimeLinks.InsertOnSubmit(otl);
                db.SubmitChanges();
                id = otl.Id;

                var linkInfo = new LinkInfo(db, link, from, id.ToCode());
                linkInfo.ShouldNotBeNull();
            }
        }
    }
}
