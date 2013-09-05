using System.Collections.Generic;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class CsvResult : ActionResult
    {
        public IEnumerable<MailingController.MailingInfo> Data { get; set; }
        public string FileName { get; set; }

        public CsvResult(IEnumerable<MailingController.MailingInfo> data, string file = "cmspeople.csv")
        {
            Data = data;
            FileName = file;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var r = context.HttpContext.Response;
            r.Clear();
            r.ContentType = "text/plain";
            r.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            r.Charset = "";
            foreach (var mi in Data)
                r.Write(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",{5},{6}\r\n",
                    mi.LabelName, mi.Address, mi.Address2, mi.City, mi.State, mi.Zip.FmtZip(), mi.PeopleId));
        }
    }
}