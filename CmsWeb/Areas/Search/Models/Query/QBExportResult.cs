using System.Xml;
using System.Web.Mvc;
using UtilityExtensions;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Search.Models
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
            {
                w.WriteStartElement("Search");
                WriteClause(clause);
                w.WriteEndElement();
            }
        }
        private void WriteClause(QueryBuilderClause clause)
        {
            w.WriteStartElement("Condition");
            w.WriteAttributeString("ClauseOrder", clause.ClauseOrder.ToString());
            w.WriteAttributeString("Field", clause.Field);
            if (clause.Description.HasValue())
                w.WriteAttributeString("Description", clause.Description);
            w.WriteAttributeString("Comparison", clause.Comparison);
            if (clause.TextValue.HasValue())
                w.WriteAttributeString("TextValue", clause.TextValue);
            if (clause.DateValue.HasValue)
                w.WriteAttributeString("DateValue", clause.DateValue.ToString());
            if (clause.CodeIdValue.HasValue())
                w.WriteAttributeString("CodeIdValue", clause.CodeIdValue);
            if (clause.StartDate.HasValue)
                w.WriteAttributeString("StartDate", clause.StartDate.ToString());
            if (clause.EndDate.HasValue)
                w.WriteAttributeString("EndDate", clause.EndDate.ToString());
            if (clause.Program > 0)
                w.WriteAttributeString("Program", clause.Program.ToString());
            if (clause.Division > 0)
                w.WriteAttributeString("Division", clause.Division.ToString());
            if (clause.Organization > 0)
                w.WriteAttributeString("Organization", clause.Organization.ToString());
            if (clause.Days > 0)
                w.WriteAttributeString("Days", clause.Days.ToString());
            if (clause.Quarters.HasValue())
                w.WriteAttributeString("Quarters", clause.Quarters);
            if (clause.Tags.HasValue())
                w.WriteAttributeString("Tags", clause.Tags);
            if (clause.Schedule > 0)
                w.WriteAttributeString("Schedule", clause.Schedule.ToString());
            if (clause.Age.HasValue)
                w.WriteAttributeString("Age", clause.Age.ToString());
            foreach (var qc in clause.Clauses)
                WriteClause(qc);
            w.WriteEndElement();
        }
    }
}