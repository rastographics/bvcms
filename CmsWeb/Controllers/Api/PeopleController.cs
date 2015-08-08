using System.Linq;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;

namespace CmsWeb.Controllers.Api
{
    public class PeopleController : ODataController
    {
        public PeopleController()
        {
            Mapper.CreateMap<Person, ApiPerson>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(x => x.BDate));
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.People.Project().To<ApiPerson>().AsQueryable());
        }
    }
}
