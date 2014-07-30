using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost, Route("Contributions/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Contributions(int id, int? page, int? size, string sort, string dir)
        {
            var m = new ContributionsModel(id);
            m.Pager.Set("/Person2/Contributions/" + id, page, size, sort, dir);
            return View("Giving/Contributions", m);
        }
        [HttpPost]
        public ActionResult Statements(int id)
        {
            if(!DbUtil.Db.CurrentUserPerson.CanViewStatementFor(DbUtil.Db, id))
                return Content("No permission to view statement");
            var m = new ContributionsModel(id);
            return View("Giving/Statements", m);
        }
        public ActionResult Statement(int id, string fr, string to)
        {
            if (Util.UserPeopleId != id && !User.IsInRole("Finance"))
                return Content("No permission to view statement");
            var p = DbUtil.Db.LoadPersonById(id);
            if (p == null)
                return Content("Invalid Id");

            var frdt = Util.ParseMMddyy(fr);
            var todt = Util.ParseMMddyy(to);
            if (!(frdt.HasValue && todt.HasValue))
                return Content("date formats invalid");

            DbUtil.LogActivity("Contribution Statement for ({0})".Fmt(id));

            return new Finance.Models.Report.ContributionStatementResult
            {
                PeopleId = id,
                FromDate = frdt.Value,
                ToDate = todt.Value,
                typ = p.PositionInFamilyId == PositionInFamily.PrimaryAdult ? 2 : 1,
                noaddressok = true,
                useMinAmt = false,
                singleStatement = true,
            };
        }
        // the datetime arguments come across as sortable dates to make them universal for all cultures
        [HttpGet, Route("ContributionStatement/{id:int}/{fr:datetime}/{to:datetime}")]
		public ActionResult ContributionStatement(int id, DateTime fr, DateTime to)
		{
            if(!DbUtil.Db.CurrentUserPerson.CanViewStatementFor(DbUtil.Db, id))
				return Content("No permission to view statement");
			var p = DbUtil.Db.LoadPersonById(id);
			if (p == null)
				return Content("Invalid Id");

			DbUtil.LogActivity("Contribution Statement for ({0})".Fmt(id));

			return new Finance.Models.Report.ContributionStatementResult 
			{ 
				PeopleId = id, 
				FromDate = fr,
				ToDate = to,
				typ = p.PositionInFamilyId == PositionInFamily.PrimaryAdult && p.ContributionOptionsId == StatementOptionCode.Joint ? 2 : 1,
				noaddressok = true,
				useMinAmt = false,
                singleStatement = true,
			};
		}

        [HttpGet]
        public ActionResult ManageGiving()
        {
            int org = (from o in DbUtil.Db.Organizations
                where o.RegistrationTypeId == RegistrationTypeCode.ManageGiving
                select o.OrganizationId).FirstOrDefault();
            if (org > 0)
                return Redirect("/OnlineReg/" + org);
            return new EmptyResult();
        }
        [HttpGet]
        public ActionResult OneTimeGift()
        {
            int org = (from o in DbUtil.Db.Organizations
                where o.RegistrationTypeId == RegistrationTypeCode.OnlineGiving
                select o.OrganizationId).FirstOrDefault();
            if (org > 0)
                return Redirect("/OnlineReg/" + org);
            return new EmptyResult();
        }
    }
}
