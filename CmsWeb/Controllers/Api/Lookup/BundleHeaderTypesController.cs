using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class BundleHeaderTypesController : ODataController
    {
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BundleHeaderType, ApiLookup>();
            });
            return Ok(DbUtil.Db.BundleHeaderTypes.ProjectTo<ApiLookup>(config));
        }
    }
}
