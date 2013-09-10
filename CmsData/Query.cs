using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;

namespace CmsData
{
    public partial class Query
    {
        public QueryBuilderClause2 ToClause()
        {
            var c = QueryBuilderClause2.Import(Text);
            return c;
        }
    }
}
