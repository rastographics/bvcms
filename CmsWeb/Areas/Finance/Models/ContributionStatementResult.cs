using CmsData;
using CmsData.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ContributionStatementResult : ActionResult
    {
        private CMSDataContext CurrentDatabase;

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

        public ContributionStatementResult(CMSDataContext db)
        {
            CurrentDatabase = db;
            useMinAmt = true;
            noaddressok = !CurrentDatabase.Setting("RequireAddressOnStatement", true);

            showCheckNo = CurrentDatabase.Setting("RequireCheckNoOnStatement");
            showNotes = CurrentDatabase.Setting("RequireNotesOnStatement");
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/pdf";
            var filename = $"Statement-{ToDate:d}".SlugifyString("-", false);
            response.AddHeader("content-disposition", $"filename={filename}.pdf");
            var cs = ContributionStatements.GetStatementSpecification(CurrentDatabase, statementType ?? "all");

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
                        q = APIContribution.Contributors(CurrentDatabase, FromDate, ToDate, PeopleId, SpouseId, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 2:
                        FamilyId = CurrentDatabase.People.Single(p => p.PeopleId == PeopleId).FamilyId;
                        q = APIContribution.Contributors(CurrentDatabase, FromDate, ToDate, 0, 0, FamilyId, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 3:
                        q = APIContribution.Contributors(CurrentDatabase, FromDate, ToDate, 0, 0, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                }
                c.Run(response.OutputStream, CurrentDatabase, q, cs);
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
                        q = APIContribution.Contributors(CurrentDatabase, FromDate, ToDate, PeopleId, SpouseId, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 2:
                        FamilyId = CurrentDatabase.People.Single(p => p.PeopleId == PeopleId).FamilyId;
                        q = APIContribution.Contributors(CurrentDatabase, FromDate, ToDate, 0, 0, FamilyId, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                    case 3:
                        q = APIContribution.Contributors(CurrentDatabase, FromDate, ToDate, 0, 0, 0, cs.Funds, noaddressok, useMinAmt, singleStatement: singleStatement);
                        break;
                }
                c.Run(response.OutputStream, CurrentDatabase, q, cs);
            }
        }
    }
}

