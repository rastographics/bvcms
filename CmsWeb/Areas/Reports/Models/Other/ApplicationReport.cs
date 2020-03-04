using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsData.View;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class ApplicationReport : ActionResult
    {
        private int orgId;
        private int peopleId;
        private string content;

        public ApplicationReport(int orgid, int peopleid, string content)
        {
            orgId = orgid;
            peopleId = peopleid;
            this.content = content;
        }

        public override void ExecuteResult(ControllerContext context)
        {

        }
    }
}
