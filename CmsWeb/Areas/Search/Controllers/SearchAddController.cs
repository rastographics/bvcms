using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaUrl = "SearchAdd2")]
    public class SearchAddController : CmsStaffController
    {
        [POST("SearchAdd2/Dialog/{type}/{typeid?}")]
        public ActionResult Dialog(string type, string typeid)
        {
            var m = new SearchAddModel(type, typeid);
            return View("SearchPerson", m);
        }

        [POST("SearchAdd2/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, SearchAddModel m)
        {
            DbUtil.Db.SetNoLock();            
            m.Pager.Set("/SearchAdd2/Results", page ?? 1, size ?? 15, "na", "na");
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/ResultsFamily/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult ResultsFamily(int? page, int? size, string sort, string dir, SearchAddModel m)
        {
            DbUtil.Db.SetNoLock();
            m.Pager.Set("/SearchAdd2/ResultsFamily", page ?? 1, size ?? 15, "na", "na");
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/SearchPerson")]
        public ActionResult SearchPerson(SearchAddModel m)
        {
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/SearchFamily")]
        public ActionResult SearchFamily(SearchAddModel m)
        {
            string first, last;
            Util.NameSplit(m.Name, out first, out last);
            m.Name = last;
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/CancelPerson/{id}")]
        public ActionResult CancelPerson(int id, SearchAddModel m)
        {
            m.PendingList.RemoveAt(id);
            ModelState.Clear();
            if (m.PendingList.Count > 0)
                return View("List", m);
            return View("SearchPerson", m);
        }

        [POST("SearchAdd2/Select/{id}")]
        public ActionResult Select(int id, SearchAddModel m)
        {
            if (m.PendingList.Any(li => li.PeopleId == id))
                return View("List", m);

            m.AddExisting(id);
			if (m.OnlyOne)
				return CommitAdd(m);
            ModelState.Clear();
            return View("List", m);
        }

        [POST("SearchAdd2/NewPerson/{familyid}")]
        public ActionResult NewPerson(int familyid, SearchAddModel m)
        {
            m.NewPerson(familyid);
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/AddNewPerson/{noCheckDuplicate?}")]
        public ActionResult AddNewPerson(SearchAddModel m, string noCheckDuplicate)
        {
            var p = m.PendingList[m.PendingList.Count - 1];
            if(!noCheckDuplicate.HasValue())
                p.CheckDuplicate();
            if (!ModelState.IsValid || p.PotentialDuplicate.HasValue())
                return View("NewPerson", m);
            p.LoadAddress();
            if(p.IsNewFamily)
                return View("NewAddress", m);
            return View("List", m);
        }

        [POST("SearchAdd2/AddNewAddress/{NoCheck?}")]
        public ActionResult AddNewAddress(SearchAddModel m, string noCheck)
        {
            var p = m.PendingList[m.PendingList.Count - 1];
            if(noCheck.HasValue() == false)
                p.AddressInfo.ValidateAddress(ModelState);
            if (!ModelState.IsValid)
                return View("NewAddress", m);
            if (p.AddressInfo.Error.HasValue())
            {
                ModelState.Clear();
                return View("NewAddress", m);
            }
            return View("List", m);
        }

        [POST("SearchAdd2/CommitAdd")]
        public ActionResult CommitAdd(SearchAddModel m)
        {
            var ret = m.CommitAdd();
            return Json(ret);
        }

    }
}
