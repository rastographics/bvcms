using System;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.SqlClient;

namespace CmsWeb.Models
{
    public class ExtraValueExcelResult : ActionResult
    {
        private object qid;
        public ExtraValueExcelResult(object qid)
        {
            this.qid = qid;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=CMSPeople.xls");
            Response.Charset = "";

            var name = "ExtraExcelResult " + DateTime.Now;
            var tag = DbUtil.Db.PopulateSpecialTag(qid, DbUtil.TagTypeId_ExtraValues);

            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var xml = XDocument.Parse(DbUtil.Db.Content("StandardExtraValues.xml", "<Fields/>"));
            var fields = (from ff in xml.Root.Elements("Field")
                          let vroles = ff.Attribute("VisibilityRoles")
                          where vroles != null && (vroles.Value.Split(',').All(rr => !roles.Contains(rr)))
                          select ff.Attribute("name").Value);
            var nodisplaycols = string.Join("|", fields);

            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
			cmd.Parameters.AddWithValue("@p1", tag.Id);
			cmd.Parameters.AddWithValue("@p2", "");
			cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();

            var dg = new DataGrid();
            dg.DataSource = cmd.ExecuteReader();
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}