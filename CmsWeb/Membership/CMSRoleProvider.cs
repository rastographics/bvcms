using System;
using System.Web.Security;
using System.Collections.Specialized;

using System.Linq;
using System.Collections.Generic;
using CmsData;
using CmsWeb.Lifecycle;

namespace CmsWeb.Membership
{
	public class CMSRoleProvider : RoleProvider
	{
		public static CMSRoleProvider provider
		{
			get { return Roles.Provider as CMSRoleProvider; }
		}

		public override string ApplicationName { get { return "cms"; } set { } }

        public IRequestManager RequestManager { get; set; }
        public CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;

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
			var qu = CurrentDatabase.Users.Where(u => usernames.Contains(u.Username));
			var qr = CurrentDatabase.Roles.Where(r => rolenames.Contains(r.RoleName));
			foreach (var user in qu)
				foreach (var role in qr)
					user.UserRoles.Add(new UserRole { Role = role });
			CurrentDatabase.SubmitChanges();
		}
		public override void CreateRole(string rolename)
		{
			CurrentDatabase.Roles.InsertOnSubmit(new Role { RoleName = rolename });
			CurrentDatabase.SubmitChanges();
		}

		public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
		{
			var role = CurrentDatabase.Roles.Single(r => r.RoleName == rolename);
			CurrentDatabase.UserRoles.DeleteAllOnSubmit(role.UserRoles);
			CurrentDatabase.Roles.DeleteOnSubmit(role);
			CurrentDatabase.SubmitChanges();
			return true;
		}

		public override string[] GetAllRoles()
		{
			return CurrentDatabase.Roles.Select(r => r.RoleName).ToArray();
		}

		public override string[] GetRolesForUser(string username)
		{
			username = username?.Split('\\').LastOrDefault();
            var q = from r in CurrentDatabase.UserRoles
					where r.User.Username == username
					select r.Role.RoleName;
			return q.ToArray();
		}

		public override string[] GetUsersInRole(string rolename)
		{
			var q = from u in CurrentDatabase.Users
					where u.UserRoles.Any(ur => ur.Role.RoleName == rolename)
					select u.Username;
			return q.ToArray();
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

        public bool IsUserInRole(string username, string rolename, CMSDataContext db)
        {
            username = username?.Split('\\').LastOrDefault();
			var q = from ur in db.UserRoles
					where rolename == ur.Role.RoleName
					where username == ur.User.Username
					select ur;
			return q.Count() > 0;
        }

        public override bool IsUserInRole(string username, string rolename)
        {
            return IsUserInRole(username, rolename, CurrentDatabase);
        }

		public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
		{
			var q = from ur in CurrentDatabase.UserRoles
					where rolenames.Contains(ur.Role.RoleName) && usernames.Contains(ur.User.Username)
					select ur;
			CurrentDatabase.UserRoles.DeleteAllOnSubmit(q);
			CurrentDatabase.SubmitChanges();
		}

		public override bool RoleExists(string rolename)
		{
			return CurrentDatabase.Roles.Count(r => r.RoleName == rolename) > 0;
		}

		public override string[] FindUsersInRole(string rolename, string usernameToMatch)
		{
			var q = from u in CurrentDatabase.Users
					where u.UserRoles.Any(ur => ur.Role.RoleName == rolename)
					select u;
			bool left = usernameToMatch.StartsWith("%");
			bool right = usernameToMatch.EndsWith("%");
			usernameToMatch = usernameToMatch.Trim('%');
			if (left && right)
				q = q.Where(u => u.Username.Contains(usernameToMatch));
			else if (left)
				q = q.Where(u => u.Username.EndsWith(usernameToMatch));
			else if (right)
				q = q.Where(u => u.Username.StartsWith(usernameToMatch));
			return q.Select(u => u.Username).ToArray();
		}
	}
}
