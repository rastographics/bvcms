using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/Contributions/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Contributions(int id, int? page, int? size, string sort, string dir)
        {
            var m = new ContributionsModel(id);
            m.Pager.Set("/Person2/Contributions/" + id, page, size, sort, dir);
            return View("Giving/Contributions", m);
        }
        public ActionResult ContributionStatement(int id, string fr, string to)
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
            };
        }
    }
}
