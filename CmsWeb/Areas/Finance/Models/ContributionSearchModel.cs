/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsWeb.Code;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ContributionSearchModel : PagerModel2
    {
        private APIContributionSearchModel api { get; set; }
        public ContributionSearchInfo SearchInfo { get; set; }

        private string _name = null;
        public string Name
        {
            get
            {
                if (!_name.HasValue())
                {
                    _name = (from p in DbUtil.Db.People
                             where p.PeopleId == SearchInfo.PeopleId
                             select p.Name).SingleOrDefault() ?? "";
                }

                return _name;
            }
        }

        private void Setup()
        {
            GetCount = api.Count;
            Sort = "Date";
            Direction = "desc";
            SearchInfo = api.model;
        }

        public ContributionSearchModel(APIContributionSearchModel api)
        {
            this.api = api;
            Setup();
        }
        public ContributionSearchModel()
        {
            api = new APIContributionSearchModel(DbUtil.Db);
            Setup();
        }
        public ContributionSearchModel(ContributionSearchInfo m)
        {
            api = new APIContributionSearchModel(DbUtil.Db, m);
            Setup();
        }

        public IEnumerable<ContributionInfo> ContributionsList()
        {
            var q = api.FetchContributions();
            q = ApplySort(q).Skip(StartRow).Take(PageSize);
            return api.ContributionsList(q);
        }
        public EpplusResult ContributionsListAllExcel()
        {
            var q = api.FetchContributions();
            return api.ContributionsList(q).ToDataTable().ToExcel("Contributions.xlsx");
        }

        public IQueryable<Contribution> ApplySort(IQueryable<Contribution> q)
        {
            if ((Direction ?? "desc") == "asc")
            {
                switch (Sort)
                {
                    case "Date":
                        q = q.OrderBy(c => c.ContributionDate);
                        break;
                    case "Amount":
                        q = from c in q
                            orderby c.ContributionAmount, c.ContributionDate descending
                            select c;
                        break;
                    case "Type":
                        q = from c in q
                            orderby c.ContributionTypeId, c.ContributionDate descending
                            select c;
                        break;
                    case "Status":
                        q = from c in q
                            orderby c.ContributionStatusId, c.ContributionDate descending
                            select c;
                        break;
                    case "Fund":
                        q = from c in q
                            orderby c.FundId, c.ContributionDate descending
                            select c;
                        break;
                    case "Name":
                        q = from c in q
                            orderby c.Person.Name2
                            select c;
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "Date":
                        q = q.OrderByDescending(c => c.ContributionDate);
                        break;
                    case "Amount":
                        q = from c in q
                            orderby c.ContributionAmount descending, c.ContributionDate descending
                            select c;
                        break;
                    case "Type":
                        q = from c in q
                            orderby c.ContributionTypeId descending, c.ContributionDate descending
                            select c;
                        break;
                    case "Status":
                        q = from c in q
                            orderby c.ContributionStatusId descending, c.ContributionDate descending
                            select c;
                        break;
                    case "Fund":
                        q = from c in q
                            orderby c.FundId descending, c.ContributionDate descending
                            select c;
                        break;
                    case "Name":
                        q = from c in q
                            orderby c.Person.Name2 descending
                            select c;
                        break;
                }
            }

            return q;
        }

        public class BundleInfo
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public decimal Total { get; set; }
            public int Count { get; set; }
        }
        public IEnumerable<BundleInfo> BundlesList()
        {
            var q = (from c in api.FetchContributions()
                     let bhid = c.BundleDetails.First().BundleHeaderId
                     group c by new { bhid, c.ContributionDate.Value.Date } into g
                     select new BundleInfo()
                     {
                         Id = g.Key.bhid,
                         Total = g.Sum(t => t.ContributionAmount ?? 0),
                         Date = g.Key.Date,
                         Count = g.Count()
                     }).OrderBy(x => x.Date);
            return q;
        }
        public SelectList ContributionStatuses()
        {
            return new SelectList(new CodeValueModel().ContributionStatuses(),
                "Id", "Value", SearchInfo.Status.ToString());
        }
        public SelectList ContributionTypes()
        {
            return new SelectList(new CodeValueModel().ContributionTypes0(),
                "Id", "Value", SearchInfo.Type.ToString());
        }
        public SelectList Campuses()
        {
            return new SelectList(new CodeValueModel().AllCampuses0(),
                "Id", "Value", SearchInfo.Type.ToString());
        }
        public SelectList OnlineOptions()
        {
            return new SelectList(
                new List<CodeValueItem>
                {
                    new CodeValueItem { Id = 2, Value = "Both Online & Not" },
                    new CodeValueItem { Id = 1, Value = "Online" },
                    new CodeValueItem { Id = 0, Value = "Not Online" },
                }, "Id", "Value", SearchInfo.Online.ToString());
        }
        public IEnumerable<SelectListItem> BundleTypes()
        {
            var list = new CodeValueModel().BundleHeaderTypes0().ToList();
            list.Add(new CodeValueItem { Id = 9999, Value = "No Bundle" });
            return new SelectList(list, "Id", "Value", SearchInfo.Type.ToString());
        }
        public IEnumerable<SelectListItem> Years()
        {
            // todo: the "years" dropdown on contribution/index doesn't correctly show the years if coming from a giving statement of the spouse in all cases because this search is only off of the donors peopleid.
            var q = from c in DbUtil.Db.Contributions
                    where c.PeopleId == SearchInfo.PeopleId || SearchInfo.PeopleId == null
                    group c by c.ContributionDate.Value.Year
                        into g
                    orderby g.Key descending
                    select new SelectListItem { Text = g.Key.ToString() };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public IEnumerable<SelectListItem> Funds()
        {
            var list = (from c in DbUtil.Db.Contributions
                        where c.PeopleId == SearchInfo.PeopleId || SearchInfo.PeopleId == null
                        group c by new { c.FundId, c.ContributionFund.FundName }
                            into g
                        orderby g.Key.FundName
                        select new SelectListItem
                        {
                            Value = g.Key.FundId.ToString(),
                            Text = g.Key.FundName
                        }).ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public string FundName => api.FundName();
        public string Campus => api.Campus();
        public string Online => api.Online();
        public string TaxDedNonTax => api.TaxDedNonTax();
        public decimal? Total => api.Total();
        public int? FamilyCount=> api.FamilyCount();
        public int? Count => api.Count();

        public void Return(int cid)
        {
            var c = DbUtil.Db.Contributions.Single(ic => ic.ContributionId == cid);
            var r = CreateContributionRecord(c);
            c.ContributionStatusId = ContributionStatusCode.Returned;
            r.ContributionTypeId = ContributionTypeCode.ReturnedCheck;
            r.ContributionDesc = "Returned Check for Contribution Id = " + c.ContributionId;

            DbUtil.Db.Contributions.InsertOnSubmit(r);
            DbUtil.Db.SubmitChanges();
        }

        public void Reverse(int cid)
        {
            var c = DbUtil.Db.Contributions.Single(ic => ic.ContributionId == cid);
            var r = CreateContributionRecord(c);
            c.ContributionStatusId = ContributionStatusCode.Reversed;
            r.ContributionTypeId = ContributionTypeCode.Reversed;
            r.ContributionDesc = "Reversed Contribution Id = " + c.ContributionId;
            DbUtil.Db.Contributions.InsertOnSubmit(r);
            DbUtil.Db.SubmitChanges();
        }

        public static Contribution CreateContributionRecord(Contribution c)
        {
            var now = Util.Now;
            var r = new Contribution
            {
                ContributionStatusId = ContributionStatusCode.Recorded,
                CreatedBy = Util.UserId1,
                CreatedDate = now,
                PeopleId = c.PeopleId,
                ContributionAmount = c.ContributionAmount,
                ContributionDate = now.Date,
                PostingDate = now,
                FundId = c.FundId,
            };
            return r;
        }

        public SelectList TaxTypes()
        {
            return new SelectList(
                new List<CodeValueItem>
                {
                    new CodeValueItem { Code = "TaxDed", Value = "Tax Deductible" },
                    new CodeValueItem { Code = "NonTaxDed", Value = "Non-Tax Deductible" },
                    new CodeValueItem { Code = "Both", Value = "Both Tax & Non-Tax" },
                    new CodeValueItem { Code = "Pledge", Value = "Pledges" },
                    new CodeValueItem { Code = "All", Value = "All Items" },
                }, "Code", "Value", SearchInfo.TaxNonTax);
        }

        public string CheckConversion()
        {
            if (!HttpContextFactory.Current.User.IsInRole("conversion"))
            {
                return null;
            }

            if (!SearchInfo.Name.HasValue())
            {
                return null;
            }

            if (SearchInfo.FundId == 0)
            {
                return null;
            }

            var re = new Regex(@"move to fundid (\d+)");
            var match = re.Match(SearchInfo.Name);
            if (!match.Success)
            {
                return null;
            }

            var newfundid = match.Groups[1].Value.ToInt2();
            if (!(newfundid > 0))
            {
                return null;
            }

            var oldfund = DbUtil.Db.ContributionFunds.Single(ff => ff.FundId == SearchInfo.FundId);
            var newfund = DbUtil.Db.ContributionFunds.SingleOrDefault(ff => ff.FundId == newfundid) ??
                          DbUtil.Db.FetchOrCreateFund(newfundid.Value, oldfund.FundDescription);

            SearchInfo.Name = null;
            var q = api.FetchContributions();
            var sb = new StringBuilder();
            foreach (var c in q)
            {
                sb.AppendFormat(@"
-- for pid = {3}, moving {4:c} from '{5}' to '{6}'
UPDATE dbo.Contribution SET FundId = {0} WHERE ContributionId = {1} AND FundId = {2}
", newfundid, c.ContributionId, c.FundId,
                c.PeopleId, c.ContributionAmount, oldfund.FundDescription, newfund.FundDescription);
            }
            var sql = sb.ToString();
            DbUtil.Db.ContentText($"MovedFunds-{DateTime.Now}", sql);
            DbUtil.Db.ExecuteCommand(sql);
            return "/Contributions?fundId=" + newfundid;
        }
    }
}
