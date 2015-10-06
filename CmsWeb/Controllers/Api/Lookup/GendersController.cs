using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class GendersController : ODataController
    {
        public GendersController()
        {
            Mapper.CreateMap<Gender, ApiLookup>();
        }

        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.Genders.Project().To<ApiLookup>().AsQueryable());
        }
    }
}
