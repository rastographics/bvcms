using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "Ministry"), Route("{action=index}/{id?}")]
    public class MinistryController : CmsStaffController
    {
        public MinistryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var m = CurrentDatabase.Ministries.AsEnumerable();
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create()
        {
            var m = new Ministry { MinistryName = "NEW" };
            CurrentDatabase.Ministries.InsertOnSubmit(m);
            CurrentDatabase.SubmitChanges();
            return Redirect($"/Ministry/#{m.MinistryId}");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Edit(string id, string value)
        {
            var a = id.Split('.');
            var c = new ContentResult();
            c.Content = value;
            var min = CurrentDatabase.Ministries.SingleOrDefault(m => m.MinistryId == a[1].ToInt());
            if (min == null)
            {
                return c;
            }

            switch (a[0])
            {
                case "MinistryName":
                    min.MinistryName = value;
                    break;
            }
            CurrentDatabase.SubmitChanges();
            return c;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public EmptyResult Delete(string id)
        {
            id = id.Substring(1);
            var min = CurrentDatabase.Ministries.SingleOrDefault(m => m.MinistryId == id.ToInt());
            if (min == null)
            {
                return new EmptyResult();
            }

            CurrentDatabase.Ministries.DeleteOnSubmit(min);
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }
    }
}
