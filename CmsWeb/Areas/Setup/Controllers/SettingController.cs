using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [ValidateInput(false)]
    [RouteArea("Setup", AreaPrefix = "Setting"), Route("{action}/{id?}")]
    public class SettingController : CmsStaffController
    {
        [Route("~/Settings")]
        public ActionResult Index()
        {
            var m = DbUtil.Db.Settings.AsEnumerable();
            return View(m);
        }

        [HttpPost]
        public ActionResult Create(string id)
        {
            var m = new Setting { Id = id };
            DbUtil.Db.Settings.InsertOnSubmit(m);
            DbUtil.Db.SubmitChanges();
            DbUtil.Db.SetSetting(id, null);
            return Redirect("/Setting/");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            DbUtil.Db.SetSetting(id, value);
            DbUtil.Db.SubmitChanges();
            var c = new ContentResult();
            c.Content = value;
            return c;
        }

        [HttpPost]
        public EmptyResult Delete(string id)
        {
            id = id.Substring(1);
            var set = DbUtil.Db.Settings.SingleOrDefault(m => m.Id == id);
            if (set == null)
                return new EmptyResult();
            DbUtil.Db.Settings.DeleteOnSubmit(set);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
        public ActionResult Batch(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                var q = from s in DbUtil.Db.Settings
                        orderby s.Id
                        select "{0}:\t{1}".Fmt(s.Id, s.SettingX);
                ViewData["text"] = string.Join("\n", q.ToArray());
                return View();
            }
            var batch = from s in text.Split('\n')
                        where s.HasValue()
                        let a = s.SplitStr(":", 2)
                        select new { name = a[0], value = a[1].Trim() };

            var settings = DbUtil.Db.Settings.ToList();

            var upds = from s in settings
                       join b in batch on s.Id equals b.name
                       select new { s = s, value = b.value };

            foreach (var pair in upds)
                pair.s.SettingX = pair.value;

            var adds = from b in batch
                       join s in settings on b.name equals s.Id into g
                       from s in g.DefaultIfEmpty()
                       where s == null
                       select b;

            foreach (var b in adds)
                DbUtil.Db.Settings.InsertOnSubmit(new Setting { Id = b.name, SettingX = b.value });

            var dels = from s in settings
                       where !batch.Any(b => b.name == s.Id)
                       select s;

            DbUtil.Db.Settings.DeleteAllOnSubmit(dels);
            DbUtil.Db.SubmitChanges();

            return RedirectToAction("Index");
        }
        public ActionResult RemoveFakePeople()
        {
            DbUtil.Db.PurgeAllPeopleInCampus(99);
            return Content(@"<a href=""/"">home</a><br/><h2>Done</h2>");
        }
        [HttpPost]
        public ContentResult DeleteImage(string id)
        {
            var iid = id.Substring(1).ToInt();
            var img = ImageData.DbUtil.Db.Images.SingleOrDefault(m => m.Id == iid);
            if (img == null)
                return Content("#r0");
            ImageData.DbUtil.Db.Images.DeleteOnSubmit(img);
            ImageData.DbUtil.Db.SubmitChanges();
            return Content("#r" + iid);
        }
    }
}






