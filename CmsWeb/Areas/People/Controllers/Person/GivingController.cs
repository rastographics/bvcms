using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Models.Report;
using CmsWeb.Areas.People.Models;
using Dapper;
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
            if (!DbUtil.Db.CurrentUserPerson.CanViewStatementFor(DbUtil.Db, m.PeopleId))
                return Content("No permission to view statement");
            return View("Giving/Statements", m);
        }

        public ActionResult Statement(int id, string fr, string to)
        {
            if (!DbUtil.Db.CurrentUserPerson.CanViewStatementFor(DbUtil.Db, id))
                return Content("No permission to view statement");
            var p = DbUtil.Db.LoadPersonById(id);
            if (p == null)
                return Content("Invalid Id");

            var frdt = Util.ParseMMddyy(fr);
            var todt = Util.ParseMMddyy(to);
            if (!(frdt.HasValue && todt.HasValue))
                return Content("date formats invalid");

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
            if (!DbUtil.Db.CurrentUserPerson.CanViewStatementFor(DbUtil.Db, id))
                return Content("No permission to view statement");
            var p = DbUtil.Db.LoadPersonById(id);
            if (p == null)
                return Content("Invalid Id");

            if (p.PeopleId == p.Family.HeadOfHouseholdSpouseId)
            {
                var hh = DbUtil.Db.LoadPersonById(p.Family.HeadOfHouseholdId ?? 0);
                if ((hh.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint
                    && (p.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                    p = p.Family.HeadOfHousehold;
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
            var org = (from o in DbUtil.Db.Organizations
                       where o.RegistrationTypeId == RegistrationTypeCode.ManageGiving
                       select o.OrganizationId).FirstOrDefault();
            if (org > 0)
                return Redirect("/OnlineReg/" + org);
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult OneTimeGift()
        {
            var sql = @"
SELECT OrganizationId FROM dbo.Organizations
WHERE RegistrationTypeId = 8
AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";
            var oid = DbUtil.Db.Connection.ExecuteScalar(sql) as int?;
            if (oid > 0)
                return Redirect("/OnlineReg/" + oid);
            return new EmptyResult();
        }
    }
}
