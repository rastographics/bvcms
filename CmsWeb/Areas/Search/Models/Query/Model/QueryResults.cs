using CmsData;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class QueryResults : PagedTableModel<Person, PeopleInfo>
    {
        internal CMSDataContext Db;
        public string Description { get { return topclause.Description; } }
        public string SaveToDescription { get { return topclause.PreviousName ?? topclause.Description; } }
        public Guid? QueryId { get; set; }

        private Condition topclause;
        public Condition TopClause
        {
            get
            {
                if (topclause != null)
                {
                    return topclause;
                }

                if (QueryId == null)
                {
                    topclause = Db.FetchLastQuery();
                }
                else
                {
                    topclause = Db.LoadCopyOfExistingQuery(QueryId.Value);
                }

                return topclause;
            }
            set
            {
                topclause = value;
                QueryId = value.Id;
            }
        }

        public QueryResults()
            : base("na", "asc")
        {
        }

        public override IQueryable<Person> DefineModelList()
        {
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));

            if (TopClause.PlusParentsOf)
            {
                q = Db.PersonQueryPlusParents(q);
            }
            else if (TopClause.ParentsOf)
            {
                q = Db.PersonQueryParents(q);
            }

            if (TopClause.FirstPersonSameEmail)
            {
                q = Db.PersonQueryFirstPersonSameEmail(q);
            }
            return q;
        }

        public override IQueryable<Person> DefineModelSort(IQueryable<Person> q)
        {
            switch (SortExpression)
            {
                case "Name":
                    return from p in q
                           orderby p.LastName, p.FirstName, p.PeopleId
                           select p;
                case "Status":
                    return from p in q
                           orderby p.MemberStatus.Code, p.LastName, p.FirstName, p.PeopleId
                           select p;
                case "Address":
                    return from p in q
                           orderby p.PrimaryState, p.PrimaryCity, p.PrimaryAddress, p.PeopleId
                           select p;
                case "Fellowship Leader":
                    return from p in q
                           orderby p.BFClass.LeaderName, p.LastName, p.FirstName, p.PeopleId
                           select p;
                case "Employer":
                    return from p in q
                           orderby p.EmployerOther, p.LastName, p.FirstName, p.PeopleId
                           select p;
                case "Communication":
                    return from p in q
                           orderby p.EmailAddress, p.LastName, p.FirstName, p.PeopleId
                           select p;
                case "Age":
                    return from p in q
                           orderby p.Age
                           select p;
                case "DOB":
                    return from p in q
                           orderby p.BirthMonth, p.BirthDay, p.LastName, p.FirstName
                           select p;
                case "Status desc":
                    return from p in q
                           orderby p.MemberStatus.Code descending, p.LastName descending, p.FirstName descending,
                               p.PeopleId descending
                           select p;
                case "Address desc":
                    return from p in q
                           orderby p.PrimaryState descending, p.PrimaryCity descending, p.PrimaryAddress descending,
                               p.PeopleId descending
                           select p;
                case "Name desc":
                    return from p in q
                           orderby p.LastName descending, p.LastName descending, p.PeopleId descending
                           select p;
                case "Fellowship Leader desc":
                    return from p in q
                           orderby p.BFClass.LeaderName descending, p.LastName descending, p.FirstName descending,
                               p.PeopleId descending
                           select p;
                case "Employer desc":
                    return from p in q
                           orderby p.EmployerOther descending, p.LastName descending, p.FirstName descending,
                               p.PeopleId descending
                           select p;
                case "Communication desc":
                    return from p in q
                           orderby p.EmailAddress descending, p.LastName descending, p.FirstName descending,
                               p.PeopleId descending
                           select p;
                case "Age desc":
                    return from p in q
                           orderby p.Age descending
                           select p;
                case "DOB desc":
                    return from p in q
                           orderby p.BirthMonth descending, p.BirthDay descending, p.LastName descending,
                               p.FirstName descending
                           select p;
            }
            return q;
        }

        public override IEnumerable<PeopleInfo> DefineViewList(IQueryable<Person> q)
        {
            return from p in q
                   select new PeopleInfo
                   {
                       PeopleId = p.PeopleId,
                       Name = p.Name,
                       AltName = p.AltName,
                       BirthYear = p.BirthYear,
                       BirthMon = p.BirthMonth,
                       BirthDay = p.BirthDay,
                       Address = p.PrimaryAddress,
                       Address2 = p.PrimaryAddress2,
                       CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                       HomePhone = p.HomePhone,
                       CellPhone = p.CellPhone,
                       WorkPhone = p.WorkPhone,
                       PhonePref = p.PhonePrefId,
                       MemberStatus = p.MemberStatus.Description,
                       Email = p.EmailAddress,
                       BFTeacher = p.BFClass.LeaderName,
                       BFTeacherId = p.BFClass.LeaderId,
                       Employer = p.EmployerOther,
                       Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                       HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName
                           && t.Tag.PeopleId == Util2.CurrentTagOwnerId
                           && t.Tag.TypeId == DbUtil.TagTypeId_Personal),
                   };
        }
    }
}
