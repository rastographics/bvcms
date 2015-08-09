using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class FundsController : ODataController
    {
        public FundsController()
        {
            Mapper.CreateMap<ContributionFund, ApiContributionFund>();
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.ContributionFunds.Project().To<ApiContributionFund>().AsQueryable());
        }
    }
}
