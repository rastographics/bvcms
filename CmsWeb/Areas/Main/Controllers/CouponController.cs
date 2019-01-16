using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [Authorize(Roles = "Coupon")]
    [RouteArea("Main", AreaPrefix = "Coupon"), Route("{action}/{id?}")]
    public class CouponController : CmsStaffController
    {
        public CouponController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Coupons")]
        public ActionResult Index()
        {
            var m = new CouponModel();
            return View(m);
        }

        [HttpPost]
        public ActionResult Create(CouponModel m)
        {
            m.name = m.name?.Trim() ?? "";
            m.couponcode = m.couponcode?.Trim() ?? "";
            if (m.couponcode.HasValue())
            {
                if (CouponModel.IsExisting(m.couponcode))
                {
                    return Content("code already exists");
                }
            }

            m.CreateCoupon();
            return View(m);
        }

        public ActionResult Cancel(string id)
        {
            var c = CurrentDatabase.Coupons.SingleOrDefault(cp => cp.Id == id);
            if (!c.Canceled.HasValue)
            {
                c.Canceled = DateTime.Now;
                CurrentDatabase.SubmitChanges();
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
            {
                return m.CouponsAsDataTable().ToExcel("Coupons.xlsx");
            }

            return View(m);
        }
    }
}
