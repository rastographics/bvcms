using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class CampusesController : ODataController
    {
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Campu, ApiLookup>();
            });
            return Ok(DbUtil.Db.Campus.ProjectTo<ApiLookup>(config));
        }
    }
}
