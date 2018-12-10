using CmsData;
using CmsWeb.Lifecycle;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "MetroZips"), Route("{action=index}/{id?}")]
    public class MetroZipController : CmsStaffController
    {
        public MetroZipController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index(string msg)
        {
            var m = CurrentDatabase.Zips.AsEnumerable();
            ViewData["msg"] = msg;
            ViewBag.TotalZipCodesAdded = 0;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string zipcode)
        {
            var zip = CurrentDatabase.Zips.SingleOrDefault(mz => mz.ZipCode == zipcode);
            if (zip == null)
            {
                var m = new Zip { ZipCode = zipcode };
                CurrentDatabase.Zips.InsertOnSubmit(m);
                CurrentDatabase.SubmitChanges();
            }
            return Redirect($"/MetroZips/#{zipcode}");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public int CreateRange(int startwith, int endwith, int marginalcode)
        {
            int totalZipCodesAdded = CurrentDatabase.CreateZipCodesRange(startwith, endwith, marginalcode);
            ViewBag.TotalZipCodesAdded = totalZipCodesAdded;
            return 6;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Edit(string id, string value)
        {
            id = id.Substring(1);
            var zip = CurrentDatabase.Zips.SingleOrDefault(m => m.ZipCode == id);
            zip.MetroMarginalCode = value.ToInt();
            CurrentDatabase.SubmitChanges();
            var c = new ContentResult();
            c.Content = zip.ResidentCode.Description;
            return c;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public EmptyResult Delete(string id)
        {
            id = id.Substring(1);
            var zip = CurrentDatabase.Zips.SingleOrDefault(m => m.ZipCode == id);
            if (zip == null)
            {
                return new EmptyResult();
            }

            CurrentDatabase.Zips.DeleteOnSubmit(zip);
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateMetroZips()
        {
            CurrentDatabase.UpdateResCodes();
            return Content("All addresses were updated.");
        }

        public JsonResult ResidentCodes()
        {
            var q = from c in CurrentDatabase.ResidentCodes
                    select new
                    {
                        value = c.Id,
                        text = c.Description,
                    };

            return Json(q.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
