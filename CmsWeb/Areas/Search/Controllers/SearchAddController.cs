using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "")]
    public class SearchAddController : CmsStaffController
    {
        public SearchAddController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("SearchAdd2/Dialog/{type}/{typeid?}")]
        public ActionResult Dialog(string type, string typeid, bool displaySkipSearch = true)
        {
            var m = new SearchAddModel(type, typeid, displaySkipSearch);
            return View("SearchPerson", m);
        }

        [HttpPost, Route("SearchAdd2/Results")]
        public ActionResult Results(SearchAddModel m)
        {
            CurrentDatabase.SetNoLock();
            ModelState.Clear();
            m.OnlineRegTypeSearch = Util2.OnlineRegTypeSearchAdd;

            if (m.ShowLimitedSearch)
            {
                if (string.IsNullOrWhiteSpace(m.FirstName))
                {
                    ModelState.AddModelError("FirstName", "First name is required");
                }

                if (string.IsNullOrWhiteSpace(m.LastName))
                {
                    ModelState.AddModelError("LastName", "Last name is required");
                }

                if (string.IsNullOrWhiteSpace(m.Email))
                {
                    ModelState.AddModelError("Email", "Email is required");
                }

                if (!ModelState.IsValid)
                {
                    return View("SearchPerson", m);
                }
            }

            if (m.Count() == 0 && m.ShowLimitedSearch)
            {
                NewPerson(0, m);
                m.PendingList[0].FirstName = m.FirstName;
                m.PendingList[0].LastName = m.LastName;
                m.PendingList[0].EmailAddress = m.Email;
                return View("NewPerson", m);
            }

            if (m.Count() == 1 && m.ShowLimitedSearch)
            {
                m.AddExisting(m.ViewList().First().PeopleId);
            }

            return View(m);
        }

        [HttpPost, Route("SearchAdd2/ResultsFamily")]
        public ActionResult ResultsFamily(SearchAddModel m)
        {
            CurrentDatabase.SetNoLock();
            ModelState.Clear();
            return View(m);
        }

        [HttpPost, Route("SearchAdd2/SearchPerson")]
        public ActionResult SearchPerson(SearchAddModel m)
        {
            ModelState.Clear();
            return View(m);
        }

        [HttpPost, Route("SearchAdd2/SearchFamily")]
        public ActionResult SearchFamily(SearchAddModel m)
        {
            string first, last;
            Util.NameSplit(m.Name, out first, out last);
            m.Name = last;
            ModelState.Clear();
            m.OnlineRegTypeSearch = false;
            return View(m);
        }

        [HttpPost, Route("SearchAdd2/CancelPerson/{id}")]
        public ActionResult CancelPerson(int id, SearchAddModel m)
        {
            m.PendingList.RemoveAt(id);
            ModelState.Clear();
            if (m.PendingList.Count > 0)
            {
                return View("List", m);
            }

            return View("SearchPerson", m);
        }
        [HttpPost, Route("SearchAdd2/CancelSearch")]
        public ActionResult CancelSearch(SearchAddModel m)
        {
            if (m.PendingList.Count > 0)
            {
                return View("List", m);
            }

            return View("SearchPerson", m);
        }

        [HttpPost, Route("SearchAdd2/Select/{id}")]
        public ActionResult Select(int id, SearchAddModel m)
        {
            if (m.PendingList.Any(li => li.PeopleId == id))
            {
                return View("List", m);
            }

            m.AddExisting(id);
            if (m.OnlyOne)
            {
                return CommitAdd(m);
            }

            ModelState.Clear();
            return View("List", m);
        }

        [HttpPost, Route("SearchAdd2/NewPerson/{familyid}")]
        public ActionResult NewPerson(int familyid, SearchAddModel m)
        {
            if (familyid == 0 && string.Compare(m.AddContext, "family", StringComparison.OrdinalIgnoreCase) == 0)
            {
                familyid = (from pp in CurrentDatabase.People
                            where pp.PeopleId == m.PrimaryKeyForContextType.ToInt()
                            select pp.FamilyId).Single();
            }

            m.NewPerson(familyid);
            ModelState.Clear();
            return View(m);
        }

        [HttpPost, Route("SearchAdd2/AddNewPerson/{noCheckDuplicate?}")]
        public ActionResult AddNewPerson(string noCheckDuplicate, SearchAddModel m)
        {
            var p = m.PendingList[m.PendingList.Count - 1];
            if (ModelState.IsValid && !noCheckDuplicate.HasValue())
            {
                p.CheckDuplicate();
            }

            if (!ModelState.IsValid || p.PotentialDuplicate.HasValue())
            {
                return View("NewPerson", m);
            }

            p.LoadAddress();
            if (p.IsNewFamily)
            {
                return View("NewAddress", m);
            }

            return View("List", m);
        }

        [HttpPost, Route("SearchAdd2/AddNewAddress/{NoCheck?}")]
        public ActionResult AddNewAddress(SearchAddModel m, string noCheck)
        {
            var p = m.PendingList[m.PendingList.Count - 1];
            if (noCheck.HasValue() == false)
            {
                p.AddressInfo.ValidateAddress(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return View("NewAddress", m);
            }

            if (p.AddressInfo.Error.HasValue())
            {
                ModelState.Clear();
                return View("NewAddress", m);
            }
            return View("List", m);
        }

        [HttpPost, Route("SearchAdd2/CommitAdd")]
        public ActionResult CommitAdd(SearchAddModel m)
        {
            var ret = m.CommitAdd();
            return Json(ret);
        }

        [HttpPost, Route("SearchAdd2/OrgContactee/{id}")]
        public ActionResult OrgContactee(int id)
        {
            ViewBag.ContactId = id;

            var m = new OrgSearchModel();
            return View("SearchOrganization", m);
        }

        [HttpPost, Route("SearchAdd2/OrgContacteeResults/{id}")]
        public ActionResult ResOrgContacteeResultsults(OrgSearchModel m, int id)
        {
            ViewBag.ContactId = id;

            CurrentDatabase.SetNoLock();
            ModelState.Clear();
            return View("ResultsOrganization", m);
        }

        [HttpPost, Route("SearchAdd2/SelectOrgContactee/{cid}/{oid}")]
        public ActionResult SelectOrg(OrgSearchModel m, int cid, int oid)
        {
            // TODO Reimplement with new paradigm
            return null;
        }

        public class OrgReturnResult
        {
            public int cid;
            public string from;
            public string message;
        }
    }
}
