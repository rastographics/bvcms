using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Membership;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace SharedTestFixtures
{
    public class DatabaseTestBase : IDisposable
    {
        private CMSDataContext _db;
        public CMSDataContext db
        {
            get => _db ?? (_db = CMSDataContext.Create(Host));
            set => _db = value;
        }

        private string _host;
        public string Host => _host ?? (_host = DatabaseFixture.Host);

        protected User CreateUser(string username, string password, Family family = null, params string[] roles)
        {
            if (family == null)
            {
                family = new Family();
                db.Families.InsertOnSubmit(family);
                db.SubmitChanges();
            }
            var person = new Person
            {
                Family = family,
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.PrimaryAdult,
            };
            db.People.InsertOnSubmit(person);
            db.SubmitChanges();

            var createDate = DateTime.Now;
            var machineKey = GetValidationKeyFromWebConfig();
            var passwordhash = CMSMembershipProvider.EncodePassword(password, System.Web.Security.MembershipPasswordFormat.Hashed, machineKey);
            var user = new User
            {
                PeopleId = person.PeopleId,
                Username = username,
                Password = passwordhash,
                MustChangePassword = false,
                IsApproved = true,
                CreationDate = createDate,
                LastPasswordChangedDate = createDate,
                LastActivityDate = createDate,
                IsLockedOut = false,
                LastLockedOutDate = createDate,
                FailedPasswordAttemptWindowStart = createDate,
                FailedPasswordAnswerAttemptWindowStart = createDate,
            };
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            if (roles.Any())
            {
                user.AddRoles(db, roles);
                db.SubmitChanges();
            }
            return user;
        }

        protected Organization CreateOrganization(string name = null, int? fromId = null, int? type = null, int? campus = null)
        {
            Organization org = null;
            var newOrg = new Organization();
            if (fromId != null)
            {
                org = db.LoadOrganizationById(fromId);
            }
            if (org == null)
            {
                org = db.Organizations.First();
            }

            newOrg.CreatedDate = DateTime.Now;
            newOrg.CreatedBy = 0;
            newOrg.OrganizationName = name ?? RandomString();
            newOrg.EntryPointId = org.EntryPointId;
            newOrg.OrganizationTypeId = type ?? org.OrganizationTypeId;
            newOrg.OrganizationStatusId = 30;
            newOrg.DivisionId = org.DivisionId;
            newOrg.CampusId = campus;

            db.Organizations.InsertOnSubmit(newOrg);
            db.SubmitChanges();

            return newOrg;
        }

        private string GetValidationKeyFromWebConfig()
        {
            var config = LoadWebConfig();
            var machineKey = config.SelectSingleNode("configuration/system.web/machineKey");
            return machineKey.Attributes["validationKey"].Value;
        }
        
        protected string PickFirst(params string[] values)
        {
            foreach(var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            return null;
        }

        protected static XmlDocument LoadWebConfig()
        {
            var config = new XmlDocument();
            var filename = FindWebConfig();
            config.Load(filename);
            return config;
        }

        private static string FindWebConfig()
        {
            string webConfig = "web.config";
            var paths = new[] { @"..\..\..\CmsWeb\web.config", @"..\..\..\..\CmsWeb\web.config" };
            foreach (var path in paths)
            {
                webConfig = Path.GetFullPath(path);
                if (File.Exists(webConfig)) break;
            }
            return webConfig;
        }

        static Random randomizer = new Random();
        protected static string RandomString(int length = 8, string prefix = "")
        {
            string rndchars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
            string s = prefix;
            while (s.Length < length)
            {
                s += rndchars.Substring(randomizer.Next(0, rndchars.Length), 1);
            }
            return s;
        }

        protected static string RandomPhoneNumber()
        {
            return RandomNumber(2145550000, int.MaxValue).ToString();
        }

        protected static int RandomNumber(int min = 0, int max = 65535)
        {
            return randomizer.Next(min, max);
        }

        public virtual void Dispose()
        {
            _db = null;
        }
    }
}
