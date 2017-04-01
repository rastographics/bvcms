using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Attributes"), Route("{action}/{cid:int}")]
    public class AttributesController : CmsController
    {
        [HttpGet, Route("~/Person/Attributes/{field}/{pid:int}")]
        public ActionResult Index(int pid, string field)
        {
            var m = DbUtil.Db.PeopleExtras.Single(vv => vv.Field == field && vv.PeopleId == pid);
            return View(m);
        }
        [HttpGet, Route("~/Person/Attributes/Edit/{field}/{pid:int}")]
        public ActionResult Edit(int pid, string field)
        {
            var m = DbUtil.Db.PeopleExtras.Single(vv => vv.Field == field && vv.PeopleId == pid);
            return View(m);
        }
        [HttpPost, Route("~/Person/Attributes/Update")]
        public ActionResult Update(PeopleExtra ev)
        {
            var m = DbUtil.Db.PeopleExtras.Single(vv => vv.Field == ev.Field && vv.PeopleId == ev.PeopleId);
            m.Data = ev.Data;
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Updated Attributes", peopleid:m.PeopleId);
            return Redirect($"/Person/Attributes/{ev.Field}/{ev.PeopleId}");
        }
    }
}