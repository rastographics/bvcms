using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel 
    {
        public static QueryModel QueryCode(CMSDataContext db, string code)
        {
            var qb = db.ScratchPadCondition();
            qb.Reset();
            var c = Condition.Parse(code, qb.Id);
            c.Save(db);
            var m = new QueryModel(c.Id, db);
            return m;
        }
    }
}
