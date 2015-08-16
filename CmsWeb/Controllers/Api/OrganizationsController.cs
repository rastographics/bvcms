using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class OrganizationsController : ODataController
    {
        public OrganizationsController()
        {
            Mapper.CreateMap<Organization, ApiOrganization>();
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.Organizations.Project().To<ApiOrganization>().AsQueryable());
        }
    }
}
