using System;
using System.Web.Mvc;
using UtilityExtensions;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Models
{
    public class ContributionsExcelResult : ActionResult
    {
        public DateTime Dt1 { get; set; }
        public DateTime Dt2 { get; set; }
        public int fundid { get; set; }
    	public int campusid { get; set; }
    	public int Online { get; set; }
        public string TaxDedNonTax { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public string type { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            bool? nontaxdeductible = null; // both
            switch (TaxDedNonTax)
            {
                case "TaxDed":
                    nontaxdeductible = false;
                    break;
                case "NonTaxDed":
                    nontaxdeductible = true;
                    break;
            }

            var Response = context.HttpContext.Response;
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            var dg = new DataGrid();
            string filename = null;
            switch (type)
            {
                case "donorfundtotals":
                    filename = "DonorFundTotals";
    				dg.DataSource = ExportPeople.ExcelDonorFundTotals(Dt1, Dt2, fundid, campusid, false, nontaxdeductible, IncUnclosedBundles);
                    break;
                case "donortotals":
                    filename = "DonorTotals";
                    dg.DataSource = ExportPeople.ExcelDonorTotals(Dt1, Dt2, campusid, false, nontaxdeductible, IncUnclosedBundles);
                    break;
                case "donordetails":
                    filename = "DonorDetails";
                    dg.DataSource = ExportPeople.DonorDetails(Dt1, Dt2, fundid, campusid, false, nontaxdeductible, IncUnclosedBundles);
                    break;
            }
            dg.DataBind();
            Response.AddHeader("Content-Disposition", "attachment;filename={0}.xls".Fmt(filename));
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}