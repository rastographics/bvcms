using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class ContributionsModel : PagedTableModel<Contribution, ContributionInfo>
    {
        public CmsData.Person person;
        public bool ShowNames;
        public bool ShowTypes;

        public ContributionsModel(int id)
            : base("Date", "desc")
        {
            person = DbUtil.Db.LoadPersonById(id);
        }

        public override IQueryable<Contribution> DefineModelList()
        {
            var q = from c in DbUtil.Db.Contributions
                    where c.PeopleId == person.PeopleId || (c.PeopleId == person.SpouseId && person.ContributionOptionsId == EnvelopeOptionCode.Joint)
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
            return from c in q
                   let online = c.BundleDetails.Single().BundleHeader.BundleHeaderTypeId == BundleTypeCode.Online
                   select new ContributionInfo()
                   {
                       Amount = c.ContributionAmount ?? 0,
                       CheckNo = c.CheckNo,
                       Date = c.ContributionDate.Value,
                       Fund = c.ContributionFund.FundDescription,
                       Name = c.Person.PeopleId == person.PeopleId ? c.Person.PreferredName : c.Person.Name,
                       Type = ContributionTypeCode.SpecialTypes.Contains(c.ContributionTypeId) ? c.ContributionType.Description : online ? "Online" : "Check/Cash",
                   };
        }
    }
}