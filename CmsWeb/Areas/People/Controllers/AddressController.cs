using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using System.Web.Routing;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "")]
    [ValidateInput(false)]
    [SessionExpire]
    public class AddressController : CmsStaffController
    {
        public AddressController(IRequestManager requestManager) : base(requestManager)
        {
        }

        protected override void Initialize(RequestContext requestContext)
        {
            NoCheckRole = true;
            base.Initialize(requestContext);
        }

        [HttpPost, Route("Address/Edit/{type}/{id}")]
        public ActionResult Edit(int id, string type)
        {
            var m = AddressInfo.GetAddressInfo(id, type);
            return View(m);
        }

        [HttpPost, Route("Address/EditAgain")]
        public ActionResult EditAgain(AddressInfo m)
        {
            return View("Edit", m);
        }

        [HttpPost, Route("Address/Update/{noCheck?}")]
        public ActionResult Update(AddressInfo m, string noCheck)
        {
            if (noCheck.HasValue() == false)
            {
                m.ValidateAddress(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", m);
            }

            if (m.Error.HasValue())
            {
                ModelState.Clear();
                return View("Edit", m);
            }
            m.UpdateAddress(ModelState);
            return View("Saved", m);
        }

        [HttpPost, Route("Address/ForceSave")]
        public ActionResult ForceSave(AddressInfo m)
        {
            m.UpdateAddress(ModelState, forceSave: true);
            return View("Saved", m);
        }

    }
}
