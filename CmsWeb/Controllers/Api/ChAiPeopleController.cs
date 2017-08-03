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
        public ChAiPeopleController()
        {
            Mapper.CreateMap<CmsData.View.ChAiIndividualDatum, ApiChAiPerson>();
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.ViewChAiIndividualDatas.Project().To<ApiChAiPerson>().AsQueryable());
        }
    }
}
