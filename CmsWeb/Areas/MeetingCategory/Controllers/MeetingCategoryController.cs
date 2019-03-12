using CmsWeb.Lifecycle;
using CmsWeb.Services.MeetingCategory;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.MeetingCategory.Controllers
{
    [RouteArea("MeetingCategory"), Route("{action}/{id?}")]
    public class MeetingCategoryController : CmsStaffController
    {
        private readonly IMeetingCategoryService _meetingCategoryService;

        public MeetingCategoryController(IRequestManager requestManager, IMeetingCategoryService meetingCategoryService) : base(requestManager)
        {
            _meetingCategoryService = meetingCategoryService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult Index(bool includeExpired = true)
        {
            var categories = _meetingCategoryService.GetMeetingCategories(includeExpired).OrderBy(c => c.Description);
            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var category = new CmsData.MeetingCategory()
            {
                Description = $"New meeting category - {DateTime.UtcNow.Millisecond}"
            };
            return View("Editor", category);
        }

        [HttpGet]
        [Route("{meetingCategoryId:long}")]
        public ActionResult Edit(long meetingCategoryId)
        {
            var category = _meetingCategoryService.GetById(meetingCategoryId);
            return View("Editor", category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Edit")]
        [Route("{meetingCategoryId:long}")]
        public ActionResult Edit(long meetingCategoryId, CmsData.MeetingCategory meetingCategory)
        {
            var result = _meetingCategoryService.CreateOrUpdate(meetingCategory);
            return RedirectToAction("Index");
        }
    }
}
