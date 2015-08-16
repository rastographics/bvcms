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
        public OrganizationMembersController()
        {
            Mapper.CreateMap<OrganizationMember, ApiOrganizationMember>();
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.OrganizationMembers.Project().To<ApiOrganizationMember>().AsQueryable());
        }
    }
}
