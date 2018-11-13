using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class OrganizationMembersController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<OrganizationMember, ApiOrganizationMember>();
            });
            return Ok(CurrentDatabase.OrganizationMembers.ProjectTo<ApiOrganizationMember>(config));
        }
    }
}
