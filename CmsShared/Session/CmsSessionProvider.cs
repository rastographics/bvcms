using CmsData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using UtilityExtensions.Session;

namespace CMSShared.Session
{
    public class CmsSessionProvider : ISessionProvider
    {
        private CMSDataContext db { get; set; }
        private Dictionary<string, string> LocalCache { get; set; }
        public string CurrentSessionId { get; set; } = GetSessionId();

        private static string GetSessionId()
        {
            var context = HttpContextFactory.Current;
            if (context != null)
            {
                return context.Session?.SessionID ?? context.Request?.Cookies?["_sess"]?.Value;
            }
            return null;
        }

        public IEnumerable<string> Keys => LocalCache.Keys;

        public CmsSessionProvider()
        {
            db = CMSDataContext.Create(HttpContextFactory.Current);
            InitCache();
        }

        public CmsSessionProvider(CMSDataContext context)
        {
            db = context;
            InitCache();
        }

        private void InitCache()
        {
            LocalCache = db.SessionValues.Where(v => v.SessionId == CurrentSessionId).ToDictionary(v => v.Name, v => v.Value);
        }

        public void Add<T>(string name, T value) where T : class
        {
            FetchOrCreateSessionValue(name, CreateString(value));
        }

        public void Clear()
        {
            db.SessionValues.DeleteAllOnSubmit(db.SessionValues.Where(v => v.SessionId == CurrentSessionId));
            db.SubmitChanges();
            LocalCache.Clear();
        }

        public void Delete(string name)
        {
            FetchOrCreateSessionValue(name, null);
        }

        public T Get<T>(string name) where T : class => Get<T>(name, null);
        public T Get<T>(string name, T defaultValue) where T : class
        {
            var value = defaultValue;
            if (CurrentSessionId != null)
            {
                var sv = FetchSessionValue(name, out _);
                if (sv != null)
                {
                    value = JsonConvert.DeserializeObject<T>(sv.Value);
                }
            }
            return value ?? defaultValue;
        }

        private string CreateString<T>(T value) where T : class
        {
            string strValue = null;
            if (value != null)
            {
                strValue = JsonConvert.SerializeObject(value);
            }
            return strValue;
        }

        private SessionValue FetchSessionValue(string key, out bool cached)
        {
            cached = false;
            if (LocalCache.ContainsKey(key))
            {
                cached = true;
                return new SessionValue { Name = key, Value = LocalCache[key] };
            }
            var sv = db.SessionValues.FirstOrDefault(v => v.SessionId == CurrentSessionId && v.Name == key);
            if (sv != null)
            {
                LocalCache[key] = sv.Value;
            }
            return sv;
        }

        private SessionValue FetchOrCreateSessionValue(string key, string value)
        {
            var sv = FetchSessionValue(key, out bool cached);
            if (sv == null && value != null && CurrentSessionId != null)
            {
                sv = new SessionValue
                {
                    Name = key,
                    SessionId = CurrentSessionId,
                    CreatedDate = DateTime.UtcNow
                };
                db.SessionValues.InsertOnSubmit(sv);
            }
            if (sv != null)
            {
                if (value == null)
                {
                    db.SessionValues.DeleteAllOnSubmit(db.SessionValues.Where(v => v.Name == key && v.SessionId == CurrentSessionId));
                    LocalCache.Remove(key);
                }
                else
                {
                    LocalCache[key] = value;
                    if (cached && value != sv.Value)
                    {
                        sv = db.SessionValues.FirstOrDefault(v => v.Name == key && v.SessionId == CurrentSessionId);
                    }
                    sv.Value = value;
                }
                db.SubmitChanges();
            }
            return sv;
        }
    }
}
