using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models
{
    public class BundleModel
    {
        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
            {
                _count = FetchBundleItems().Count();
            }

            return _count.Value;
        }

        public int BundleStatusId { get; set; }
        public bool IsAdmin => HttpContextFactory.Current.User.IsInRole("Admin") || HttpContextFactory.Current.User.IsInRole("FinanceAdmin");
        public bool IsDataEntry => HttpContextFactory.Current.User.IsInRole("FinanceDataEntry");
        public bool CanEdit => BundleStatusId == BundleStatusCode.Open || BundleStatusId == BundleStatusCode.OpenForDataEntry || IsAdmin;

        public bool CanChangeStatus
        {
            get
            {
                if (BundleStatusId == BundleStatusCode.Closed)
                {
                    return IsAdmin; // only an Admin with Finance or a FinanceAdmin can reopen
                }

                return Bundle.BundleDetails.All(bd => bd.Contribution.PeopleId != null)
                       && TotalItems() == TotalHeader(); // anybody can close it if they have gotten to this page.
            }
        }

        public int BundleId
        {
            get { return bundleId; }
            set
            {
                bundleId = value;
                var q = (from bb in DbUtil.Db.BundleHeaders
                         where bb.BundleHeaderId == BundleId
                         select new
                         {
                             StatusId = bb.BundleStatusId,
                             Status = bb.BundleStatusType.Description,
                             Type = bb.BundleHeaderType.Description,
                             DefaultFund = bb.Fund.FundName,
                             bundle = bb
                         }).SingleOrDefault();
                if (q == null)
                {
                    return;
                }

                Status = q.Status;
                BundleStatusId = q.StatusId;
                Type = q.Type;
                DefaultFund = q.DefaultFund;
                Bundle = q.bundle;
            }
        }

        public string Status;
        public string Type;
        public string DefaultFund;

        public BundleHeader Bundle;

        public BundleModel()
        {
        }
        public BundleModel(int id)
        {
            BundleId = id;

        }

        private IQueryable<Contribution> bundleItems;
        private int bundleId;

        public decimal TotalHeader()
        {
            return (Bundle.TotalCash ?? 0)
                   + (Bundle.TotalChecks ?? 0)
                   + (Bundle.TotalEnvelopes ?? 0);
        }
        public decimal TotalItems()
        {
            var q = from d in DbUtil.Db.BundleDetails
                    where d.BundleHeaderId == BundleId
                    where d.Contribution.ContributionStatusId == ContributionStatusCode.Recorded
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(d.Contribution.ContributionTypeId)
                    select d.Contribution;
            return q.Sum(c => (decimal?)c.ContributionAmount) ?? 0;
        }

        private IQueryable<Contribution> FetchBundleItems()
        {
            if (bundleItems == null)
            {
                bundleItems = from d in DbUtil.Db.BundleDetails
                              where d.BundleHeaderId == BundleId
                              let sort = d.BundleSort1 > 0 ? d.BundleSort1 : d.BundleDetailId
                              orderby sort, d.ContributionId
                              select d.Contribution;
            }

            return bundleItems;
        }

        public IEnumerable<ContributionInfo> Contributions()
        {
            var q = FetchBundleItems();
            var q3 = from c in q
                     select new ContributionInfo
                     {
                         ContributionId = c.ContributionId,
                         PeopleId = c.PeopleId,
                         Fund = $"{c.ContributionFund.FundName} ({c.ContributionFund.FundId})",
                         Type = c.ContributionType.Description,
                         Name = c.Person.Name2
                              + (c.Person.DeceasedDate.HasValue ? " [DECEASED]" : ""),
                         Date = c.ContributionDate,
                         Amount = c.ContributionAmount,
                         Status = c.ContributionStatus.Description,
                         Check = c.CheckNo,
                         Notes = c.ContributionDesc,
                         ReversedReturned = c.ContributionStatusId > 0,
                         PostingDate = c.PostingDate,
                         ImageId = c.ImageID
                     };
            return q3;
        }

        public IEnumerable<SelectListItem> BundleHeaderList()
        {
            return new SelectList(DbUtil.Db.BundleHeaderTypes, "Id", "Description", Bundle.BundleHeaderTypeId);
        }
        public IEnumerable<SelectListItem> ContributionFundList(bool sortByName = true)
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

        public IEnumerable<SelectListItem> BundleStatusList()
        {
            var q = from bs in DbUtil.Db.BundleStatusTypes
                    let hasDataEntryRole = DbUtil.Db.Roles.Any(rr => rr.RoleName == "FinanceDataEntry")
                    where bs.Id < 2 || hasDataEntryRole
                    select bs;
            return new SelectList(q, "Id", "Description", Bundle.BundleStatusId);
        }


        public class ContributionInfo
        {
            public int ContributionId { get; set; }
            public string Fund { get; set; }
            public string Type { get; set; }
            public int? PeopleId { get; set; }
            public string Name { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? PostingDate { get; set; }
            public decimal? Amount { get; set; }
            public string Status { get; set; }
            public string Check { get; set; }
            public string Notes { get; set; }
            public bool ReversedReturned { get; set; }
            public int ImageId { get; set; }
        }
    }
}
