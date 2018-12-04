using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using UtilityExtensions;

namespace CmsData
{
    public partial class User
    {
        public bool InRole(string role)
        {
            return Roles.Any(ro => ro == role);
        }

        public bool IsOnLine
        {
            get
            {
                var onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
                var compareTime = Util.Now.Subtract(onlineSpan);
                return LastActivityDate > compareTime && LastActivityDate != CreationDate;
            }
        }

        public string BestName
        {
            get { return PeopleId.HasValue ? Name2 : Username; }
        }

        public string[] Roles
        {
            get { return UserRoles.Select(ur => ur.Role.RoleName).ToArray(); }
        }

        public static List<Role> AllRoles(CMSDataContext Db)
        {
            var roles = Db.Roles.ToList();
            return roles.OrderBy(rr => rr.RoleName == "NEW" ? 1 : 0).ThenBy(rr => rr.RoleName).ToList();
        }

        public void SetRoles(CMSDataContext db, string[] value, bool log = true)
        {
            if (value == null)
            {
                db.UserRoles.DeleteAllOnSubmit(UserRoles);
                db.SubmitChanges();
                return;
            }
            var deletes = (from r in db.UserRoles
                           where r.UserId == UserId
                           where !value.Contains(r.Role.RoleName)
                           select new { r, r.Role.RoleName }).ToList();

            db.UserRoles.DeleteAllOnSubmit(deletes.Select(rr => rr.r));

            var addlist = (from s in value
                           join r in UserRoles on s equals r.Role.RoleName into g
                           from t in g.DefaultIfEmpty()
                           where t == null
                           select s).ToList();

            foreach (var s in addlist)
            {
                var role = db.Roles.SingleOrDefault(r => r.RoleName == s);
                var roleid = role?.RoleId;
                if (role == null)
                {
                    roleid = CreateRole(db, s);
                }

                UserRoles.Add(new UserRole { RoleId = roleid.Value, UserId = UserId });
            }
            db.SubmitChanges();
            if (!log)
            {
                return;
            }

            if (deletes.Count > 0)
            {
                db.LogActivity($"Remove Roles {string.Join(",", deletes.Select(rr => rr.RoleName))} from user {Username}", pid: PeopleId, uid: Util.UserPeopleId);
            }

            if (addlist.Count > 0)
            {
                db.LogActivity($"Add Roles {string.Join(",", addlist)} to user {Username}", pid: PeopleId, uid: Util.UserPeopleId);
            }
        }

        public void AddRoles(CMSDataContext db, params string[] value)
        {
            var q = from s in value
                    join r in UserRoles on s equals r.Role.RoleName into g
                    from t in g.DefaultIfEmpty()
                    where t == null
                    select s;

            foreach (var s in q)
            {
                var role = db.Roles.SingleOrDefault(r => r.RoleName == s);
                if (role == null)
                {
                    throw new Exception($"Role {s} does not exist");
                }

                UserRoles.Add(new UserRole { Role = role });
            }
        }

        public void AddRole(CMSDataContext db, string value)
        {
            var a = new[] { value };
            AddRoles(db, a);
        }

        public void RemoveRoles(CMSDataContext Db, params string[] values)
        {
            foreach (var s in values)
            {
                var role = Db.Roles.SingleOrDefault(r => r.RoleName == s);
                if (role == null)
                {
                    continue;
                }

                Db.UserRoles.DeleteOnSubmit(UserRoles.Single(x => x.Role.RoleName == role.RoleName));
            }
            Db.SubmitChanges();
        }

        public void ChangePassword(string newpassword)
        {
            CMSMembershipProvider.provider.AdminOverride = true;
            var mu = CMSMembershipProvider.provider.GetUser(Username, false);
            if (mu == null)
            {
                return;
            }

            mu.UnlockUser();
            mu.ChangePassword(mu.ResetPassword(), newpassword);
            TempPassword = newpassword;
            CMSMembershipProvider.provider.AdminOverride = false;
        }

        public string PasswordSetOnly { get; set; }

        public bool CanAssign(CMSDataContext db, string role)
        {
            if (role == "Finance" || role == "FinanceAdmin")
            {
                return db.CurrentUser.InRole("Finance") && db.CurrentUser.InRole("Admin");
            }

            if (role == "Developer")
            {
                return db.CurrentUser.InRole("Developer");
            }

            if (role == "Delete")
            {
                return db.CurrentUser.InRole("Developer");
            }

            return db.CurrentUser.InRole("Admin");
        }

        public static string[] BasicLevel1 =
        {
            "Access", "Attendance", "Edit", "ManageGroups", "ViewVolunteerApplication"
        };

        public static string[] BasicLevel2 =
        {
            "Coupon", "SendSMS", "ManageEmails", "ScheduleEmails"
        };

        public static string[] SpecialPurpose =
        {
            "ApplicationReview",
            "BackgroundCheck",
            "Checkin",
            "ContentEdit",
            "CreditCheck",
            "Coupon2",
            "Design",
            "Membership",
            "MemberDocs",
            "MissionGiving",
            "OrgLeadersOnly",
            "CheckinCoordinator"
        };

        public static string[] Financial =
        {
            "Finance", "FinanceAdmin", "ManageTransactions", "FinanceViewOnly", "FinanceDataEntry", "FundManager"
        };

        public static string[] Advanced =
        {
            "Admin",
            "Delete",
            "Developer",
            "Manager",
            "Manager2",
            "OrgTagger",
            "ManagePrivateContacts",
            "ManageTasks",
            "ManageOrgMembers"
        };

        public static List<string> Hardwired()
        {
            var list = new List<string>();
            list.AddRange(BasicLevel1);
            list.AddRange(BasicLevel2);
            list.AddRange(SpecialPurpose);
            list.AddRange(Financial);
            list.AddRange(Advanced);
            return list;
        }

        public static string[] CustomRoles(CMSDataContext db)
        {
            var allroles = AllRoles(db);
            var hardwired = Hardwired();
            var q = from r in allroles
                    where !hardwired.Contains(r.RoleName)
                    select r.RoleName;
            return q.ToArray();
        }

        public static int CreateRole(CMSDataContext db, string name)
        {
            bool? hardwired = null;
            if (Hardwired().Contains(name))
            {
                hardwired = true;
            }

            var role = new Role { Hardwired = hardwired, RoleName = name };
            db.Roles.InsertOnSubmit(role);
            db.SubmitChanges();
            return role.RoleId;
        }
    }
}
