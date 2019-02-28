/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace UtilityExtensions
{
    public static partial class Util
    {
        private const string STR_SessionStarting = "SessionStarting";
        private const string STR_Auditing = "Auditing";
        private const string STR_Helpfile = "Helpfile";
        private const string STR_Version = "Version";
        private const string STR_SMTPDEBUG = "SMTPDebug";
        public static int CreateAccountCode = -1952;

        public static string ScratchPad2
        {
            get { return "scratchpad"; }
        }

        public static string SessionId
        {
            get
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        return HttpContextFactory.Current.Session.SessionID;
                var s = (string)HttpRuntime.Cache["SessionId"] ?? (SessionId = Guid.NewGuid().ToString());
                return s;
            }
            set { HttpRuntime.Cache["SessionId"] = value; }
        }

        public static bool SessionStarting
        {
            get
            {
                bool tf = false;
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_SessionStarting] != null)
                            tf = (bool)HttpContextFactory.Current.Session[STR_SessionStarting];
                return tf;
            }
            set { HttpContextFactory.Current.Session[STR_SessionStarting] = value; }
        }

        public static bool Auditing
        {
            get
            {
                object tf = HttpContextFactory.Current.Items[STR_Auditing];
                if (tf.IsNotNull())
                    return (bool)tf;
                else return true;
            }
            set { HttpContextFactory.Current.Items[STR_Auditing] = value; }
        }

        public static string Helpfile
        {
            get
            {
                var tag = "MainPage";
                if (HttpContextFactory.Current.Session[STR_Helpfile] != null)
                    tag = HttpContextFactory.Current.Session[STR_Helpfile].ToString();
                return tag;
            }
            set { HttpContextFactory.Current.Session[STR_Helpfile] = value; }
        }

        public static bool IsLocalNetworkRequest
        {
            get
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Request.IsLocal)
                        return true;
                    string hostPrefix = HttpContextFactory.Current.Request.UserHostAddress;
                    string[] ipClass = hostPrefix.Split(new char[] { '.' });
                    int classA = Convert.ToInt16(ipClass[0]);
                    int classB = Convert.ToInt16(ipClass[1]);
                    if (classA == 10 || classA == 127)
                        return true;
                    else if (classA == 192 && classB == 168)
                        return true;
                    else if (classA == 172 && (classB > 15 && classB < 33))
                        return true;
                    return false;
                }
                return false;
            }
        }

        public static string AppRoot
        {
            get
            {
                var approot = Util.ResolveUrl("~");
                if (approot == "/")
                    approot = "";
                return approot;
            }
        }

        public static string Version
        {
            get
            {
                var version = "?";
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_Version] != null)
                            version = HttpContextFactory.Current.Session[STR_Version].ToString();
                return version;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[STR_Version] = value;
            }
        }

        public static bool AppOffline
        {
            get
            {
                var path = ConfigurationManager.AppSettings["RebootDbInProgress"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (File.Exists(path))
                    return true;
                string output = ConfigurationManager.AppSettings["SharedFolder"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!Directory.Exists(output))
                    return false;
                path = ConfigurationManager.AppSettings["AppOfflineFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                var exists = File.Exists(path);
                if (exists)
                    return true;
                return File.Exists(HttpContextFactory.Current.Server.MapPath("~/AppOffline.txt"));
            }
        }

        public static string SimSunFont
        {
            get
            {
                var path = ConfigurationManager.AppSettings["SimSunFont"];
                if (path != null)
                    path = path.Replace("%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"));
                return path;
            }
        }

        public static string UrgentMessage
        {
            get
            {
                var path = ConfigurationManager.AppSettings["UrgentTextFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!path.HasValue())
                    return HttpContextFactory.Current.Application["UrgentMessage"] as string;
                string fileContent = HttpRuntime.Cache["UrgentMessage"] as string;
                if (fileContent == null && File.Exists(path))
                {
                    fileContent = File.ReadAllText(path);
                    HttpRuntime.Cache.Insert("UrgentMessage", fileContent, new System.Web.Caching.CacheDependency(path));
                }
                return fileContent;
            }
            set
            {
                var path = ConfigurationManager.AppSettings["UrgentTextFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!path.HasValue())
                {
                    if (value.HasValue())
                        HttpContextFactory.Current.Application["UrgentMessage"] = value;
                    else
                        HttpContextFactory.Current.Application.Remove("UrgentMessage");
                    return;
                }
                File.WriteAllText(path, value);
                HttpRuntime.Cache.Insert("UrgentMessage", value, new System.Web.Caching.CacheDependency(path));
            }
        }
        public static string AdminMessage
        {
            get
            {
                var text = ConfigurationManager.AppSettings["NotamText"];
                if (text.HasValue())
                    return text;
                var path = ConfigurationManager.AppSettings["NotamTextFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!path.HasValue())
                    return HttpContextFactory.Current.Application["AdminMessage"] as string;
                string fileContent = HttpRuntime.Cache["AdminMessage"] as string;
                if (fileContent == null && File.Exists(path))
                {
                    fileContent = File.ReadAllText(path);
                    HttpRuntime.Cache.Insert("AdminMessage", fileContent, new System.Web.Caching.CacheDependency(path));
                }
                return fileContent;
            }
            set
            {
                var path = ConfigurationManager.AppSettings["NotamTextFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!path.HasValue())
                {
                    if (value.HasValue())
                        HttpContextFactory.Current.Application["AdminMessage"] = value;
                    else
                        HttpContextFactory.Current.Application.Remove("AdminMessage");
                    return;
                }
                File.WriteAllText(path, value);
                HttpRuntime.Cache.Insert("AdminMessage", value, new System.Web.Caching.CacheDependency(path));
            }
        }

        public static bool SmtpDebug
        {
            get
            {
                bool? deb = false;
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_SMTPDEBUG] != null)
                            deb = (bool)HttpContextFactory.Current.Session[STR_SMTPDEBUG];
                }
                else
                {
                    var localDataStoreSlot = Thread.GetNamedDataSlot(STR_SMTPDEBUG);
                    deb = (bool?)Thread.GetData(localDataStoreSlot);
                }
                return deb ?? false;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                        HttpContextFactory.Current.Session[STR_SMTPDEBUG] = value;
                }
                else
                    Thread.SetData(Thread.GetNamedDataSlot(STR_SMTPDEBUG), value);
            }
        }

        public static int TrialDbOffset
        {
            get
            {
                return Host != "trialdb" ? 0 : (ConfigurationManager.AppSettings["TrialDbOffset"] ?? "0").ToInt();
            }
        }

        public static T QueryString<T>(this System.Web.HttpRequest req, string param)
        {
            if (req.QueryString[param].HasValue())
                return (T)req.QueryString[param].ChangeType(typeof(T));
            return default(T);
        }

        public static string HelpLink(string page)
        {
            var h = ConfigurationManager.AppSettings["helpurl"];
            return string.Format(h, page);
        }

        public static void Cookie(string name, string value, int days)
        {
            if (Cookie(name) == value)
                return;
            var c = new HttpCookie(name, value);
            c.Expires = DateTime.Now.AddDays(days);
            HttpContextFactory.Current.Response.Cookies.Add(c);
            HttpContextFactory.Current.Items["tCookie-" + name] = value;
        }

        public static string Cookie(string name)
        {
            return Cookie(name, null);
        }

        public static string Cookie(string name, string defaultValue)
        {
            var v = (string)HttpContextFactory.Current.Items["tCookie-" + name];
            if (v.HasValue())
                return v;
            var c = HttpContextFactory.Current.Request.Cookies[name];
            if (c != null && c.Value.HasValue())
                return c.Value;
            return defaultValue;
        }

        public static void EndShowMessage(this HttpResponse Response, string message)
        {
            Response.EndShowMessage(message, "javascript: history.go(-1)", "Go Back");
        }

        public static void EndShowMessage(this HttpResponse Response, string message, string href, string text)
        {
            Response.Clear();
            Response.Write(EndShowMessage(message, href, text));
            Response.End();
        }

        public static string EndShowMessage(string message, string href, string text)
        {
            return $"<h3 style='color:red'>{message}</h3>\n<a href='{href}'>{text}</a>";
        }

        public static void NoCache(this HttpResponse response)
        {
            NoCache(new HttpResponseWrapper(response));
        }

        public static void NoCache(this HttpResponseBase response)
        {
            response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetValidUntilExpires(false);
        }

        public static void SetCacheMinutes(this HttpResponseBase Response, int minutes)
        {
            Response.Cache.SetExpires(DateTime.Now.AddMinutes(minutes));
            Response.Cache.SetValidUntilExpires(true);
        }

        public static void ShowError(string message)
        {
            HttpContextFactory.Current.Response.Redirect(
                $"/Home/ShowError/?error={HttpContextFactory.Current.Server.UrlEncode(message)}&url={HttpContextFactory.Current.Request.Url.OriginalString}");
        }

        public static string ResolveUrl(string originalUrl)
        {
            if (originalUrl == null)
                return null;
            if (originalUrl.IndexOf("://") != -1)
                return originalUrl;
            return originalUrl.StartsWith("~")
                ? VirtualPathUtility.ToAbsolute(originalUrl)
                : originalUrl;
        }


        public static string PickFirst(params string[] args)
        {
            foreach (var s in args)
                if (s.HasValue())
                    return s;
            return "";
        }

        public static void Serialize<T>(T m, XmlWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, m);
        }

        public static string Serialize<T>(T m)
        {
            using (var sw = new StringWriter())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var slz = new XmlSerializer(typeof(T));
                slz.Serialize(sw, m, ns);
                return sw.ToString();
            }
        }

        public static T DeSerialize<T>(string s) where T : class
        {
            var sr = new StringReader(s);
            return (new XmlSerializer(typeof(T)).Deserialize(sr) as T);
        }

        public static bool FastFileExists(string path)
        {
            var task = new Task<bool>(() =>
            {
                var fi = new FileInfo(path);
                return fi.Exists;
            });
            task.Start();
            return task.Wait(1000) && task.Result;
        }

        public static bool SessionTimedOut()
        {
            if (HttpContextFactory.Current.Session != null)
                if (HttpContextFactory.Current.Session.IsNewSession)
                {
                    string sessionCookie = HttpContextFactory.Current.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        return true;
                }
            return false;
        }

        public static string GetIpAddress()
        {
            var context = HttpContextFactory.Current;
            if (context == null)
                return null;
            var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipAddress.HasValue())
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                    return addresses[0];
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        public static HttpWebResponse GetHttpResponse(this HttpWebRequest request)
        {
            try
            {
                if(IsDebug())
                    return null;
                return (HttpWebResponse) request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError)
                    return null;
                return (HttpWebResponse) ex.Response;
            }
        }

        public static bool IsDebug()
        {
            var d = false;
#if DEBUG
            d = true;
#endif
            return d;
        }
        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key)
        {
            return dict.GetValueOrDefault(key, default(V));
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defVal)
        {
            return dict.GetValueOrDefault(key, () => defVal);
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, Func<V> defValSelector)
        {
            V value;
            return dict.TryGetValue(key, out value) ? value : defValSelector();
        }

        public static bool IsCultureUS()
        {
            return CultureInfo.CurrentUICulture.Name.Equal("en-US");
        }
    }
}
