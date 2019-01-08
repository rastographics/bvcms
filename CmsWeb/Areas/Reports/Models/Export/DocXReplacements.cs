using CmsData;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UtilityExtensions;
using Xceed.Words.NET;

namespace CmsWeb.Models
{
    public class DocXReplacements : ActionResult
    {
        private readonly Guid guid;
        private readonly int? peopleId;
        private readonly string template;
        private readonly string filename;
        public DocXReplacements(Guid id, string filename, string template)
        {
            this.template = template;
            this.filename = filename;
            guid = id;
        }
        public DocXReplacements(int id, string filename, string template)
        {
            this.template = template;
            this.filename = filename;
            peopleId = id;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var wc = new WebClient();
            var bytes = wc.DownloadData(template);
            var ms = new MemoryStream(bytes);
            var doctemplate = DocX.Load(ms);
            var replacements = new EmailReplacements(DbUtil.Db, doctemplate);
            var q = peopleId.HasValue
                ? DbUtil.Db.PeopleQuery2(peopleId)
                : DbUtil.Db.PeopleQuery(guid);
            if (!q.Any())
            {
                throw new Exception("no people in query");
            }

            var finaldoc = replacements.DocXReplacements(q.First());
            foreach (var p in q.Skip(1))
            {
                finaldoc.InsertParagraph().InsertPageBreakAfterSelf();
                var doc = replacements.DocXReplacements(p);
                finaldoc.InsertDocument(doc);
            }
            var response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            response.AddHeader("content-disposition", $"attachment;filename={filename}-{DateTime.Now.ToSortableDateTime()}.docx");

            ms = new MemoryStream();
            finaldoc.SaveAs(ms);
            ms.WriteTo(response.OutputStream);
            response.End();
        }
    }
}
