using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Linq;
using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class ContributionsModel : PagedTableModel<Contribution, ContributionInfo>
    {
        public int PeopleId { get; set; }
        public Person person;
        public bool ShowNames;
        public bool ShowTypes;
        [DisplayName("Electronic Only"), TrackChanges]
        public bool ElectronicStatement { get; set; }
        [DisplayName("Statement Option")]
        public CodeInfo ContributionOptions { get; set; }
        [DisplayName("Envelope Option")]
        public CodeInfo EnvelopeOptions { get; set; }

        public ContributionsModel(int id, PagerModel2 pager = null)
            : base("Date", "desc", pager)
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
            ContributionOptions = new CodeInfo(person.ContributionOptionsId, "ContributionOptions");
            EnvelopeOptions = new CodeInfo(person.EnvelopeOptionsId, "EnvelopeOptions");
            ElectronicStatement = person.ElectronicStatement ?? false;
        }

        public override IQueryable<Contribution> DefineModelList()
        {
            var q = from c in DbUtil.Db.Contributions
                    where c.PeopleId == person.PeopleId
                        || (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    select c;
            ShowNames = q.Any(c => c.PeopleId != person.PeopleId);
            ShowTypes = q.Any(c => ContributionTypeCode.SpecialTypes.Contains(c.ContributionTypeId));
            return q;
        }

        public override IQueryable<Contribution> DefineModelSort(IQueryable<Contribution> q)
        {
            switch (Pager.SortExpression)
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
                         Date = c.ContributionDate.Value,
                         Fund = c.ContributionFund.FundDescription,
                         Name = c.Person.PeopleId == person.PeopleId ? c.Person.PreferredName : c.Person.Name,
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

        public static IEnumerable<StatementInfo> Statements(int id)
        {
            var person = DbUtil.Db.LoadPersonById(id);
            return from c in DbUtil.Db.Contributions2(new DateTime(1900, 1, 1), new DateTime(3000, 12, 31), 0, false, null, true)
                   where c.PeopleId == person.PeopleId
                        || (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                   group c by c.DateX.Value.Year into g
                   orderby g.Key descending
                   select new StatementInfo()
                   {
                       Count = g.Count(),
                       Amount = g.Sum(cc => cc.Amount ?? 0),
                       StartDate = new DateTime(g.Key, 1, 1),
                       EndDate = new DateTime(g.Key, 12, 31)
                   };
        }
    }
}