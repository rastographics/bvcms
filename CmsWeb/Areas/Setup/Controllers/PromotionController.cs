using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "PromotionSetup"), Route("{action=index}/{id?}")]
    public class PromotionController : CmsStaffController
    {
        public PromotionController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var m = new Models.PromotionModel();
            return View(m);
        }

        [HttpPost]
        public ActionResult Create()
        {
            var m = new Promotion();
            CurrentDatabase.Promotions.InsertOnSubmit(m);
            CurrentDatabase.SubmitChanges();
            return Redirect("/PromotionSetup/");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            var iid = id.Substring(1).ToInt();
            var c = new ContentResult();
            c.Content = value;
            var pro = CurrentDatabase.Promotions.SingleOrDefault(p => p.Id == iid);
            if (pro == null)
            {
                return c;
            }

            switch (id.Substring(0, 1))
            {
                case "d":
                    pro.Description = value;
                    break;
                case "s":
                    pro.Sort = value;
                    break;
            }
            CurrentDatabase.SubmitChanges();
            return c;
        }

        [HttpPost]
        public ContentResult EditDiv(string id, string value)
        {
            var iid = id.Substring(1).ToInt();
            var pro = CurrentDatabase.Promotions.SingleOrDefault(m => m.Id == iid);
            var fts = id.Substring(0, 1);
            switch (fts)
            {
                case "f":
                    pro.FromDivId = value.ToInt();
                    break;
                case "t":
                    pro.ToDivId = value.ToInt();
                    break;
            }
            CurrentDatabase.SubmitChanges();
            var c = new ContentResult();
            if (fts == "f")
            {
                c.Content = pro.FromDivision.Name;
            }
            else if (fts == "t")
            {
                c.Content = pro.ToDivision.Name;
            }

            return c;
        }

        [HttpPost]
        public EmptyResult Delete(string id)
        {
            var iid = id.Substring(1).ToInt();
            var pro = CurrentDatabase.Promotions.SingleOrDefault(m => m.Id == iid);
            if (pro == null)
            {
                return new EmptyResult();
            }

            CurrentDatabase.Promotions.DeleteOnSubmit(pro);
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        public JsonResult DivisionCodes(int id)
        {
            var q = from c in CurrentDatabase.Divisions
                    orderby c.Name
                    where c.DivOrgs.Any(od => od.Organization.DivOrgs.Any(od2 => od2.Division.ProgId == id))
                    select new
                    {
                        value = c.Id,
                        text = c.Name,
                    };
            return Json(q.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Promote(string id)
        {
            var iid = id.Substring(1).ToInt();
            var m = new Models.PromotionModel();
            m.Promote(iid);
            return RedirectToAction("Index");
        }

    }
}
