using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class GendersController : ODataController
    {
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Gender, ApiLookup>();
            });
            return Ok(DbUtil.Db.Genders.ProjectTo<ApiLookup>(config));
        }
    }
}
