using CmsWeb.Lifecycle;
using System.Web.Mvc;
using CmsWeb.Areas.People.Models;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Attributes"), Route("{action}/{cid:int}")]
    public class AttributesController : CmsController
    {
        [HttpGet, Route("~/Person/Attributes/{field}/{pid:int}")]
        public ActionResult Index(int pid, string field)
        {
            var m = new AttributesModel(CurrentDatabase, field, pid);
            return View(m);
        }
        [HttpGet, Route("~/Person/Attributes/Edit/{field}/{pid:int}")]
        public ActionResult Edit(int pid, string field)
        {
            var m = new AttributesModel(CurrentDatabase, field, pid);
            return View(m);
        }
        [HttpPost, Route("~/Person/Attributes/Update")]
        public ActionResult Update(int peopleid, string field, string data)
        {
            var m = new AttributesModel(CurrentDatabase, field, peopleid);
            m.Update(data);
            return Redirect($"/Person/Attributes/{field}/{peopleid}");
        }

        public AttributesController(IRequestManager requestManager) : base(requestManager)
        {
        }
    }
}
