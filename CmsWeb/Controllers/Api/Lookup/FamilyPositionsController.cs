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
        public FamilyPositionsController()
        {
            Mapper.CreateMap<FamilyPosition, ApiLookup>();
        }

        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.FamilyPositions.Project().To<ApiLookup>().AsQueryable());
        }
    }
}
