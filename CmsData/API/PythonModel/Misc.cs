using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownDeep;
using RestSharp;
using RestSharp.Extensions;
using UtilityExtensions;
using System.Collections;
using CmsData.API;
using IronPython.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Method = RestSharp.Method;

namespace CmsData
{
    public partial class PythonModel
    {
        public string CmsHost => db.ServerLink().TrimEnd('/');
        public bool FromMorningBatch { get; set; }
        public string UserName => Util.UserName;
        public dynamic Data { get; }

        public string CallScript(string scriptname)
        {
            var script = db.ContentOfTypePythonScript(scriptname);
            return ExecutePython(script, new PythonModel(db.Host, dictionary));
        }

        public string Content(string name)
        {
            var c = db.Content(name);
#if DEBUG
            if (c == null)
            {
                var s = System.IO.File.ReadAllText(name);
                return s;
            }
#endif
            return c.Body;
        }

        public bool DataHas(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public string Dictionary(string s)
        {
            if (dictionary != null && dictionary.ContainsKey(s))
                return dictionary[s].ToString();
            return "";
        }

        public void DictionaryAdd(string key, string value)
        {
            dictionary.Add(key, value);
        }

        public string FmtPhone(string s, string prefix = null)
        {
            return s.FmtFone(prefix);
        }

        public string FmtZip(string s)
        {
            return s.FmtZip();
        }

        public string HtmlContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Body;
        }
        public string TitleContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Title;
        }
        public string Draft(string name)
        {
            var c = db.ContentOfTypeSavedDraft(name);
            return c.Body;
        }
        public string DraftTitle(string name)
        {
            var c = db.ContentOfTypeSavedDraft(name);
            return c.Title;
        }

        public string Replace(string text, string pattern, string replacement)
        {
            return Regex.Replace(text, pattern, replacement);
        }
        public static string Markdown(string text)
        {
            if (text == null)
                return "";
            var md = new Markdown();
            return md.Transform(text.Trim());
        }

        public string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }
        public string RestGet(string url, PythonDictionary headers, string user = null, string password = null)
        {
            var ttt = System.IO.File.ReadAllText(@"E:\GitHub\bvcms\ttt.json");
            return ttt;
/*
            var client = new RestClient(url);
            if(user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);

            var request = new RestRequest(Method.GET);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            var response = client.Execute(request);
            return response.Content;
*/
        }
        public string RestPost(string url, PythonDictionary headers, object body, string user = null, string password = null)
        {
            var client = new RestClient(url);
            if(user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);

            var request = new RestRequest(Method.POST);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            var response = client.Execute(request);
            return response.Content;
        }

        public dynamic JsonDeserialize(string s)
        {
            dynamic d = JObject.Parse(s);
            return d;
        }

        public Person FindAddPerson(string first, string last, string dob, string email, string phone)
        {
            return Person.FindAddPerson(db, "python", first, last, dob, email, phone);
        }
        public Person FindAddPerson(dynamic first, dynamic last, dynamic dob, dynamic email, dynamic phone)
        {
            return FindAddPerson((string)first, (string)last, (string)dob, (string)email, (string)phone);
        }
        public int FindAddPeopleId(dynamic first, dynamic last, dynamic dob, dynamic email, dynamic phone)
        {
            return FindAddPerson((string)first, (string)last, (string)dob, (string)email, (string)phone).PeopleId;
        }
    }
}
/*
 * 

 */
