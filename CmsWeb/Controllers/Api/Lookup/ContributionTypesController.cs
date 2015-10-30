using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class ContributionTypesController : ODataController
    {
        public ContributionTypesController()
        {
            Mapper.CreateMap<ContributionType, ApiLookup>();
        }

        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.ContributionTypes.Project().To<ApiLookup>().AsQueryable());
        }
    }
}
