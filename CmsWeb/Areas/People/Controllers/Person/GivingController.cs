using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Models.Report;
using CmsWeb.Areas.People.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult Contributions(ContributionsModel m)
        {
            return View("Giving/Contributions", m);
        }

        [HttpPost]
        public ActionResult Statements(ContributionsModel m)
        {
            if (!CurrentDatabase.CurrentUserPerson.CanViewStatementFor(CurrentDatabase, m.PeopleId))
            {
                return Content("No permission to view statement");
            }

            var hasCustomStatementsXml = CurrentDatabase.Content("CustomStatements", "") != string.Empty;
            var hasStandardFundLabel = CurrentDatabase.Setting("StandardFundSetName", string.Empty) != string.Empty;
            var hasContributionFundStatementsEnabled = CurrentDatabase.Setting("EnableContributionFundsOnStatementDisplay", false);

            var useNewStatementView = hasCustomStatementsXml && hasStandardFundLabel && hasContributionFundStatementsEnabled;

            return View(useNewStatementView ? "Giving/StatementsWithFund" : "Giving/Statements", m);
        }

        public ActionResult Statement(int id, string fr, string to)
        {
            if (!CurrentDatabase.CurrentUserPerson.CanViewStatementFor(CurrentDatabase, id))
            {
                return Content("No permission to view statement");
            }

            var p = CurrentDatabase.LoadPersonById(id);
            if (p == null)
            {
                return Content("Invalid Id");
            }

            var frdt = Util.ParseMMddyy(fr);
            var todt = Util.ParseMMddyy(to);
            if (!(frdt.HasValue && todt.HasValue))
            {
                return Content("date formats invalid");
            }

            DbUtil.LogPersonActivity($"Contribution Statement for ({id})", id, p.Name);

            return new ContributionStatementResult
            {
                PeopleId = id,
                FromDate = frdt.Value,
                ToDate = todt.Value,
                typ = p.PositionInFamilyId == PositionInFamily.PrimaryAdult ? 2 : 1,
                noaddressok = true,
                useMinAmt = false,
                singleStatement = true
            };
        }

        // the datetime arguments come across as sortable dates to make them universal for all cultures
        [HttpGet, Route("ContributionStatement/{id:int}/{fr:datetime}/{to:datetime}")]
        public ActionResult ContributionStatement(int id, DateTime fr, DateTime to, string custom = null)
        {
            if (!CurrentDatabase.CurrentUserPerson.CanViewStatementFor(CurrentDatabase, id))
            {
                return Content("No permission to view statement");
            }

            var p = CurrentDatabase.LoadPersonById(id);
            if (p == null)
            {
                return Content("Invalid Id");
            }

            if (p.PeopleId == p.Family.HeadOfHouseholdSpouseId)
            {
                var hh = CurrentDatabase.LoadPersonById(p.Family.HeadOfHouseholdId ?? 0);
                if ((hh.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint
                    && (p.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                {
                    p = p.Family.HeadOfHousehold;
                }
            }

            DbUtil.LogPersonActivity($"Contribution Statement for ({id})", id, p.Name);

            return new ContributionStatementResult
            {
                PeopleId = p.PeopleId,
                FromDate = fr,
                ToDate = to,
                typ = p.PositionInFamilyId == PositionInFamily.PrimaryAdult
                      && (p.ContributionOptionsId ?? (p.SpouseId > 0
                          ? StatementOptionCode.Joint
                          : StatementOptionCode.Individual))
                      == StatementOptionCode.Joint ? 2 : 1,
                noaddressok = true,
                useMinAmt = false,
                singleStatement = true,
                statementType = custom
            };
        }

        [HttpGet]
        public ActionResult ManageGiving()
        {
            var org = (from o in CurrentDatabase.Organizations
                       where o.RegistrationTypeId == RegistrationTypeCode.ManageGiving
                       select o.OrganizationId).FirstOrDefault();
            if (org > 0)
            {
                return Redirect("/OnlineReg/" + org);
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult OneTimeGift(int? id)
        {
            // check for one time gift campus route mapping.
            if (id.HasValue)
            {
                var setting = $"OneTimeGiftCampusRoute-{id}";
                var route = CurrentDatabase.GetSetting(setting, string.Empty);
                if (!string.IsNullOrWhiteSpace(route))
                {
                    return Redirect($"/{route}");
                }
            }
            
            var oid = CmsData.API.APIContribution.OneTimeGiftOrgId(CurrentDatabase);
            if (oid > 0)
            {
                return Redirect("/OnlineReg/" + oid);
            }

            return new EmptyResult();
        }

    }
}
