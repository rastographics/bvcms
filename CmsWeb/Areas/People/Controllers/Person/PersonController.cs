using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;
using System.Web.Routing;

namespace CmsWeb.Areas.People.Controllers
{
    [ValidateInput(false)]
    [SessionExpire]
    [RouteArea("People", AreaUrl = "Person2")]
    public partial class PersonController : CmsStaffController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            NoCheckRole = true;
            base.Initialize(requestContext);
        }
        [GET("Person2/Current")]
        public ActionResult Current()
        {
            return Redirect("/Person2/" + Util2.CurrentPeopleId);
        }
        [GET("Person2/{id:int}")]
        [GET("{id:int}")]
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
                return Content("no id");

            var m = new PersonModel(id.Value);
            var noview = m.CheckView();
            if (noview.HasValue())
                return Content(noview);

            ViewBag.Comments = Util.SafeFormat(m.Person.Comments);
            ViewBag.PeopleId = id.Value;
            Util2.CurrentPeopleId = id.Value;
            Session["ActivePerson"] = m.Person.Name;
            DbUtil.LogActivity("Viewing Person: {0}".Fmt(m.Person.Name));
            InitExportToolbar(id);
            return View(m);
        }

        private void InitExportToolbar(int? id)
        {
            var qb = DbUtil.Db.QueryIsCurrentPerson();
            ViewBag.queryid = qb.QueryId;
            ViewBag.PeopleId = Util2.CurrentPeopleId;
            ViewBag.TagAction = "/Person2/Tag/" + id;
            ViewBag.UnTagAction = "/Person2/UnTag/" + id;
            ViewBag.AddContact = "/Person2/AddContactReceived/" + id;
            ViewBag.AddTasks = "/Person2/AddAboutTask/" + id;
        }

        [POST("Person2/Tag/{id:int}")]
        public ActionResult Tag(int id, string tagname, bool? cleartagfirst)
        {
            if (Util2.CurrentTagName == tagname && !(cleartagfirst ?? false))
            {
                CmsData.Person.Tag(DbUtil.Db, id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                return Content("OK");
            }
            var tag = DbUtil.Db.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            if (cleartagfirst ?? false)
                DbUtil.Db.ClearTag(tag);
            CmsData.Person.Tag(DbUtil.Db, id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            Util2.CurrentTag = tagname;
            DbUtil.Db.TagCurrent();
            return Content("OK");
        }
        [HttpPost]
        public ActionResult UnTag(int id)
        {
            CmsData.Person.UnTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
    }
}
