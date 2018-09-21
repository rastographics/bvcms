/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class PostBundleModel
    {
        private BundleHeader bundle;

        public PostBundleModel()
        {
        }

        public PostBundleModel(int id)
        {
            this.id = id;
            PLNT = Bundle.BundleHeaderTypeId == BundleTypeCode.Pledge ? "PL" :
                Bundle.BundleHeaderTypeId == BundleTypeCode.GiftsInKind ? "GK" :
                    Bundle.BundleHeaderTypeId == BundleTypeCode.Stock ? "SK" : "CN";
        }

        public int id { get; set; }
        public int? editid { get; set; }
        public string pid { get; set; }
        public decimal? amt { get; set; }
        public int? splitfrom { get; set; }
        public int fund { get; set; }
        public string PLNT { get; set; }
        public string notes { get; set; }
        public string checkno { get; set; }
        public DateTime? contributiondate { get; set; }
        public string FundName { get; set; }
        public bool DefaultFundIsPledge { get; set; }
        public int? campusid { get; set; }
        public int imageid { get; set; }

        public BundleHeader Bundle
        {
            get
            {
                if (bundle == null)
                {
                    bundle = DbUtil.Db.BundleHeaders.SingleOrDefault(bh => bh.BundleHeaderId == id);
                    if (bundle?.FundId != null)
                    {
                        FundName = bundle.Fund.FundName;
                        DefaultFundIsPledge = bundle.Fund.FundPledgeFlag;
                    }
                }
                return bundle;
            }
        }

        public decimal TotalItems
        {
            get { return Bundle.BundleDetails.Sum(dd => dd.Contribution.ContributionAmount) ?? 0; }
        }

        public int TotalCount => Bundle.BundleDetails.Count();

        public IEnumerable<ContributionInfo> FetchContributions(int? cid = null)
        {
            var q = from d in DbUtil.Db.BundleDetails
                    where d.BundleHeaderId == id || cid != null
                    where cid == null || d.ContributionId == cid
                    let sort = d.BundleSort1 > 0 ? d.BundleSort1 : d.BundleDetailId
                    orderby sort descending, d.ContributionId ascending
                    select new ContributionInfo
                    {
                        ContributionId = d.ContributionId,
                        BundleTypeId = d.BundleHeader.BundleHeaderTypeId,
                        PeopleId = d.Contribution.PeopleId,
                        Name = d.Contribution.Person.Name2
                               + (d.Contribution.Person.DeceasedDate.HasValue ? " [DECEASED]" : ""),
                        Amt = d.Contribution.ContributionAmount,
                        Fund = d.Contribution.ContributionFund.FundName,
                        FundId = d.Contribution.FundId,
                        Notes = d.Contribution.ContributionDesc,
                        CheckNo = d.Contribution.CheckNo,
                        eac = d.Contribution.BankAccount,
                        Address = d.Contribution.Person.PrimaryAddress,
                        City = d.Contribution.Person.PrimaryCity,
                        State = d.Contribution.Person.PrimaryState,
                        Zip = d.Contribution.Person.PrimaryZip,
                        Age = d.Contribution.Person.Age,
                        extra = d.Contribution.ExtraDatum.Data,
                        Date = d.Contribution.ContributionDate,
                        PLNT = ContributionTypeCode.SpecialTypes.Contains(d.Contribution.ContributionTypeId) ? d.Contribution.ContributionType.Code : "",
                        memstatus = d.Contribution.Person.MemberStatus.Description,
                        campusid = d.Contribution.CampusId,
                        ImageId = d.Contribution.ImageID
                    };
            var list = q.ToList();
            foreach (var c in list)
            {
                string s = null;
                if (!c.PeopleId.HasValue)
                {
                    s = c.extra ?? "";
                    if (c.eac.HasValue())
                    {
                        s += " not associated";
                    }

                    if (s.HasValue())
                    {
                        c.Name = s;
                    }
                }
            }
            return list;
        }

        public IEnumerable<FundTotal> TotalsByFund()
        {
            var q = from d in DbUtil.Db.BundleDetails
                    where d.BundleHeaderId == id
                    group d by new { d.Contribution.ContributionFund.FundName, d.Contribution.ContributionFund.FundId }
                    into g
                    orderby g.Key.FundName
                    select new FundTotal
                    {
                        FundId = g.Key.FundId,
                        Name = g.Key.FundName,
                        Total = g.Sum(d => d.Contribution.ContributionAmount)
                    };
            return q;
        }

        public IEnumerable<SelectListItem> Funds()
        {
            var fundSortSetting = DbUtil.Db.Setting("SortContributionFundsByFieldName", "FundId");

            var query = DbUtil.Db.ContributionFunds.Where(cf => cf.FundStatusId == 1);

            if (fundSortSetting == "FundName")
            {
                query = query.OrderBy(cf => cf.FundName).ThenBy(cf => cf.FundId);
            }
            else
            {
                query = query.OrderBy(cf => cf.FundId);
            }

            // HACK: Change text based on sorting option for funds. If sorting by name, make it show first otherwise leave the id first to enable selecting by keystroke until ui adjusted
            if (fundSortSetting == "FundId")
            {
                var items = query.ToList().Select(x => new { x.FundId, x.FundName, FundDisplay = $"{x.FundId} . {x.FundName}" });
                return new SelectList(items, "FundId", "FundDisplay", Bundle.FundId);
            }
            else
            {
                var items = query.ToList().Select(x => new { x.FundId, x.FundName, FundDisplay = $"{x.FundName} ({x.FundId})" });
                return new SelectList(items, "FundId", "FundDisplay", Bundle.FundId);
            }
        }

        public object GetNamePidFromId()
        {
            IEnumerable<object> q;
            if (!string.IsNullOrEmpty(pid) && (pid[0] == 'e' || pid[0] == '-'))
            {
                var env = pid.Substring(1).ToInt();
                q = from e in DbUtil.Db.PeopleExtras
                    where e.Field == "EnvelopeNumber"
                    where e.IntValue == env
                    orderby e.Person.Family.HeadOfHouseholdId == e.PeopleId ? 1 : 2
                    select new
                    {
                        e.PeopleId,
                        name = e.Person.Name2 + (e.Person.DeceasedDate.HasValue ? "[DECEASED]" : "")
                    };
            }
            else
            {
                q = from i in DbUtil.Db.People
                    where i.PeopleId == pid.ToInt()
                    select new
                    {
                        i.PeopleId,
                        name = i.Name2 + (i.DeceasedDate.HasValue ? "[DECEASED]" : "")
                    };
            }
            var o = q.FirstOrDefault();
            if (o == null)
            {
                return new { error = "not found" };
            }

            return o;
        }

        public static IEnumerable<NamesInfo> Names(string q, int limit)
        {
            var qp = FindNames(q);

            var rp = from p in qp
                     let spouse = DbUtil.Db.People.SingleOrDefault(ss =>
                         ss.PeopleId == p.SpouseId
                         && ss.ContributionOptionsId == StatementOptionCode.Joint
                         && p.ContributionOptionsId == StatementOptionCode.Joint)
                     orderby p.Name2
                     select new NamesInfo
                     {
                         Pid = p.PeopleId,
                         name = p.Name2,
                         age = p.Age,
                         spouse = spouse.Name,
                         addr = p.PrimaryAddress ?? "",
                         altname = p.AltName,
                     };
            return rp.Take(limit);
        }

        public static IEnumerable<NamesInfo> Names2(string q, int limit)
        {
            var qp = FindNames(q);

            var rp = from p in qp
                     let spouse = DbUtil.Db.People.SingleOrDefault(ss =>
                         ss.PeopleId == p.SpouseId
                         && ss.ContributionOptionsId == StatementOptionCode.Joint
                         && p.ContributionOptionsId == StatementOptionCode.Joint)
                     orderby p.Name2
                     select new NamesInfo
                     {
                         Pid = p.PeopleId,
                         name = p.Name2,
                         age = p.Age,
                         email = p.EmailAddress,
                         spouse = spouse.Name,
                         addr = p.PrimaryAddress ?? "",
                         altname = p.AltName,
                         recent = (from c in p.Contributions
                                   where c.ContributionStatusId == 0
                                   orderby c.ContributionDate descending
                                   select new RecentContribution
                                   {
                                       Amount = c.ContributionAmount,
                                       DateGiven = c.ContributionDate,
                                       CheckNo = c.CheckNo
                                   }).Take(4).ToList()
                     };
            return rp.Take(limit);
        }

        private static IQueryable<Person> FindNames(string q)
        {
            string First, Last;
            var qp = DbUtil.Db.People.AsQueryable();
            qp = from p in qp
                 where p.DeceasedDate == null
                 select p;

            Util.NameSplit(q, out First, out Last);
            var hasfirst = First.HasValue();

            if (q.AllDigits())
            {
                string phone = null;
                if (q.HasValue() && q.AllDigits() && q.Length == 7)
                {
                    phone = q;
                }

                if (phone.HasValue())
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where
                             p.PeopleId == id
                             || p.CellPhone.Contains(phone)
                             || p.Family.HomePhone.Contains(phone)
                             || p.WorkPhone.Contains(phone)
                         select p;
                }
                else
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where p.PeopleId == id
                         select p;
                }
            }
            else
            {
                if (DbUtil.Db.Setting("UseAltnameContains"))
                {
                    qp = from p in qp
                         where
                         (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last) || p.AltName.Contains(Last)
                          || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                         &&
                         (!hasfirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                          p.MiddleName.StartsWith(First)
                          || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                         select p;
                }
                else
                {
                    qp = from p in qp
                         where
                         (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                          || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                         &&
                         (!hasfirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                          p.MiddleName.StartsWith(First)
                          || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                         select p;
                }
            }
            return qp;
        }

        public object ContributionRowData(PostBundleController ctl, int cid, decimal? othersplitamt = null)
        {
            var cinfo = FetchContributions(cid).Single();
            var body = ViewExtensions2.RenderPartialViewToString(ctl, "Row", cinfo);
            var q = from c in DbUtil.Db.Contributions
                    let bh = c.BundleDetails.First().BundleHeader
                    where c.ContributionId == cid
                    select new
                    {
                        row = body,
                        amt = c.ContributionAmount.ToString2("N2"),
                        cid,
                        totalitems = bh.BundleDetails.Sum(d =>
                            d.Contribution.ContributionAmount).ToString2("C2"),
                        diff = ((bh.TotalCash.GetValueOrDefault() + bh.TotalChecks.GetValueOrDefault() + bh.TotalEnvelopes.GetValueOrDefault()) - bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount.GetValueOrDefault())),
                        difference = ((bh.TotalCash.GetValueOrDefault() + bh.TotalChecks.GetValueOrDefault() + bh.TotalEnvelopes.GetValueOrDefault()) - bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount)).ToString2("C2"),
                        itemcount = bh.BundleDetails.Count(),
                        othersplitamt = othersplitamt.ToString2("N2")
                    };
            return q.First();
        }

        public object PostContribution(PostBundleController ctl)
        {
            try
            {
                var bd = new BundleDetail
                {
                    BundleHeaderId = id,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Now
                };
                int type;
                switch (PLNT)
                {
                    case "PL":
                        type = ContributionTypeCode.Pledge;
                        break;
                    case "NT":
                        type = ContributionTypeCode.NonTaxDed;
                        break;
                    case "GK":
                        type = ContributionTypeCode.GiftInKind;
                        break;
                    case "SK":
                        type = ContributionTypeCode.Stock;
                        break;
                    default:
                        type = ContributionTypeCode.CheckCash;
                        break;
                }

                decimal? othersplitamt = null;
                if (splitfrom > 0)
                {
                    var q = from c in DbUtil.Db.Contributions
                            where c.ContributionId == splitfrom
                            select new
                            {
                                c,
                                bd = c.BundleDetails.First()
                            };
                    var i = q.Single();
                    othersplitamt = i.c.ContributionAmount - amt;
                    contributiondate = i.c.ContributionDate;
                    i.c.ContributionAmount = othersplitamt;
                    imageid = i.c.ImageID;
                    DbUtil.Db.SubmitChanges();
                    bd.BundleSort1 = i.bd.BundleDetailId;
                }

                bd.Contribution = new Contribution
                {
                    CreatedBy = Util.UserId,
                    CreatedDate = bd.CreatedDate,
                    FundId = fund,
                    PeopleId = pid.ToInt2(),
                    ContributionDate = contributiondate ?? Bundle.ContributionDate,
                    ContributionAmount = amt,
                    ContributionStatusId = 0,
                    ContributionTypeId = type,
                    ContributionDesc = notes,
                    CheckNo = (checkno ?? "").Trim().Truncate(20),
                    ImageID = imageid
                };
                Bundle.BundleDetails.Add(bd);
                DbUtil.Db.SubmitChanges();
                return ContributionRowData(ctl, bd.ContributionId, othersplitamt);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public object UpdateContribution(PostBundleController ctl)
        {
            var c = DbUtil.Db.Contributions.SingleOrDefault(cc => cc.ContributionId == editid);
            if (c == null)
            {
                return null;
            }

            var identifier = DbUtil.Db.CardIdentifiers.SingleOrDefault(ci => ci.Id == c.BankAccount);

            if (identifier != null)
            {
                identifier.PeopleId = pid.ToInt2();
            }

            var type = c.ContributionTypeId;

            switch (PLNT)
            {
                case "PL":
                    type = ContributionTypeCode.Pledge;
                    break;
                case "NT":
                    type = ContributionTypeCode.NonTaxDed;
                    break;
                case "GK":
                    type = ContributionTypeCode.GiftInKind;
                    break;
                case "SK":
                    type = ContributionTypeCode.Stock;
                    break;
                default:
                    type = ContributionTypeCode.CheckCash;
                    break;
            }
            c.FundId = fund;
            //            c.CampusId = campusid;
            c.PeopleId = pid.ToInt2();
            c.ContributionAmount = amt;
            c.ContributionTypeId = type;
            c.ContributionDesc = notes;
            c.ContributionDate = contributiondate;
            c.CheckNo = checkno?.Trim();

            DbUtil.Db.SubmitChanges();

            return ContributionRowData(ctl, c.ContributionId);
        }

        public object DeleteContribution()
        {
            var bd = Bundle.BundleDetails.SingleOrDefault(d => d.ContributionId == editid);
            if (bd != null)
            {
                var c = bd.Contribution;
                DbUtil.Db.BundleDetails.DeleteOnSubmit(bd);
                Bundle.BundleDetails.Remove(bd);
                DbUtil.Db.Contributions.DeleteOnSubmit(c);
                DbUtil.Db.SubmitChanges();
            }

            var totalItems = Bundle.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            var diff = (Bundle.TotalCash.GetValueOrDefault() + Bundle.TotalChecks.GetValueOrDefault() + Bundle.TotalEnvelopes.GetValueOrDefault()) - totalItems;
            return new
            {
                totalitems = totalItems.ToString2("C2"),
                diff,
                difference = diff.ToString2("C2"),
                itemcount = Bundle.BundleDetails.Count()
            };
        }

        public static string Tip(int? pid, int? age, string memstatus, string address, string city, string state, string zip)
        {
            return $"<label>People Id:</label> {pid}<br/><label>Age:</label> {age}<br/>{memstatus}<br/>{address}<br/>{Util.FormatCSZ(city, state, zip)}";
        }

        public class FundTotal
        {
            public int FundId { get; set; }
            public string Name { get; set; }
            public decimal? Total { get; set; }
        }

        public class RecentContribution
        {
            public decimal? Amount;
            public string CheckNo;
            public DateTime? DateGiven;
        }

        public class NamesInfo
        {
            public NamesInfo()
            {
                showaltname = DbUtil.Db.Setting("ShowAltNameOnSearchResults");
            }
            public string Name => displayname + (age.HasValue ? $" ({Person.AgeDisplay(age, Pid)})" : "");

            internal bool showaltname;
            internal string name;
            internal string altname;
            internal int? age;

            public int Pid { get; set; }
            internal List<RecentContribution> recent { get; set; }

            internal string addr { get; set; }
            public string Addr => addr.HasValue() ? $"<br>{addr}" : "";

            internal string spouse { get; set; }
            public string Spouse => spouse.HasValue() ? $"<br>Giving with: {spouse}" : "";

            internal string email { get; set; }
            public string Email => email.HasValue() ? $"<br>{email}" : "";
            internal string displayname => (showaltname ? $"{name} {altname}" : name);

            public string RecentGifts
            {
                get
                {
                    if (recent == null)
                    {
                        return "";
                    }

                    const string row =
                        "<tr><td class='right'>{0}</td><td class='center nowrap'>&nbsp;{1}</td><td>&nbsp;{2}</td></tr>";
                    var list = from rr in recent
                               select string.Format(row, rr.Amount.ToString2("N2"), rr.DateGiven.ToSortableDate(), rr.CheckNo);
                    var s = string.Join("\n", list);
                    return s.HasValue() ? $"<table style='margin-left:2em'>{s}</table>" : "";
                }
            }
        }

        public class ContributionInfo
        {
            public int ContributionId { get; set; }
            public string eac { get; set; }
            public string extra { get; set; }
            public int? PeopleId { get; set; }
            public int? Age { get; set; }
            public int BundleTypeId { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public DateTime? Date { get; set; }
            public string PLNT { get; set; }
            public string memstatus { get; set; }
            public int? campusid { get; set; }

            public string CityStateZip => Util.FormatCSZ(City, State, Zip);

            public string Name { get; set; }
            public decimal? Amt { get; set; }

            public string AmtDisplay => Amt.ToString2("N2");

            public string Fund { get; set; }
            public int FundId { get; set; }

            public string FundDisplay => $"{Fund} ({FundId})";

            public string Notes { get; set; }
            public string CheckNo { get; set; }

            public string tip => Tip(PeopleId, Age, memstatus, Address, City, State, Zip);

            public int ImageId { get; set; }
        }
    }
}
