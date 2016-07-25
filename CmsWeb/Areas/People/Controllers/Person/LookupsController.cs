using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpGet]
        public JsonResult Campuses()
        {
            var q = from c in DbUtil.Db.Campus
                    select new { value = c.Id, text = c.Description };
            var list = q.ToList();
            if (DbUtil.Db.Setting("CampusRequired", "false") != "true" 
                    || DbUtil.Db.CurrentUser.Roles.Length != 0)
                // MyData user cannot use (not specified)
                list.Insert(0, new { value = 0, text = "(not specified)" });
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FamilyPositions()
        {
            Response.SetCacheMinutes(5);
            var q = from c in DbUtil.Db.FamilyPositions
                    select new { value = c.Id, text = c.Description };
            var list = q.ToList();
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Schools(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.SchoolOther.Contains(query)
                     group p by p.SchoolOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Employers(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.EmployerOther.Contains(query)
                     group p by p.EmployerOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Occupations(string query)
        {
            var qu = from p in DbUtil.Db.People
                     where p.OccupationOther.Contains(query)
                     group p by p.OccupationOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Churches(string query)
        {
            var qu = from r in DbUtil.Db.ViewChurches
                     where r.C.Contains(query)
                     select r.C;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
    }
}
