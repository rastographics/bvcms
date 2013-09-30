using System.Data.SqlClient;
using System.Web.Mvc;
using UtilityExtensions;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Models
{
    public class StatusFlagsExcelResult : ActionResult
    {
        private object qid;
        public StatusFlagsExcelResult(object qid)
        {
            this.qid = qid;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=CMSStatusFlags.xls");
            Response.Charset = "";

            var tag = DbUtil.Db.PopulateSpecialTag(qid, DbUtil.TagTypeId_StatusFlags);

            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);

            var cmd = new SqlCommand("dbo.StatusGrid @p1");
			cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();

            var dg = new DataGrid();
            dg.DataSource = cmd.ExecuteReader();
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}