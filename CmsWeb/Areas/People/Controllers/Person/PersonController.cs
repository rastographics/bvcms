using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Person2"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [SessionExpire]
    public partial class PersonController : CmsStaffController
    {
        public PersonController(IRequestManager requestManager) : base(requestManager)
        {
        }

        protected override void Initialize(RequestContext requestContext)
        {
            NoCheckRole = true;
            base.Initialize(requestContext);
        }
        [HttpGet]
        public ActionResult Current()
        {
            return Redirect("/Person2/" + Util2.CurrentPeopleId);
        }

        [HttpGet, Route("User/{id:int}")]
        public ActionResult UserPerson(int? id)
        {
            var pid = (from p in CurrentDatabase.People
                       where p.Users.Any(uu => uu.UserId == id)
                       select p.PeopleId).SingleOrDefault();

            if (pid == 0)
            {
                return Content("no person");
            }

            return Redirect("/Person2/" + pid);
        }

        [HttpGet, Route("~/Family/{id:int}")]
        public ActionResult Family(int? id)
        {
            var pid = (from f in CurrentDatabase.Families
                       where f.FamilyId == id
                       select f.HeadOfHouseholdId).SingleOrDefault();

            if (pid == 0)
            {
                return Content("no family");
            }

            return Redirect("/Person2/" + pid);
        }

        [HttpGet, Route("~/Person2/{id:int}")]
        [Route("~/Person/Index/{id:int}")]
        [Route("~/Person/{id:int}")]
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                return Content("no id");
            }

            if (id == 0 && Util.UserPeopleId.HasValue)
            {
                id = Util.UserPeopleId;
            }

            var m = new PersonModel(id.Value);
            var noview = m.CheckView();
            if (noview.HasValue())
            {
                return Content(noview);
            }

            ViewBag.Comments = Util.SafeFormat(m.Person.Comments);
            ViewBag.PeopleId = id.Value;
            Util2.CurrentPeopleId = id.Value;
            Session["ActivePerson"] = m.Person.Name;
            DbUtil.LogPersonActivity($"Viewing Person: {m.Person.Name}", id.Value, m.Person.Name);
            InitExportToolbar(id);
            return View(m);
        }

        [HttpGet, Route("~/Person2/{id:int}/Resources")]
        [Route("~/Person/Index/{id:int}/Resources")]
        [Route("~/Person/{id:int}/Resources")]
        public ActionResult Resources(int? id)
        {
            if (!id.HasValue)
            {
                return Content("no id");
            }

            if (id == 0 && Util.UserPeopleId.HasValue)
            {
                id = Util.UserPeopleId;
            }

            var m = new PersonModel(id.Value);
            var noview = m.CheckView();
            if (noview.HasValue())
            {
                return Content(noview);
            }

            ViewBag.Comments = Util.SafeFormat(m.Person.Comments);
            ViewBag.PeopleId = id.Value;
            Util2.CurrentPeopleId = id.Value;
            Session["ActivePerson"] = m.Person.Name;
            DbUtil.LogPersonActivity($"Viewing Person: {m.Person.Name}", id.Value, m.Person.Name);
            InitExportToolbar(id);
            return View("Resources/Index", m);
        }

        private void InitExportToolbar(int? id)
        {
            var qb = CurrentDatabase.QueryIsCurrentPerson();
            ViewBag.queryid = qb.QueryId;
            ViewBag.PeopleId = Util2.CurrentPeopleId;
            ViewBag.TagAction = "/Person2/Tag/" + id;
            ViewBag.UnTagAction = "/Person2/UnTag/" + id;
            ViewBag.AddContact = "/Person2/AddContactReceived/" + id;
            ViewBag.AddTasks = "/Person2/AddTaskAbout/" + id;
        }

        [HttpPost]
        public ActionResult Tag(int id, string tagname, bool? cleartagfirst)
        {
            if (Util2.CurrentTagName == tagname && !(cleartagfirst ?? false))
            {
                Person.Tag(CurrentDatabase, id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                CurrentDatabase.SubmitChanges();
                return Content("OK");
            }

            var tag = CurrentDatabase.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            if (cleartagfirst ?? false)
            {
                CurrentDatabase.ClearTag(tag);
            }

            Person.Tag(CurrentDatabase, id, tag.Name, tag.PersonOwner.PeopleId, DbUtil.TagTypeId_Personal);
            CurrentDatabase.SubmitChanges();
            Util2.CurrentTag = tag.Name;
            CurrentDatabase.TagCurrent();
            return Content("OK");
        }

        [HttpPost]
        public ActionResult UnTag(int id)
        {
            Person.UnTag(CurrentDatabase, id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult InlineEdit(int id, int pk, string name, string value)
        {
            var m = new PersonModel(id);
            switch (name)
            {
                case "ContributionOptions":
                    m.Person.UpdateContributionOption(CurrentDatabase, value.ToInt());
                    break;
                case "EnvelopeOptions":
                    m.Person.UpdateEnvelopeOption(CurrentDatabase, value.ToInt());
                    break;
                case "ElectronicStatement":
                    m.Person.UpdateElectronicStatement(CurrentDatabase, value.ToBool());
                    break;
            }
            return new EmptyResult();
        }

        [HttpGet, Route("InlineCodes/{name}")]
        public ActionResult InlineCodes(string name)
        {
            var q = from v in new List<string>()
                    select new { value = "", text = "" };
            switch (name)
            {
                case "ContributionOptions":
                case "EnvelopeOptions":
                    q = from c in CurrentDatabase.EnvelopeOptions
                        select new { value = c.Id.ToString(), text = c.Description };
                    break;
            }
            var j = JsonConvert.SerializeObject(q.ToArray());
            return Content(j);
        }
    }
}
