using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownDeep;
using RestSharp;
using UtilityExtensions;
using System.Linq;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using CmsData.API;
using IronPython.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

        public DynamicData DynamicData()
        {
            return new DynamicData();
        }
        public DynamicData DynamicData(PythonDictionary dict)
        {
            return new DynamicData(dict);
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

        public string RegexMatch(string s, string regex)
        {
            return Regex.Match(s, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;
        }
        public string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }
        public string RestGet(string url, PythonDictionary headers, string user = null, string password = null)
        {
#if DEBUG2
            var ttt = System.IO.File.ReadAllText(@"C:\dev\bvcms\ttt.json");
            return ttt;
#else
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);

            var request = new RestRequest(Method.GET);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            var response = client.Execute(request);
            return response.Content;
#endif
        }
        public string RestPost(string url, PythonDictionary headers, object body, string user = null, string password = null)
        {
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);

            var request = new RestRequest(Method.POST);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            request.AddBody(body);
            var response = client.Execute(request);
            return response.Content;
        }
        public string RestPostJson(string url, PythonDictionary headers, object obj, string user = null, string password = null)
        {
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new RestSharp.Serializers.Shared.JsonSerializer();
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            request.AddJsonBody(obj);
            var response = client.Execute(request);
            return response.Content;
        }
        public static dynamic JsonDeserialize(string s)
        {
            dynamic d = JObject.Parse(s);
            return d;
        }
        public static IEnumerable<dynamic> JsonDeserialize2(string s)
        {
            var  list =  JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(s);
            var list2 = list.Select(vv => new DynamicData(vv));
            return list2;
        }
        public string JsonSerialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public string Setting(string name, string def = "")
        {
            return db.Setting(name, def);
        }
        public void SetSetting(string name, object value)
        {
            db.SetSetting(name, value.ToString());
            db.SubmitChanges();
        }

        public string FormatJson(string json)
        {
            var d = JsonConvert.DeserializeObject(json);
            var s = JsonConvert.SerializeObject(d, Formatting.Indented);
            return s.Replace("\r\n", "\n");
        }

        public string Md5Hash(string s)
        {
            return s.Md5Hash();
        }

        public string ReplaceCodeStr(string text, string codes)
        {
            codes = Regex.Replace(codes, @"//\w*", ","); // replace comments
            codes = Regex.Replace(codes, @"\s*", ""); // remove spaces
            foreach (var pair in codes.SplitStr(","))
            {
                var a = pair.SplitStr("=", 2);
                text = text.Replace(a[0], a[1]);
            }
            return text;
        }
        public void ReplaceQueryFromCode(string encodedguid, string code)
        {
            var queryid = encodedguid.ToGuid();
            var query = db.LoadQueryById2(queryid);
            var c = Condition.Parse(code, queryid);
            query.Text = c.ToXml();
            db.SubmitChanges();
        }

        public class StatusFlag
        {
            public string Id { get; set; }
            public string Flag { get; set; }
            public string Desc { get; set; }
            public string Code { get; set; }
        }

        public List<StatusFlag> StatusFlagList()
        {
            var q = from c in db.Queries
                    where c.Name.StartsWith("F") && c.Name.Contains(":")
                    orderby c.Name
                    select new { c.Name, c.QueryId, c.Text };

            const string findPrefix = @"^F\d+:.*";
            var re = new Regex(findPrefix, RegexOptions.Singleline | RegexOptions.Multiline);
            var q2 = from s in q.ToList()
                     where re.Match(s.Name).Success
                     let a = s.Name.SplitStr(":", 2)
                     let c = Condition.Import(s.Text)
                     orderby a[0]
                     select new StatusFlag()
                     {
                         Flag = a[0],
                         Id = s.QueryId.ToCode(),
                         Desc = a[1],
                         Code = Regex.Replace(c.ToCode(), "^", "\t", RegexOptions.Multiline),
                     };
            return q2.ToList();
        }
        public Dictionary<string, StatusFlag> StatusFlagDictionary(string flags = null)
        {
            var filter = flags?.Split(',');

            var q = from c in db.Queries
                    where c.Name.StartsWith("F") && c.Name.Contains(":")
                    orderby c.Name
                    select new { c.Name, c.QueryId, c.Text };

            const string findPrefix = @"^F\d+:.*";
            var re = new Regex(findPrefix, RegexOptions.Singleline | RegexOptions.Multiline);
            var q2 = from s in q.ToList()
                     where re.Match(s.Name).Success
                     let a = s.Name.SplitStr(":", 2)
                     let c = Condition.Import(s.Text)
                     where (filter == null || filter.Contains(a[0]))
                     orderby a[0]
                     select new StatusFlag()
                     {
                         Flag = a[0],
                         Id = s.QueryId.ToCode(),
                         Desc = a[1],
                         Code = Regex.Replace(c.ToCode(), "^", "\t", RegexOptions.Multiline),
                     };
            return q2.ToDictionary(vv => vv.Flag, vv => vv);
        }

        public void UpdateStatusFlags()
        {
            db.UpdateStatusFlags();
        }
        public void UpdateStatusFlag(string flagid, string encodedguid)
        {
            var temptag = db.PopulateTempTag(new List<int>());
            var queryid = encodedguid.ToGuid();
            var qq = db.PeopleQuery(queryid ?? Guid.Empty);
            db.TagAll2(qq, temptag);
            db.ExecuteCommand("dbo.UpdateStatusFlag {0}, {1}", flagid, temptag.Id);
        }
    }
}
