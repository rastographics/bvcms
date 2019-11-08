using CmsData;
using CmsData.Classes.ProtectMyMinistry;
using CmsData.Codes;
using CmsData.ExtraValue;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CmsDataTests.Classes.ExtraValues
{
    public class ViewsTests
    {
        [Fact]
        public void Should_GetViews()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var actual = Views.GetViews(db);
                actual.ShouldNotBeNull();
            }
        }

        [Fact]
        public void Should_GetStandardExtraValuesOrdered()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var actual = Views.GetStandardExtraValuesOrdered(db, "People", "Entry");
                actual.ShouldNotBeNull();
            }
        }

        //[Fact]
        //public void Should_Name_Be_Timmed_GetStandardExtraValuesOrdered()
        //{
        //    using (var db = CMSDataContext.Create(DatabaseFixture.Host))
        //    {
        //        var actual = Views.GetStandardExtraValuesOrdered(db, "People", "Entry");
        //        foreach (var item in actual)
        //        {
        //            item.Name.ShouldBe(item.Name.Trim());
        //        }
        //    }
        //}
    }
}
