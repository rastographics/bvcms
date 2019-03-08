using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using CmsData;
using CmsWeb.Code;
using Moq;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace UnitTests
{
    [Collection("Database collection")]
    public class CodeValueModelTests 
    {
        [Fact]
        public void Should_be_able_to_get_BundleStatusTypes()
        {
            var b = CodeValueModel.BundleStatusTypes();
            b.Count().ShouldBe(2);
        }
        [Fact]
        public void Should_be_able_to_get_ContactReasonCodes()
        {
            var db = DbUtil.Create(Util.Host);
            var cv = new CodeValueModel(db);
            var b = cv.ContactReasonCodes();
            b.Count().ShouldBe(8);
            db.Dispose();
        }
        [Fact]
        public void Should_be_able_to_get_MemberTypeCodes()
        {
            var b = CodeValueModel.MemberTypeCodes();
            b.Count().ShouldBe(16);
        }
    }
}
