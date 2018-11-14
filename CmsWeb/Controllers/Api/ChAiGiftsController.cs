using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsData.View;
using CmsWeb.Models.Api;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api
{
    public class ChAiGiftsController : ODataController
    {
        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChAiGiftDatum, ApiChAiGift>();
            });
            return Ok(DbUtil.Db.ViewChAiGiftDatas.ProjectTo<ApiChAiGift>(config));
        }
    }
}
