using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
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
    }
}
