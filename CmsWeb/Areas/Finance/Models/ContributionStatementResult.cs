/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData.API;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections;
using CmsData;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Diagnostics;
using iTextSharp.text.html.simpleparser;
using CmsWeb.Areas.Reports.Models;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ContributionStatementResult : ActionResult
    {
        public int FamilyId { get; set; }
        public int PeopleId { get; set; }
        public int? SpouseId { get; set; }
        public int typ { get; set; }
		public bool useMinAmt { get; set; }
		public bool noaddressok { get; set; }
        public bool showCheckNo { get; set; }
        public bool showNotes { get; set; }
        public bool singleStatement { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

		public ContributionStatementResult()
		{
			useMinAmt = true;
		    noaddressok = DbUtil.Db.Setting("RequireAddressOnStatement", true);

		    showCheckNo = DbUtil.Db.Setting("RequireCheckNoOnStatement", false);
		    showNotes = DbUtil.Db.Setting("RequireNotesOnStatement", false);
		}

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "filename=foo.pdf");

            if (showCheckNo || showNotes)
            {
                var c = new ContributionStatementsExtra
                {
                    FamilyId = FamilyId,
                    FromDate = FromDate,
                    PeopleId = PeopleId,
                    SpouseId = SpouseId,
                    ToDate = ToDate,
                    typ = typ,
                    ShowCheckNo = showCheckNo,
                    ShowNotes = showNotes
                };

                IEnumerable<ContributorInfo> q = null;
                switch (typ)
                {
                    case 1:
                        q = APIContribution.contributors(DbUtil.Db, FromDate, ToDate, PeopleId, SpouseId, 0, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 2:
                        FamilyId = DbUtil.Db.People.Single(p => p.PeopleId == PeopleId).FamilyId;
                        q = APIContribution.contributors(DbUtil.Db, FromDate, ToDate, 0, 0, FamilyId, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 3:
                        q = APIContribution.contributors(DbUtil.Db, FromDate, ToDate, 0, 0, 0, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                }
                c.Run(Response.OutputStream, DbUtil.Db, q);
            }
            else
            {
                var c = new ContributionStatements
                {
                    FamilyId = FamilyId,
                    FromDate = FromDate,
                    PeopleId = PeopleId,
                    SpouseId = SpouseId,
                    ToDate = ToDate,
                    typ = typ
                };

                IEnumerable<ContributorInfo> q = null;
                switch (typ)
                {
                    case 1:
                        q = APIContribution.contributors(DbUtil.Db, FromDate, ToDate, PeopleId, SpouseId, 0, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 2:
                        FamilyId = DbUtil.Db.People.Single(p => p.PeopleId == PeopleId).FamilyId;
                        q = APIContribution.contributors(DbUtil.Db, FromDate, ToDate, 0, 0, FamilyId, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 3:
                        q = APIContribution.contributors(DbUtil.Db, FromDate, ToDate, 0, 0, 0, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                }
                c.Run(Response.OutputStream, DbUtil.Db, q);
            }
        }
    }
}

