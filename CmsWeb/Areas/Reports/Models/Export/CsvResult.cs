using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class CsvResult : ActionResult
    {
        public IEnumerable<MailingController.MailingInfo> Data { get; set; }
        public string FileName { get; set; }
        private bool couples;

        public CsvResult(IEnumerable<MailingController.MailingInfo> data, string file = "cmspeople.csv", bool couples = false)
        {
            this.couples = couples;
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
            var re = new Regex(@"(?<line1>.*)\n((?<line2>.*)\n){0,1}(?<city>.*),\s(?<state>[a-z]*)\s*(?<zip>.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            foreach (var mi in Data)
            {
                var label = mi.LabelName;
                if (couples && mi.CoupleName.HasValue())
                    label = mi.CoupleName;
                if (mi.MailingAddress.HasValue() && re.IsMatch(mi.MailingAddress))
                {
                    var m = re.Match(mi.MailingAddress);
                    var line1 = m.Groups["line1"].Value;
                    var line2 = m.Groups["line2"].Value;
                    var city = m.Groups["city"].Value;
                    var state = m.Groups["state"].Value;
                    var zip = m.Groups["zip"].Value;
                    r.Write($"\"{label}\",\"{line1}\",\"{line2}\",\"{city}\",\"{state}\",{zip},{mi.PeopleId}\r\n");
                }
                else
                    r.Write($"\"{label}\",\"{mi.Address}\",\"{mi.Address2}\",\"{mi.City}\",\"{mi.State}\",{mi.Zip.FmtZip()},{mi.PeopleId}\r\n");
            }
        }
    }
}
