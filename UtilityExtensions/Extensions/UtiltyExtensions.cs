/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.IO;
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
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        return HttpContext.Current.Session.SessionID;
                return (string)HttpRuntime.Cache["SessionId"];
            }
            set { HttpRuntime.Cache["SessionId"] = value; }
        }

        public static bool SessionStarting
        {
            get
            {
                bool tf = false;
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_SessionStarting] != null)
                            tf = (bool)HttpContext.Current.Session[STR_SessionStarting];
                return tf;
            }
            set { HttpContext.Current.Session[STR_SessionStarting] = value; }
        }

        public static bool Auditing
        {
            get
            {
                object tf = HttpContext.Current.Items[STR_Auditing];
                if (tf.IsNotNull())
                    return (bool)tf;
                else return true;
            }
            set { HttpContext.Current.Items[STR_Auditing] = value; }
        }

        public static string Helpfile
        {
            get
            {
                var tag = "MainPage";
                if (HttpContext.Current.Session[STR_Helpfile] != null)
                    tag = HttpContext.Current.Session[STR_Helpfile].ToString();
                return tag;
            }
            set { HttpContext.Current.Session[STR_Helpfile] = value; }
        }

        public static bool IsLocalNetworkRequest
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.IsLocal)
                        return true;
                    string hostPrefix = HttpContext.Current.Request.UserHostAddress;
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
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_Version] != null)
                            version = HttpContext.Current.Session[STR_Version].ToString();
                return version;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_Version] = value;
            }
        }

        public static bool AppOffline
        {
            get
            {
                string output = ConfigurationManager.AppSettings["SharedFolder"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!Directory.Exists(output)) 
                    return false;
                var path = ConfigurationManager.AppSettings["AppOfflineFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                return File.Exists(path);
            }
        }

        public static string UrgentMessage
        {
            get
            {
                var path = ConfigurationManager.AppSettings["UrgentTextFile"].Replace("%USERPROFILE%",
                    Environment.GetEnvironmentVariable("USERPROFILE"));
                if (!path.HasValue())
                    return HttpContext.Current.Application["UrgentMessage"] as string;
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
                        HttpContext.Current.Application["UrgentMessage"] = value;
                    else
                        HttpContext.Current.Application.Remove("UrgentMessage");
                    return;
                }
                File.WriteAllText(path, value);
                HttpRuntime.Cache.Insert("UrgentMessage", value, new System.Web.Caching.CacheDependency(path));
            }
        }

        public static bool SmtpDebug
        {
            get
            {
                bool? deb = false;
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_SMTPDEBUG] != null)
                            deb = (bool)HttpContext.Current.Session[STR_SMTPDEBUG];
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
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session[STR_SMTPDEBUG] = value;
                }
                else
                    Thread.SetData(Thread.GetNamedDataSlot(STR_SMTPDEBUG), value);
            }
        }

        public static T QueryString<T>(this System.Web.UI.Page page, string param)
        {
            return QueryString<T>(HttpContext.Current.Request, param);
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
            return h.Fmt(page);
        }

        public static string IpAddress()
        {
            string strIpAddress;
            strIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (strIpAddress == null)
                strIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return strIpAddress;
        }

        public static void Cookie(string name, string value, int days)
        {
            if (Cookie(name) == value)
                return;
            var c = new HttpCookie(name, value);
            c.Expires = DateTime.Now.AddDays(days);
            HttpContext.Current.Response.Cookies.Add(c);
            HttpContext.Current.Items["tCookie-" + name] = value;
        }

        public static string Cookie(string name)
        {
            return Cookie(name, null);
        }

        public static string Cookie(string name, string defaultValue)
        {
            var v = (string)HttpContext.Current.Items["tCookie-" + name];
            if (v.HasValue())
                return v;
            var c = HttpContext.Current.Request.Cookies[name];
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
            return "<h3 style='color:red'>{0}</h3>\n<a href='{1}'>{2}</a>".Fmt(message, href, text);
        }

        public static void NoCache(this HttpResponse Response)
        {
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetValidUntilExpires(false);
        }

        public static void NoCache(this HttpResponseBase Response)
        {
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetValidUntilExpires(false);
        }

        public static void SetCacheMinutes(this HttpResponseBase Response, int minutes)
        {
            Response.Cache.SetExpires(DateTime.Now.AddMinutes(minutes));
            Response.Cache.SetValidUntilExpires(true);
        }

        public static void ShowError(string message)
        {
            HttpContext.Current.Response.Redirect(
                "/Home/ShowError/?error={0}&url={1}".Fmt(HttpContext.Current.Server.UrlEncode(message),
                    HttpContext.Current.Request.Url.OriginalString));
        }

        public static string ResolveUrl(string originalUrl)
        {
            if (originalUrl == null)
                return null;
            if (originalUrl.IndexOf("://") != -1)
                return originalUrl;
            if (originalUrl.StartsWith("~"))
                return VirtualPathUtility.ToAbsolute(originalUrl);
            return originalUrl;
        }

        public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;
            var newUrl = ResolveUrl(serverUrl);
            var originalUri = HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : Scheme()) +
                     "://" + originalUri.Authority + newUrl;
            return newUrl;
        }

        public static string ResolveServerUrl(string serverUrl)
        {
            return ResolveServerUrl(serverUrl, false);
        }

        public static string ServerLink(string path = "")
        {
            if (HttpContext.Current != null)
            {
                var Request = HttpContext.Current.Request;
                return URLCombine(Scheme() + "://" + Request.Url.Authority, path);
            }
            var h = ConfigurationManager.AppSettings["cmshost"];
            if(h.HasValue())
                return h.Replace("{church}", Host, ignoreCase: true);
            return "";
        }

        public static string Scheme()
        {
            var Request = HttpContext.Current.Request;
            var scheme = Request.Url.Scheme;
            if (Request.Headers["X-Forwarded-Proto"] == "https")
                scheme = "https";
            return scheme;
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
            var sw = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var slz = new XmlSerializer(typeof(T));
            slz.Serialize(sw, m, ns);
            return sw.ToString();
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
            if (HttpContext.Current.Session != null)
                if (HttpContext.Current.Session.IsNewSession)
                {
                    string sessionCookie = HttpContext.Current.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        return true;
                }
            return false;
        }

        public static string GetIPAddress()
        {
            var context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipAddress.HasValue())
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                    return addresses[0];
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static bool IsDebug()
        {
            var d = false;
#if DEBUG
            d = true;
#endif
            return d;
        }
    }
}