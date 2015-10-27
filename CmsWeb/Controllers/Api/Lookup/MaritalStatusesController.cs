using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class MaritalStatusesController : ODataController
    {
        public MaritalStatusesController()
        {
            Mapper.CreateMap<MaritalStatus, ApiLookup>();
        }

        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.MaritalStatuses.Project().To<ApiLookup>().AsQueryable());
        }
    }
}
