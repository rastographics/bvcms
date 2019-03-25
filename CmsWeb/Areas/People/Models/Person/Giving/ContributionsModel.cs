using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
                Person = DbUtil.Db.LoadPersonById(peopleid);
                ContributionOptions = new CodeInfo(Person.ContributionOptionsId, "ContributionOptions");
                EnvelopeOptions = new CodeInfo(Person.EnvelopeOptionsId, "EnvelopeOptions");
                ElectronicStatement = Person.ElectronicStatement ?? false;
            }
        }
        private int peopleid;
        public Person Person;
        public bool ShowNames;
        public bool ShowTypes;
        [DisplayName("Electronic Only"), TrackChanges]
        public bool ElectronicStatement { get; set; }
        [DisplayName("Statement Option")]
        public CodeInfo ContributionOptions { get; set; }
        [DisplayName("Envelope Option")]
        public CodeInfo EnvelopeOptions { get; set; }
        public ContributionsModel()
            : base("Date", "desc", true)
        { }
        public override IQueryable<Contribution> DefineModelList()
        {
            IQueryable<Contribution> contributionRecords;
            var currentUser = DbUtil.Db.CurrentUserPerson;
            var isFinanceUser = Roles.GetRolesForUser().Contains("Finance");
            var isCurrentUser = currentUser.PeopleId == Person.PeopleId;
            var isSpouse = currentUser.PeopleId == Person.SpouseId;
            var isFamilyMember = currentUser.FamilyId == Person.FamilyId;
            if (isCurrentUser || (isSpouse && (Person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint) || isFamilyMember || isFinanceUser)
            {
                contributionRecords = from c in DbUtil.Db.Contributions
                                      where (c.PeopleId == Person.PeopleId || (c.PeopleId == Person.SpouseId && (Person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint))
                                      && c.ContributionStatusId == ContributionStatusCode.Recorded
                                      && !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                                      select c;
            }
            else
            {
                contributionRecords = from c in DbUtil.Db.Contributions
                                      join f in DbUtil.Db.ContributionFunds.ScopedByRoleMembership() on c.FundId equals f.FundId
                                      where c.PeopleId == Person.PeopleId
                                      && c.ContributionStatusId == ContributionStatusCode.Recorded
                                      && !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                                      select c;
            }
            var items = contributionRecords.ToList();
            ShowNames = items.Any(c => c.PeopleId != Person.PeopleId);
            ShowTypes = items.Any(c => ContributionTypeCode.SpecialTypes.Contains(c.ContributionTypeId));
            return contributionRecords;
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
                     };
            return q2;
        }
        public static IEnumerable<StatementInfoWithFund> Statements(int? id)
        {
            return Statements(id, null);
        }

        public static IEnumerable<StatementInfoWithFund> Statements(int? id, int[] includedFundIds)
        {
            if (!id.HasValue)
            {
                throw new ArgumentException("Missing id");
            }
            var dbContext = DbUtil.Db;
            var person = dbContext.LoadPersonById(id.Value);
            var contributions = (from c in dbContext.Contributions2(new DateTime(1900, 1, 1), new DateTime(3000, 12, 31), 0, false, null, true)
                                 where (c.PeopleId == person.PeopleId || (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint))
                                 select c).ToList();
            var currentUser = dbContext.CurrentUserPerson;
            if (currentUser.PeopleId != person.PeopleId)
            {
                var authorizedFunds = dbContext.ContributionFunds.ScopedByRoleMembership();
                var authorizedContributions = from c in contributions
                                              join f in authorizedFunds on c.FundId equals f.FundId
                                              select c;
                contributions = authorizedContributions.ToList();
            }
            if (includedFundIds != null)
            {
                contributions = contributions.Where(c => includedFundIds.Contains(c.FundId)).ToList();
            }

            var shouldGroupByFunds = dbContext.Setting("EnableContributionFundsOnStatementDisplay");

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

                var displayNameHelper = new CustomFundSetDisplayHelper(dbContext);
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
