using CmsData;
using CmsWeb.Lifecycle;
using SharedTestFixtures;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace IntegrationTests.Support
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            DatabaseFixture.EnsureDatabaseExists();
        }

        private CMSDataContext _db;
        public CMSDataContext db
        {
            get => _db ?? (_db = CMSDataContext.Create(DatabaseFixture.Host));
            set => _db = value;
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
