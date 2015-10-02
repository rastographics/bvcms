using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class ContributionsController : ODataController
    {
        public ContributionsController()
        {
            Mapper.CreateMap<Contribution, ApiContribution>()
                .ForMember(d => d.FamilyId, opt => opt.MapFrom(c => c.Person.FamilyId));
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.Contributions.Project().To<ApiContribution>().AsQueryable());
        }
    }
}
