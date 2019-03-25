using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsWeb.Lifecycle;
using CmsWeb.Models.Api;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api
{
    public class ChAiPeopleController : CMSBaseODataController
    {
        public ChAiPeopleController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CmsData.View.ChAiIndividualDatum, ApiChAiPerson>();
            });
            return Ok(CurrentDatabase.ViewChAiIndividualDatas.ProjectTo<ApiChAiPerson>(config));
        }
    }
}
