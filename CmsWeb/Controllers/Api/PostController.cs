using CmsData;
using CmsData.Codes;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using CmsWeb.Models.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UtilityExtensions;

namespace CmsWeb.Controllers.Api
{
    [ApiWriteAuthorize]
    public class PostController : ApiController
    {
        //todo: Inheritance chain
        private readonly RequestManager RequestManager;
        private CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;

        public PostController(RequestManager requestManager)
        {
            RequestManager = requestManager;
        }

        [HttpPost, Route("~/API/AddContribution/")]
        public HttpResponseMessage AddContribution([FromBody] AddContributionModel m)
        {
            try
            {
                if (m == null)
                {
                    throw new Exception("Missing data");
                }

                var result = m.Add(CurrentDatabase);
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
                var c = CurrentDatabase.Contributions.SingleOrDefault(ic => ic.ContributionId == id);
                if (c == null)
                {
                    throw new Exception($"Contribution not found: {id}");
                }

                if (c.ContributionStatusId == ContributionStatusCode.Reversed)
                {
                    throw new Exception($"Contribution already reversed: {id}");
                }

                var r = ContributionSearchModel.CreateContributionRecord(c);
                c.ContributionStatusId = ContributionStatusCode.Reversed;
                r.ContributionTypeId = ContributionTypeCode.Reversed;
                r.ContributionDesc = "Reversed Contribution from API: Id = " + c.ContributionId;
                CurrentDatabase.Contributions.InsertOnSubmit(r);
                CurrentDatabase.SubmitChanges();
                var result = new ReverseReturn();
                result.ContributionId = r.ContributionId;
                if (r.PeopleId.HasValue)
                {
                    result.PeopleId = r.PeopleId.Value;
                }

                result.Source = $"API Reverse (source={source})";

                var oid = CmsData.API.APIContribution.OneTimeGiftOrgId(CurrentDatabase);
                CurrentDatabase.SendEmail(Util.TryGetMailAddress(CurrentDatabase.StaffEmailForOrg(oid ?? 0)),
                $"API Contribution Reversal {source}",
                $@"<table>
<tr><td>Name</td><td>{c.Person?.Name ?? "unknown"}</td></tr>
<tr><td>ContributionId</td><td>{c.ContributionId}</td></tr>
<tr><td>Amount</td><td>{c.ContributionAmount:N2}</td></tr>
<tr><td>Date</td><td>{DateTime.Now.FormatDateTm()}</td></tr>
</table>", Util.EmailAddressListFromString(CurrentDatabase.StaffEmailForOrg(oid ?? 0)));
                DbUtil.LogActivity("API Reversal for " + c.ContributionId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Error = ex.Message });
            }
        }
        public class ReverseReturn
        {
            public string Operation => "Reversal";
            public int PeopleId { get; set; }
            public int ContributionId { get; set; }
            public string Source { get; set; }
            public override string ToString()
            {
                return $"Id:{ContributionId},PeopleId:{PeopleId},Source:{Source}";
            }
        }
    }
}
