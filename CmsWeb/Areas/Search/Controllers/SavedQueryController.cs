using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaUrl = "SavedQuery2")]
    public class SavedQueryController : CmsStaffController
    {
        [GET("SavedQuery2/")]
        public ActionResult Index()
        {
            var m = new SavedQueryModel();
            return View(m);
        }
        [POST("SavedQuery2/PostData/{pk:int}/{name}/{value}")]
        public ActionResult PostData(int pk, string name, string value)
        {
            var c = DbUtil.Db.QueryBuilderClauses.SingleOrDefault(cc => cc.QueryId == pk);
            switch (name.ToLower())
            {
                case "description":
                    c.Description = value;
                    break;
                case "owner":
                    c.SavedBy = value;
                    break;
                case "public":
                    c.IsPublic = value == "yes";
                    break;
                case "delete":
                    DbUtil.Db.DeleteQueryBuilderClauseOnSubmit(c);
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return Content(value);
        }
        [POST("SavedQuery2/Results")]
        public ActionResult Results(SavedQueryModel m)
        {
            return View(m);
        }
    }
}
