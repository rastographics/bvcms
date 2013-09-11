using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;

namespace CmsData
{
    public partial class Query
    {
        public Condition ToClause()
        {
            var c = Condition.Import(Text);
            return c;
        }
    }
}
