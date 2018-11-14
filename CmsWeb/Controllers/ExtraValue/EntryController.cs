using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models.ExtraValues;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController : CmsStaffController
    {
        public ExtraValueController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("ExtraValue/EditEntry")]
        public ActionResult EditEntry(string pk, string name, string value)
        {
            DbUtil.LogActivity("ExtraValue", peopleid: pk.ToInt());
            EntryModel.EditValue(pk.ToInt(), name, value);
            return new EmptyResult();
        }
        [HttpGet, Route("ExtraValue/OriginList")]
        public ActionResult OriginList()
        {
            return Content(EntryModel.OriginList());
        }
        [HttpGet, Route("ExtraValue/EntryPointList")]
        public ActionResult EntryPointList()
        {
            return Content(EntryModel.EntryPointList());
        }
        [HttpGet, Route("ExtraValue/InterestPointList")]
        public ActionResult InterestPointList()
        {
            return Content(EntryModel.InterestPointList());
        }
    }
}
