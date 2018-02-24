/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
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
