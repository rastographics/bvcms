using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using CmsWeb.Models.Api;
using UtilityExtensions;

namespace CmsWeb.Controllers.Api
{
    [ApiWriteAuthorize]
    public class PostController : ApiController
    {
        [HttpPost, Route("~/API/AddContribution/")]
        public HttpResponseMessage AddContribution([FromBody] AddContributionModel m)
        {
            try
            {
                if (m == null)
                    throw new Exception("Missing data");
                var result = m.Add(DbUtil.Db);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Error = ex.Message });
            }
        }
        [HttpPost, Route("~/API/ReverseContribution/{id}/{source}")]
        public HttpResponseMessage ReverseContribution(int id, string source)
        {
            try
            {
                var c = DbUtil.Db.Contributions.SingleOrDefault(ic => ic.ContributionId == id);
                if (c == null)
                    throw new Exception($"Contribution not found: {id}");
                if(c.ContributionStatusId == ContributionStatusCode.Reversed)
                    throw new Exception($"Contribution already reversed: {id}");
                var r = ContributionSearchModel.CreateContributionRecord(c);
                c.ContributionStatusId = ContributionStatusCode.Reversed;
                r.ContributionTypeId = ContributionTypeCode.Reversed;
                r.ContributionDesc = "Reversed Contribution from API: Id = " + c.ContributionId;
                DbUtil.Db.Contributions.InsertOnSubmit(r);
                DbUtil.Db.SubmitChanges();
                var result = new AddContributionModel.Result();
                result.ContributionId = r.ContributionId;
                if(r.PeopleId.HasValue)
                    result.PeopleId = r.PeopleId.Value;
                result.Source = $"API Reverse (source={source})";

                var oid = CmsData.API.APIContribution.OneTimeGiftOrgId(DbUtil.Db);
                DbUtil.Db.SendEmail(Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(oid ?? 0)),
                $"API Contribution Reversal {source}",
                $@"<table>
<tr><td>Name</td><td>{c.Person?.Name ?? "unknown"}</td></tr>
<tr><td>ContributionId</td><td>{c.ContributionId}</td></tr>
<tr><td>Amount</td><td>{c.ContributionAmount:N2}</td></tr>
<tr><td>Date</td><td>{DateTime.Now.FormatDateTm()}</td></tr>
</table>", Util.EmailAddressListFromString(DbUtil.Db.StaffEmailForOrg(oid ?? 0)));
            DbUtil.LogActivity("API Reversal for " + c.ContributionId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Error = ex.Message });
            }
        }
    }
}
