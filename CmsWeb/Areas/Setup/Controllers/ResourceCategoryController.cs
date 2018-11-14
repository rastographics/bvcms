using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "ManageResources")]
    [RouteArea("Setup", AreaPrefix = "ResourceCategory"), Route("{action}/{id?}")]
    public class ResourceCategoryController : CmsStaffController
    {
        public ResourceCategoryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/ResourceCategories")]
        public ActionResult Index()
        {
            var m = CurrentDatabase.ResourceCategories.OrderBy(rt => rt.ResourceType.DisplayOrder).ThenBy(rt => rt.DisplayOrder);
            return View(m);
        }

        [HttpPost]
        public ActionResult Create(int? resourceTypeId)
        {
            ResourceType resourceType = null;
            if (resourceTypeId.HasValue)
            {
                resourceType = CurrentDatabase.ResourceTypes.FirstOrDefault(x => x.ResourceTypeId == resourceTypeId);
            }

            if (resourceType == null)
            {
                resourceType = CurrentDatabase.ResourceTypes.FirstOrDefault();
            }

            if (resourceType == null)
            {
                TempData["Error"] = "You need to configure at least one Resource Type first.";
                return Redirect("~/ResourceCategories");
            }

            var ResourceCategory = new ResourceCategory { Name = "new resource category", ResourceTypeId = resourceType.ResourceTypeId };
            CurrentDatabase.ResourceCategories.InsertOnSubmit(ResourceCategory);
            CurrentDatabase.SubmitChanges();
            return Redirect($"/ResourceCategories/#{ResourceCategory.ResourceCategoryId}");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            var a = id.Split('.');
            var c = new ContentResult();
            c.Content = value;
            var resourceCategory = CurrentDatabase.ResourceCategories.SingleOrDefault(m => m.ResourceCategoryId == a[1].ToInt());
            if (resourceCategory == null)
            {
                return c;
            }

            switch (a[0])
            {
                case "Name":
                    resourceCategory.Name = value;
                    break;
                case "DisplayOrder":
                    int displayOrder = resourceCategory.DisplayOrder;
                    int.TryParse(value, out displayOrder);
                    resourceCategory.DisplayOrder = displayOrder;
                    break;
            }
            CurrentDatabase.SubmitChanges();
            return c;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult EditResourceType(string id, string value)
        {
            var iid = id.Substring(1).ToInt();
            var mt = CurrentDatabase.ResourceCategories.SingleOrDefault(m => m.ResourceCategoryId == iid);
            mt.ResourceTypeId = value.ToInt();
            CurrentDatabase.SubmitChanges();
            return Content(mt.ResourceType.Name);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            id = id.Substring(1);
            var category = CurrentDatabase.ResourceCategories.SingleOrDefault(m => m.ResourceCategoryId == id.ToInt());
            if (category == null)
            {
                return new EmptyResult();
            }

            if (category.Resources.Any())
            {
                return Json(new { error = "Resources have that category, not deleted" });
            }

            CurrentDatabase.ResourceCategories.DeleteOnSubmit(category);
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }
    }
}
