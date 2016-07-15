using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "ResourceCategory"), Route("{action}/{id?}")]
    public class ResourceCategoryController : CmsStaffController
    {
        [Route("~/ResourceCategories")]
        public ActionResult Index()
        {
            var m = from rt in DbUtil.Db.ResourceCategories
                    orderby rt.Name
                    select rt;
            return View(m);
        }

        [HttpPost]
        public ActionResult Create()
        {
            var ResourceCategory = new ResourceCategory { Name = "new resource category" };
            DbUtil.Db.ResourceCategories.InsertOnSubmit(ResourceCategory);
            DbUtil.Db.SubmitChanges();
            return Redirect($"/ResourceCategories/#{ResourceCategory.ResourceCategoryId}");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            var a = id.Split('.');
            var c = new ContentResult();
            c.Content = value;
            var ResourceCategory = DbUtil.Db.ResourceCategories.SingleOrDefault(m => m.ResourceCategoryId == a[1].ToInt());
            if (ResourceCategory == null)
                return c;
            switch (a[0])
            {
                case "Name":
                    ResourceCategory.Name = value;
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return c;
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            id = id.Substring(1);
            var p = DbUtil.Db.ResourceCategories.SingleOrDefault(m => m.ResourceCategoryId == id.ToInt());
            if (p == null)
                return new EmptyResult();

            DbUtil.Db.ResourceCategories.DeleteOnSubmit(p);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
    }
}