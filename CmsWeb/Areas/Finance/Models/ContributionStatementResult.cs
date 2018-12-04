/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using CmsData.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
        public string statementType { get; set; }

        public ContributionStatementResult()
        {
            useMinAmt = true;
            noaddressok = !DbUtil.Db.Setting("RequireAddressOnStatement", true);

            showCheckNo = DbUtil.Db.Setting("RequireCheckNoOnStatement");
            showNotes = DbUtil.Db.Setting("RequireNotesOnStatement");
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/pdf";
            response.AddHeader("content-disposition", "filename=foo.pdf");
            var cs = ContributionStatements.GetStatementSpecification(DbUtil.Db, statementType ?? "all");

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
                        q = APIContribution.Contributors(DbUtil.Db, FromDate, ToDate, PeopleId, SpouseId, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 2:
                        FamilyId = DbUtil.Db.People.Single(p => p.PeopleId == PeopleId).FamilyId;
                        q = APIContribution.Contributors(DbUtil.Db, FromDate, ToDate, 0, 0, FamilyId, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 3:
                        q = APIContribution.Contributors(DbUtil.Db, FromDate, ToDate, 0, 0, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                }
                c.Run(response.OutputStream, DbUtil.Db, q, cs);
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
                        q = APIContribution.Contributors(DbUtil.Db, FromDate, ToDate, PeopleId, SpouseId, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 2:
                        FamilyId = DbUtil.Db.People.Single(p => p.PeopleId == PeopleId).FamilyId;
                        q = APIContribution.Contributors(DbUtil.Db, FromDate, ToDate, 0, 0, FamilyId, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 3:
                        q = APIContribution.Contributors(DbUtil.Db, FromDate, ToDate, 0, 0, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                }
                c.Run(response.OutputStream, DbUtil.Db, q, cs);
            }
        }
    }
}

