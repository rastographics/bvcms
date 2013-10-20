using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsWeb.Models.ExtraValues;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController : CmsStaffController
    {
        [POST("ExtraValue/EditEntry")]
        public ActionResult EditEntry(string pk, string name, string value)
        {
            EntryModel.EditValue(pk.ToInt(), name, value);
            return new EmptyResult();
        }
        [GET("ExtraValue/OriginList")]
        public ActionResult OriginList()
        {
            return Content(EntryModel.OriginList());
        }
        [GET("ExtraValue/EntryPointList")]
        public ActionResult EntryPointList()
        {
            return Content(EntryModel.EntryPointList());
        }
   }
}
