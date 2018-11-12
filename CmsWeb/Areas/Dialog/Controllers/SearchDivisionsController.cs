using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "SearchDivisions"), Route("{action}/{id?}")]
    public class SearchDivisionsController : CmsStaffController
    {
        public SearchDivisionsController(RequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/SearchDivisions/{id:int}")]
        public ActionResult Index(int id)
        {
            var m = new SearchDivisionsModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult ModalDialog(SearchDivisionsModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult MoveToTop(SearchDivisionsModel m)
        {
            DbUtil.Db.SetMainDivision(m.Id, m.TargetDivision);
            return View("ModalDialog", m);
        }
        [HttpPost]
        public ActionResult AddRemoveDiv(SearchDivisionsModel m)
        {
            m.AddRemoveDiv();
            return View("ModalDialog", m);
        }

    }
}
