using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Controllers;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models
{
    public class BundleModel
    {
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchBundleItems().Count();
            return _count.Value;
        }

        public bool IsOpen
        {
            get { return Bundle != null && Bundle.BundleStatusId == BundleStatusCode.Open; }
            set { Bundle.BundleStatusId = value ? BundleStatusCode.Open : BundleStatusCode.Closed; }
        }

        public bool IsAdmin
        {
            get 
            { 
                return HttpContext.Current.User.IsInRole("Admin") 
                    || HttpContext.Current.User.IsInRole("FinanceAdmin"); 
            }
        }
        public bool CanEdit { get { return IsOpen || IsAdmin; } } 
        public bool CanChangeStatus
        {
            get
            {
                if (!IsOpen)
                    return IsAdmin;
                return Bundle.BundleDetails.All(bd => bd.Contribution.PeopleId != null)
                       && TotalItems() == TotalHeader();
            }
        }

        public int BundleId
        {
            get { return _bundleId; }
            set
            {
                _bundleId = value;
                var q = (from bb in DbUtil.Db.BundleHeaders
                         where bb.BundleHeaderId == BundleId
                         select new
                         {
                             Status = bb.BundleStatusType.Description,
                             Type = bb.BundleHeaderType.Description,
                             DefaultFund = bb.Fund.FundName,
                             bundle = bb
                         }).SingleOrDefault();
                if (q == null)
                    return;
                Status = q.Status;
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

        private IQueryable<Contribution> _bundleItems;
        private int _bundleId;

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
            if (_bundleItems == null)
                _bundleItems = from d in DbUtil.Db.BundleDetails
                               where d.BundleHeaderId == BundleId
            				   let sort = d.BundleSort1 > 0 ? d.BundleSort1 : d.BundleDetailId
                               orderby sort, d.ContributionId
                               select d.Contribution;
            return _bundleItems;
        }

        public IEnumerable<ContributionInfo> Contributions()
        {
            var q = FetchBundleItems();
            var q3 = from c in q
                     select new ContributionInfo
                     {
                         PeopleId = c.PeopleId,
                         Fund = c.ContributionFund.FundName,
                         Type = c.ContributionType.Description,
                         Name = c.Person.Name2
                              + (c.Person.DeceasedDate.HasValue ? " [DECEASED]" : ""),
                         Date = c.ContributionDate,
                         Amount = c.ContributionAmount,
                         Status = c.ContributionStatus.Description,
                         Check = c.CheckNo,
                         Notes = c.ContributionDesc,
                         ReversedReturned = c.ContributionStatusId > 0,
                         PostingDate = c.PostingDate
                     };
            return q3;
        }

        public IEnumerable<SelectListItem> BundleHeaderList()
        {
            return new SelectList(DbUtil.Db.BundleHeaderTypes, "Id", "Description", Bundle.BundleHeaderTypeId);
        }
        public IEnumerable<SelectListItem> ContributionFundList()
        {
            return new SelectList(DbUtil.Db.ContributionFunds.Where(ff => ff.FundStatusId == 1), "FundId", "FundName",
                Bundle.FundId);
        }


        public class ContributionInfo
        {
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
        }
    }
}