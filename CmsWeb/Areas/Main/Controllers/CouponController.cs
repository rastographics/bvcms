using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [Authorize(Roles = "Coupon")]
    [RouteArea("Main", AreaPrefix = "Coupon"), Route("{action}/{id?}")]
    public class CouponController : CmsStaffController
    {
        private CouponModel CouponModel = new CouponModel();

        public CouponController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Coupons")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        [Route("~/Coupon/Create")]
        public JsonResult Create([System.Web.Http.FromBody] CouponModel m)
        {
            List<CouponResponseModel> response = new List<CouponResponseModel>();
            try
            {
                m.name = m.name?.Trim() ?? "";
                m.couponcode = m.couponcode?.Trim() ?? "";
                if (m.couponcode.HasValue())
                {
                    if (CouponModel.IsExisting(m.couponcode))
                    {
                        response.Add(new CouponResponseModel { Message = "code already exists" });
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }

                m.CreateCoupon();
                response.Add(new CouponResponseModel { Message = "Ok" });
            }
            catch (Exception ex)
            {
                response.Add(new CouponResponseModel { Message = ex.Message });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Coupon/Cancel")]
        public JsonResult Cancel([System.Web.Http.FromBody] string id)
        {
            List<CouponResponseModel> response = new List<CouponResponseModel>();
            try
            {
                var c = CurrentDatabase.Coupons.SingleOrDefault(cp => cp.Id == id);
                if (!c.Canceled.HasValue)
                {
                    c.Canceled = DateTime.Now;
                    CurrentDatabase.SubmitChanges();
                }
                var m = new CouponModel();

                response.Add(new CouponResponseModel { Message = "Ok" });
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                response.Add(new CouponResponseModel { Message = ex.Message });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Coupon/GetCoupons")]
        public JsonResult GetCoupons([System.Web.Http.FromBody] CouponModel m)
        {
            return Json(CouponModel.GetCoupons(m), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Coupon/DownloadExcel/{name}/{useridfilter}/{regidfilter}/{startdateIso}/{enddateIso}/{usedfilter}")]
        public ActionResult DownloadExcel(string name, int useridfilter, string regidfilter, string startdateIso, string enddateIso, string usedfilter)
        {
            CouponModel ExcelModel = new CouponModel();
            ExcelModel.name = name == "null" ? null : regidfilter;
            ExcelModel.useridfilter = useridfilter;
            ExcelModel.regidfilter = regidfilter == "null" ? null : regidfilter;
            ExcelModel.startdate = startdateIso == "null" ? null : startdateIso;
            ExcelModel.enddate = enddateIso == "null" ? null : enddateIso;
            ExcelModel.usedfilter = usedfilter == "null" ? null : usedfilter;

            return ExcelModel.CouponsAsDataTable().ToExcel("Coupons.xlsx");
        }

        [HttpGet]
        [Route("~/Coupon/GetUsers")]
        public JsonResult GetUsers()
        {
            return Json(CouponModel.Users(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Coupon/GetOnlineRegs")]
        public JsonResult GetOnlineRegs()
        {
            return Json(CouponModel.OnlineRegs(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Coupon/GetCouponStatus")]
        public JsonResult GetCouponStatus()
        {
            return Json(CouponModel.CouponStatus(), JsonRequestBehavior.AllowGet);
        }
    }
}
