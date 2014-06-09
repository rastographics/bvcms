using System;
using System.Collections.Generic;
using System.Xml;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Models
{
    public class OrgsMembersExcelResult : ActionResult
    {
        private OrgSearchModel m;
        public OrgsMembersExcelResult(OrgSearchModel m)
        {
            this.m = m;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=CMSOrganizations.xls");
            Response.Charset = "";
            var d = m.OrgsMemberList();
            var dg = new DataGrid();
            dg.DataSource = d;
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}