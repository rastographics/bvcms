using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel
    {
        public void UpdateCondition()
        {
            var c = Current;
            this.CopyPropertiesTo(c);
            TopClause.Save(Db, increment: true);
        }
        public void EditCondition()
        {
            var c = Current;
            SelectedId = c.Id;
            this.CopyPropertiesFrom(c);
        }
    }
}