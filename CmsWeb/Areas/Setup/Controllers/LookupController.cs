using CmsData;
using CmsWeb.Lifecycle;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin,Finance")]
    [RouteArea("Setup", AreaPrefix = "Lookup"), Route("{action}/{id?}")]
    public class LookupController : CmsStaffController
    {
        public LookupController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public class Row
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public bool? Hardwired { get; set; }
        }
        [Route("~/Lookups")]
        [Route("~/Lookup/{id}")]
        public ActionResult Index(string id)
        {
            if (!id.HasValue())
            {
                ViewData["MeetingCategories"] = CurrentDatabase.Setting("AttendanceUseMeetingCategory", false);
                return View("list");
            }

            if (!User.IsInRole("Admin") && string.Compare(id, "funds", ignoreCase: true) != 0)
            {
                return Content("must be admin");
            }

            ViewData["type"] = id;
            ViewData["description"] = Regex.Replace(id, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            var q = CurrentDatabase.ExecuteQuery<Row>("select * from lookup." + id);

            // hide the add button on appropriate views.
            switch (id)
            {
                case "AddressType":
                case "EnvelopeOption":
                case "OrganizationStatus":
                case "BundleStatusTypes":
                case "ContributionStatus":
                    ViewData["HideAdd"] = true;
                    break;
                case "Gender":
                    if (!CurrentDatabase.Setting("AllowNewGenders"))
                    {
                        ViewData["HideAdd"] = true;
                    }

                    break;

            }

            return View(q);
        }

        [HttpPost]
        public ActionResult Create(int? id, string type)
        {
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Id must be a number.";
            }
            else
            {
                var q = CurrentDatabase.ExecuteQuery<Row>("select * from lookup." + type + " where id = {0}", id);
                if (!q.Any())
                {
                    CurrentDatabase.ExecuteCommand("insert lookup." + type + " (id, code, description) values ({0}, '', '')", id);
                }
            }

            return Redirect($"/Lookup/{type}/#{id}");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            var a = id.SplitStr(".");
            var iid = a[0].Substring(1).ToInt();
            if (id.StartsWith("t"))
            {
                CurrentDatabase.ExecuteCommand(
                    "update lookup." + a[1] + " set Description = {0} where id = {1}",
                    value, iid);
            }
            else if (id.StartsWith("c"))
            {
                CurrentDatabase.ExecuteCommand(
                    "update lookup." + a[1] + " set Code = {0} where id = {1}",
                    value, iid);
            }

            return Content(value);
        }

        [HttpPost]
        public ActionResult Delete(string id, string type)
        {
            try
            {
                var iid = id.Substring(1).ToInt();
                CurrentDatabase.ExecuteCommand("delete lookup." + type + " where id = {0}", iid);
                return new EmptyResult();
            }
            catch (SqlException)
            {
                return Json(new { error = $"Cannot delete {type} because it is in use" });
            }
        }
    }
}
