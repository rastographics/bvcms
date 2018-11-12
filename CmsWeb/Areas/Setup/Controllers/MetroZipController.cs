using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "MetroZips"), Route("{action=index}/{id?}")]
    public class MetroZipController : CmsStaffController
    {
        public MetroZipController(RequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index(string msg)
        {
            var m = DbUtil.Db.Zips.AsEnumerable();
            ViewData["msg"] = msg;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string zipcode)
        {
            var zip = DbUtil.Db.Zips.SingleOrDefault(mz => mz.ZipCode == zipcode);
            if (zip == null)
            {
                var m = new Zip { ZipCode = zipcode };
                DbUtil.Db.Zips.InsertOnSubmit(m);
                DbUtil.Db.SubmitChanges();
            }
            return Redirect($"/MetroZips/#{zipcode}");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Edit(string id, string value)
        {
            id = id.Substring(1);
            var zip = DbUtil.Db.Zips.SingleOrDefault(m => m.ZipCode == id);
            zip.MetroMarginalCode = value.ToInt();
            DbUtil.Db.SubmitChanges();
            var c = new ContentResult();
            c.Content = zip.ResidentCode.Description;
            return c;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public EmptyResult Delete(string id)
        {
            id = id.Substring(1);
            var zip = DbUtil.Db.Zips.SingleOrDefault(m => m.ZipCode == id);
            if (zip == null)
            {
                return new EmptyResult();
            }

            DbUtil.Db.Zips.DeleteOnSubmit(zip);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateMetroZips()
        {
            DbUtil.Db.UpdateResCodes();
            return Content("All addresses were updated.");
        }

        public JsonResult ResidentCodes()
        {
            var q = from c in DbUtil.Db.ResidentCodes
                    select new
                    {
                        value = c.Id,
                        text = c.Description,
                    };

            return Json(q.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
