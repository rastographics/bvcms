using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class ContributionTypesController : ODataController
    {
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ContributionType, ApiLookup>();
            });
            return Ok(DbUtil.Db.ContributionTypes.ProjectTo<ApiLookup>(config));
        }
    }
}
