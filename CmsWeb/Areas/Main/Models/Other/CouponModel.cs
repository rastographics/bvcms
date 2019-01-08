/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class CouponModel
    {
        public string regid { get; set; }
        public decimal amount { get; set; }
        public string name { get; set; }
        public string couponcode { get; set; }

        public int useridfilter { get; set; }
        public string regidfilter { get; set; }
        public string usedfilter { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }

        public CouponModel() { }
        public string Registration()
        {
            return OnlineRegs().Single(r => r.Value == regid).Text;
        }

        public IEnumerable<CouponInfo> Coupons()
        {
            var roles = DbUtil.Db.CurrentUser.UserRoles.Select(uu => uu.Role.RoleName).ToArray();
            var q = from c in DbUtil.Db.Coupons
                    let o = c.Organization
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where c.DivOrg == regidfilter || regidfilter == "0" || regidfilter == null
                    where c.UserId == useridfilter || useridfilter == 0
                    select c;
            switch (usedfilter)
            {
                case "Used":
                    q = q.Where(c => c.Used != null && c.Canceled == null);
                    break;
                case "UnUsed":
                    q = q.Where(c => c.Used == null && c.Canceled == null);
                    break;
                case "Canceled":
                    q = q.Where(c => c.Canceled != null);
                    break;
            }
            if (name.HasValue())
            {
                q = q.Where(c => c.Name.Contains(name) || c.Person.Name.Contains(name));
            }

            if (startdate.HasValue() && enddate.HasValue())
            {
                DateTime bd;
                DateTime ed;
                if (DateTime.TryParse(startdate, out bd) && DateTime.TryParse(enddate, out ed))
                {
                    q = q.Where(c => c.Created.Date >= bd && c.Created.Date <= ed);
                }
            }

            var q2 = from c in q
                     orderby c.Created descending
                     select new CouponInfo
                     {
                         Amount = c.Amount,
                         Canceled = c.Canceled,
                         Code = c.Id,
                         Created = c.Created,
                         OrgDivName = c.OrgId != null ? c.Organization.OrganizationName : c.Division.Name,
                         Used = c.Used,
                         PeopleId = c.PeopleId,
                         Name = c.Name,
                         Person = c.Person.Name,
                         UserId = c.UserId,
                         UserName = c.User.Name,
                         RegAmt = c.RegAmount
                     };
            return q2.Take(200);
        }

        public DataTable CouponsAsDataTable()
        {
            var roles = DbUtil.Db.CurrentUser.UserRoles.Select(uu => uu.Role.RoleName).ToArray();
            var q = from c in DbUtil.Db.Coupons
                    let o = c.Organization
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where c.DivOrg == regidfilter || regidfilter == "0" || regidfilter == null
                    where c.UserId == useridfilter || useridfilter == 0
                    select c;
            switch (usedfilter)
            {
                case "Used":
                    q = q.Where(c => c.Used != null && c.Canceled == null);
                    break;
                case "UnUsed":
                    q = q.Where(c => c.Used == null && c.Canceled == null);
                    break;
                case "Canceled":
                    q = q.Where(c => c.Canceled != null);
                    break;
            }
            if (name.HasValue())
            {
                q = q.Where(c => c.Name.Contains(name) || c.Person.Name.Contains(name));
            }

            if (startdate.HasValue() && enddate.HasValue())
            {
                DateTime bd;
                DateTime ed;
                if (DateTime.TryParse(startdate, out bd) && DateTime.TryParse(enddate, out ed))
                {
                    q = q.Where(c => c.Created.Date >= bd && c.Created.Date <= ed);
                }
            }

            var q2 = from c in q
                     orderby c.Created descending
                     select new CouponInfo
                     {
                         Amount = c.Amount ?? 0,
                         Canceled = c.Canceled ?? DateTime.Parse("1/1/80"),
                         Code = c.Id,
                         Created = c.Created,
                         OrgDivName = c.OrgId != null ? c.Organization.OrganizationName : c.Division.Name,
                         Used = c.Used ?? DateTime.Parse("1/1/80"),
                         PeopleId = c.PeopleId ?? 0,
                         Name = c.Name,
                         Person = c.Person.Name,
                         UserId = c.UserId ?? 0,
                         UserName = c.User.Name,
                         RegAmt = c.RegAmount ?? 0
                     };
            return q2.Take(200).ToDataTable();
        }

        public List<SelectListItem> OnlineRegs()
        {
            var roles = DbUtil.Db.CurrentUser.UserRoles.Select(uu => uu.Role.RoleName).ToArray();
            var organizations = from o in DbUtil.Db.Organizations
                                where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                                select o;
            var orgregtypes = new[]
            {
                RegistrationTypeCode.JoinOrganization,
                RegistrationTypeCode.UserSelects,
                RegistrationTypeCode.ComputeOrgByAge,
            };

            var q = (from o in organizations
                     where orgregtypes.Contains(o.RegistrationTypeId.Value)
                     where (o.ClassFilled ?? false) != true
                     where (o.RegistrationClosed ?? false) == false
                     select new { DivisionName = o.Division.Name, o.OrganizationName, o.OrganizationId, o.RegistrationTypeId }).ToList();

            var qq = from i in q
                     let os = DbUtil.Db.CreateRegistrationSettings(i.OrganizationId)
                     where orgregtypes.Contains(i.RegistrationTypeId.Value)
                         || os.Fee > 0
                         || os.AskItems.Where(aa => aa.Type == "AskDropdown").Any(aa => ((AskDropdown)aa).list.Any(dd => dd.Fee > 0))
                         || os.AskItems.Where(aa => aa.Type == "AskCheckboxes").Any(aa => ((AskCheckboxes)aa).list.Any(dd => dd.Fee > 0))
                     select new SelectListItem
                     {
                         Text = i.DivisionName + ":" + i.OrganizationName,
                         Value = "org." + i.OrganizationId
                     };

            var list = qq.OrderBy(n => n.Text).ToList();

            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public List<SelectListItem> Users()
        {
            var q = from c in DbUtil.Db.Coupons
                    where c.UserId != null
                    group c by c.UserId into g
                    select new SelectListItem
                    {
                        Value = g.Key.ToString(),
                        Text = g.First().User.Name,
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public List<SelectListItem> CouponStatus()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "(not specified)" },
                new SelectListItem { Text = "Used" },
                new SelectListItem { Text = "UnUsed" },
                new SelectListItem { Text = "Canceled" },
            };
        }

        public static bool IsExisting(string code)
        {
            var existing = DbUtil.Db.Coupons.Any(cp => cp.Id == code && cp.Used == null && cp.Canceled == null);
            return existing;
        }

        public Coupon CreateCoupon()
        {
            var code = couponcode;
            if (!couponcode.HasValue())
            {
                do
                {
                    code = Util.RandomPassword(12);
                }
                while (IsExisting(code));
            }

            var c = new Coupon
            {
                Id = code,
                Created = DateTime.Now,
                Amount = amount,
                Name = name,
                UserId = Util.UserId,
            };
            SetDivOrgIds(c);
            DbUtil.Db.Coupons.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            couponcode = Util.fmtcoupon(c.Id);

            return c;
        }

        private void SetDivOrgIds(Coupon c)
        {
            if (regid.HasValue())
            {
                var a = regid.Split('.');
                switch (a[0])
                {
                    case "org":
                        c.OrgId = a[1].ToInt();
                        break;
                    case "div":
                        c.DivId = a[1].ToInt();
                        break;
                }
            }
        }

        public class CouponInfo
        {
            public string Code { get; set; }
            public string Coupon => Util.fmtcoupon(Code);
            public string OrgDivName { get; set; }
            public DateTime Created { get; set; }
            public DateTime? Used { get; set; }
            public DateTime? Canceled { get; set; }
            public decimal? Amount { get; set; }
            public decimal? RegAmt { get; set; }
            public int? PeopleId { get; set; }
            public string Name { get; set; }
            public string Person { get; set; }
            public int? UserId { get; set; }
            public string UserName { get; set; }
        }
    }
}
