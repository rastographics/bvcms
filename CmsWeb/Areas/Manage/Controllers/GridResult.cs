using System.Data;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    public class GridResult : ActionResult
    {
        private readonly IDataReader rd;
        public GridResult(IDataReader rd)
        {
            this.rd = rd;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var t = PythonModel.HtmlTable(rd);
            t.RenderControl(new HtmlTextWriter(context.HttpContext.Response.Output));
        }
        public static string Table(IDataReader rd, string title = null, int? maxrows = null, string excellink = null)
        {
            var t = PythonModel.HtmlTable(rd, title, maxrows);
            var sb = new StringBuilder();

            if (excellink.HasValue())
            {
                var tc = new TableCell()
                {
                    ColumnSpan = rd.FieldCount,
                    Text = excellink,
                };
                var tr = new TableFooterRow();
                tr.Cells.Add(tc);
                t.Rows.Add(tr);
            }
            t.RenderControl(new HtmlTextWriter(new StringWriter(sb)));
            return sb.ToString();
        }
    }
}
