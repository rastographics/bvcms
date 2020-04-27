using CmsData;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CmsDataTests.QueryBuilder.Expression
{
    [Collection(Collections.Database)]
    public class ConditionsTests 
    {
        [Theory]        
        [InlineData("12019,Ministry12019")]
        [InlineData("1,Ministry1")]
        [InlineData("302,whatever text")]
        public void ministryint_prop_should_split_string_and_get_first_value_only(string ministry)
        {
            Condition c = new Condition();
            c.Ministry = ministry;
            c.MinistryInt.ShouldBeOfType(typeof(int));
        }
    }
}
