using System.Web.Mvc;
using System.Linq;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Models
{
    public class TaskNotesExcelResult : ActionResult
    {
        object qid;
        public TaskNotesExcelResult(object id)
        {
            qid = id;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=CMSTasks.xls");
            Response.Charset = "";

            var q = DbUtil.Db.PeopleQuery(qid);
            var q2 = from p in q
                     let t = p.TasksAboutPerson.OrderByDescending(t => t.CreatedOn).FirstOrDefault(t => t.Notes != null)
                     where t != null
                     select new
                     {
                         p.Name,
                         t.Notes,
                         t.CreatedOn
                     };
            var dg = new DataGrid();
            dg.DataSource = q2;
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}