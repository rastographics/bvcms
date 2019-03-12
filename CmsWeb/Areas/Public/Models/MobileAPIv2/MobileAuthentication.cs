using CmsData;
using System;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
    public class MobileAuthentication
    {
        private readonly CMSDataContext db;

        private User user;
        private MobileAppDevice device;

        private Error error = Error.UNKNOWN;
        private Type type = Type.NONE;

        private string instance = "";

        private string username = "";
        private string password = "";

        public MobileAuthentication(CMSDataContext db)
        {
            this.db = db;
        }

        public void authenticate(string instanceID, string previousID = "", bool allowQuick = false, int userID = 0)
        {
            if (string.IsNullOrEmpty(HttpContextFactory.Current.Request.Headers["Authorization"]))
            {
                error = Error.NO_HEADER;

                return;
            }

            string authHeader = HttpContextFactory.Current.Request.Headers["Authorization"];
            string[] headerParts = authHeader.SplitStr(" ");

            if (headerParts.Length != 2)
            {
                error = Error.INVALID_HEADER;

                return;
            }

            switch (headerParts[0].ToLower())
            {
                case "basic":
                    {
                        type = Type.BASIC;
                        error = validateBasic(headerParts[1]);

                        break;
                    }

                case "pin":
                    {
                        type = Type.PIN;
                        error = validatePIN(headerParts[1], instanceID, previousID);

                        break;
                    }

                case "quick":
                    {
                        if (allowQuick)
                        {
                            type = Type.QUICK;
                            error = validateQuickLogin(headerParts[1], instanceID, userID);
                        }
                        else
                        {
                            error = Error.INVALID_HEADER_TYPE;
                        }

                        break;
                    }

                default:
                    {
                        error = Error.INVALID_HEADER_TYPE;

                        break;
                    }
            }
        }

        private Error validateBasic(string value)
        {
            string credentials;
            bool userFound;

            try
            {
                credentials = Encoding.ASCII.GetString(Convert.FromBase64String(value));
            }
            catch (Exception)
            {
                return Error.MALFORMED_BASE64;
            }

            string[] userAndPassword = credentials.SplitStr(":");

            if (userAndPassword.Length != 2)
            {
                return Error.INVALID_HEADER;
            }

            if (string.IsNullOrEmpty(userAndPassword[0]) || string.IsNullOrEmpty(userAndPassword[1]))
            {
                return Error.MISSING_CREDENTIALS;
            }

            NetworkCredential networkCredential = new NetworkCredential(userAndPassword[0], userAndPassword[1]);
            username = networkCredential.UserName;
            password = networkCredential.Password;

            bool impersonating = password == db.Setting("ImpersonatePassword", Guid.NewGuid().ToString());

            IQueryable<User> userQuery = db.Users.Where(uu => uu.Username == username || uu.Person.EmailAddress == username || uu.Person.EmailAddress2 == username);

            try
            {
                userFound = userQuery.Any();
            }
            catch (Exception)
            {
                return Error.DATABASE_ERROR;
            }

            foreach (User foundUser in userQuery.ToList())
            {
                if (!impersonating && !Membership.Provider.ValidateUser(username, password))
                {
                    continue;
                }

                db.Refresh(RefreshMode.OverwriteCurrentValues, foundUser);
                user = foundUser;

                // TODO: Remove the connection to Util and Util2 in the future
                Util.UserPeopleId = user.PeopleId;

                if (CMSRoleProvider.provider.IsUserInRole(user.Username, "OrgLeadersOnly"))
                {
                    Util2.OrgLeadersOnly = true;
                    db.SetOrgLeadersOnly();
                }

                break;
            }

            return checkUser(userFound, impersonating);
        }

        private Error validatePIN(string value, string instanceID, string previousID)
        {
            string credentials;

            try
            {
                credentials = Encoding.ASCII.GetString(Convert.FromBase64String(value));
            }
            catch (Exception)
            {
                return Error.MALFORMED_BASE64;
            }

            string[] userAndPassword = credentials.SplitStr(":");

            if (userAndPassword.Length != 2)
            {
                return Error.INVALID_HEADER;
            }

            if (string.IsNullOrEmpty(userAndPassword[0]) || string.IsNullOrEmpty(userAndPassword[1]))
            {
                return Error.MISSING_CREDENTIALS;
            }

            string hashString = createHash(instanceID, userAndPassword[0], userAndPassword[1]);

            device = db.MobileAppDevices.FirstOrDefault(d => d.InstanceID == instanceID && d.Authentication == hashString);

            if (device == null)
            {
                if (previousID.Length > 0)
                {
                    hashString = createHash(previousID, userAndPassword[0], userAndPassword[1]);

                    device = db.MobileAppDevices.FirstOrDefault(d => d.InstanceID == previousID && d.Authentication == hashString);

                    if (device == null)
                    {
                        return Error.INVALID_PASSWORD;
                    }

                    device.InstanceID = instanceID;
                    device.Authentication = createHash(instanceID, userAndPassword[0], userAndPassword[1]);

                    db.SubmitChanges();
                }
                else
                {
                    return Error.INVALID_PASSWORD;
                }
            }

            device.LastSeen = DateTime.Now;
            db.SubmitChanges();

            user = device.User;
            instance = device.InstanceID;

            // TODO: Remove the connection to Util and Util2 in the future
            Util.UserPeopleId = user.PeopleId;

            if (CMSRoleProvider.provider.IsUserInRole(user.Username, "OrgLeadersOnly"))
            {
                Util2.OrgLeadersOnly = true;
                db.SetOrgLeadersOnly();
            }

            return checkUser(true, false);
        }

        private Error validateQuickLogin(string code, string instanceID, int userID)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(instanceID))
            {
                return Error.MISSING_CREDENTIALS;
            }

            device = db.MobileAppDevices.FirstOrDefault(d => d.InstanceID == instanceID && d.Code == code);

            if (device == null)
            {
                return Error.INVALID_PASSWORD;
            }

            device.LastSeen = DateTime.Now;
            db.SubmitChanges();

            if (userID > 0)
            {
                user = db.Users.FirstOrDefault(u => u.UserId == userID);

                if (user == null)
                {
                    return Error.USER_NOT_FOUND;
                }

                if (user.Person.EmailAddress != device.CodeEmail && user.Person.EmailAddress2 != device.CodeEmail)
                {
                    user = null;

                    return Error.USER_MISMATCH;
                }

                username = user.Username;

                // TODO: Remove the connection to Util and Util2 in the future
                Util.UserPeopleId = user.PeopleId;

                if (CMSRoleProvider.provider.IsUserInRole(user.Username, "OrgLeadersOnly"))
                {
                    Util2.OrgLeadersOnly = true;
                    db.SetOrgLeadersOnly();
                }
            }

            return Error.AUTHENTICATED;
        }

        public bool setPIN(int deviceTypeID, string instanceID, string key, string pin)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            string hashString = createHash(instanceID, username, pin);

            MobileAppDevice appDevice = db.MobileAppDevices.FirstOrDefault(d => d.InstanceID == instanceID);

            if (appDevice != null)
            {
                appDevice.UserID = user.UserId;
                appDevice.PeopleID = user.PeopleId;
                appDevice.Authentication = hashString;
            }
            else
            {
                appDevice = new MobileAppDevice
                {
                    Created = DateTime.Now,
                    LastSeen = DateTime.Now,
                    DeviceTypeID = deviceTypeID,
                    InstanceID = instanceID,
                    NotificationID = key,
                    UserID = user.UserId,
                    PeopleID = user.PeopleId,
                    Authentication = hashString,
                    Code = "",
                    CodeExpires = SqlDateTime.MinValue.Value,
                    CodeEmail = ""
                };

                db.MobileAppDevices.InsertOnSubmit(appDevice);
            }

            db.SubmitChanges();

            return true;
        }

        public void setDeviceUser()
        {
            if (device == null || user == null)
            {
                return;
            }

            device.UserID = user.UserId;
            device.PeopleID = user.Person.PeopleId;
            device.Code = "";
            device.CodeExpires = new DateTime(1970, 01, 01);
            device.CodeEmail = "";

            db.SubmitChanges();
        }

        private static string createHash(string instanceID, string username, string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"{instanceID}:{username}:{password}");

            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] hash = sha256Managed.ComputeHash(bytes);

            StringBuilder hashString = new StringBuilder(64);

            foreach (byte x in hash)
            {
                hashString.Append(x.ToString("x2"));
            }

            return hashString.ToString();
        }

        private Error checkUser(bool userFound, bool impersonating)
        {
            if (user == null && userFound)
            {
                DbUtil.LogActivity($"Mobile: Failed password by {username}");

                return Error.INVALID_PASSWORD;
            }

            if (user == null)
            {
                DbUtil.LogActivity($"Mobile: Attempt to login by unknown user {username}");

                return Error.USER_NOT_FOUND;
            }

            if (user.IsLockedOut)
            {
                DbUtil.LogActivity($"Mobile: Attempt to login by locked out user {username}");

                return Error.USER_LOCKED_OUT;
            }

            if (!user.IsApproved)
            {
                DbUtil.LogActivity($"Mobile: Attempt to login by unapproved user {username}");

                return Error.USER_NOT_APPROVED;
            }

            if (impersonating && user.Roles.Contains("Finance"))
            {
                DbUtil.LogActivity($"Mobile: Attempt to impersonate finance by {username}");

                return Error.CANNOT_IMPERSONATE_FINANCE;
            }

            if (user.Roles.Contains("APIOnly"))
            {
                return Error.CANNOT_USE_API_ONLY;
            }

            return Error.AUTHENTICATED;
        }

        public bool hasError()
        {
            return error != Error.AUTHENTICATED;
        }

        public MobileAppDevice getDevice()
        {
            return device;
        }

        public int getError()
        {
            return (int)error;
        }

        public string getErrorMessage()
        {
            return ERROR_MESSAGES[Math.Abs((int)error)];
        }

        public Type getType()
        {
            return type;
        }

        public User getUser()
        {
            return user;
        }

        public string getInstanceID()
        {
            return instance;
        }

        public enum Type
        {
            NONE = 0,
            BASIC = 1,
            PIN = 2,
            QUICK = 3
        }

        private enum Error
        {
            AUTHENTICATED = 0,
            UNKNOWN = -1,
            NO_HEADER = -2,
            INVALID_HEADER = -3,
            INVALID_HEADER_TYPE = -4,
            MALFORMED_BASE64 = -5,
            MISSING_CREDENTIALS = -6,
            DATABASE_ERROR = -7,
            USER_NOT_FOUND = -8,
            USER_LOCKED_OUT = -9,
            USER_NOT_APPROVED = -10,
            USER_MISMATCH = -11,
            INVALID_PASSWORD = -12,
            CANNOT_IMPERSONATE_FINANCE = -13,
            CANNOT_USE_API_ONLY = -14
        }

        private static readonly string[] ERROR_MESSAGES = {
            "Authenticated", // 0
			"Unknown Error", // -1
			"No authentication header", // -2
			"Invalid authentication header", // -3
			"Invalid authentication header type", // -4
			"Malformed Base64 in header", // -5
			"Missing credentials", // -6
			"Database error", // -7
			"User not found", // -8
			"User locked out", // -9
			"User not approved", // -10
			"User did not match", // -11
			"Invalid password", // -12
			"Cannot impersonate finance user", // -13
			"Cannot access with API only user" // -14
		};
    }
}
