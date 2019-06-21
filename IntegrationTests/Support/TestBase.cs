using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.IO;
using System.Xml;

namespace IntegrationTests.Support
{
    public class TestBase : IDisposable
    {
        public TestBase() { }

        private CMSDataContext _db;
        public CMSDataContext db
        {
            get => _db ?? (_db = CMSDataContext.Create(Host));
            set => _db = value;
        }

        private string _host;
        public string Host => _host ?? (_host = GetHostFromWebConfig());

        private string GetHostFromWebConfig()
        {
            var config = LoadWebConfig();
            var hostKey = config.SelectSingleNode("configuration/appSettings/add[@key='host']");
            return PickFirst(hostKey?.Attributes["value"].Value, "localhost");
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
            config.Load(Path.GetFullPath(@"..\..\..\CmsWeb\web.config"));
            return config;
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
