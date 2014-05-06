using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Models
{
    public class StatusFlagsExcelResult : ActionResult
    {
        private string flags;
        private Guid qid;
        public StatusFlagsExcelResult(Guid qid, string flags)
        {
            this.flags = flags;
            this.qid = qid;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=CMSStatusFlags.xls");
            Response.Charset = "";

            var collist = from ss in DbUtil.Db.ViewStatusFlagNamesRoles.ToList()
                          where ss.Role == null || HttpContext.Current.User.IsInRole(ss.Role)
                          select ss;

            IEnumerable<string> cq = null;

            if (flags.HasValue())
                cq = from f in flags.Split(',')
                     join c in collist on f equals c.Flag
                     select "\tss.{0} as [{0}_{1}]".Fmt(c.Flag, c.Name);
            else
                cq = from c in collist
                     where c.Role == null || HttpContext.Current.User.IsInRole(c.Role)
                     select "\tss.{0} as [{1}]".Fmt(c.Flag, c.Name);

            var tag = DbUtil.Db.PopulateSpecialTag(qid, DbUtil.TagTypeId_StatusFlags);
            var cols = string.Join(",\n", cq);
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var cmd = new SqlCommand(@"
SELECT 
    md.PeopleId, 
	md.[First],
	md.[Last],
	md.Age,
	md.Marital,
	md.Decision,
	md.DecisionDt,
    md.JoinDt,
	md.Baptism,
" + cols + @"
FROM StatusFlagColumns ss
JOIN MemberData md ON md.PeopleId = ss.PeopleId
JOIN dbo.TagPerson tp ON tp.PeopleId = md.PeopleId
WHERE tp.Id = @p1", cn);
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            var rd = cmd.ExecuteReader();
            var dg = new DataGrid();
            dg.DataSource = rd;
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}