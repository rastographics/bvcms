using System.Web.Mvc;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ContributionStatementsResult : ActionResult
    {
        public string outputfile { get; set; }

        public ContributionStatementsResult(string file)
        {
            outputfile = file;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            if (outputfile.EndsWith(".pdf"))
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=statements.pdf");
            }
            else
            {
                Response.ContentType = "text/plain";
                Response.AddHeader("content-disposition", "attachment;filename=statements.txt");
            }
            Response.TransmitFile(outputfile);
        }
    }
}

