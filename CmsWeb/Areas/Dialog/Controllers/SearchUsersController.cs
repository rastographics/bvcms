using System.Linq;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Dialog.Controllers
{
    // todo: use bootstrap
    [RouteArea("Dialog", AreaPrefix= "SearchUsers"), Route("{action=index}/{id?}")]
    public class SearchUsersController : CmsStaffController
    {
        [HttpGet]
        public ActionResult Index(bool? singlemode, bool? ordered, int? topid)
        {
            Response.NoCache();
            var m = new SearchUsersModel 
            { 
                singlemode = singlemode ?? false, 
                ordered = ordered ?? false, 
                topid = topid 
            };
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SearchUsersModel m)
        {
            return View("Index", m);
        }
        [HttpPost]
        public ActionResult MoveToTop(SearchUsersModel m)
        {
            return View("Results", m);
        }
        [HttpPost]
        public ActionResult TagUntag(int id, bool ischecked, bool isordered)
        {
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var count = t.PersonTags.Count();
            var topid = "";
            var tp = DbUtil.Db.TagPeople.SingleOrDefault(tt => tt.PeopleId == id && tt.Id == t.Id);
            if (ischecked)
            {
				if (tp != null)
					DbUtil.Db.TagPeople.DeleteOnSubmit(tp);
            }
            else if (tp == null)
            {
                if (count == 0 && isordered)
                    topid = id.ToString();
                t.PersonTags.Add(new TagPerson {PeopleId = id});
            }
            DbUtil.Db.SubmitChanges();
            return Content(topid);
        }
    }
}
