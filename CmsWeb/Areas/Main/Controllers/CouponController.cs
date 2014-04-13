using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using CmsData;
using CmsWeb.Models;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Areas.Main.Controllers
{
    [Authorize(Roles="Coupon")]
    [RouteArea("Main", AreaPrefix="Coupon"), Route("{action}/{id?}")]
    public class CouponController : CmsStaffController
    {
        [Route("~/Coupons")]
        public ActionResult Index()
        {
            var m = new CouponModel();
            return View(m);
        }
        [HttpPost]
        public ActionResult Create(CouponModel m)
        {
            if (m.couponcode.HasValue())
                if (CouponModel.IsExisting(m.couponcode))
                    return Content("code already exists");
            m.CreateCoupon();
            return View(m);
        }
        public ActionResult Cancel(string id)
        {
            var c = DbUtil.Db.Coupons.SingleOrDefault(cp => cp.Id == id);
            if (!c.Canceled.HasValue)
            {
                c.Canceled = DateTime.Now;
                DbUtil.Db.SubmitChanges();
            }
            var m = new CouponModel();
            return View("List", m);
        }
        public ActionResult List()
        {
            var m = new CouponModel();
            return View(m);
        }
        [HttpPost]
        public ActionResult List(string submit, CouponModel m)
        {
            if (submit == "Excel")
                return new CouponExcelResult(m);
            return View(m);
        }
    }
    public class CouponExcelResult : ActionResult
    {
        private CouponModel m;
        public CouponExcelResult(CouponModel m)
        {
            this.m = m;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=CMSOrganizations.xls");
            Response.Charset = "";
            var d = m.Coupons2();
            var dg = new DataGrid();
            dg.DataSource = d;
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
        }
    }
}
