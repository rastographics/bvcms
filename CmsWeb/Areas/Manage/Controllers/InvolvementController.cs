using System.Linq;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Areas.Manage.Models.Involvement;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,Design")]
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix = "Involvement"), Route("{action}/{id?}")]
    public class InvolvementController : CmsStaffController
    {
        public InvolvementController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var model = new CustomizeInvolvementModel();

            model.Current.ReadXml(CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Current.Name)?.Body ?? Resource1.InvolvementTableCurrent);
            model.Pending.ReadXml(CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Pending.Name)?.Body ?? Resource1.InvolvementTablePending);
            model.Previous.ReadXml(CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Previous.Name)?.Body ?? Resource1.InvolvementTablePrevious);

            return View(model);
        }
    }
}
