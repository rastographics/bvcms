using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsData.View;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class ContributionsController : ODataController
    {
        public ContributionsController()
        {
            Mapper.CreateMap<ContributionsBasic, ApiContribution>();
            //.ForMember(d => d.FamilyId, opt => opt.MapFrom(c => c.Person.FamilyId));
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.ViewContributionsBasics.Project().To<ApiContribution>().AsQueryable());
        }
    }
}
