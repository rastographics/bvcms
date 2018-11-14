using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api
{
    public class PeopleController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CmsData.View.PeopleBasicModifed, ApiPerson>();
            });
            return Ok(DbUtil.Db.ViewPeopleBasicModifeds.ProjectTo<ApiPerson>(config));
        }
    }
}
