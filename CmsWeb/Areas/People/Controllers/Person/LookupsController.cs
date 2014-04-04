using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpGet, Route("Person2/Campuses")]
        public JsonResult Campuses()
        {
            Response.SetCacheMinutes(5);
            var q = from c in DbUtil.Db.Campus
                    select new { value = c.Id, text = c.Description };
            var list = q.ToList();
            list.Insert(0, new { value = 0, text = "(not specified)" });
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost, Route("Person2/Schools")]
        public JsonResult Schools(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.SchoolOther.Contains(query)
                     group p by p.SchoolOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost, Route("Person2/Employers")]
        public JsonResult Employers(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.EmployerOther.Contains(query)
                     group p by p.EmployerOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost, Route("Person2/Occupations")]
        public JsonResult Occupations(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.OccupationOther.Contains(query)
                     group p by p.OccupationOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost, Route("Person2/Churches")]
        public JsonResult Churches(string query)
        {
            var qu = from r in DbUtil.Db.ViewChurches
                     where r.C.Contains(query)
                     select r.C;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
    }
}
