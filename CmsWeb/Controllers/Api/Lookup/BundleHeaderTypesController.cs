using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models.Api.Lookup;
using System.Web.Http;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class BundleHeaderTypesController : CMSBaseODataController
    {
        public BundleHeaderTypesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BundleHeaderType, ApiLookup>();
            });
            return Ok(CurrentDatabase.BundleHeaderTypes.ProjectTo<ApiLookup>(config));
        }
    }
}
