/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.API;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData;

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
                    _name = (from p in DbUtil.Db.People
                             where p.PeopleId == SearchInfo.PeopleId
                             select p.Name).SingleOrDefault() ?? "";
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

        public ContributionSearchModel(int? peopleId, int? year)
        {
            api = new APIContributionSearchModel(DbUtil.Db);
            Setup();
            SearchInfo.PeopleId = peopleId;
            SearchInfo.Year = year;
        }

        public IEnumerable<ContributionInfo> ContributionsList()
        {
            var q = api.FetchContributions();
            q = ApplySort(q).Skip(StartRow).Take(PageSize);
            return api.ContributionsList(q);
        }
        public IEnumerable<ContributionInfo> ContributionsListAll()
        {
            var q = api.FetchContributions();
            return api.ContributionsList(q);
        }

        public IQueryable<Contribution> ApplySort(IQueryable<Contribution> q)
        {
            if ((Direction ?? "desc") == "asc")
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
            else
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
            return q;
        }
        public SelectList ContributionStatuses()
        {
            return new SelectList(new CodeValueModel().ContributionStatuses99(),
                "Id", "Value", SearchInfo.Status.ToString());
        }
        public SelectList ContributionTypes()
        {
            return new SelectList(new CodeValueModel().ContributionTypes0(),
                "Id", "Value", SearchInfo.Type.ToString());
        }
        public SelectList BundleTypes()
        {
            return new SelectList(new CodeValueModel().BundleHeaderTypes0(),
                "Id", "Value", SearchInfo.Type.ToString());
        }
        public IEnumerable<SelectListItem> Years()
        {
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
                            select new SelectListItem()
                                   {
                                       Value = g.Key.FundId.ToString(),
                                       Text = g.Key.FundName
                                   }).ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public decimal Total()
        {
            return api.Total();
        }

        public int Count()
        {
            return api.Count();
        }

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

        private static Contribution CreateContributionRecord(Contribution c)
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
    }
}
