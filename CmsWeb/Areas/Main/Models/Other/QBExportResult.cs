using System;
using System.Collections.Generic;
using System.Xml;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
using CmsData;

namespace CmsWeb.Models
{
    public class QBExportResult : ActionResult
    {
        private CmsData.QueryBuilderClause clause;
        private XmlWriter w;
        public QBExportResult(int QueryId)
        {
            clause = DbUtil.Db.QueryBuilderClauses.Single(cc => cc.QueryId == QueryId);
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            using (w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
                clause.SendToWriter(w, "export", clause.QueryId);
        }
    }
}