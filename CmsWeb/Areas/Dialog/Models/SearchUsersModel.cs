/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class SearchUsersModel
    {
        public string name { get; set; }
        public int maxitems { get; set; }
        public int count { get; set; }
        public int listcount { get; set; }
        public bool singlemode { get; set; }
        public bool ordered { get; set; }
        public int? topid { get; set; }

        public SearchUsersModel()
        {
            maxitems = 25;
        }
        private IQueryable<Person> people;
        public IQueryable<Person> FetchPeople()
        {
            if (people != null)
            {
                return people;
            }

            if (Util2.OrgLeadersOnly)
            {
                people = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);
            }
            else
            {
                people = DbUtil.Db.People.AsQueryable();
            }

            people = people.Where(p => p.Users.Any(uu => uu.UserRoles.Any(ur => ur.Role.RoleName == "Access")));
            if (name.HasValue())
            {
                string First, Last;
                Util.NameSplit(name, out First, out Last);
                if (First.HasValue())
                {
                    people = from p in people
                             where (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                                 || p.LastName.StartsWith(name) || p.MaidenName.StartsWith(name))
                             && (p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) || p.MiddleName.StartsWith(First))
                             select p;
                }
                else
                    if (Last.AllDigits())
                {
                    people = from p in people
                             where p.PeopleId == Last.ToInt()
                             select p;
                }
                else
                {
                    people = from p in people
                             where p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                                 || p.LastName.StartsWith(name) || p.MaidenName.StartsWith(name)
                             select p;
                }
            }
            return people;
        }

        public List<UserInfo> PeopleList()
        {
            var people = FetchPeople();

            var list = new List<UserInfo>();
            var n = 0;
            if (!singlemode)
            {
                var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
                n = t.People(DbUtil.Db).Count();
                list = CheckedPeopleList(
                        from p in t.People(DbUtil.Db)
                        orderby p.PeopleId == topid ? "0" : "1"
                        select p).ToList();
                var ids = list.Select(p => p.PeopleId).ToArray();
                people = people.Where(p => !ids.Contains(p.PeopleId));
            }
            count = people.Count();
            people = people.OrderBy(p => p.LastName).ThenBy(p => p.Name);
            list.AddRange(PeopleList(people).Take(maxitems));
            maxitems += n;
            listcount = list.Count;
            return list;
        }
        private IEnumerable<UserInfo> CheckedPeopleList(IQueryable<Person> query)
        {
            return PeopleList(query, ck: true);
        }
        private IEnumerable<UserInfo> PeopleList(IQueryable<Person> query)
        {
            return PeopleList(query, ck: false);
        }
        private IEnumerable<UserInfo> PeopleList(IQueryable<Person> query, bool ck)
        {
            var q = from p in query
                    select new UserInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        EmailAddress = p.EmailAddress,
                        HasTag = ck
                    };
            return q;
        }
        public class UserInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public bool HasTag { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}
