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
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ContributionsBasic, ApiContribution>();
            });
            return Ok(CurrentDatabase.ViewContributionsBasics.ProjectTo<ApiContribution>(config));
        }
    }
}
