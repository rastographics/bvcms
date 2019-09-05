using System;
using System.Web.Security;
using System.Collections.Specialized;

using System.Linq;
using System.Collections.Generic;
using CmsData;
using CmsWeb.Lifecycle;
using UtilityExtensions;

namespace CmsWeb.Membership
{
	public class CMSRoleProvider : RoleProvider
	{
        private static CMSRoleProvider _currentProvider;

        public static CMSRoleProvider provider
		{
			get { return _currentProvider ?? Roles.Provider as CMSRoleProvider; }
		}

        public static void SetCurrentProvider(CMSRoleProvider provider) => _currentProvider = provider;

        public override string ApplicationName { get { return "cms"; } set { } }

        public CMSDataContext CurrentDatabase => CMSDataContext.Create(HttpContextFactory.Current);

        public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null)
				throw new ArgumentNullException("config");

			if (name == null || name.Length == 0)
				name = "CMSRoleProvider";
			if (String.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "CMS Role provider");
			}
			base.Initialize(name, config);
		}

		public override void AddUsersToRoles(string[] usernames, string[] rolenames)
		{
            using (var db = CurrentDatabase)
            {
                var qu = db.Users.Where(u => usernames.Contains(u.Username));
                var qr = db.Roles.Where(r => rolenames.Contains(r.RoleName));
                foreach (var user in qu)
                    foreach (var role in qr)
                        user.UserRoles.Add(new UserRole { Role = role });
                db.SubmitChanges();
            }
		}
		public override void CreateRole(string rolename)
		{
            using (var db = CurrentDatabase)
            {
                db.Roles.InsertOnSubmit(new Role { RoleName = rolename });
                db.SubmitChanges();
            }
		}

		public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
		{
            using (var db = CurrentDatabase)
            {
                var role = db.Roles.Single(r => r.RoleName == rolename);
                db.UserRoles.DeleteAllOnSubmit(role.UserRoles);
                db.Roles.DeleteOnSubmit(role);
                db.SubmitChanges();
            }
			return true;
		}

		public override string[] GetAllRoles() => CurrentDatabase.Roles.Select(r => r.RoleName).ToArray();

		public override string[] GetRolesForUser(string username)
		{
			username = username?.Split('\\').LastOrDefault();
            string[] roles;
            using (var db = CurrentDatabase)
            {
                var q = from ur in db.UserRoles
                        where ur.User.Username == username
                        select ur;
                roles = q.Select(r => r.Role.RoleName).ToArray();
            }
            return roles;
		}

		public override string[] GetUsersInRole(string rolename)
		{
            string[] users;
            using (var db = CurrentDatabase)
            {
                var q = from u in db.Users
                        where u.UserRoles.Any(ur => ur.Role.RoleName == rolename)
                        select u.Username;
                users = q.ToArray();
            }
            return users;
		}

		public IEnumerable<User> GetRoleUsers(string rolename)
		{
            return CurrentDatabase.GetRoleUsers(rolename);
		}
		public IEnumerable<Person> GetAdmins()
		{
            return CurrentDatabase.GetAdmins();
		}
		public IEnumerable<Person> GetFinance()
		{
			return GetRoleUsers("Finance").Select(u => u.Person).Distinct();
		}
		public IEnumerable<Person> GetDevelopers()
		{
			return GetRoleUsers("Developer").Select(u => u.Person);
        }

        public override bool IsUserInRole(string username, string rolename)
        {
            username = username?.Split('\\').LastOrDefault();
            return CurrentDatabase.UserRoles.Any(ur => ur.Role.RoleName == rolename && ur.User.Username == username);
        }

		public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
		{
            using (var db = CurrentDatabase)
            {
                var q = from ur in db.UserRoles
                        where rolenames.Contains(ur.Role.RoleName) && usernames.Contains(ur.User.Username)
                        select ur;
                db.UserRoles.DeleteAllOnSubmit(q);
                db.SubmitChanges();
            }
		}

		public override bool RoleExists(string rolename)
		{
			return CurrentDatabase.Roles.Any(r => r.RoleName == rolename);
		}

		public override string[] FindUsersInRole(string rolename, string usernameToMatch)
		{
            string[] users;
            using (var db = CurrentDatabase)
            {
                var q = from u in db.Users
                        where u.UserRoles.Any(ur => ur.Role.RoleName == rolename)
                        select u;
                bool left = usernameToMatch.StartsWith("%");
                bool right = usernameToMatch.EndsWith("%");
                usernameToMatch = usernameToMatch.Trim('%');
                if (left && right)
                {
                    q = q.Where(u => u.Username.Contains(usernameToMatch));
                }
                else if (left)
                {
                    q = q.Where(u => u.Username.EndsWith(usernameToMatch));
                }
                else if (right)
                {
                    q = q.Where(u => u.Username.StartsWith(usernameToMatch));
                }
			    users = q.Select(u => u.Username).ToArray();
            }
            return users;
		}
	}
}
