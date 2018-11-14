using CmsWeb.Areas.OnlineReg.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        [HttpPost]
        public ActionResult ApplyCoupon(PaymentForm pf)
        {
            OnlineRegModel m = null;
            if (pf.PayBalance == false)
            {
                m = OnlineRegModel.GetRegistrationFromDatum(pf.DatumId);
                if (m == null)
                {
                    return Json(new { error = "coupon not find your registration" });
                }

                m.ParseSettings();
            }

            if (!pf.Coupon.HasValue())
            {
                return Json(new { error = "empty coupon" });
            }

            var coupon = pf.Coupon.ToUpper().Replace(" ", "");
            var admincoupon = CurrentDatabase.Setting("AdminCoupon", "ifj4ijweoij").ToUpper().Replace(" ", "");
            if (coupon == admincoupon)
            {
                if (pf.PayBalance)
                {
                    var tic = pf.CreateTransaction(CurrentDatabase, pf.AmtToPay);
                    return Json(new { confirm = $"/onlinereg/ConfirmDuePaid/{tic.Id}?TransactionID=AdminCoupon&Amount={tic.Amt}" });
                }
                else
                {
                    return Json(new { confirm = $"/OnlineReg/Confirm/{pf.DatumId}?TransactionId=AdminCoupon" });
                }
            }

            var c = CurrentDatabase.Coupons.SingleOrDefault(cp => cp.Id == coupon);
            if (c == null)
            {
                return Json(new { error = "coupon not found" });
            }

            if (pf.OrgId.HasValue && c.Organization != null && c.Organization.OrgPickList.HasValue())
            {
                var a = c.Organization.OrgPickList.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
                if (!a.Contains(pf.OrgId.Value))
                {
                    return Json(new { error = "coupon and org do not match" });
                }
            }
            else if (pf.OrgId != c.OrgId)
            {
                return Json(new { error = "coupon and org do not match" });
            }

            if (c.Used.HasValue && c.Id.Length == 12)
            {
                return Json(new { error = "coupon already used" });
            }

            if (c.Canceled.HasValue)
            {
                return Json(new { error = "coupon canceled" });
            }

            var ti = pf.CreateTransaction(CurrentDatabase, Math.Min(c.Amount ?? 0m, pf.AmtToPay ?? 0m));
            if (m != null) // Start this transaction in the chain
            {
                m.HistoryAdd("ApplyCoupon");
                m.TranId = ti.OriginalId;
                m.UpdateDatum();
            }
            var tid = $"Coupon({Util.fmtcoupon(coupon)})";

            if (!pf.PayBalance)
            {
                OnlineRegModel.ConfirmDuePaidTransaction(ti, tid, false);
            }

            var msg = $"<i class='red'>Your coupon for {c.Amount:n2} has been applied, your balance is now {ti.Amtdue:n2}</i>.";
            if (ti.Amt < pf.AmtToPay)
            {
                msg += "You still must complete this transaction with a payment";
            }

            if (m != null)
            {
                m.UseCoupon(ti.TransactionId, ti.Amt ?? 0);
            }
            else
            {
                c.UseCoupon(ti.FirstTransactionPeopleId(), ti.Amt ?? 0);
            }

            CurrentDatabase.SubmitChanges();

            if (pf.PayBalance)
            {
                return Json(new { confirm = $"/onlinereg/ConfirmDuePaid/{ti.Id}?TransactionID=Coupon({Util.fmtcoupon(coupon)})&Amount={ti.Amt}" });
            }

            pf.AmtToPay -= ti.Amt;
            if (pf.AmtToPay <= 0)
            {
                return Json(new { confirm = $"/OnlineReg/Confirm/{pf.DatumId}?TransactionId={"Coupon"}" });
            }

            return Json(new { tiamt = pf.AmtToPay, amtdue = ti.Amtdue, amt = pf.AmtToPay.ToString2("N2"), msg });
        }

        [HttpPost]
        public ActionResult PayWithCoupon(int id, string Coupon)
        {
            if (!Coupon.HasValue())
            {
                return Json(new { error = "empty coupon" });
            }

            var m = OnlineRegModel.GetRegistrationFromDatum(id);
            m.ParseSettings();
            var coupon = Coupon.ToUpper().Replace(" ", "");
            var admincoupon = CurrentDatabase.Setting("AdminCoupon", "ifj4ijweoij").ToUpper().Replace(" ", "");
            if (coupon == admincoupon)
            {
                return Json(new { confirm = $"/onlinereg/Confirm/{id}?TransactionID=Coupon(Admin)" });
            }

            var c = CurrentDatabase.Coupons.SingleOrDefault(cp => cp.Id == coupon);
            if (c == null)
            {
                return Json(new { error = "coupon not found" });
            }

            if (m.Orgid != c.OrgId)
            {
                return Json(new { error = "coupon org not match" });
            }

            if (DateTime.Now.Subtract(c.Created).TotalHours > 24)
            {
                return Json(new { error = "coupon expired" });
            }

            if (c.Used.HasValue && c.Id.Length == 12)
            {
                return Json(new { error = "coupon already used" });
            }

            if (c.Canceled.HasValue)
            {
                return Json(new { error = "coupon canceled" });
            }

            return Json(new
            {
                confirm = $"/onlinereg/confirm/{id}?TransactionID=Coupon({Util.fmtcoupon(coupon)})"
            });
        }

        [HttpPost]
        public ActionResult PayWithCoupon2(int id, string Coupon, decimal Amount)
        {
            if (!Coupon.HasValue())
            {
                return Json(new { error = "empty coupon" });
            }

            var ti = CurrentDatabase.Transactions.SingleOrDefault(tt => tt.Id == id);
            var coupon = Coupon.ToUpper().Replace(" ", "");
            var admincoupon = CurrentDatabase.Setting("AdminCoupon", "ifj4ijweoij").ToUpper().Replace(" ", "");
            if (coupon == admincoupon)
            {
                return Json(new { confirm = $"/onlinereg/ConfirmDuePaid/{id}?TransactionID=Coupon(Admin)&Amount={Amount}" });
            }

            var c = CurrentDatabase.Coupons.SingleOrDefault(cp => cp.Id == coupon);
            if (c == null)
            {
                return Json(new { error = "coupon not found" });
            }

            if (ti.OrgId != c.OrgId)
            {
                return Json(new { error = "coupon org not match" });
            }

            if (DateTime.Now.Subtract(c.Created).TotalHours > 24)
            {
                return Json(new { error = "coupon expired" });
            }

            if (c.Used.HasValue)
            {
                return Json(new { error = "coupon already used" });
            }

            if (c.Canceled.HasValue)
            {
                return Json(new { error = "coupon canceled" });
            }

            if (c.Amount.HasValue)
            {
                Amount = c.Amount.Value;
            }

            return Json(new
            {
                confirm = $"/onlinereg/ConfirmDuePaid/{id}?TransactionID=Coupon({Util.fmtcoupon(coupon)})&Amount={Amount}"
            });
        }

        // todo: I hope we can get rid of this method
        [HttpPost]
        public ActionResult PayWithCouponOld(int id, string Coupon, decimal Amount)
        {
            if (!Coupon.HasValue())
            {
                return Json(new { error = "empty coupon" });
            }

            var ed = CurrentDatabase.ExtraDatas.SingleOrDefault(e => e.Id == id);
            var ti = Util.DeSerialize<TransactionInfo>(ed.Data.Replace("CMSWeb.Models", "CmsWeb.Models"));
            var coupon = Coupon.ToUpper().Replace(" ", "");
            var admincoupon = CurrentDatabase.Setting("AdminCoupon", "ifj4ijweoij").ToUpper().Replace(" ", "");
            if (coupon == admincoupon)
            {
                return Json(new { confirm = $"/onlinereg/Confirm2/{id}?TransactionID=Coupon(Admin)&Amount={Amount}" });
            }

            var c = CurrentDatabase.Coupons.SingleOrDefault(cp => cp.Id == coupon);
            if (c == null)
            {
                return Json(new { error = "coupon not found" });
            }

            if (ti.orgid != c.OrgId)
            {
                return Json(new { error = "coupon org not match" });
            }

            if (DateTime.Now.Subtract(c.Created).TotalHours > 24)
            {
                return Json(new { error = "coupon expired" });
            }

            if (c.Used.HasValue)
            {
                return Json(new { error = "coupon already used" });
            }

            if (c.Canceled.HasValue)
            {
                return Json(new { error = "coupon canceled" });
            }

            return Json(new
            {
                confirm = $"/onlinereg/Confirm2/{id}?TransactionID=Coupon({Util.fmtcoupon(coupon)})&Amount={Amount}"
            });
        }
    }
}
