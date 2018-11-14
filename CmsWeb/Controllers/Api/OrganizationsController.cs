using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api
{
    public class OrganizationsController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Organization, ApiOrganization>();
            });
            return Ok(DbUtil.Db.Organizations.ProjectTo<ApiOrganization>(config));
        }
    }
}
