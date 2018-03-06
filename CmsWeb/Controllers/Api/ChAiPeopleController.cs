using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class ChAiPeopleController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CmsData.View.ChAiIndividualDatum, ApiChAiPerson>();
            });
            return Ok(DbUtil.Db.ViewChAiIndividualDatas.ProjectTo<ApiChAiPerson>(config));
        }
    }
}
