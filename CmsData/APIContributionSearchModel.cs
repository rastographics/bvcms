/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData.API
{
    public class ContributionSearchInfo
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public int? BundleType { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public decimal? MinAmt { get; set; }
        public decimal? MaxAmt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TaxNonTax { get; set; }
        public int? PeopleId { get; set; }
        public int? Year { get; set; }
        public int? FundId { get; set; }
    }

    public class APIContributionSearchModel
    {
        public ContributionSearchInfo model { get; set; }

        private CMSDataContext db;

        public APIContributionSearchModel(CMSDataContext db, ContributionSearchInfo m)
        {
            this.db = db;
            model = m;
        }

        public APIContributionSearchModel(CMSDataContext db)
        {
            this.db = db;
            model = new ContributionSearchInfo();
        }

        public IEnumerable<ContributionInfo> ContributionsList(int startRow, int pageSize)
        {
            contributions = FetchContributions();
            if (!_count.HasValue)
                _count = contributions.Count();
            contributions = contributions.Skip(startRow).Take(pageSize);
            return ContributionsList(contributions);
        }

        public string ContributionsXML(int startRow, int pageSize)
        {
            var xs = new XmlSerializer(typeof (ContributionElements));
            var sw = new StringWriter();
            var cc = FetchContributions();
            if (!_count.HasValue)
                _count = contributions.Count();
            cc = cc.OrderByDescending(m => m.ContributionDate).Skip(startRow).Take(pageSize);
            var a = new ContributionElements
                {
                    NumberOfPages = (int)Math.Ceiling((_count ?? 0) / 100.0),
                    NumberOfItems = _count ?? 0,
                    TotalAmount = Total(),
                    List = (from c in cc
                            let bd = c.BundleDetails.FirstOrDefault()
                            select new APIContribution.Contribution
                                {
                                    ContributionId = c.ContributionId,
                                    PeopleId = c.PeopleId ?? 0,
                                    Name = c.Person.Name,
                                    Date = c.ContributionDate.FormatDate(),
                                    Amount = c.ContributionAmount ?? 0,
                                    Fund = c.ContributionFund.FundName,
                                    Description = c.ContributionDesc,
                                    CheckNo = c.CheckNo
                                }).ToArray()
                };
            xs.Serialize(sw, a);
            return sw.ToString();
        }

        public IEnumerable<ContributionInfo> ContributionsList(IQueryable<Contribution> query)
        {
            var q2 = from c in query
                     let bd = c.BundleDetails.FirstOrDefault()
                     select new ContributionInfo
                         {
                             BundleId = bd == null ? 0 : bd.BundleHeaderId,
                             ContributionAmount = c.ContributionAmount ?? 0,
                             ContributionDate = c.ContributionDate ?? SqlDateTime.MinValue.Value,
                             ContributionId = c.ContributionId,
                             ContributionType = c.ContributionType.Description,
                             ContributionTypeId = c.ContributionTypeId,
                             Fund = c.ContributionFund.FundName,
                             NonTaxDed =
                                 c.ContributionTypeId == ContributionTypeCode.NonTaxDed ||
                                 (c.ContributionFund.NonTaxDeductible ?? false),
                             StatusId = c.ContributionStatusId ?? -1,
                             Status = c.ContributionStatus.Description,
                             Name = c.Person.Name,
                             PeopleId = c.PeopleId ?? 0,
                             Description = c.ContributionDesc,
                             CheckNo = c.CheckNo
                         };
            return q2;
        }

        private int? _count;

        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchContributions().Count();
            return _count.Value;
        }

        private IQueryable<Contribution> contributions;

        public IQueryable<Contribution> FetchContributions()
        {
            if (contributions != null)
                return contributions;

            if(!model.TaxNonTax.HasValue())
                model.TaxNonTax = "TaxDed";

            contributions = from c in db.Contributions
                            where c.PeopleId == model.PeopleId || model.PeopleId == null
                            where model.TaxNonTax == "All"
                                  ||
                                  (model.TaxNonTax == "TaxDed" &&
                                   !ContributionTypeCode.NonTaxTypes.Contains(c.ContributionTypeId))
                                  ||
                                  (model.TaxNonTax == "NonTaxDed" &&
                                   c.ContributionTypeId == ContributionTypeCode.NonTaxDed)
                                  ||
                                  (model.TaxNonTax == "Pledge" &&
                                   c.ContributionTypeId == ContributionTypeCode.Pledge)
                            select c;

            if (model.MinAmt.HasValue)
                contributions = from c in contributions
                                where c.ContributionAmount >= model.MinAmt
                                select c;
            if (model.MaxAmt.HasValue)
                contributions = from c in contributions
                                where c.ContributionAmount <= model.MaxAmt
                                select c;

            if (model.StartDate.HasValue)
                contributions = from c in contributions
                                where c.ContributionDate >= model.StartDate
                                select c;

            if (model.EndDate.HasValue)
                contributions = from c in contributions
                                where c.ContributionDate <= model.EndDate.Value.AddDays(1)
                                select c;

            var i = model.Name.ToInt();
            if (i > 0)
                contributions = from c in contributions
                                where c.Person.PeopleId == i
                                select c;
            else if (model.Name.HasValue())
                contributions = from c in contributions
                                where c.Person.Name.Contains(model.Name)
                                select c;

            if (model.Comments.HasValue())
                contributions = from c in contributions
                                where c.ContributionDesc.Contains(model.Comments)
                                      || c.CheckNo == model.Comments
                                select c;

            if ((model.Type ?? 0) != 0)
                contributions = from c in contributions
                                where c.ContributionTypeId == model.Type
                                select c;

            if ((model.BundleType ?? 0) != 0)
                contributions = from c in contributions
                                where c.BundleDetails.First().BundleHeader.BundleHeaderTypeId == model.BundleType
                                select c;

            if ((model.Status ?? 99) != 99)
                contributions = from c in contributions
                                where c.ContributionStatusId == model.Status
                                select c;

            if (model.Year.HasValue && model.Year > 0)
                contributions = from c in contributions
                                where c.ContributionDate.Value.Year == model.Year
                                select c;

            if (model.FundId.HasValue && model.FundId > 0)
                contributions = from c in contributions
                                where c.FundId == model.FundId
                                select c;

            return contributions;
        }

        public decimal Total()
        {
            var q = FetchContributions();
            q = from c in q
                where c.ContributionStatusId == ContributionStatusCode.Recorded
                where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                where model.TaxNonTax != "All" || c.ContributionTypeId != ContributionTypeCode.Pledge
                select c;
            var t = q.Sum(c => c.ContributionAmount);
            if (t.HasValue)
                return t.Value;
            return 0;
        }

        [Serializable]
        [XmlRoot("Contributions")]
        public class ContributionElements
        {
            public int NumberOfPages { get; set; }
            public int NumberOfItems { get; set; }
            public decimal TotalAmount { get; set; }
            [XmlElement("Contribution")]
            public APIContribution.Contribution[] List { get; set; }
        }
    }
}