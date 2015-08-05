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
            Mapper.CreateMap<FamilyPosition, ApiFamilyPosition>();
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.FamilyPositions.Project().To<ApiFamilyPosition>().AsQueryable());
        }
    }
}
