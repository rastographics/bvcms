using CmsData;
using CmsData.API;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIContributionController : CmsController
    {
        public APIContributionController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost]
        public ActionResult PostContribution(int PeopleId, decimal Amount, string desc, int FundId, string date, int? contributiontype, string checkno)
        {
            var ret = AuthenticateDeveloper(addrole: "Finance");
            if (ret.StartsWith("!"))
            {
                return Content(@"<PostContribution status=""error"">" + ret.Substring(1) + "</PostContribution>");
            }

            DbUtil.LogActivity("APIContribution PostContribution");
            return Content(new APIContribution(CurrentDatabase).PostContribution(PeopleId, Amount, FundId, desc, date, contributiontype, checkno), "text/xml");
        }

        [HttpGet]
        public ActionResult Contributions(int id, int Year)
        {
            var ret = AuthenticateDeveloper(addrole: "Finance");
            if (ret.StartsWith("!"))
            {
                return Content(@"<Contributions status=""error"">" + ret.Substring(1) + "</Contributions>");
            }

            DbUtil.LogActivity($"APIContribution Contributions for ({id})");
            return Content(new APIContribution(CurrentDatabase).Contributions(id, Year), "text/xml");
        }

        [HttpPost]
        public ActionResult ContributionSearch(ContributionSearchInfo m, int? page)
        {
            var ret = AuthenticateDeveloper(addrole: "Finance");
            if (ret.StartsWith("!"))
            {
                return Content(@"<Contributions status=""error"">" + ret.Substring(1) + "</Contributions>");
            }

            DbUtil.LogActivity("APIContribution ContributionSearch");
            return Content(new APIContributionSearchModel(CurrentDatabase, m).ContributionsXML(((page ?? 1) - 1) * 100, 100), "text/xml");
        }
    }
}
