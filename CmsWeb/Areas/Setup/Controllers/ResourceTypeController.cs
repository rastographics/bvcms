using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "ResourceType"), Route("{action}/{id?}")]
    public class ResourceTypeController : CmsStaffController
    {
        [Route("~/ResourceTypes")]
        public ActionResult Index()
        {
            var m = from rt in DbUtil.Db.ResourceTypes
                    orderby rt.Name
                    select rt;
            return View(m);
        }

        [HttpPost]
        public ActionResult Create()
        {
            var resourceType = new ResourceType { Name = "new resource type" };
            DbUtil.Db.ResourceTypes.InsertOnSubmit(resourceType);
            DbUtil.Db.SubmitChanges();
            return Redirect($"/ResourceTypes/#{resourceType.ResourceTypeId}");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            var a = id.Split('.');
            var c = new ContentResult();
            c.Content = value;
            var resourceType = DbUtil.Db.ResourceTypes.SingleOrDefault(m => m.ResourceTypeId == a[1].ToInt());
            if (resourceType == null)
                return c;
            switch (a[0])
            {
                case "Name":
                    resourceType.Name = value;
                    break;                
            }
            DbUtil.Db.SubmitChanges();
            return c;
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            id = id.Substring(1);
            var p = DbUtil.Db.ResourceTypes.SingleOrDefault(m => m.ResourceTypeId == id.ToInt());
            if (p == null)
                return new EmptyResult();            
            
            DbUtil.Db.ResourceTypes.DeleteOnSubmit(p);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
    }
}