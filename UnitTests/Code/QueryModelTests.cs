using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using Moq;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace UnitTests
{
    [Collection("Database collection")]
    public class QueryModelTests
    {
        [Fact]
        public void Should_be_able_to_parse_and_execute_querycode()
        {
            const string code = "PeopleId < 4";
            var db = DbUtil.Create(Util.Host);
            var m = QueryModel.QueryCode(db, code);
            m.Count().ShouldBe(3);
            db.Dispose();
        }
    }
}
