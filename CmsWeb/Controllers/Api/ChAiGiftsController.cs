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
    public class ChAiGiftsController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ChAiGiftDatum, ApiChAiGift>();
            });
            return Ok(DbUtil.Db.ViewChAiGiftDatas.ProjectTo<ApiChAiGift>(config));
        }
    }
}
