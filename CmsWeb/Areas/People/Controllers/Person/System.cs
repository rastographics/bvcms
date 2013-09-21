using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Person;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/Changes/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Changes(int id, int? page, int? size, string sort, string dir)
        {
            var m = new ChangesModel(id);
            m.Pager.Set("/Person2/Changes/" + id, page, size, sort, dir);
            return View("System/Changes", m);
        }
        [POST("Person2/Reverse/{id:int}")]
        public ActionResult Reverse(int id, string field, string value, string pf)
        {
            var m = new ChangesModel(id);
            m.Reverse(field, value, pf);
            return View("System/Changes", m);
        }
        [POST("Person2/Duplicates/{id:int}")]
        public ActionResult Duplicates(int id)
        {
            var m = new DuplicatesModel(id);
            return View("System/Duplicates", m);
        }
    }
}
