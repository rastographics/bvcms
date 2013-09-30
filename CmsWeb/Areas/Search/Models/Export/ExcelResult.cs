using System.Collections;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CmsWeb.Areas.Search.Models
{
    public class ExcelResult : ActionResult
    {
        public IEnumerable Data { get; set; }
        public string FileName { get; set; }

        public ExcelResult(IEnumerable data, string file = "cmspeople.xls" )
        {
            Data = data;
            FileName = file;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var r = context.HttpContext.Response;
            r.Clear();
            r.ContentType = "application/vnd.ms-excel";
            if (!string.IsNullOrEmpty(FileName))
                r.AddHeader("content-disposition",
                    "attachment;filename=" + FileName);
            const string header = 
@"<html xmlns:x=""urn:schemas-microsoft-com:office:excel"">
<head>
	<meta http-equiv=Content-Type content=""text/html; charset=utf-8""> 
    <style>
    <!--table
    br {mso-data-placement:same-cell;}
    tr {vertical-align:top;}
    -->
    </style>
</head>
<body>";
            r.Write(header);
            r.Charset = "";

            var dg = new DataGrid();
            dg.EnableViewState = false;
            dg.DataSource = Data;
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(r.Output));
            r.Write("</body></HTML>");
        }
    }
}