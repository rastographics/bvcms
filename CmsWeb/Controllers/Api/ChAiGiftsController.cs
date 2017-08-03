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
        public ChAiGiftsController()
        {
            Mapper.CreateMap<CmsData.View.ChAiGiftDatum, ApiChAiGift>();
        }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(DbUtil.Db.ViewChAiGiftDatas.Project().To<ApiChAiGift>().AsQueryable());
        }
    }
}
