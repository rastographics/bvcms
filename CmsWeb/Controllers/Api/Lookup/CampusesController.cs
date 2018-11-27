using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models.Api.Lookup;
using System.Web.Http;

namespace CmsWeb.Controllers.Api.Lookup
{
    public class CampusesController : CMSBaseODataController
    {
        public CampusesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Campu, ApiLookup>();
            });
            return Ok(CurrentDatabase.Campus.ProjectTo<ApiLookup>(config));
        }
    }
}
