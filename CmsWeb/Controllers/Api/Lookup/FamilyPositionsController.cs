using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class FamilyPositionsController : ODataController
    {
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<FamilyPosition, ApiLookup>();
            });
            return Ok(DbUtil.Db.FamilyPositions.ProjectTo<ApiLookup>(config));
        }
    }
}
