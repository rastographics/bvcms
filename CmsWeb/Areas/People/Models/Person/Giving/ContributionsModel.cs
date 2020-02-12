using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace CmsWeb.Areas.People.Models
{
    public class ContributionsModel : PagedTableModel<Contribution, ContributionInfo>
    {
        public int PeopleId
        {
            get { return peopleid; }
            set
            {
                peopleid = value;
                Person = CurrentDatabase.LoadPersonById(peopleid);
                ContributionOptions = new CodeInfo(Person.ContributionOptionsId, "ContributionOptions");
                EnvelopeOptions = new CodeInfo(Person.EnvelopeOptionsId, "EnvelopeOptions");
                ElectronicStatement = Person.ElectronicStatement ?? false;
            }
        }
        //public string Filter { get; set; }
        private int peopleid;
        public Person Person;
        public bool ShowNames;
        public bool ShowTypes;
        public bool isPledges = false;
        public bool showNegativePledgeBalances = false;
        public List<PledgesSummary> PledgesSummary { get; set; }
        public List<GivingSummary> GivingSummary { get; set; }
        [DisplayName("Electronic Only"), TrackChanges]
        public bool ElectronicStatement { get; set; }
        [DisplayName("Statement Option")]
        public CodeInfo ContributionOptions { get; set; }
        [DisplayName("Envelope Option")]
        public CodeInfo EnvelopeOptions { get; set; }

        private List<string> GivingYears = new List<string>() { "Year To Date", "Previous And Current", "All Years" };
        public DateTime GivingStartDate { get; set; }
        public DateTime GivingEndDate { get; set; }

        public bool givingSumCollapse = true;
        public bool givingDetCollapse = true;
        public bool pledgeSumCollapse = true;
        public bool pledgeDetCollapse = true;

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public ContributionsModel()
        {
            Init();
        }

        public ContributionsModel(CMSDataContext db) : base(db)
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
            Sort = "Completed";
            Direction = "desc";
            AjaxPager = true;
        }

        public void SetGivingYears(IQueryable<Contribution> contributions)
        {
            if (contributions != null && contributions.Count() > 0)
            {
                var years = contributions.Select(c => c.ContributionDate.Value.Year).Distinct()
                    .OrderByDescending(c=>c).Select(c=>c.ToString()).ToList();
                GivingYears.AddRange(years);
            }
        }

        public override IQueryable<Contribution> DefineModelList()
        {
            IQueryable<Contribution> contributionRecords;
            contributionRecords = GetContributionRecords();
            IQueryable<Contribution> filteredRecords = ApplyFilter(contributionRecords);
            if (Filter != "Pledges")
            {
                filteredRecords = ApplyYear(filteredRecords);
            }
            var items = filteredRecords.ToList();
            ShowNames = filteredRecords.Any(c => c.PeopleId != Person.PeopleId);
            ShowTypes = filteredRecords.Any(c => ContributionTypeCode.SpecialTypes.Contains(c.ContributionTypeId));
            return filteredRecords;
        }

        private IQueryable<Contribution> ApplyFilter(IQueryable<Contribution> contributionRecords)
        {
            switch (Filter)
            {
                case "Contributions":
                case "CombineGiving":
                    return contributionRecords.Where(p => p.ContributionTypeId != ContributionTypeCode.Pledge);
                case "Pledges":
                    isPledges = true;
                    return contributionRecords.Where(p => p.ContributionTypeId == ContributionTypeCode.Pledge);
                default:
                    return contributionRecords;
            }
        }

        private IQueryable<Contribution> ApplyYear(IQueryable<Contribution> contributionRecords)
        {
            GivingEndDate = DateTime.Now;
            var Year1 = Year;
            if (!GivingYears.Any(p => p == Year))
                Year1 = "YearToDate";

            switch (Year1.Replace(" ", ""))
            {
                case null:
                case "YearToDate":
                    GivingStartDate = new DateTime(GivingEndDate.Year, 1, 1);
                    break;
                case "AllYears":
                    GivingStartDate = new DateTime(1980, 1, 1);
                    break;
                case "PreviousAndCurrent":
                    GivingStartDate = GivingStartDate = new DateTime(GivingEndDate.Year - 1, 1, 1);
                    break;
                default:
                    GivingStartDate = new DateTime(int.Parse(Year1), 1, 1);
                    GivingEndDate = new DateTime(int.Parse(Year1), 12, 31);
                    break;
            }
            return contributionRecords.Where(p => p.ContributionDate >= GivingStartDate && p.ContributionDate <= GivingEndDate);
        }

        private IQueryable<Contribution> GetContributionRecords()
        {
            var currentUser = CurrentDatabase.CurrentUserPerson;
            var isFinanceUser = Roles.GetRolesForUser().Contains("Finance");
            var isCurrentUser = currentUser.PeopleId == Person.PeopleId;
            var isSpouse = currentUser.PeopleId == Person.SpouseId;
            var isFamilyMember = currentUser.FamilyId == Person.FamilyId;
            if (isCurrentUser || (isSpouse && (Person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint) || isFamilyMember || isFinanceUser)
            {
                return from c in CurrentDatabase.Contributions
                       where (c.PeopleId == Person.PeopleId || (c.PeopleId == Person.SpouseId && (Person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint))
                       && c.ContributionStatusId == ContributionStatusCode.Recorded
                       && !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                       select c;
            }
            else
            {
                return from c in CurrentDatabase.Contributions
                       join f in CurrentDatabase.ContributionFunds.ScopedByRoleMembership(CurrentDatabase) on c.FundId equals f.FundId
                       where c.PeopleId == Person.PeopleId
                       && c.ContributionStatusId == ContributionStatusCode.Recorded
                       && !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                       select c;
            }
        }

        public List<PledgesSummary> GetPledgesSummary()
        {
            showNegativePledgeBalances = CurrentDatabase.Setting("ShowNegativePledgeBalances");
            IQueryable<Contribution> contributionRecords = GetContributionRecords();
            PledgesSummary = new List<PledgesSummary>();
            foreach (Contribution contribution in contributionRecords.Where(p => p.ContributionTypeId == ContributionTypeCode.Pledge).OrderByDescending(c => c.ContributionDate))
            {
                AddSummaryPledge(contribution, contributionRecords);
            }
            return OrderPledgesByOnlineSort();
        }

        private List<PledgesSummary> OrderPledgesByOnlineSort()
        {
            var withSort = PledgesSummary.Where(p => p.FundOnlineSort != null).OrderBy(c => c.FundOnlineSort).ToList();
            var withoutSort = PledgesSummary.Where(p => p.FundOnlineSort == null).OrderBy(c => c.Fund).ToList();
            withSort.AddRange(withoutSort);
            return withSort;
        }

        private List<GivingSummary> OrderGivingsByOnlineSort()
        {
            var withSort = GivingSummary.Where(p => p.FundOnlineSort != null).OrderBy(c => c.FundOnlineSort).ToList();
            var withoutSort = GivingSummary.Where(p => p.FundOnlineSort == null).OrderBy(c => c.Fund).ToList();
            withSort.AddRange(withoutSort);
            return withSort;
        }

        public IEnumerable<SelectListItem> GivingYearsList()
        {
            SetGivingYears(ApplyFilter(GetContributionRecords()));
            return GivingYears.Select(i => new SelectListItem { Text = i, Selected = Year == i });
        }

        public List<GivingSummary> GetGivingSummary()
        {
            IQueryable<Contribution> contributionRecords = ApplyYear(GetContributionRecords());
            GivingSummary = new List<GivingSummary>();
            foreach (Contribution contribution in contributionRecords.Where(p => p.ContributionTypeId != ContributionTypeCode.Pledge).OrderByDescending(c => c.ContributionDate))
            {
                AddGivingSummary(contribution, contributionRecords);
            }
            return OrderGivingsByOnlineSort();
        }

        private void AddSummaryPledge(Contribution contribution, IQueryable<Contribution> contributionRecords)
        {
            var fundName = contribution.ContributionFund.FundName;
            var fundOnlineSort = contribution.ContributionFund.OnlineSort;
            if (!PledgesSummary.Any(p => p.Fund == fundName))
            {
                var lastPledgeDate = contribution.ContributionDate;
                decimal amountPledged = contributionRecords.Where(c => c.ContributionTypeId == ContributionTypeCode.Pledge && c.ContributionFund.FundName == fundName)
                                                    .Sum(c => c.ContributionAmount ?? 0);
                List<Contribution> contributionsThisFund = contributionRecords
                    .Where(c => c.ContributionTypeId != ContributionTypeCode.Pledge && c.ContributionFund.FundName == fundName).ToList();
                decimal amountContributed = 0;
                if (contributionsThisFund.Count != 0)
                {
                    amountContributed = contributionsThisFund.Sum(c => c.ContributionAmount ?? 0);
                }
                var pledgeBalance = amountPledged - amountContributed;
                if (pledgeBalance < 0 && !showNegativePledgeBalances)
                {
                    pledgeBalance = 0;
                }
                PledgesSummary.Add(new PledgesSummary()
                {
                    FundId = contribution.ContributionFund.FundId,
                    Fund = fundName,
                    AmountPledged = amountPledged,
                    AmountContributed = amountContributed,
                    Balance = pledgeBalance,
                    LastPledgeDate = contribution.ContributionDate.Value,
                    FundOnlineSort = contribution.ContributionFund.OnlineSort
                });
            }
        }

        private void AddGivingSummary(Contribution contribution, IQueryable<Contribution> contributionRecords)
        {
            var fundName = contribution.ContributionFund.FundName;
            if (!GivingSummary.Any(p => p.Fund == fundName))
            {
                List<Contribution> contributionsThisFund = contributionRecords
                    .Where(c => c.ContributionTypeId != ContributionTypeCode.Pledge && c.ContributionFund.FundName == fundName).ToList();
                decimal amountContributed = 0;
                if (contributionsThisFund.Count != 0)
                {
                    amountContributed = contributionsThisFund.Sum(c => c.ContributionAmount ?? 0);
                }
                GivingSummary.Add(new GivingSummary()
                {
                    FundId = contribution.ContributionFund.FundId,
                    Fund = fundName,
                    AmountContributed = amountContributed,
                    FundOnlineSort = contribution.ContributionFund.OnlineSort
                });
            }
        }

        public override IQueryable<Contribution> DefineModelSort(IQueryable<Contribution> q)
        {
            switch (SortExpression)
            {
                case "Name":
                    return from c in q
                           orderby c.Person.Name2, c.ContributionDate
                           select c;
                case "Name desc":
                    return from c in q
                           orderby c.Person.Name2 descending, c.ContributionDate
                           select c;
                case "Type":
                    return from c in q
                           let online = c.BundleDetails.Single().BundleHeader.BundleHeaderTypeId == BundleTypeCode.Online
                           orderby c.ContributionType.Description, online, c.ContributionDate
                           select c;
                case "Type desc":
                    return from c in q
                           let online = c.BundleDetails.Single().BundleHeader.BundleHeaderTypeId == BundleTypeCode.Online
                           orderby c.ContributionType.Description descending, online descending, c.ContributionDate
                           select c;
                case "Fund":
                    return from c in q
                           orderby c.ContributionFund.FundDescription, c.ContributionDate
                           select c;
                case "Fund desc":
                    return from c in q
                           orderby c.ContributionFund.FundDescription descending, c.ContributionDate
                           select c;
                case "Amount":
                    return from c in q
                           orderby c.ContributionAmount, c.ContributionDate
                           select c;
                case "Amount desc":
                    return from c in q
                           orderby c.ContributionAmount descending, c.ContributionDate
                           select c;
                case "CheckNo":
                    return from c in q
                           orderby c.CheckNo, c.ContributionDate
                           select c;
                case "CheckNo desc":
                    return from c in q
                           orderby c.CheckNo descending, c.ContributionDate
                           select c;
                case "Date":
                    return q.OrderBy(c => c.ContributionDate);
                case "Date desc":
                default:
                    return q.OrderByDescending(c => c.ContributionDate);
            }
        }

        public override IEnumerable<ContributionInfo> DefineViewList(IQueryable<Contribution> q)
        {
            var q2 = from c in q
                     let online = c.BundleDetails.Single().BundleHeader.BundleHeaderType.Description.Contains("Online")
                     select new ContributionInfo()
                     {
                         Amount = c.ContributionAmount ?? 0,
                         CheckNo = c.CheckNo,
                         ImageId = c.ImageID,
                         ContributionId = c.ContributionId,
                         Date = c.ContributionDate.Value,
                         Fund = c.ContributionFund.FundDescription,
                         Name = c.Person.PeopleId == PeopleId ? c.Person.PreferredName : c.Person.Name,
                         Type = ContributionTypeCode.SpecialTypes.Contains(c.ContributionTypeId)
                              ? c.ContributionType.Description
                              : !online
                                  ? c.ContributionType.Description
                                  : c.ContributionDesc == "Recurring Giving"
                                      ? c.ContributionDesc
                                      : "Online",
                         PledgeFund = c.ContributionFund.FundPledgeFlag,
                         TranId = c.TranId
                     };
            return q2;
        }

        public static IEnumerable<StatementInfoWithFund> Statements(CMSDataContext db, int? id, int[] includedFundIds = null)
        {
            if (!id.HasValue)
            {
                throw new ArgumentException("Missing id");
            }

            var person = db.LoadPersonById(id.Value);
            var contributions = (from c in db.Contributions2(new DateTime(1900, 1, 1), new DateTime(3000, 12, 31), 0, false, null, true, null)
                                 where (c.PeopleId == person.PeopleId || (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint))
                                 select c).ToList();
            var currentUser = db.CurrentUserPerson;
            if (currentUser.PeopleId != person.PeopleId)
            {
                var authorizedFunds = db.ContributionFunds.ScopedByRoleMembership(db);
                var authorizedContributions = from c in contributions
                                              join f in authorizedFunds on c.FundId equals f.FundId
                                              select c;
                contributions = authorizedContributions.ToList();
            }
            if (includedFundIds != null)
            {
                contributions = contributions.Where(c => includedFundIds.Contains(c.FundId)).ToList();
            }

            var shouldGroupByFunds = db.Setting("EnableContributionFundsOnStatementDisplay");

            IEnumerable<StatementInfoWithFund> result;

            if (shouldGroupByFunds)
            {
                result = (from c in contributions
                          group c by new { c.DateX.Value.Year, c.FundName, c.FundId } into g
                          orderby g.Key.Year descending, g.Key.FundName ascending
                          select new StatementInfoWithFund()
                          {
                              Count = g.Count(),
                              Amount = g.Sum(cc => cc.Amount ?? 0),
                              StartDate = new DateTime(g.Key.Year, 1, 1),
                              EndDate = new DateTime(g.Key.Year, 12, 31),
                              FundName = g.Key.FundName,
                              FundId = g.Key.FundId,
                              FundGroupName = string.Empty
                          }).ToList();

                var displayNameHelper = new CustomFundSetDisplayHelper(db);
                displayNameHelper.ProcessList(result);

                // hack: grouping done in memory since these fundids are stored as XML and not easily accessed in SQL
                // task: FundGrouping table to avoid using XML for this data in the future with UI to make management easier?
                result = (from c in result
                          group c by new { c.StartDate.Year, c.FundGroupName } into g
                          orderby g.Key.Year descending, g.Key.FundGroupName ascending
                          select new StatementInfoWithFund()
                          {
                              Count = g.Sum(cc => cc.Count),
                              Amount = g.Sum(cc => cc.Amount),
                              StartDate = new DateTime(g.Key.Year, 1, 1),
                              EndDate = new DateTime(g.Key.Year, 12, 31),
                              FundName = "",
                              FundId = 0,
                              FundGroupName = g.Key.FundGroupName
                          }).ToList();
            }
            else
            {
                result = (from c in contributions
                          group c by new { c.DateX.Value.Year } into g
                          orderby g.Key.Year descending
                          select new StatementInfoWithFund()
                          {
                              Count = g.Count(),
                              Amount = g.Sum(cc => cc.Amount ?? 0),
                              StartDate = new DateTime(g.Key.Year, 1, 1),
                              EndDate = new DateTime(g.Key.Year, 12, 31),
                              FundName = string.Empty,
                              FundId = 0
                          }).ToList();
            }

            return result;
        }
    }
}
