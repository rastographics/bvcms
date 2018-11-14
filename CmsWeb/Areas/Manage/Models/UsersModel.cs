using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{

    public class UsersModel : PagerModel2
    {
        public class UserInfo
        {
            public int? peopleid { get; set; }
            public int? userid { get; set; }
            public string username { get; set; }
            public string name { get; set; }
            public bool online { get; set; }
            public string email { get; set; }
            public DateTime? activity { get; set; }
            public string roles { get; set; }
        }

        public string name { get; set; }
        public string[] Role { get; set; }

        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
            {
                _count = FetchUsers().Count();
            }

            return _count.Value;
        }
        public UsersModel()
        {
            Sort = "Activity";
            Direction = "desc";
            GetCount = Count;
        }
        public IEnumerable<UserInfo> Users()
        {
            var q = ApplySort();
            var q2 = q.Skip(StartRow).Take(PageSize).ToList();
            var dt = DateTime.Now.AddMinutes(-10);
            var q3 = from u in q2
                     let online = u.LastActivityDate > dt
                     select new UserInfo
                     {
                         userid = u.UserId,
                         name = u.Person.Name,
                         username = u.Username,
                         email = u.EmailAddress,
                         online = online,
                         peopleid = u.PeopleId,
                         activity = u.LastActivityDate,
                         roles = string.Join(", ", u.Roles.OrderBy(rr => rr))
                     };
            return q3;
        }

        public IEnumerable<SelectListItem> Roles()
        {
            var q = from r in DbUtil.Db.Roles
                    orderby r.RoleName
                    select new SelectListItem
                    {
                        Value = r.RoleId.ToString(),
                        Text = r.RoleName
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "-1", Text = "(not assigned)" });
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)", Selected = true });
            return list;
        }

        private IQueryable<User> _users;
        private IQueryable<User> FetchUsers()
        {
            if (_users != null)
            {
                return _users;
            }

            _users = from u in DbUtil.Db.Users select u;
            if (name.AllDigits())
            {
                _users = from u in _users
                         where u.UserId == name.ToInt()
                         select u;
            }
            else if (name.HasValue())
            {
                _users = from u in _users
                         where u.Username == name || u.Person.Name.Contains(name)
                         select u;
            }

            if (Role != null && Role.Length > 0)
            {
                var rids = Role.Select(rr => rr.ToInt()).ToArray();
                _users = from u in _users
                             /* below we use a trick for match all
                         * if they select more than role to match on
                         * then we get a count of the number of roles that match
                         * and that count should equal the length of the array.
                         * We also look for not specified (-1) and not assigned (0)
                         */
                         let rc = u.UserRoles.Count(ur => rids.Contains(ur.RoleId))
                         where rc == Role.Length || rids[0] <= 0
                         where !u.UserRoles.Any() || rids[0] != -1
                         select u;

            }
            return _users;
        }
        public IQueryable<User> ApplySort()
        {
            var q = FetchUsers();
            if (Direction == "asc")
            {
                switch (Sort)
                {
                    case "User":
                        q = from u in q
                            orderby u.Username
                            select u;
                        break;
                    case "Name":
                        q = from u in q
                            orderby u.Person.Name
                            select u;
                        break;
                    case "Online":
                        q = from u in q
                            orderby u.Person.Name
                            select u;
                        break;
                    case "Email":
                        q = from u in q
                            orderby u.EmailAddress
                            select u;
                        break;
                    case "Activity":
                        q = from u in q
                            orderby u.LastActivityDate
                            select u;
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "User":
                        q = from u in q
                            orderby u.Username descending
                            select u;
                        break;
                    case "Name":
                        q = from u in q
                            orderby u.Person.Name descending
                            select u;
                        break;
                    case "Online":
                        q = from u in q
                            orderby u.Person.Name descending
                            select u;
                        break;
                    case "Email":
                        q = from u in q
                            orderby u.EmailAddress descending
                            select u;
                        break;
                    case "Activity":
                        q = from u in q
                            orderby u.LastActivityDate descending
                            select u;
                        break;
                }
            }

            return q;
        }
    }
}
