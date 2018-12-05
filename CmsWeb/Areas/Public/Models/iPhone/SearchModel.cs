using CmsData;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;


namespace CmsWeb.Models.iPhone
{
    public class SearchModel
    {
        public string Name { get; set; }
        public string Comm { get; set; }
        public string Addr { get; set; }
        public SearchModel() { }
        public SearchModel(string name, string comm, string addr)
        {
            Name = name;
            Comm = comm;
            Addr = addr;
        }

        private IEnumerable<PeopleInfo> PeopleList(IQueryable<Person> query)
        {
            var q = from p in query
                    select new PeopleInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        First = p.FirstName,
                        Last = p.LastName,
                        Address = p.PrimaryAddress,
                        CityStateZip = p.PrimaryCity + ", " + p.PrimaryState + " " + p.PrimaryZip.Substring(0, 5),
                        Zip = p.PrimaryZip.Substring(0, 5),
                        Age = p.Age,
                        BirthYear = p.BirthYear,
                        BirthMon = p.BirthMonth,
                        BirthDay = p.BirthDay,
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        HasPicture = p.PictureId != null,
                    };
            return q;
        }

        public int Count(bool fromAddGuest = false)
        {
            return ApplySearch(fromAddGuest).Count();
        }
        public IEnumerable<PeopleInfo> PeopleList()
        {
            var q = ApplySearch().OrderBy(p => p.Name2).Take(50);
            return PeopleList(q);
        }

        private IQueryable<Person> query = null;
        public IQueryable<Person> ApplySearch(bool fromAddGuest = false)
        {
            if (query.IsNotNull())
            {
                return query;
            }

            //var db = Db;
            var ignoreOrgleadersonly = DbUtil.Db.Setting("RelaxAppAddGuest", "false").ToBool() && fromAddGuest;
            if (ignoreOrgleadersonly)
            {
                query = DbUtil.Db.People.Select(p => p);
            }
            else
            {
                query = Util2.OrgLeadersOnly
                    ? DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db)
                    : DbUtil.Db.People.Select(p => p);
            }

            //query = query.Where(pp => pp.DeceasedDate == null);

            if (Name.HasValue())
            {
                string first, last;
                Util.NameSplit(Name, out first, out last);
                DbUtil.LogActivity($"iphone search '{first}' '{last}'");
                if (first.HasValue())
                {
                    query = from p in query
                            where p.LastName.StartsWith(last) || p.MaidenName.StartsWith(last)
                                || p.LastName.StartsWith(Name) || p.MaidenName.StartsWith(Name) // gets Bob St Clair
                            where
                                p.FirstName.StartsWith(first) || p.NickName.StartsWith(first) || p.MiddleName.StartsWith(first)
                                || p.LastName.StartsWith(Name) || p.MaidenName.StartsWith(Name) // gets Bob St Clair
                            select p;
                }
                else
                    if (last.AllDigits())
                {
                    query = from p in query
                            where p.PeopleId == last.ToInt()
                            select p;
                }
                else
                {
                    query = from p in query
                            where p.LastName.StartsWith(Name) || p.MaidenName.StartsWith(Name)
                            || p.FirstName.StartsWith(Name) || p.NickName.StartsWith(Name) || p.MiddleName.StartsWith(Name)
                            select p;
                }
            }
            if (Addr.IsNotNull())
            {
                Addr = Addr.Trim();
                if (Addr.HasValue())
                {
                    query = from p in query
                            where p.Family.AddressLineOne.Contains(Addr)
                               || p.Family.AddressLineTwo.Contains(Addr)
                               || p.Family.CityName.Contains(Addr)
                               || p.Family.ZipCode.Contains(Addr)
                            select p;
                }
            }
            if (Comm.IsNotNull())
            {
                Comm = Comm.Trim();
                if (Comm.HasValue())
                {
                    query = from p in query
                            where p.CellPhone.Contains(Comm) || p.EmailAddress.Contains(Comm)
                            || p.Family.HomePhone.Contains(Comm)
                            || p.WorkPhone.Contains(Comm)
                            select p;
                }
            }
            return query;
        }
    }
}
