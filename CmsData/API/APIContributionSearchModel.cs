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
        public int Status { get; set; }
        public decimal? MinAmt { get; set; }
        public decimal? MaxAmt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TaxNonTax { get; set; }
        public int? CampusId { get; set; }
        public int? PeopleId { get; set; }
        public int? Year { get; set; }
        public int? FundId { get; set; }
        public bool IncludeUnclosedBundles { get; set; }
        public bool Mobile { get; set; }
        public int Online { get; set; }
        public bool FilterByActiveTag { get; set; }

        internal string Campus;
        internal string FundName;
        internal decimal? Total;
        internal int? Count;

        public ContributionSearchInfo()
        {
            TaxNonTax = "TaxDed";
            Online = 2; // Both
        }
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
            PopulateTotals();
            contributions = contributions.Skip(startRow).Take(pageSize);
            return ContributionsList(contributions);
        }

        public string ContributionsXML(int startRow, int pageSize)
        {
            var xs = new XmlSerializer(typeof(ContributionElements));
            var sw = new StringWriter();
            PopulateTotals();
            var cc = contributions.OrderByDescending(m => m.ContributionDate).Skip(startRow).Take(pageSize);
            var a = new ContributionElements
            {
                NumberOfPages = (int) Math.Ceiling((model.Count ?? 0)/100.0),
                NumberOfItems = model.Count ?? 0,
                TotalAmount = model.Total ?? 0,
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

        public IQueryable<ContributionInfo> ContributionsList(IQueryable<Contribution> query)
        {
            var q2 = from c in query
                     let bd = c.BundleDetails.FirstOrDefault()
                     let contributionType = c.BundleDetails.Single().BundleHeader.BundleHeaderType
                                  .Description.Contains("Online")
                         ? c.ContributionDesc == "Recurring Giving" ? c.ContributionDesc : "Online"
                         : c.ContributionType.Description
                     select new ContributionInfo
                     {
                         BundleId = bd == null ? 0 : bd.BundleHeaderId,
                         ContributionAmount = c.ContributionAmount ?? 0,
                         ContributionDate = c.ContributionDate ?? SqlDateTime.MinValue.Value,
                         ContributionId = c.ContributionId,
                         ContributionType = contributionType,
                         ContributionTypeId = c.ContributionTypeId,
                         Fund = c.ContributionFund.FundName,
                         BundleType = bd.BundleHeader.BundleHeaderType.Description,
                         NonTaxDed =
                             c.ContributionTypeId == ContributionTypeCode.NonTaxDed ||
                             (c.ContributionFund.NonTaxDeductible ?? false),
                         StatusId = c.ContributionStatusId ?? -1,
                         Status = c.ContributionStatus.Description,
                         Name = c.Person.Name,
                         Pledge = c.PledgeFlag ?? false,
                         PeopleId = c.PeopleId ?? 0,
                         Description = c.ContributionDesc,
                         CheckNo = c.CheckNo,
                         FamilyId = c.Person.FamilyId,
                         MemberStatus = c.Person.MemberStatus.Description,
                         JoinDate = c.Person.JoinDate,
                         Address = c.Person.PrimaryAddress,
                         Address2 = c.Person.PrimaryAddress2,
                         City = c.Person.PrimaryCity,
                         State = c.Person.PrimaryState,
                         Zip = c.Person.PrimaryZip
                     };
            return q2;
        }


        private IQueryable<Contribution> contributions;

        public IQueryable<Contribution> FetchContributions()
        {
            if (contributions != null)
                return contributions;

            if (!model.TaxNonTax.HasValue())
                model.TaxNonTax = "TaxDed";

            contributions = db.Contributions.AsQueryable();

            if (!model.IncludeUnclosedBundles)
                contributions = from c in contributions
                                where c.BundleDetails.Any(dd => dd.BundleHeader.BundleStatusId == BundleStatusCode.Closed)
                                select c;
            if (model.Mobile)
                contributions = from c in contributions
                                where c.Source > 0
                                select c;

            switch (model.TaxNonTax)
            {
                case "TaxDed":
                    contributions = from c in contributions
                                    where !ContributionTypeCode.NonTaxTypes.Contains(c.ContributionTypeId)
                                    select c;
                    break;
                case "NonTaxDed":
                    contributions = from c in contributions
                                    where c.ContributionTypeId == ContributionTypeCode.NonTaxDed
                                    select c;
                    break;
                case "Both":
                    contributions = from c in contributions
                                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                                    select c;
                    break;
                case "Pledge":
                    contributions = from c in contributions
                                    where c.ContributionTypeId == ContributionTypeCode.Pledge
                                    select c;
                    break;
            }

            switch (model.Status)
            {
                case ContributionStatusCode.Recorded:
                    if(!model.PeopleId.HasValue)
                        contributions = from c in contributions
                                        where c.ContributionStatusId == ContributionStatusCode.Recorded
                                        where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                                        select c;
                    break;
                case ContributionStatusCode.Returned:
                    contributions = from c in contributions
                                    where c.ContributionStatusId == ContributionStatusCode.Returned
                                          || c.ContributionTypeId == ContributionTypeCode.ReturnedCheck
                                    select c;
                    break;
                case ContributionStatusCode.Reversed:
                    contributions = from c in contributions
                                    where c.ContributionStatusId == ContributionStatusCode.Reversed
                                          || c.ContributionTypeId == ContributionTypeCode.Reversed
                                    select c;
                    break;
            }


            if (model.PeopleId > 0)
                contributions = from c in contributions
                                where c.PeopleId == model.PeopleId
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
                                where c.ContributionDate < model.EndDate.Value.AddDays(1)
                                select c;

            if (model.Online == 1)
                contributions = from c in contributions
                                where c.BundleDetails.Any(dd => dd.BundleHeader.BundleHeaderTypeId == BundleTypeCode.Online)
                                select c;
            else if (model.Online == 0)
                contributions = from c in contributions
                                where c.BundleDetails.All(dd => dd.BundleHeader.BundleHeaderTypeId != BundleTypeCode.Online)
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
                                      || c.ContributionId == model.Comments.ToInt()
                                select c;

            if ((model.Type ?? 0) != 0)
                contributions = from c in contributions
                                where c.ContributionTypeId == model.Type
                                select c;

            if ((model.CampusId ?? 0) != 0)
                contributions = from c in contributions
                                where c.CampusId == model.CampusId
                                select c;

            if (model.BundleType == 9999)
                contributions = from c in contributions
                                where !c.BundleDetails.Any()
                                select c;
            else if ((model.BundleType ?? 0) != 0)
                contributions = from c in contributions
                                where c.BundleDetails.First().BundleHeader.BundleHeaderTypeId == model.BundleType
                                select c;

            if (model.Year.HasValue && model.Year > 0)
                contributions = from c in contributions
                                where c.ContributionDate.Value.Year == model.Year
                                select c;

            if (model.FundId.HasValue && model.FundId != 0)
                contributions = from c in contributions
                                where c.FundId == model.FundId
                                select c;

            if (model.FilterByActiveTag)
            {
                var tagid = db.TagCurrent().Id;
                contributions = from c in contributions
                    where db.TagPeople.Any(vv => vv.PeopleId == c.PeopleId && vv.Id == tagid)
                    select c;
            }
            return contributions;
        }

        private void PopulateTotals()
        {
            if (model.Count.HasValue)
                return;

            var q = FetchContributions();
            var count = q.Count();
            decimal total = 0;
            if (count > 0)
                total = q.Sum(cc => cc.ContributionAmount ?? 0);
            var fund = db.ContributionFunds.Where(ff => ff.FundId == model.FundId).Select(ff => ff.FundName).SingleOrDefault();
            var campus = db.Campus.Where(cc => cc.Id == model.CampusId).Select(cc => cc.Description).SingleOrDefault();

            model.Total = total;
            model.Count = count;
            model.FundName = fund;
            model.Campus = campus;
        }

        public string FundName()
        {
            PopulateTotals();
            return model.FundName;
        }

        public string Campus()
        {
            PopulateTotals();
            return model.Campus;
        }

        public string Online()
        {
            return model.Online == 2 ? "Both" : model.Online == 1 ? "Online" : "Not Online";
        }

        public string TaxDedNonTax()
        {
            return model.TaxNonTax == "All" ? "Both" : model.TaxNonTax == "TaxDed" ? "Tax Deductible" : "Non Tax Deductible";
        }

        public decimal Total()
        {
            PopulateTotals();
            return model.Total ?? 0;
        }

        public int Count()
        {
            PopulateTotals();
            return model.Count ?? 0;
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