using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api.Lookup;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class BundleHeaderTypesController : ODataController
    {
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<BundleHeaderType, ApiLookup>();
            });
            return Ok(CurrentDatabase.BundleHeaderTypes.ProjectTo<ApiLookup>(config));
        }
    }
}
