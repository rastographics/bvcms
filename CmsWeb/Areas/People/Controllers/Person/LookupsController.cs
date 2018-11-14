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
            var qc = CurrentDatabase.Campus.AsQueryable();
            qc = CurrentDatabase.Setting("SortCampusByCode")
                ? qc.OrderBy(cc => cc.Code)
                : qc.OrderBy(cc => cc.Description);
            var list = (from c in qc
                        select new
                        {
                            value = c.Id,
                            text = c.Description,
                        }).ToList();
            list.Insert(0, new { value = 0, text = "(not specified)" });
            if (CurrentDatabase.Setting("CampusRequired")
                && Util.UserPeopleId == Util2.CurrentPeopleId
                && !User.IsInRole("Admin"))
                list.RemoveAt(0);
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FamilyPositions()
        {
            Response.SetCacheMinutes(5);
            var q = from c in CurrentDatabase.FamilyPositions
                    select new { value = c.Id, text = c.Description };
            var list = q.ToList();
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Schools(string query)
        {
            var qu = from p in CurrentDatabase.People
                     where p.SchoolOther.Contains(query)
                     group p by p.SchoolOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Employers(string query)
        {
            var qu = from p in CurrentDatabase.People
                     where p.EmployerOther.Contains(query)
                     group p by p.EmployerOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Occupations(string query)
        {
            var qu = from p in CurrentDatabase.People
                     where p.OccupationOther.Contains(query)
                     group p by p.OccupationOther into g
                     select g.Key;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Churches(string query)
        {
            var qu = from r in CurrentDatabase.ViewChurches
                     where r.C.Contains(query)
                     select r.C;
            return Json(qu.Take(10).ToArray(), JsonRequestBehavior.AllowGet);
        }
    }
}
