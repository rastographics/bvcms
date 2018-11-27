using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData.View;
using CmsWeb.Lifecycle;
using CmsWeb.Models.Api;
using System.Web.Http;
using System.Web.OData;

namespace CmsWeb.Controllers.Api
{
    public class ChAiGiftsController : CMSBaseODataController
    {
        public ChAiGiftsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChAiGiftDatum, ApiChAiGift>();
            });
            return Ok(CurrentDatabase.ViewChAiGiftDatas.ProjectTo<ApiChAiGift>(config));
        }
    }
}
