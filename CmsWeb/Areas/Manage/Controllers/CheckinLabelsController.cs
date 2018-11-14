using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Manage", AreaPrefix = "CheckinLabels"), Route("{action}/{id?}")]
    public class CheckinLabelsController : CMSBaseController
    {
        public CheckinLabelsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/CheckinLabels")]
        [Route("~/CheckinLabels/{id:int}")]
        public ActionResult Index(int id = 0)
        {
            ViewBag.ID = id;
            return View();
        }

        public ActionResult Save(int id = 0, string labelFormat = "")
        {
            if (id == 0 || labelFormat.Length == 0)
            {
                return new RedirectResult("/CheckinLabels");
            }

            var label = (from e in CurrentDatabase.LabelFormats
                         where e.Id == id
                         select e).FirstOrDefault();

            if (label != null)
            {
                label.Format = labelFormat.Replace("\n", "").Replace("\r", "");
                CurrentDatabase.SubmitChanges();
            }

            return new RedirectResult("/CheckinLabels/" + id);
        }
    }
}
