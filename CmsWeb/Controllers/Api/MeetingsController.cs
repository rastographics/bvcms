using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models.Api;
using System.Web.Http;
using Microsoft.AspNet.OData;
using CmsWeb.Lifecycle;

namespace CmsWeb.Controllers.Api
{
    public class MeetingsController : CMSBaseODataController
    {
        public MeetingsController(IRequestManager requestManager) : base(requestManager) { }

        [EnableQuery(PageSize = ApiOptions.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Meeting, ApiMeeting>();
            });
            return Ok(CurrentDatabase.OrganizationMembers.ProjectTo<ApiMeeting>(config));
        }
    }
}
