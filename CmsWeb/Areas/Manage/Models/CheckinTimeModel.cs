using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using CmsData;
using UtilityExtensions;
using System.Data.Linq;

namespace CmsWeb.Models
{
    /*
     * Update for Special Access

        UPDATE c
        SET c.AccessTypeID = 4
        FROM dbo.CheckInTimes c
        INNER JOIN dbo.People ON c.PeopleId = dbo.People.PeopleId
        WHERE MemberStatusId != 10
	    AND GuestOfId IS NULL
    
     * Update for Members
    
        UPDATE c
        SET c.AccessTypeID = 1
        FROM dbo.CheckInTimes c
        INNER JOIN dbo.People ON c.PeopleId = dbo.People.PeopleId
        WHERE MemberStatusId = 10
    
     * Update for Guest of Members
     
        UPDATE c
        SET c.AccessTypeID = 3
        FROM dbo.CheckInTimes c
        INNER JOIN dbo.People ON c.PeopleId = dbo.People.PeopleId
        WHERE MemberStatusId != 10
	        AND GuestOfId IS NOT NULL
    */
    public class CheckinTimeModel
    {
        public static string ALL_ACTIVITIES = "- All Activities -";

        public string namesearch { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        //public int peopleid { get; set; }
        public int accesstype { get; set; }
        public string location { get; set; }
        public string activity { get; set; }

        public PagerModel2 Pager { get; set; }

        public CheckinTimeModel()
        {
            Pager = new PagerModel2();
            Pager.setCountDelegate(Count);
            Pager.Direction = "desc";
            Pager.Sort = "Date/Time";
            var locs = Locations();
            location = DbUtil.Db.UserPreference("checkintimes-location", locs.FirstOrDefault());
        }

        public List<string> Activities()
        {
            var q = from a in DbUtil.Db.CheckInActivities
                    group a.Activity by a.Activity into g
                    select g.Key;
            var list = q.ToList();
            list.Insert(0, ALL_ACTIVITIES);
            return list;
        }

        public List<string> AccessTypes(bool bForDropDown = false)
        {
            var q = from a in DbUtil.Db.BuildingAccessTypes
                    select a.Description;
            var list = q.ToList();

            if (bForDropDown) list.Insert(0, "- All Types -");
            else list.Insert(0, "Unknown");

            return list;
        }

        private List<string> locations;
        public List<string> Locations()
        {
            if (locations == null)
            {
                var q = from t in DbUtil.Db.CheckInTimes
                        where t.Location != null
                        group t.Location by t.Location
                            into g
                            select g.Key;
                locations = q.ToList();
            }
            return locations;
        }

        public class CheckinTimeEx
        {
            public CheckInTime ctime { get; set; }
            public string name { get; set; }
            public string activities { get; set; }
            public EntitySet<CheckInTime> guests { get; set; }
            public int guestcount { get; set; }
            public int accesstype { get; set; }
        }

        public class CountInfo
        {
            public int members { get; set; }
            public int guests { get; set; }
            /*
            private string _name;
            public string name
            {
                get { return _name.HasValue() ? _name : "Not Specified"; }
                set { _name = value; }
            }
             */
        }

        private CountInfo _counts;
        public CountInfo counts
        {
            get
            {
                if (_counts == null)
                    FetchTimes();
                return _counts;
            }
        }

        public int Count()
        {
            return counts.guests = counts.members;
        }

        public class ActivityCount
        {
            public string name;
            public int count;
        }

        public List<ActivityCount> FetchActivityCount()
        {
            var q1 = from e in DbUtil.Db.CheckInTimes
                     join c in DbUtil.Db.CheckInActivities on e.Id equals c.Id
                     where e.Location == location
                     where e.CheckInTimeX >= dateStart || dateStart == null
                     where e.CheckInTimeX < dateEnd || dateEnd == null
                     group c by c.Activity into grouped
                     select new ActivityCount() { name = grouped.Key, count = grouped.Count() };

            var q2 = from e in DbUtil.Db.CheckInTimes
                     where e.Location == location
                     where e.CheckInTimeX >= dateStart || dateStart == null
                     where e.CheckInTimeX < dateEnd || dateEnd == null
                     group e by e.PeopleId into grouped
                     select new { key = grouped.Key };

            var q3 = from e in DbUtil.Db.CheckInTimes
                     where e.Location == location
                     where e.CheckInTimeX >= dateStart || dateStart == null
                     where e.CheckInTimeX < dateEnd || dateEnd == null
                     select e;

            var p = new ActivityCount() { name = "Total Guests", count = q3.Count() };
            var q = new ActivityCount() { name = "Unique People", count = q2.Count() };


            var lq = q1.ToList();
            lq.Insert(0, q);
            lq.Insert(0, p);

            return lq;
        }

        private IEnumerable<CheckinTimeEx> _times;
        public IEnumerable<CheckinTimeEx> FetchTimes()
        {
            if (_times != null)
                return _times;

            // filter
            if (dateEnd != null)
                dateEnd = dateEnd.Value.AddHours(24);

            var q = from t in DbUtil.Db.CheckInTimes
                    where t.Location == location
                    where t.CheckInTimeX >= dateStart || dateStart == null
                    where t.CheckInTimeX < dateEnd || dateEnd == null
                    //where peopleid == 0 || t.PeopleId == peopleid || t.Guests.Any(g => g.PeopleId == peopleid)
                    where accesstype == 0 || t.AccessTypeID == accesstype || t.Guests.Any(g => g.AccessTypeID == accesstype)
                    where t.GuestOfId == null
                    select t;

            // count
            var q2 = from t in DbUtil.Db.CheckInTimes
                     where t.Location == location
                     where t.CheckInTimeX >= dateStart || dateStart == null
                     where t.CheckInTimeX < dateEnd || dateEnd == null
                     //where peopleid == 0 || t.PeopleId == peopleid || t.Guests.Any(g => g.PeopleId == peopleid)
                     where accesstype == 0 || t.AccessTypeID == accesstype || t.Guests.Any(g => g.AccessTypeID == accesstype)
                     select t;

            if (namesearch != null && namesearch.Length > 0)
            {
                q = ApplyNameSearch(q, namesearch);
                q2 = ApplyNameSearch(q2, namesearch);
            }

            if (activity != null && !activity.Equals(ALL_ACTIVITIES))
            {
                q = from t in q
                    where t.CheckInActivities.Any(z => z.Activity == activity)
                    select t;

                q2 = from t in q2
                    where t.CheckInActivities.Any(z => z.Activity == activity)
                    select t;
            }

            CountInfo ci = new CountInfo();

            ci.members = (from c in q2
                          where c.AccessTypeID == 1
                          select c).Count();

            ci.guests = (from c in q2
                          where c.AccessTypeID != 1
                          select c).Count();

            _counts = ci;

            // sort
            q = SortItems(q);

            // transform to view model
            _times = from t in q.Skip(Pager.StartRow).Take(Pager.PageSize)
                     select new CheckinTimeEx
                     {
                         ctime = t,
                         activities = string.Join(",",
                             t.CheckInActivities.Select(a => a.Activity)),
                         name = t.Person.Name,
                         guestcount = t.Guests.Count(),
                         guests = t.Guests,
                         accesstype = t.AccessTypeID ?? 0,
                     };
            return _times;
        }

        public IQueryable<CheckInTime> SortItems(IQueryable<CheckInTime> results)
        {
            if (Pager.Sort == "Host")
                return from z in results
                       orderby z.CheckInTimeX
                       select z;
            switch (Pager.Direction)
            {
                case "asc":
                    switch (Pager.Sort)
                    {
                        case "Person":
                            results = results.OrderBy(z => z.Person.Name2);
                            break;
                        case "Date/Time":
                            results = results.OrderBy(z => z.CheckInTimeX);
                            break;
                        case "Activity":
                            results = from z in results
                                      orderby z.CheckInActivities.FirstOrDefault().Activity,
                                        z.CheckInTimeX
                                      select z;
                            break;
                    }
                    break;
                case "desc":
                    switch (Pager.Sort)
                    {
                        case "Person":
                            results = results.OrderByDescending(z => z.Person.Name2);
                            break;
                        case "Date/Time":
                            results = results.OrderByDescending(z => z.CheckInTimeX);
                            break;
                        case "Activity":
                            results = from z in results
                                      orderby z.CheckInActivities.FirstOrDefault().Activity descending,
                                        z.CheckInTimeX
                                      select z;
                            break;
                    }
                    break;
            }

            return results;
        }

        private static void NameSplit(string name, out string First, out string Last)
        {
            var a = name.Split(' ');
            First = "";
            if (a.Length > 1)
            {
                First = a[0];
                Last = a[1];
            }
            else
                Last = a[0];

        }

        private IQueryable<CheckInTime> ApplyNameSearch(IQueryable<CheckInTime> q, string namesearch)
        {
            string First, Last;

            NameSplit(namesearch, out First, out Last);

            if (First.HasValue())
            {
                q = from p in q
                    where (p.Person.LastName.StartsWith(Last) || p.Person.MaidenName.StartsWith(Last) || p.Person.LastName.StartsWith(namesearch) || p.Person.MaidenName.StartsWith(namesearch))
                        &&
                        (p.Person.FirstName.StartsWith(First) || p.Person.NickName.StartsWith(First) || p.Person.MiddleName.StartsWith(First) || p.Person.LastName.StartsWith(namesearch) || p.Person.MaidenName.StartsWith(namesearch))
                        ||
                        p.Guests.Any( g => (g.Person.LastName.StartsWith(Last) || g.Person.MaidenName.StartsWith(Last) || g.Person.LastName.StartsWith(namesearch) || g.Person.MaidenName.StartsWith(namesearch) )
                        &&
                        (g.Person.FirstName.StartsWith(First) || g.Person.NickName.StartsWith(First) || g.Person.MiddleName.StartsWith(First) || g.Person.LastName.StartsWith(namesearch) || g.Person.MaidenName.StartsWith(namesearch)))
                    select p;
            }
            else
            {
                if (Last.AllDigits())
                    q = from p in q
                        where p.PeopleId == Last.ToInt()
                        select p;
                else
                    q = from p in q
                        where p.Person.LastName.StartsWith(Last) || p.Person.MaidenName.StartsWith(Last) || p.Person.LastName.StartsWith(namesearch) || p.Person.MaidenName.StartsWith(namesearch)
                        ||
                        p.Guests.Any((g => g.Person.LastName.StartsWith(Last) || g.Person.MaidenName.StartsWith(Last) || g.Person.LastName.StartsWith(namesearch) || g.Person.MaidenName.StartsWith(namesearch)))
                        select p;
            }

            return q;
        }
    }
}
