using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api
{
    public class OrganizationMembersController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrganizationMember, ApiOrganizationMember>();
            });
            return Ok(DbUtil.Db.OrganizationMembers.ProjectTo<ApiOrganizationMember>(config));
        }
    }
}
